using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = System.Random;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;
using UnityComponent = UnityEngine.Component;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Popcron.SceneStaging
{
    public class StageUtils
    {
        private static UnityObject[] resources;
        private static Random random;
        private static Dictionary<string, Type> nameToType = new Dictionary<string, Type>();
        private static Dictionary<Type, MemberInfo[]> typeToMembers = new Dictionary<Type, MemberInfo[]>();
        private static Dictionary<Type, TypeType> typeToTypeType = new Dictionary<Type, TypeType>();

        /// <summary>
        /// Shoulds this specific member declared inside this type be ignored?
        /// </summary>
        private static bool ShouldIgnore(Type type, MemberInfo member)
        {
            //ignore if it has a not stage saved attribute
            bool notStageSaved = member.GetCustomAttribute<NotStageSerializedAttribute>() != null;
            if (notStageSaved)
            {
                return true;
            }

            //member is deprecated
            if (member.GetCustomAttribute<ObsoleteAttribute>() != null)
            {
                return true;
            }

            if (member.MemberType == MemberTypes.Property)
            {
                if (member.Name == "name")
                {
                    if (typeof(UnityObject).IsAssignableFrom(type))
                    {
                        //redundant information
                        return true;
                    }
                }
                else if (member.Name == "isActiveAndEnabled")
                {
                    if (typeof(Behaviour).IsAssignableFrom(type))
                    {
                        //pointless
                        return true;
                    }
                }
                else if (member.Name == "tag" || member.Name == "gameObject" || member.Name == "transform")
                {
                    if (typeof(UnityComponent).IsAssignableFrom(type))
                    {
                        //redundant information
                        return true;
                    }
                }
                else if (member.Name == "material" || member.Name == "materials")
                {
                    if (typeof(Renderer).IsAssignableFrom(type))
                    {
                        //ignore these memory leakers
                        return true;
                    }
                }
            }

            return false;
        }

        private static MemberInfo[] FindMembers(Type type)
        {
            List<MemberInfo> members = new List<MemberInfo>();
            while (!type.Equals(typeof(MonoBehaviour)) && !(type is null))
            {
                MemberInfo[] allMembers = type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                int membersCount = allMembers.Length;
                for (int i = 0; i < membersCount; i++)
                {
                    MemberInfo member = allMembers[i];

                    //skip duplicates
                    if (members.Contains(member))
                    {
                        continue;
                    }

                    if (ShouldIgnore(type, member))
                    {
                        continue;
                    }

                    if (member is FieldInfo field)
                    {
                        if (field.IsPublic)
                        {
                            //if its public, then add it
                            members.Add(field);
                        }
                        else
                        {
                            //if its private but has a serialize attribute, then add it
                            bool serializeField = field.GetCustomAttribute<SerializeField>() != null;
                            if (serializeField)
                            {
                                members.Add(field);
                            }
                        }
                    }
                    else if (member is PropertyInfo property)
                    {
                        bool addProperty = false;
                        if (property.GetMethod != null && property.GetMethod.IsPublic)
                        {
                            addProperty = true;
                        }

                        if (!addProperty && property.SetMethod != null && property.SetMethod.IsPublic)
                        {
                            addProperty = true;
                        }

                        //only add properties that have a public getter or setters
                        if (addProperty)
                        {
                            members.Add(property);
                        }
                    }
                }

                if (type is null || type.BaseType is null)
                {
                    break;
                }

                type = type.BaseType;
            }

            return members.ToArray();
        }

        /// <summary>
        /// Returns all fields and properties for this type.
        /// </summary>
        public static MemberInfo[] GetMembers(Type type)
        {
            if (type != null)
            {
                if (typeToMembers.TryGetValue(type, out MemberInfo[] members))
                {
                    return members;
                }
                else
                {
                    members = FindMembers(type);
                    typeToMembers[type] = members;
                    return members;
                }
            }
            else
            {
                return null;
            }
        }

        public static TypeType GetTypeType(Type type)
        {
            if (type != null)
            {
                if (typeToTypeType.TryGetValue(type, out TypeType typeType))
                {
                    return typeType;
                }
                else
                {
                    if (type.IsArray)
                    {
                        typeType = TypeType.Array;
                    }
                    else if (type.IsEnum)
                    {
                        typeType = TypeType.Enum;
                    }
                    else if (type.IsClass)
                    {
                        typeType = TypeType.Class;
                    }
                    else if (type.IsValueType)
                    {
                        typeType = TypeType.Value;
                    }
                    else
                    {
                        typeType = TypeType.Other;
                    }

                    typeToTypeType[type] = typeType;
                    return typeType;
                }
            }
            else
            {
                return TypeType.Other;
            }
        }

        /// <summary>
        /// Returns the type with this full name.
        /// </summary>
        public static Type GetType(string assemblyQualifiedName)
        {
            if (string.IsNullOrEmpty(assemblyQualifiedName))
            {
                return null;
            }

            if (nameToType.TryGetValue(assemblyQualifiedName, out Type resultType))
            {
                return resultType;
            }
            else
            {
                resultType = Type.GetType(assemblyQualifiedName);
                if (resultType != null)
                {
                    nameToType[resultType.AssemblyQualifiedName] = resultType;
                    nameToType[resultType.FullName] = resultType;
                    nameToType[resultType.Name] = resultType;
                    return resultType;
                }
            }

            return null;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Returns the real path of the asset.
        /// Editor only.
        /// </summary>
        public static string GetAssetPath(UnityObject asset)
        {
            if (resources is null)
            {
                resources = Resources.FindObjectsOfTypeAll(typeof(UnityObject));
            }

            int resourcesLength = resources.Length;
            for (int i = resourcesLength - 1; i >= 0; i--)
            {
                ref UnityObject resourceAsset = ref resources[i];
                if (resourceAsset == asset)
                {
                    string path = AssetDatabase.GetAssetPath(resourceAsset);
                    if (!string.IsNullOrEmpty(path))
                    {
                        UnityObject[] subAssets = AssetDatabase.LoadAllAssetsAtPath(path);
                        for (int a = 0; a < subAssets.Length; a++)
                        {
                            ref UnityObject subAsset = ref subAssets[a];
                            if (subAsset == asset)
                            {
                                return $"{path}/{asset.name}";
                            }
                        }

                        return path;
                    }
                }
            }

            return null;
        }
#endif

        public static void SetActiveScene(Scene scene)
        {
            SceneManager.SetActiveScene(scene);
        }

        /// <summary>
        /// Returns a new unique ID for stages.
        /// </summary>
        public static string GetID(int? seed = null) => RandomString(16, seed);

        public static string RandomString(int length, int? seed)
        {
            if (random is null)
            {
                random = new Random();
            }

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random r = random;
            if (seed != null)
            {
                r = new Random(seed.Value);
            }

            return new string(Enumerable.Repeat(chars, length).Select(s => s[r.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Returns true if this game object is the root of the prefab.
        /// </summary>
        public static bool IsPrefab(GameObject gameObject)
        {
#if UNITY_EDITOR
            if (gameObject)
            {
                GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
                if (prefabRoot)
                {
                    GameObject correspondingObject = PrefabUtility.GetCorrespondingObjectFromSource(prefabRoot);
                    if (correspondingObject)
                    {
                        return prefabRoot == gameObject;
                    }
                }
            }
#else
            if (gameObject)
            {
                Prefab prefab = gameObject.GetComponent<Prefab>();
                if (prefab)
                {
                    return true;
                }
            }
#endif

            return false;
        }

        /// <summary>
        /// Returns the path to this prefab.
        /// </summary>
        public static string GetPrefabPath(GameObject gameObject)
        {
#if UNITY_EDITOR
            if (gameObject)
            {
                GameObject prefab = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
                if (prefab)
                {
                    prefab = PrefabUtility.GetCorrespondingObjectFromSource(prefab);
                    return ReferencesDatabase.GetPath(prefab);
                }
            }
#else
            if (gameObject)
            {
                Prefab prefab = gameObject.GetComponentInParent<Prefab>();
                if (prefab)
                {
                    return prefab.Path;
                }
            }
#endif

            return null;
        }

        /// <summary>
        /// Returns true if this game object is a child of a prefab.
        /// </summary>
        public static bool IsChildOfPrefab(GameObject gameObject)
        {
#if UNITY_EDITOR
            if (gameObject)
            {
                Transform parent = gameObject.transform.parent;
                while (parent)
                {
                    bool isRoot = PrefabUtility.IsOutermostPrefabInstanceRoot(parent.gameObject);
                    if (isRoot)
                    {
                        return true;
                    }

                    parent = parent.parent;
                }
            }
#else
            if (gameObject)
            {
                Prefab prefab = gameObject.GetComponentInParent<Prefab>();
                if (prefab)
                {
                    return prefab.gameObject != gameObject;
                }
            }
#endif

            return false;
        }
    }
}