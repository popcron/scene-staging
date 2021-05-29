using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Popcron.SceneStaging
{
    [Serializable]
    public class Prop : IList<Component>, IEquatable<Prop>
    {
        [SerializeField]
        private int id;

        [SerializeField]
        private int parent;

        [SerializeField]
        private List<Component> components = new List<Component>();

        [NonSerialized]
        private string toString;

        /// <summary>
        /// The unique ID of this prop.
        /// </summary>
        public int ID => id;

        /// <summary>
        /// The ID of the prop that is this prop's parent.
        /// </summary>
        public ref int Parent => ref parent;

        /// <summary>
        /// Name of this prop as it would be on a game object.
        /// </summary>
        public string Name
        {
            get
            {
                Component component = GetComponent<GameObject>();
                return component.GetRaw("name");
            }
        }

        /// <summary>
        /// The game object that represents this prop.
        /// This property != serialized so do not rely on it during play mode in editor!
        /// </summary>
        public GameObject GameObject { get; set; }

        /// <summary>
        /// The transform that represents this prop.
        /// This property != serialized so do not rely on it during play mode in editor!
        /// </summary>
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

        /// <summary>
        /// The component that represents the prefab revelant information.
        /// </summary>
        public PrefabInformation Prefab
        {
            get
            {
                Component component = GetOrAddComponent("$prefab");
                PrefabInformation prefabInformation = new PrefabInformation(component);
                return prefabInformation;
            }
            set
            {
                Component component = GetOrAddComponent("$prefab");
                component.Clear();
                value.Write(component);
            }
        }

        bool ICollection<Component>.IsReadOnly => true;

        public Component this[int index]
        {
            get => components[index];
            set => components[index] = value;
        }

        public Prop(GameObject gameObject, int id, int parent = -1) : this(id, parent)
        {
            GameObject = gameObject;
        }

        public Prop(int id, int parent = -1)
        {
            this.id = id;
            this.parent = parent;
            components = new List<Component>();
        }

        public override string ToString()
        {
            if (toString is null)
            {
                toString = $"{Name}:{id}";
            }

            return toString;
        }

        /// <summary>
        /// Returns the parent of this prop.
        /// </summary>
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
                if (potentialParent is null)
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

        /// <summary>
        /// Adds a component of this type as a string to the prop.
        /// </summary>
        public Component AddComponent(string fullTypeName, IList<Variable> variables = null)
        {
            Component newComponent = new Component(fullTypeName, variables);
            AddComponent(newComponent);
            return newComponent;
        }

        /// <summary>
        /// Adds a component of this type to the prop.
        /// </summary>
        public Component AddComponent<T>(IList<Variable> variables = null) where T : Object => AddComponent(typeof(T).FullName, variables);

        /// <summary>
        /// Adds this component to the prop.
        /// </summary>
        public void AddComponent(Component component) => components.Add(component);

        public Component GetOrAddComponent(string fullTypeName)
        {
            int componentCount = components.Count;
            for (int i = 0; i < componentCount; i++)
            {
                Component component = components[i];
                if (component.FullTypeName == fullTypeName)
                {
                    return component;
                }
            }

            return AddComponent(fullTypeName);
        }

        /// <summary>
        /// Returns the first component of this type within this prop.
        /// </summary>
        public Component GetComponent<T>() where T : Object
        {
            Type type = typeof(T);
            int componentCount = components.Count;
            for (int i = 0; i < componentCount; i++)
            {
                Component component = components[i];
                if (type.IsAssignableFrom(component.Type))
                {
                    return component;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns all component objects of this type within this prop.
        /// </summary>
        public List<Component> GetComponents<T>() where T : Object
        {
            List<Component> result = new List<Component>();
            Type type = typeof(T);
            int componentCount = components.Count;
            for (int i = 0; i < componentCount; i++)
            {
                Component component = components[i];
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