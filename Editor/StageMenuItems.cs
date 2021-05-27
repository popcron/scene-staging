using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Popcron.SceneStaging.UnityEditor
{
    public class StageMenuItems
    {
        private const string CreateStageFromScene = "Assets/Scene Staging/Create Stage";
        private const string OpenStageAsScene = "Assets/Scene Staging/Open as Scene";
        private const string SaveAsStage = "File/Save as Stage %&s";

        [MenuItem(CreateStageFromScene)]
        private static void ConvertToStage()
        {
            if (Selection.activeObject is SceneAsset sceneAsset)
            {
                string previousScene = SceneManager.GetActiveScene().path;
                Stage stage = StageBuilder.Export(sceneAsset);
                if (stage is not null)
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
        private static bool ConvertToStageValidation()
        {
            return Selection.activeObject is SceneAsset;
        }

        [MenuItem(OpenStageAsScene)]
        private static async void ConvertToScene()
        {
            if (Selection.activeObject is StageAsset stageAsset)
            {
                Stage stage = stageAsset.Stage;
                if (stage is not null)
                {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                    scene.name = stage.DisplayName;

                    await Task.Delay(1);
                    StageBuilder.BuildAsync(stage);
                }
            }
        }

        [MenuItem(OpenStageAsScene, true)]
        private static bool ConvertToSceneValidation()
        {
            return Selection.activeObject is StageAsset;
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
                    string id = StageBuilder.GetID(stageAsset.GetInstanceID());
                    Stage stage = StageBuilder.Export(activeScene.name, id);
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