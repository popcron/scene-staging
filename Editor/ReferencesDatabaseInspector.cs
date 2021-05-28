using UnityEditor;
using UnityEngine;

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

            //show all
            for (int i = 0; i < referencesProperty.arraySize; i++)
            {
                SerializedProperty reference = referencesProperty.GetArrayElementAtIndex(i);
                SerializedProperty assetProperty = reference.FindPropertyRelative("asset");
                SerializedProperty pathProperty = reference.FindPropertyRelative("path");
                EditorGUILayout.ObjectField(pathProperty.stringValue, assetProperty.objectReferenceValue, typeof(Object), false);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}