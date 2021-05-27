using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Popcron.SceneStaging.UnityEditor
{
    [ScriptedImporter(1, Stage.Extension)]
    public class StageImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string json = File.ReadAllText(ctx.assetPath);
            Stage stage = Stage.FromJson(json);
            StageAsset asset = StageAsset.Create(stage);
            ctx.AddObjectToAsset("map", asset);
            ctx.SetMainObject(asset);
        }

        [OnOpenAsset(1)]
        public static bool FirstStepOpen(int instanceId, int line)
        {
            Object asset = EditorUtility.InstanceIDToObject(instanceId);
            if (asset is StageAsset stageAsset)
            {
                Stage stage = stageAsset.Stage;
                if (stage is not null)
                {
                    OpenStage(stage);
                    return true;
                }
            }

            return false;
        }

        private static async void OpenStage(Stage stage)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = stage.DisplayName;

            await Task.Delay(1);
            StageBuilder.BuildAsync(stage);
        }
    }
}