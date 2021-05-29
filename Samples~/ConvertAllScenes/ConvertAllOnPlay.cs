using Popcron.SceneStaging;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using Stage = Popcron.SceneStaging.Stage;

[InitializeOnLoad]
public class ConvertAllOnPlay : MonoBehaviour
{
    static ConvertAllScenes()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanging;
    }

    private static void OnPlayModeStateChanging(PlayModeStateChange playModeStateChange)
    {
        if (playModeStateChange == PlayModeStateChange.ExitingEditMode)
        {
            ConvertAll();
        }
    }

    private static void ConvertAll()
    {
        string levelsFolder = "Assets/Levels";
        if (!AssetDatabase.IsValidFolder(levelsFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Levels");
        }

        List<StageAsset> stages = new List<StageAsset>();

        //find all scenes
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            if (!scenePath.StartsWith("Assets/"))
            {
                continue;
            }

            string fileName = Path.GetFileNameWithoutExtension(scenePath);
            string stagePath = $"Assets/Levels/{fileName}.asset";

            //load the scene as an asset here
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

            //export the scene as a stage
            Stage stage = StageExporter.Export(sceneAsset);

            //try to find an existing stage asset, otherwise create one
            StageAsset stageAsset = AssetDatabase.LoadAssetAtPath<StageAsset>(stagePath);
            if (!stageAsset)
            {
                stageAsset = StageAsset.Create(sceneAsset.name);
                AssetDatabase.CreateAsset(stageAsset, stagePath);
            }

            stageAsset.Stage = stage;
            stages.Add(stageAsset);

            EditorUtility.SetDirty(stageAsset);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        //do what you want with the stages list...
    }
}
