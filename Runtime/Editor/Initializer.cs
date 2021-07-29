#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Original = UnityEngine.RuntimeInitializeOnLoadMethodAttribute;

namespace Popcron.SceneStaging
{
    public class Initializer
    {
        [Original(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeInitialize()
        {
            InitializeAttributes();
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            InitializeAttributes();
        }

        private static void InitializeAttributes()
        {
            TypeCache.MethodCollection methods = TypeCache.GetMethodsWithAttribute<RuntimeInitializeOnLoadMethodAttribute>();
            foreach (MethodInfo method in methods)
            {
                method.Invoke(null, null);
            }
        }
    }
}
#endif