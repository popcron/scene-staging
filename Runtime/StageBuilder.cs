using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using System.Collections;

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
        private static int buildSpeed = 5;
        private static StageBuildStep buildStep = StageBuildStep.Inactive;
        private static int index;
        private static PropsCollection propObjects = new PropsCollection();

        public delegate void StageFinishedBuildingDelegate(Stage stage);
        public delegate void StageStartBuildingDelegate(Stage stage);

        /// <summary>
        /// Occurs when a stage has been finished being built.
        /// </summary>
        public static StageFinishedBuildingDelegate OnBuiltStage { get; set; }

        /// <summary>
        /// Occurs when a stage building process has just started.
        /// </summary>
        public static StageStartBuildingDelegate OnStartBuilding { get; set; }

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
            if (buildStep != StageBuildStep.Inactive)
            {
                for (int b = 0; b < buildSpeed; b++)
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
                        break;
                    }
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
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
            if (!Application.isPlaying)
            {
                return PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            }
#endif

            GameObject gameObject = Object.Instantiate(prefab);
            Prefab prefabComponent = gameObject.GetComponent<Prefab>();
            if (!prefabComponent)
            {
                prefabComponent = gameObject.AddComponent<Prefab>();
            }

            prefabComponent.Path = prefabPath;
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
                PropsCollection.PropObject propObject = new PropsCollection.PropObject();
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
                    if (component.FullTypeName == "$prefab")
                    {
                        continue;
                    }

                    Type type = component.Type;
                    ComponentProcessor processor = ComponentProcessor.Get(type);
                    if (processor != null)
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
                    if (processor != null)
                    {
                        try
                        {
                            Object unityComponent = processor.GetComponent(component, gameObject);
                            processor.Stage = stage;
                            processor.LoadComponent(component, unityComponent);

                            //assign transforms references
                            FieldInfo[] fields = StageUtils.GetFields(unityComponent.GetType());
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
        public static async void BuildAsync(Stage stageToBuild, int speed = 5)
        {
            await BuildAsyncTask(stageToBuild, speed);
        }

        /// <summary>
        /// Unloads any existing stages and initiates an asynchronous build process that can be awaited for.
        /// </summary>
        public static async Task BuildAsyncTask(Stage stageToBuild, int speed = 5)
        {
            stage = stageToBuild;
            buildSpeed = Mathf.Clamp(speed, 1, int.MaxValue);
            BuildStep = StageBuildStep.Initializing;
            buildProgress = 0f;
            OnStartBuilding?.Invoke(stageToBuild);

            while (IsBuilding)
            {
                await Task.Delay(1);
            }
        }

        /// <summary>
        /// Creates a new scene to load the stage into.
        /// </summary>
        private static void CreateStageScene()
        {
            string sceneName = "New Stage";
            if (stage != null)
            {
                sceneName = stage.DisplayName;
            }

            if (!Application.isPlaying)
            {
#if UNITY_EDITOR
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                NewSceneSetup setup = NewSceneSetup.EmptyScene;
                NewSceneMode mode = NewSceneMode.Single;
                Scene newScene = EditorSceneManager.NewScene(setup, mode);
                newScene.name = sceneName;
                StageUtils.SetActiveScene(newScene);
#endif
            }
            else
            {
                Scene existingScene = SceneManager.GetSceneByName(sceneName);
                if (!existingScene.isLoaded || !existingScene.IsValid())
                {
                    existingScene = SceneManager.CreateScene(sceneName);
                }

                StageUtils.SetActiveScene(existingScene);
            }
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

        [Serializable]
        public class PropsCollection : IList<PropsCollection.PropObject>
        {
            public List<PropObject> list = new List<PropObject>();

            public PropObject this[int index]
            { 
                get => list[index]; 
                set => list[index] = value;
            }

            public int Count => list.Count;
            public bool IsReadOnly => false;

            public void Add(PropObject item) => list.Add(item);
            public void Clear() => list.Clear();
            public bool Contains(PropObject item) => list.Contains(item);
            public void CopyTo(PropObject[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
            public IEnumerator<PropObject> GetEnumerator() => list.GetEnumerator();
            public int IndexOf(PropObject item) => list.IndexOf(item);
            public void Insert(int index, PropObject item) => list.Insert(index, item);
            public bool Remove(PropObject item) => list.Remove(item);
            public void RemoveAt(int index) => list.RemoveAt(index);
            IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

            [Serializable]
            public class PropObject
            {
                public GameObject gameObject;
                public GameObject prefab;
                public string prefabPath;
            }
        }

        public struct StageBuilderRunner { }
    }
}
