using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Popcron.SceneStaging
{
    [NotStageSerialized, ExecuteAlways]
    public class Prefab : MonoBehaviour
    {
        [SerializeField]
        private string path;

        public string Path => path;

#if UNITY_EDITOR
        private void Update()
        {
            GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            string newPath = ReferencesDatabase.GetPath(prefab);
            if (path != newPath)
            {
                path = newPath;
                EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}