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
using System.Collections;

#if UNITY_EDITOR
using UnityEditor.Callbacks;
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Popcron.SceneStaging
{
    public class StageExporter
    {
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
                StageUtils.SetActiveScene(scene);
                return Export(scene.name, id);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }
#endif
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
        /// Exports the current active scene into a new stage object.
        /// </summary>
        public static Stage Export(string stageName = null, string id = null)
        {
            Scene scene = SceneManager.GetActiveScene();
            Dictionary<GameObject, Prop> stageProps = new Dictionary<GameObject, Prop>();
            Stage stage = new Stage(stageName ?? scene.name, id ?? StageUtils.GetID(scene.GetHashCode()));
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
                        Component component = prop.AddComponent<GameObject>();
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
                        bool notStageSerialized = componentType.GetCustomAttribute<NotStageSerializedAttribute>() is not null;
                        if (notStageSerialized)
                        {
                            continue;
                        }

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
                                FieldInfo[] fields = StageUtils.GetFields(unityComponent.GetType());
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
                                                    comp.Set(field.Name, transformObject.ID);
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
            Prop prop = new Prop(gameObject, id);
            Prefab prefab = gameObject.GetComponentInParent<Prefab>();
            if (prefab)
            {
                if (prefab.gameObject == gameObject)
                {
                    //only add the pointer to the prefab
                    PrefabInformation prefabInformation = new PrefabInformation();
                    prefabInformation.Path = prefab.Path;
                    prop.Prefab = prefabInformation;
                }
                else
                {
                    //child of the prefab, skip
                    return;
                }
            }

            stage.AddProp(prop);
            stageObjects[gameObject] = prop;
        }
    }
}
