#if UNITY_EDITOR
using UnityEditor;

namespace Popcron.SceneStaging
{
    [CustomEditor(typeof(StageAsset))]
    public class StageAssetInspector : Editor
    {
        private StageAsset stageAsset;
        private SerializedProperty stage;
        private SerializedProperty id;
        private SerializedProperty displayName;
        private SerializedProperty props;

        private void OnEnable()
        {
            stageAsset = target as StageAsset;
            stage = serializedObject.FindProperty("stage");
            id = stage.FindPropertyRelative("id");
            displayName = stage.FindPropertyRelative("displayName");
            props = stage.FindPropertyRelative("props");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("ID", id.stringValue);
            EditorGUILayout.PropertyField(displayName);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(props, true);
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif