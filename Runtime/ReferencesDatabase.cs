using UnityEngine;
using Object = UnityEngine.Object;
using System;
using System.Collections.Generic;

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
        private static Object[] resources;

        /// <summary>
        /// The current settings data being used.
        /// </summary>
        public static ReferencesDatabase Current
        {
            get
            {
                if (!current)
                {
                    current = GetOrCreate();
                }

                return current;
            }
        }

        [SerializeField]
        private List<Reference> references = new List<Reference>();

        /// <summary>
        /// Returns an existing console settings asset, or creates a new one if none exist.
        /// </summary>
        public static ReferencesDatabase GetOrCreate()
        {
            ReferencesDatabase[] allDatabases = Resources.LoadAll<ReferencesDatabase>("");
            if (allDatabases.Length > 0)
            {
                return allDatabases[0];
            }

#if UNITY_EDITOR
            //couldnt find one, so create
            Debug.Log("couldnt find existing one");
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            //make a file here
            ReferencesDatabase newDatabase = CreateInstance<ReferencesDatabase>();
            newDatabase.name = "Database";
            AssetDatabase.CreateAsset(newDatabase, "Assets/Resources/Database.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            return GetOrCreate();
#endif

            return null;
        }

        /// <summary>
        /// Returns the path to this asset.
        /// </summary>
        public static string GetPath(Object asset)
        {
            if (assetToPath is null)
            {
                Initialize();
            }

            int instanceId = asset.GetInstanceID();
            if (assetToPath.TryGetValue(instanceId, out string assetPath))
            {
                return assetPath;
            }

#if UNITY_EDITOR
            if (resources is null)
            {
                resources = Resources.FindObjectsOfTypeAll(typeof(Object));
            }

            int resourcesLength = resources.Length;
            for (int i = resourcesLength - 1; i >= 0; i--)
            {
                ref Object resourceAsset = ref resources[i];
                if (resourceAsset == asset)
                {
                    string path = AssetDatabase.GetAssetPath(resourceAsset);
                    if (!string.IsNullOrEmpty(path))
                    {
                        Reference newReference = new Reference(path, asset);
                        current.references.Add(newReference);
                        EditorUtility.SetDirty(current);
                        assetToPath[asset.GetInstanceID()] = path;
                        return path;
                    }
                }
            }
#endif

            return null;
        }

#if UNITY_EDITOR
        [DidReloadScripts]
#endif
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            assetToPath = new Dictionary<int, string>();
            ReferencesDatabase instance = Current;
            int referencesCount = instance.references.Count;
            for (int i = 0; i < referencesCount; i++)
            {
                Reference reference = instance.references[i];
                assetToPath[reference.Asset.GetInstanceID()] = reference.Path;
            }
        }

        /// <summary>
        /// Returns an asset with this type at this path.
        /// </summary>
        public static Object Get(Type type, string path)
        {
            ReferencesDatabase current = Current;
            for (int i = 0; i < current.references.Count; i++)
            {
                Reference reference = current.references[i];
                if (reference.Path == path && reference.Asset.GetType() == type)
                {
                    return reference.Asset;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns an asset with this type at this path.
        /// </summary>
        public static T Get<T>(string path) where T : Object => Get(typeof(T), path) as T;

        [Serializable]
        public class Reference
        {
            [SerializeField, HideInInspector]
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