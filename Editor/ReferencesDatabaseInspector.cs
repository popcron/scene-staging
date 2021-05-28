using UnityEditor;

namespace Popcron.SceneStaging.UnityEditor
{
    [CustomEditor(typeof(ReferencesDatabase))]
    public class ReferencesDatabaseInspector : Editor
    {
        private SerializedProperty referencesProperty;

        private void OnEnable()
        {
            referencesProperty = serializedObject.FindProperty("references");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(referencesProperty, true);

            //assign paths automatically
            int size = referencesProperty.arraySize;
            for (int i = size - 1; i >= 0; i--)
            {
                SerializedProperty reference = referencesProperty.GetArrayElementAtIndex(i);
                SerializedProperty assetProperty = reference.FindPropertyRelative("asset");
                SerializedProperty pathProperty = reference.FindPropertyRelative("path");

                if (assetProperty.objectReferenceValue)
                {
                    pathProperty.stringValue = AssetDatabase.GetAssetPath(assetProperty.objectReferenceValue);
                }
                else
                {
                    referencesProperty.DeleteArrayElementAtIndex(i);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}