using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using UnityComponent = UnityEngine.Component;
using System.Threading;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

#if UNITY_EDITOR
using UnityEditor.Callbacks;
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Popcron.SceneStaging
{
    public class StageBuilder
    {
        private static float buildProgress = 0f;
        private static Stage stage;
        private static StageBuildStep buildStep = StageBuildStep.Inactive;
        private static int index;
        private static List<PropObject> propObjects = new List<PropObject>();

        public delegate void StageBuiltDelegate(Stage stage);
        public static StageBuiltDelegate OnBuiltStage { get; set; }

        public static StageBuildStep BuildStep
        {
            get => buildStep;
            set
            {
                if (buildStep != value)
                {
                    buildStep = value;
                }
            }
        }

        /// <summary>
        /// Is there stage currently being built.
        /// </summary>
        public static bool IsBuilding => BuildStep != StageBuildStep.Inactive;

        /// <summary>
        /// The name of the last active scene opened.
        /// </summary>
        private static string LastActiveScene
        {
            get => PlayerPrefs.GetString(nameof(LastActiveScene));
            set => PlayerPrefs.SetString(nameof(LastActiveScene), value);
        }

        /// <summary>
        /// The name of the opened scene that represents a stage.
        /// </summary>
        private static string LastStageScene
        {
            get => PlayerPrefs.GetString(nameof(LastStageScene));
            set => PlayerPrefs.SetString(nameof(LastStageScene), value);
        }

        /// <summary>
        /// The current build progress value ranging from 0 to 1.
        /// </summary>
        public static float BuildProgress
        {
            get
            {
                if (BuildStep == StageBuildStep.Initializing)
                {
                    return 0f;
                }
                else if (BuildStep == StageBuildStep.FinishedBuilding)
                {
                    return 1f;
                }
                else if (BuildStep == StageBuildStep.CreatingProps)
                {

                }
                else
                {

                }

                return buildProgress;
            }
        }

        private static void OnUpdate()
        {
            if (buildStep == StageBuildStep.Initializing)
            {
                CreateStageScene();
                Initializing();
            }
            else if (buildStep == StageBuildStep.CreatingProps)
            {
                CreatingProps();
            }
            else if (buildStep == StageBuildStep.ParentObjects)
            {
                ParentObjects();
            }
            else if (buildStep == StageBuildStep.CreateComponents)
            {
                CreateComponents();
            }
            else if (buildStep == StageBuildStep.LoadComponents)
            {
                LoadComponents();
            }
            else if (buildStep == StageBuildStep.FinishedBuilding)
            {
                FinishedBuilding();
                buildStep = StageBuildStep.Inactive;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            LastStageScene = null;
            LastActiveScene = null;
            buildProgress = 0f;
            stage = null;
            buildStep = StageBuildStep.Inactive;
            index = 0;
            InjectRunner();
        }

        private static void InjectRunner()
        {
            PlayerLoopSystem playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            for (int i = 0; i < playerLoop.subSystemList.Length; i++)
            {
                ref PlayerLoopSystem system = ref playerLoop.subSystemList[i];
                if (system.type == typeof(Update))
                {   
                    //short circuit if already present
                    for (int j = 0; j < system.subSystemList.Length; j++)
                    {
                        if (system.subSystemList[i].type == typeof(StageBuilderRunner))
                        {
                            return;
                        }
                    }

                    List<PlayerLoopSystem> list = system.subSystemList.ToList();
                    PlayerLoopSystem runner = new PlayerLoopSystem();
                    runner.type = typeof(StageBuilderRunner);
                    runner.updateDelegate = OnUpdate;
                    list.Add(runner);
                    system.subSystemList = list.ToArray();
                }
            }

            PlayerLoop.SetPlayerLoop(playerLoop);
        }

#if UNITY_EDITOR
        [DidReloadScripts]
#endif
        private static void Recompiled()
        {
            InjectRunner();
        }

        private static void Initializing()
        {
            //gather the max id
            int maxId = 0;
            int propsCount = stage.Props.Count;
            for (int i = propsCount - 1; i >= 0; i--)
            {
                Prop prop = stage.Props[i];
                maxId = Mathf.Max(maxId, prop.ID);
            }

            propObjects.Clear();
            index = 0;
            BuildStep = StageBuildStep.CreatingProps;
        }

        private static void CreatingProps()
        {
            //create blank objects
            float progressStep = 1f / stage.Props.Count;
            buildProgress += progressStep * 1 / 3f;
            if (stage.Props.Count > 0)
            {
                Prop prop = stage.Props[index];
                PropObject propObject = new PropObject();
                propObject.gameObject = null;
                propObjects.Insert(prop.ID, propObject);

                if (!string.IsNullOrEmpty(prop.Prefab))
                {
                    GameObject prefab = ReferencesDatabase.Get<GameObject>(prop.Prefab);
                    if (prefab)
                    {
                        prefab.SetActive(false);
#if UNITY_EDITOR
                        propObject.gameObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
#else
                        propObject.gameObject = UnityEngine.Object.Instantiate(prefab);
#endif
                        propObject.gameObject.name = prop.Name;
                        propObject.prefab = prefab;
                        propObject.prefabPath = ReferencesDatabase.GetPath(prefab);
                        prefab.SetActive(true);
                    }
                    else
                    {
                        propObject.gameObject = new GameObject(prop.Name);
                    }
                }
                else
                {
                    propObject.gameObject = new GameObject(prop.Name);
                }

                prop.GameObject = propObject.gameObject;
                prop.GameObject.SetActive(false);
                index++;
            }

            //finished
            if (index >= stage.Props.Count)
            {
                buildProgress = 1f / 3f;
                BuildStep = StageBuildStep.ParentObjects;
                index = 0;
            }
        }

        private static void ParentObjects()
        {
            //parent them
            int propsCount = stage.Props.Count;
            for (int i = propsCount - 1; i >= 0; i--)
            {
                Prop prop = stage.Props[i];
                if (prop.Parent != -1)
                {
                    GameObject gameObject = propObjects[prop.ID].gameObject;
                    GameObject parentGameObject = propObjects[prop.Parent].gameObject;
                    gameObject.transform.SetParent(parentGameObject.transform);
                }
            }

            BuildStep = StageBuildStep.CreateComponents;
        }

        private static void CreateComponents()
        {
            float progressStep = 1f / stage.Props.Count;
            buildProgress += progressStep * 1 / 3f;

            //load the components in
            if (stage.Props.Count > 0)
            {
                Prop prop = stage.Props[index];
                GameObject gameObject = propObjects[prop.ID].gameObject;
                int componentsCount = prop.Count;
                for (int i = componentsCount - 1; i >= 0; i--)
                {
                    Component component = prop[i];
                    Type type = component.Type;
                    ComponentProcessor processor = ComponentProcessor.Get(type);
                    if (processor is not null)
                    {
                        processor.GetComponent(component, gameObject);
                    }
                    else
                    {
                        Debug.LogError($"no processor available for {type}");
                    }
                }

                index++;
            }

            //finished
            if (index >= stage.Props.Count)
            {
                buildProgress = 2f / 3f;
                BuildStep = StageBuildStep.LoadComponents;
                index = 0;
            }
        }

        private static void LoadComponents()
        {
            float progressStep = 1f / stage.Props.Count;
            buildProgress += progressStep * 1 / 3f;

            //load the components in
            if (stage.Props.Count > 0)
            {
                Prop prop = stage.Props[index];
                GameObject gameObject = propObjects[prop.ID].gameObject;
                int componentsCount = prop.Count;
                for (int c = componentsCount - 1; c >= 0; c--)
                {
                    Component component = prop[c];
                    Type type = component.Type;
                    ComponentProcessor processor = ComponentProcessor.Get(type);
                    if (processor is not null)
                    {
                        try
                        {
                            Object unityComponent = processor.GetComponent(component, gameObject);
                            processor.Stage = stage;
                            processor.LoadComponent(component, unityComponent);

                            //assign transforms references
                            FieldInfo[] fields = Utils.GetFields(unityComponent.GetType());
                            int fieldsCount = fields.Length;
                            for (int f = fieldsCount - 1; f >= 0; f--)
                            {
                                FieldInfo field = fields[f];
                                if (field.FieldType == typeof(Transform))
                                {
                                    //attempt to set the transform value for this field
                                    string transformValue = component.GetRaw(field.Name);
                                    if (!string.IsNullOrEmpty(transformValue) && int.TryParse(transformValue, out int transformId))
                                    {
                                        GameObject transformObject = null;
                                        if (propObjects.Count > transformId && transformId >= 0)
                                        {
                                            transformObject = propObjects[transformId].gameObject;
                                        }

                                        if (transformObject)
                                        {
                                            field.SetValue(unityComponent, transformObject.transform);
                                        }
                                    }
                                }
                            }

                            if (unityComponent is IOnLoaded onLoaded)
                            {
                                onLoaded.Loaded(component.ToList());
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                        }
                    }
                }

                index++;
            }

            //finished
            if (index >= stage.Props.Count)
            {
                buildProgress = 1f;
                BuildStep = StageBuildStep.FinishedBuilding;
                index = 0;

                //enable all gos
                int propsCount = stage.Props.Count;
                for (int p = 0; p < propsCount; p++)
                {
                    Prop prop = stage.Props[p];
                    prop.GameObject.SetActive(true);
                }
            }
        }

        private static void FinishedBuilding()
        {
            SceneSettings sceneSettings = Object.FindObjectOfType<SceneSettings>();
            if (sceneSettings)
            {
                Object.DestroyImmediate(sceneSettings.gameObject);
            }

            OnBuiltStage?.Invoke(stage);
        }

        /// <summary>
        /// Unloads any existing stages and initiates an asynchronous build process.
        /// </summary>
        public static async void BuildAsync(Stage stageToBuild) => await BuildAsyncTask(stageToBuild);

        /// <summary>
        /// Unloads any existing stages and initiates an asynchronous build process that can be awaited for.
        /// </summary>
        public static async Task BuildAsyncTask(Stage stageToBuild)
        {
            stage = stageToBuild;
            BuildStep = StageBuildStep.Initializing;
            buildProgress = 0f;

            while (IsBuilding)
            {
                await Task.Delay(1);
            }
        }

        private static void SetActiveScene(Scene scene)
        {
            SceneManager.SetActiveScene(scene);
        }

        /// <summary>
        /// Creates a new scene to load the stage into.
        /// </summary>
        private static void CreateStageScene()
        {
            string sceneName = "New Stage";
            if (stage is not null)
            {
                sceneName = stage.DisplayName;
            }

            LastActiveScene = SceneManager.GetActiveScene().name;
            if (!Application.isPlaying)
            {
#if UNITY_EDITOR
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                NewSceneSetup setup = NewSceneSetup.EmptyScene;
                NewSceneMode mode = NewSceneMode.Single;
                Scene newScene = EditorSceneManager.NewScene(setup, mode);
                newScene.name = sceneName;
                SetActiveScene(newScene);
#endif
            }
            else
            {
                Scene existingScene = SceneManager.GetSceneByName(sceneName);
                if (!existingScene.isLoaded || !existingScene.IsValid())
                {
                    existingScene = SceneManager.CreateScene(sceneName);
                }

                SetActiveScene(existingScene);
            }

            LastStageScene = sceneName;
            Debug.Log($"current stage scene name is now {sceneName}");
        }

        /// <summary>
        /// Returns a game object that is representing this prop.
        /// </summary>
        public static GameObject GetGameObject(Prop prop)
        {
            if (propObjects.Count > prop.ID && prop.ID >= 0)
            {
                return propObjects[prop.ID].gameObject;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a prop object based on this game object.
        /// </summary>
        public static PropObject GetPropObject(GameObject gameObject)
        {
            int propsCount = propObjects.Count;
            for (int i = propsCount - 1; i >= 0; i--)
            {
                PropObject prop = propObjects[i];
                if (prop.gameObject == gameObject)
                {
                    return prop;
                }
            }

            return null;
        }

        private static SceneSettings CreateSceneSettings()
        {
            SceneSettings sceneSettings = Object.FindObjectOfType<SceneSettings>();
            if (!sceneSettings)
            {
                sceneSettings = new GameObject(nameof(SceneSettings)).AddComponent<SceneSettings>();
            }

            sceneSettings.SaveSettings();
            return sceneSettings;
        }

        /// <summary>
        /// Returns a new unique ID for stages.
        /// </summary>
        public static string GetID(int? seed = null)
        {
            return Utils.RandomString(16, seed);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Exports this scene into a new stage.
        /// </summary>
        public static Stage Export(SceneAsset sceneAsset, string id = null)
        {
            if (!sceneAsset)
            {
                Debug.LogError("no scene asset given to export");
                return null;
            }

            string pathToScene = AssetDatabase.GetAssetPath(sceneAsset);
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            Scene scene = EditorSceneManager.OpenScene(pathToScene, OpenSceneMode.Single);
            SetActiveScene(scene);
            return Export(scene.name, id);
        }
#else
        public static Stage Export(object sceneAsset)
        {
            Debug.LogWarning("tried using an editor only method in build");
            return null;
        }
#endif

        /// <summary>
        /// Exports the current active scene into a new stage object.
        /// </summary>
        public static Stage Export(string stageName = null, string id = null)
        {
            Scene scene = SceneManager.GetActiveScene();
            Dictionary<GameObject, Prop> stageProps = new Dictionary<GameObject, Prop>();
            Stage stage = new Stage(stageName ?? scene.name, id ?? GetID(scene.GetHashCode()));
            SceneSettings sceneSettings = CreateSceneSettings();

            //first add all objects with their own unique id
            GameObject[] rootObjects = scene.GetRootGameObjects();
            int rootObjectsCount = rootObjects.Length;
            for (int i = 0; i < rootObjectsCount; i++)
            {
                GameObject rootObject = rootObjects[i];
                AddToStage(stage, rootObject, stageProps);
            }

            //assign parent objects also
            //give objects their variables
            foreach (KeyValuePair<GameObject, Prop> element in stageProps)
            {
                GameObject gameObject = element.Key;
                Prop prop = element.Value;
                if (gameObject)
                {
                    if (gameObject.transform.parent)
                    {
                        if (stageProps.TryGetValue(gameObject.transform.parent.gameObject, out Prop parentObject))
                        {
                            prop.Parent = parentObject.ID;
                        }
                    }

                    //put gameObject in first
                    ComponentProcessor processor = ComponentProcessor.Get(gameObject.GetType());
                    if (processor is not null)
                    {
                        Component component = prop.AddComponent("UnityEngine.GameObject");
                        Object unityComponent = processor.GetComponent(component, gameObject);
                        processor.Stage = stage;
                        processor.SaveComponent(component, unityComponent);
                    }

                    //put components in
                    Object[] components = gameObject.GetComponents<UnityComponent>();
                    int componentsCount = components.Length;
                    for (int c = componentsCount - 1; c >= 0; c--)
                    {
                        Object component = components[c];
                        Type componentType = component.GetType();
                        processor = ComponentProcessor.Get(componentType);
                        if (processor is not null)
                        {
                            Component comp = new Component(componentType.FullName);
                            Object unityComponent = processor.GetComponent(comp, gameObject);
                            if (unityComponent)
                            {
                                processor.Stage = stage;
                                processor.SaveComponent(comp, unityComponent);
                                prop.AddComponent(comp);

                                //inject transform references
                                FieldInfo[] fields = Utils.GetFields(unityComponent.GetType());
                                int fieldsCount = fields.Length;
                                for (int f = fieldsCount - 1; f >= 0; f--)
                                {
                                    FieldInfo field = fields[f];
                                    if (field.FieldType == typeof(Transform))
                                    {
                                        //attempt to set the transform value for this field
                                        Transform transformValue = field.GetValue(unityComponent) as Transform;
                                        if (transformValue)
                                        {
                                            if (stageProps.TryGetValue(transformValue.gameObject, out Prop transformObject))
                                            {
                                                bool set = false;
                                                int variablesCount = comp.Count;
                                                for (int v = variablesCount - 1; v >= 0; v--)
                                                {
                                                    Variable variable = comp[v];
                                                    if (variable.Name == field.Name)
                                                    {
                                                        set = true;
                                                        comp[v] = new Variable(variable.Name, transformObject.ID.ToString());
                                                        break;
                                                    }
                                                }

                                                //wasnt set because it doesnt exist in the list
                                                if (!set)
                                                {
                                                    comp.Add(field.Name, transformObject.ID);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (sceneSettings)
            {
                Object.DestroyImmediate(sceneSettings.gameObject);
            }

            return stage;
        }

        /// <summary>
        /// Adds this object to this stage.
        /// </summary>
        private static void AddToStage(Stage stage, GameObject gameObject, Dictionary<GameObject, Prop> stageObjects)
        {
            if (gameObject)
            {
                Add(stage, gameObject, stageObjects);
                int childCount = gameObject.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    Transform child = gameObject.transform.GetChild(i);
                    AddToStage(stage, child.gameObject, stageObjects);
                }
            }
        }

        private static void Add(Stage stage, GameObject gameObject, Dictionary<GameObject, Prop> stageObjects)
        {
            int id = stage.Props.Count;
            string prefabPath = null;
            if (!Utils.IsChildOfPrefab(gameObject))
            {
                prefabPath = Utils.GetPrefabPath(gameObject);
            }
            else
            {
                return;
            }

            Prop prop = stage.AddProp(gameObject, prefabPath, id);
            stageObjects[gameObject] = prop;
        }

        [Serializable]
        public class PropObject
        {
            public GameObject gameObject;
            public GameObject prefab;
            public string prefabPath;
        }

        public struct StageBuilderRunner
        {

        }
    }
}
