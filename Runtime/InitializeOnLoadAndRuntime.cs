using System;
using UnityEngine;
using Original = UnityEngine.RuntimeInitializeOnLoadMethodAttribute;

namespace Popcron.SceneStaging
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RuntimeInitializeOnLoadMethodAttribute : Original
    {
        public RuntimeInitializeOnLoadMethodAttribute() : base()
        {

        }

        public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType loadType) : base(loadType)
        {

        }
    }
}