using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using UnityComponent = UnityEngine.Component;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using System.Xml.Serialization;

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
            if (Application.isPlaying)
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
                            if (system.subSystemList[j].type == typeof(StageBuilderRunner))
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
            else
            {
#if UNITY_EDITOR
                EditorApplication.update += OnUpdate;
#endif
            }
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

        public static GameObject Instantiate(GameObject prefab, string prefabPath)
        {
#if UNITY_EDITOR
            GameObject gameObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
#else
            GameObject gameObject = Object.Instantiate(prefab);
            Prefab prefabComponent = gameObject.GetComponent<Prefab>();
            if (!prefabComponent)
            {
                prefabComponent = gameObject.AddComponent<Prefab>();
            }

            prefabComponent.Original = prefab;
            prefabComponent.Path = prefabPath;      
#endif

            return gameObject;
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

                string prefabPath = prop.Prefab.Path;
                if (!string.IsNullOrEmpty(prefabPath))
                {
                    GameObject prefab = ReferencesDatabase.Get<GameObject>(prefabPath);
                    if (prefab)
                    {
                        prefab.SetActive(false);
                        propObject.gameObject = Instantiate(prefab, prefabPath);
                        propObject.gameObject.name = prop.Name;
                        propObject.prefab = prefab;
                        propObject.prefabPath = prefabPath;
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

            try
            {
                string pathToScene = AssetDatabase.GetAssetPath(sceneAsset);
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                Scene scene = EditorSceneManager.OpenScene(pathToScene, OpenSceneMode.Single);
                SetActiveScene(scene);
                return Export(scene.name, id);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
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
                    ComponentProcessor processor = ComponentProcessor.Get<GameObject>();
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
            if (Utils.IsChildOfPrefab(gameObject))
            {
                return;
            }
            else
            {
                int id = stage.Props.Count;
                Prop prop = stage.AddProp(gameObject, id);
                if (Utils.IsPrefab(gameObject))
                {
                    AddPrefabInformation(stage, gameObject, prop);
                }

                stageObjects[gameObject] = prop;
            }
        }

        private static void AddPrefabInformation(Stage stage, GameObject gameObject, Prop prop)
        {
            string prefabPath = Utils.GetPrefabPath(gameObject);
            PrefabInformation prefabInformation = prop.Prefab;
            prefabInformation.Path = prefabPath;

#if UNITY_EDITOR
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(gameObject.transform);

            while (queue.Count > 0)
            {
                Transform child = queue.Dequeue();
                AddModifications(child);

                for (int i = 0; i < child.childCount; i++)
                {
                    queue.Enqueue(child.GetChild(i));
                }
            }

            void AddModifications(Transform child)
            {
                GameObject correspondingGameObject = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);
                PropertyModification[] modifications = PrefabUtility.GetPropertyModifications(gameObject);
                if (modifications is not null)
                {
                    for (int i = 0; i < modifications.Length; i++)
                    {
                        ref PropertyModification modification = ref modifications[i];
                        ComponentProcessor processor = null;
                        Object target = null;
                        if (modification.target is UnityComponent unityComponent)
                        {
                            if (unityComponent.gameObject == correspondingGameObject)
                            {
                                processor = ComponentProcessor.Get(unityComponent.GetType());
                                target = child.GetComponent(unityComponent.GetType());
                            }
                        }
                        else if (modification.target is GameObject targetGameObject)
                        {
                            if (targetGameObject == correspondingGameObject)
                            {
                                processor = ComponentProcessor.Get<GameObject>();
                                target = child.gameObject;
                            }
                        }

                        if (processor is not null)
                        {
                            string path = GetLocalPath(gameObject.transform, target);
                            IList<Variable> variables = processor.SaveComponent(target);
                            foreach (Variable variable in variables)
                            {
                                string name = $"{path}/{variable.Name}";
                                prefabInformation.Add(name, variable.Value);
                            }
                        }
                    }
                }
            }
#endif

            prop.Prefab = prefabInformation;
        }

        private static string GetLocalPath(Transform root, Object obj)
        {
            Transform child = null;
            string componentFullTypeName = null;
            if (obj is GameObject objGameObject)
            {
                child = objGameObject.transform;
                componentFullTypeName = typeof(GameObject).FullName;
            }
            else if (obj is UnityComponent objComponent)
            {
                child = objComponent.transform;
                componentFullTypeName = objComponent.GetType().FullName;
            }

            string path = null;
            if (child)
            {
                path = child.GetSiblingIndex().ToString();
                if (root != child)
                {
                    Transform check = child;
                    while (check.parent != root)
                    {
                        check = check.parent;
                        if (!check)
                        {
                            break;
                        }

                        path = check.GetSiblingIndex().ToString() + "/" + path;
                    }
                }
            }

            return $"{path}.{componentFullTypeName}";
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
