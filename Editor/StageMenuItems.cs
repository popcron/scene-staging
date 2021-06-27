using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Popcron.SceneStaging
{
    public class StageMenuItems
    {
        private const string CreateStageFromScene = "Assets/Scene Staging/Create Stage as Asset";
        private const string CreateStageFromSceneAsJSON = "Assets/Scene Staging/Create Stage as JSON";
        private const string OpenStageAsScene = "Assets/Scene Staging/Open as Scene";
        private const string SaveAsStage = "File/Save as Stage %&s";

        [MenuItem(CreateStageFromScene)]
        private static void ConvertToStage()
        {
            if (Selection.activeObject is SceneAsset sceneAsset)
            {
                string previousScene = SceneManager.GetActiveScene().path;
                Stage stage = StageExporter.Export(sceneAsset);
                if (stage != null)
                {
                    string assetPath = AssetDatabase.GetAssetPath(sceneAsset);
                    assetPath = Path.ChangeExtension(assetPath, ".asset");
                    StageAsset stageAsset = AssetDatabase.LoadAssetAtPath<StageAsset>(assetPath);
                    if (!stageAsset)
                    {
                        stageAsset = StageAsset.Create(stage);
                        AssetDatabase.CreateAsset(stageAsset, assetPath);
                    }
                    else
                    {
                        stageAsset.Stage = stage;
                        EditorUtility.SetDirty(stageAsset);
                    }

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                if (!string.IsNullOrEmpty(previousScene))
                {
                    Scene scene = EditorSceneManager.OpenScene(previousScene, OpenSceneMode.Single);
                    SceneManager.SetActiveScene(scene);
                }
            }
        }

        [MenuItem(CreateStageFromScene, true)]
        private static bool ConvertToStageValidation() => Selection.activeObject is SceneAsset;

        [MenuItem(CreateStageFromSceneAsJSON)]
        private static void CreateStageFromSceneAsJSONAction()
        {
            if (Selection.activeObject is SceneAsset sceneAsset)
            {
                string previousScene = SceneManager.GetActiveScene().path;
                Stage stage = StageExporter.Export(sceneAsset);
                if (stage != null)
                {
                    string json = stage.ToJson();
                    string assetPath = AssetDatabase.GetAssetPath(sceneAsset);
                    assetPath = Path.ChangeExtension(assetPath, ".json");
                    File.WriteAllText(assetPath, json);
                    AssetDatabase.Refresh();
                }

                if (!string.IsNullOrEmpty(previousScene))
                {
                    Scene scene = EditorSceneManager.OpenScene(previousScene, OpenSceneMode.Single);
                    SceneManager.SetActiveScene(scene);
                }
            }
        }

        [MenuItem(CreateStageFromSceneAsJSON, true)]
        private static bool CreateStageFromSceneAsJSONActionValidation() => Selection.activeObject is SceneAsset;

        [MenuItem(OpenStageAsScene)]
        private static async void ConvertToScene()
        {
            Stage stage = null;
            if (Selection.activeObject is TextAsset textAsset)
            {
                stage = Stage.FromJson(textAsset.text);
            }

            if (Selection.activeObject is StageAsset stageAsset)
            {
                stage = stageAsset.Stage;   
            }

            if (stage != null)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                scene.name = stage.DisplayName;

                await Task.Delay(1);
                StageBuilder.BuildAsync(stage, int.MaxValue);
            }
        }

        [MenuItem(OpenStageAsScene, true)]
        private static bool ConvertToSceneValidation()
        {
            if (Selection.activeObject is StageAsset)
            {
                return true;
            }

            if (Selection.activeObject is TextAsset textAsset)
            {
                return true;
            }

            return false;
        }

        [MenuItem(SaveAsStage, false, 170)]
        private static void SaveAsStageAction()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.isLoaded && activeScene.IsValid())
            {
                string assetPath;
                if (!string.IsNullOrEmpty(activeScene.path))
                {
                    assetPath = Path.ChangeExtension(activeScene.path, ".asset");
                }
                else
                {
                    assetPath = EditorUtility.SaveFilePanelInProject("Save Stage", "New Stage", "asset", "Save current Scene as a Stage");
                }

                if (!string.IsNullOrEmpty(assetPath))
                {
                    StageAsset stageAsset = AssetDatabase.LoadAssetAtPath<StageAsset>(assetPath);
                    string id = StageUtils.GetID(stageAsset.GetInstanceID());
                    Stage stage = StageExporter.Export(activeScene.name, id);
                    if (!stageAsset)
                    {
                        stageAsset = StageAsset.Create(stage);
                        AssetDatabase.CreateAsset(stageAsset, assetPath);
                    }
                    else
                    {
                        stageAsset.Stage = stage;
                        EditorUtility.SetDirty(stageAsset);
                    }

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}