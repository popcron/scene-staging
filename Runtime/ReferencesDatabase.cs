using UnityEngine;
using Object = UnityEngine.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

namespace Popcron.SceneStaging
{
    public class ReferencesDatabase : ScriptableObject
    {
        private static ReferencesDatabase current;
        private static Dictionary<int, string> assetToPath;
        private static ReadOnlyCollection<Reference> referencesReadOnly;
        private static Dictionary<(Type, string), Object> referenceCache;

        /// <summary>
        /// The current settings data being used.
        /// </summary>
        public static ReferencesDatabase Current
        {
            get
            {
                if (!current)
                {
                    Initialize();
                }

                return current;
            }
        }

        [SerializeField]
        private List<Reference> references = new List<Reference>();

        public static ReadOnlyCollection<Reference> All
        {
            get
            {
                if (referencesReadOnly is null)
                {
                    referencesReadOnly = Current.references.AsReadOnly();
                }
                else if (referencesReadOnly.Count != Current.references.Count)
                {
                    referencesReadOnly = Current.references.AsReadOnly();
                }

                return referencesReadOnly;
            }
        }

        private void OnEnable()
        {
            Sanitize();
            current = this;
            assetToPath = new Dictionary<int, string>();
            int referencesCount = references.Count;
            for (int i = 0; i < referencesCount; i++)
            {
                Reference reference = references[i];
                assetToPath[reference.Asset.GetInstanceID()] = reference.Path;
            }
        }

        private void Sanitize()
        {
            //remove duplicates and empties
            bool changesMade = false;
            int referencesCount = references.Count;
            for (int i = referencesCount - 1; i >= 0; i--)
            {
                Reference reference = references[i];
                if (!reference.Asset)
                {
                    references.RemoveAt(i);
                    changesMade = true;
                }
                else
                {
                    int count = CountDuplicates(reference.Asset);
                    if (count > 1)
                    {
                        references.RemoveAt(i);
                        changesMade = true;
                    }
                }
            }

            if (changesMade)
            {
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }

        private int CountDuplicates(Object asset)
        {
            int count = 0;
            int referencesCount = references.Count;
            for (int i = referencesCount - 1; i >= 0; i--)
            {
                Reference reference = references[i];
                if (reference.Asset == asset)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Returns the path to this asset.
        /// </summary>
        public static string GetPath(Object asset)
        {
            if (asset is null)
            {
                return null;
            }

            if (assetToPath is null || !current)
            {
                Initialize();
            }

            if (assetToPath is null)
            {
                return null;
            }

            int instanceId = asset.GetInstanceID();
            if (assetToPath.TryGetValue(instanceId, out string assetPath))
            {
#if UNITY_EDITOR
                return StageUtils.GetAssetPath(asset);
#else
                return assetPath;
#endif
            }

#if UNITY_EDITOR
            string realPath = StageUtils.GetAssetPath(asset);
            if (!string.IsNullOrEmpty(realPath))
            {
                Reference newReference = new Reference(realPath, asset);
                current.references.Add(newReference);
                referencesReadOnly = null;
                EditorUtility.SetDirty(current);
                assetToPath[asset.GetInstanceID()] = realPath;
                return realPath;
            }
#endif

            return null;
        }

#if UNITY_EDITOR
        [DidReloadScripts]
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            List<Object> preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
            bool notPreloadedYet = true;
            foreach (Object preloadedAsset in preloadedAssets)
            {
                if (preloadedAsset is ReferencesDatabase referencesDatabase)
                {
                    notPreloadedYet = false;
                    return;
                }
            }

            if (notPreloadedYet)
            {
                string[] guids = AssetDatabase.FindAssets($"t:{typeof(ReferencesDatabase).FullName}");
                ReferencesDatabase database = null;
                if (guids.Length == 0)
                {
                    database = CreateInstance<ReferencesDatabase>();
                    AssetDatabase.CreateAsset(database, "Assets/Database.asset");
                }
                else
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    database = AssetDatabase.LoadAssetAtPath<ReferencesDatabase>(path);
                }

                //add to end
                preloadedAssets.Add(database);

                //remove any empties
                for (int i = preloadedAssets.Count - 1; i >= 0; i--)
                {
                    if (!preloadedAssets[i])
                    {
                        preloadedAssets.RemoveAt(i);
                    }
                }

                PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
            }
        }
#else   
        private static void Initialize() { }
#endif

        /// <summary>
        /// Returns an asset with this type at this path.
        /// </summary>
        public static Object Get(Type type, string path)
        {
            if (referenceCache is null)
            {
                CreateCache();
            }

            if (referenceCache.TryGetValue((type, path), out Object asset))
            {
                return asset;
            }

            return null;
        }

        private static void CreateCache()
        {
            referenceCache = new Dictionary<(Type, string), Object>();
            ReferencesDatabase current = Current;
            int referencesCount = current.references.Count;
            for (int i = 0; i < referencesCount; i++)
            {
                Reference reference = current.references[i];
                Object asset = reference.Asset;
                if (asset)
                {
                    Type type = asset.GetType();
                    referenceCache[(type, reference.Path)] = asset;
                }
            }
        }

        /// <summary>
        /// Returns an asset with this type at this path.
        /// </summary>
        public static T Get<T>(string path) where T : Object => Get(typeof(T), path) as T;

        [Serializable]
        public class Reference
        {
            [SerializeField]
            private string path;

            [SerializeField]
            private Object asset;

            public string Path => path;
            public Object Asset => asset;

            public Reference(string path, Object asset)
            {
                this.path = path;
                this.asset = asset;
            }
        }
    }
}