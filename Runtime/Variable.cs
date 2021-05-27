using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Popcron.SceneStaging
{
    [Serializable]
    public struct Variable : IEquatable<Variable>
    {
        [SerializeField]
        private string name;

        [SerializeField]
        private string value;

        public string Value => value;
        public string Name => name;

        public Variable(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public override bool Equals(object obj) => obj is Variable variable && Equals(variable);
        public bool Equals(Variable other) => name == other.name && value == other.value;

        public override int GetHashCode()
        {
            int hashCode = 1477024672;
            hashCode = hashCode * -1521134295 + name.GetHashCode();
            hashCode = hashCode * -1521134295 + value.GetHashCode();
            return hashCode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Variable left, Variable right) => left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Variable left, Variable right) => !(left == right);
    }
}