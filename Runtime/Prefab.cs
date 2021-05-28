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

        /// <summary>
        /// The path to this prefab.
        /// </summary>
        public string Path
        {
            get => path;
            set => path = value;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying)
            {
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
                string newPath = ReferencesDatabase.GetPath(prefab);
                if (path != newPath && !string.IsNullOrEmpty(newPath))
                {
                    path = newPath;
                    EditorUtility.SetDirty(this);
                }
            }
        }
#endif
    }
}