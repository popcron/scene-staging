using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Popcron.SceneStaging
{
    [Serializable]
    public class Prop : IList<Component>, IEquatable<Prop>
    {
        [SerializeField]
        private string name;

        [SerializeField]
        private string prefab;

        [SerializeField]
        private int id;

        [SerializeField]
        private int parent;

        [SerializeField]
        private List<Component> components = new List<Component>();

        [NonSerialized]
        private string toString;

        public int ID => id;
        public ref int Parent => ref parent;
        public string Name => name;
        public string Prefab => prefab;
        public GameObject GameObject { get; set; }

        public Transform Transform
        {
            get
            {
                if (GameObject)
                {
                    return GameObject.transform;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The amount of components on this prop.
        /// </summary>
        public int Count => components.Count;
        bool ICollection<Component>.IsReadOnly => true;
        public Component this[int index]
        {
            get => components[index];
            set => components[index] = value;
        }

        public override string ToString()
        {
            if (toString is null)
            {
                toString = $"{name}:{id}";
            }

            return toString;
        }

        public Prop GetParent(Stage stage)
        {
            if (parent == -1)
            {
                return null;
            }

            int propsCount = stage.Props.Count;
            for (int i = propsCount - 1; i >= 0; i--)
            {
                Prop prop = stage.Props[i];
                if (prop != this && prop.IsParentOf(this, stage))
                {
                    return prop;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the children of this prop.
        /// </summary>
        public List<Prop> GetChildren(Stage stage)
        {
            List<Prop> children = new List<Prop>();
            int propsCount = stage.Props.Count;
            for (int i = propsCount - 1; i >= 0; i--)
            {
                Prop prop = stage.Props[i];
                if (prop != this && prop.IsChildOf(this, stage))
                {
                    children.Add(prop);
                }
            }

            return children;
        }

        /// <summary>
        /// Is this prop a child of this other prop?
        /// </summary>
        public bool IsChildOf(Prop parentProp, Stage stage)
        {
            int p = parent;
            while (p != -1)
            {
                Prop potentialParent = stage.GetProp(p);
                if (potentialParent == null)
                {
                    return false;
                }
                else if (potentialParent == parentProp)
                {
                    return true;
                }
                else
                {
                    p = potentialParent.parent;
                }
            }

            return false;
        }

        /// <summary>
        /// Is this prop parent of this other prop?
        /// </summary>
        public bool IsParentOf(Prop childProp, Stage stage) => childProp.IsChildOf(this, stage);

        public Prop(GameObject gameObject, string prefab, int id, int parent) : this(gameObject.name, prefab, id, parent)
        {
            GameObject = gameObject;
        }

        public Prop(string name, string prefab, int id, int parent)
        {
            this.name = name;
            this.prefab = prefab;
            this.id = id;
            this.parent = parent;
            components = new List<Component>();
        }

        public Component AddComponent(string fullTypeName, IList<Variable> variables = null)
        {
            Component newComponent = new Component(fullTypeName, variables);
            components.Add(newComponent);
            return newComponent;
        }

        public void AddComponent(Component component)
        {
            components.Add(component);
        }

        /// <summary>
        /// Returns all component objects of this type within this prop.
        /// </summary>
        public List<Component> GetComponent<T>() where T : UnityEngine.Component
        {
            List<Component> result = new List<Component>();
            Type type = typeof(T);
            foreach (Component component in components)
            {
                if (type.IsAssignableFrom(component.Type))
                {
                    result.Add(component);
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Removes all components off this prop.
        /// </summary>
        public void Clear() => components.Clear();

        int IList<Component>.IndexOf(Component component) => components.IndexOf(component);
        void IList<Component>.Insert(int index, Component component) => components.Insert(index, component);
        void IList<Component>.RemoveAt(int index) => components.RemoveAt(index);
        void ICollection<Component>.Add(Component component) => components.Add(component);
        public bool Contains(Component component) => components.Contains(component);
        public void CopyTo(Component[] array, int arrayIndex) => components.CopyTo(array, arrayIndex);
        public bool Remove(Component component) => components.Remove(component);
        public IEnumerator<Component> GetEnumerator() => components.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => components.GetEnumerator();

        public override bool Equals(object obj) => Equals(obj as Prop);
        public bool Equals(Prop other) => other?.GetHashCode() == GetHashCode();

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = -271854945;
                hashCode = hashCode * -1521134295 + name.GetHashCode();
                hashCode = hashCode * -1521134295 + prefab.GetHashCode();
                hashCode = hashCode * -1521134295 + id.GetHashCode();
                hashCode = hashCode * -1521134295 + parent.GetHashCode();

                int componentsCount = components.Count;
                for (int i = componentsCount - 1; i >= 0; i--)
                {
                    Component comp = components[i];
                    hashCode = hashCode * 31 + comp.GetHashCode();
                }

                return hashCode;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Prop left, Prop right) => EqualityComparer<Prop>.Default.Equals(left, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Prop left, Prop right) => !(left == right);
    }
}