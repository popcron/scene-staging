using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = System.Random;
using System.Threading.Tasks;
using System.Threading;

#if UNITY_EDITOR
using UnityEditor.Callbacks;
using UnityEditor;
#endif

namespace Popcron.SceneStaging
{
    public class Utils
    {
        private static Random random;
        private static Dictionary<string, Type> fullTypeNameToType;
        private static Dictionary<Type, FieldInfo[]> typeToFieldInfos;
        private static Dictionary<Type, TypeType> typeToTypeType;
        private static Type[] all;

        [RuntimeInitializeOnLoadMethod]
#if UNITY_EDITOR
        [DidReloadScripts]
#endif
        private static void Initialize()
        {
            if (random is null)
            {
                random = new Random();
                fullTypeNameToType = new Dictionary<string, Type>();
                typeToFieldInfos = new Dictionary<Type, FieldInfo[]>();
                typeToTypeType = new Dictionary<Type, TypeType>();
                List<Type> list = new List<Type>();
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly assembly in assemblies)
                {
                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        fullTypeNameToType[type.FullName] = type;
                        typeToFieldInfos[type] = FindFields(type).ToArray();

                        if (type.IsEnum)
                        {
                            typeToTypeType[type] = TypeType.Enum;
                        }
                        else if (type.IsClass)
                        {
                            typeToTypeType[type] = TypeType.Class;
                        }
                        else if (type.IsValueType)
                        {
                            typeToTypeType[type] = TypeType.Value;
                        }
                        else
                        {
                            typeToTypeType[type] = TypeType.Other;
                        }

                        list.Add(type);
                    }
                }

                all = list.ToArray();
            }
        }

        private static List<FieldInfo> FindFields(Type type)
        {
            List<FieldInfo> fields = new List<FieldInfo>();
            while (!type.Equals(typeof(MonoBehaviour)) && !(type is null))
            {
                FieldInfo[] allFields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                for (int i = 0; i < allFields.Length; i++)
                {
                    FieldInfo field = allFields[i];

                    //skip duplicates
                    if (fields.Contains(field))
                    {
                        continue;
                    }

                    //skip static fields
                    if (field.IsStatic)
                    {
                        continue;
                    }

                    //ignore if it has a not stage saved attribute
                    bool notStageSaved = field.GetCustomAttribute<NotStageSerializedAttribute>() is not null;
                    if (notStageSaved)
                    {
                        continue;
                    }

                    if (field.IsPublic)
                    {
                        //if its public, then add it
                        fields.Add(field);
                    }
                    else if (!field.IsPublic)
                    {
                        //if its private but has serialized attributes, then add it
                        bool serializeField = field.GetCustomAttribute<SerializeField>() is not null;
                        if (serializeField)
                        {
                            fields.Add(field);
                        }
                    }
                }

                if (type is null || type.BaseType is null)
                {
                    break;
                }

                type = type.BaseType;
            }

            return fields;
        }

        /// <summary>
        /// Returns all fields for this type.
        /// </summary>
        public static FieldInfo[] GetFields(Type type)
        {
            if (typeToFieldInfos is null)
            {
                Initialize();
            }

            if (type is not null && typeToFieldInfos.TryGetValue(type, out FieldInfo[] fields))
            {
                return fields;
            }
            else
            {
                return null;
            }
        }

        public static TypeType GetTypeType(Type type)
        {
            if (typeToTypeType is null)
            {
                Initialize();
            }

            if (typeToTypeType.TryGetValue(type, out TypeType typeType))
            {
                return typeType;
            }
            else
            {
                if (type.IsArray)
                {
                    return TypeType.Array;
                }
                else
                {
                    return TypeType.Other;
                }
            }
        }

        /// <summary>
        /// Returns the type with this full name.
        /// </summary>
        public static Type GetType(string fullTypeName)
        {
            if (string.IsNullOrEmpty(fullTypeName))
            {
                return null;
            }

            if (fullTypeNameToType is null)
            {
                Initialize();
            }

            if (fullTypeNameToType.TryGetValue(fullTypeName, out Type type))
            {
                return type;
            }
            else
            {
                return null;
            }
        }

        public static IEnumerable<Type> GetAllAssignableFrom<T>()
        {
            if (all is null)
            {
                Initialize();
            }

            Type baseType = typeof(T);
            foreach (Type type in all)
            {
                if (baseType.IsAssignableFrom(type))
                {
                    yield return type;
                }
            }
        }

        public static string RandomString(int length, int? seed)
        {
            if (random is null)
            {
                Initialize();
            }

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random r = random;
            if (seed is not null)
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