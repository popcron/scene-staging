using UnityEditor;

namespace Popcron.SceneStaging.UnityEditor
{
    [CustomEditor(typeof(StageImporter))]
    public class StageImporterDrawer : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return false;
        }

        public override void OnInspectorGUI()
        {

        }
    }
}