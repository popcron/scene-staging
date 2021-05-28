using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Popcron.SceneStaging
{
    public class Prefab : MonoBehaviour
    {
        [SerializeField]
        private string path;

        public string Path => path;

        private void OnValidate()
        {
#if UNITY_EDITOR
            GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            string newPath = ReferencesDatabase.GetPath(prefab);
            if (path != newPath)
            {
                path = newPath;
            }
#endif
        }
    }
}