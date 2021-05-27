using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Popcron.SceneStaging
{
    [Serializable]
    public class Component : IList<Variable>
    {
        [SerializeField]
        private string fullTypeName;

        [SerializeField]
        private List<Variable> variables = new List<Variable>(32);

        [NonSerialized]
        private Type type;

        /// <summary>
        /// The type of this component.
        /// </summary>
        public Type Type
        {
            get
            {
                if (type is null)
                {
                    type = Utils.GetType(fullTypeName);
                }

                return type;
            }
        }

        /// <summary>
        /// The full name of the component type.
        /// </summary>
        public string FullTypeName => fullTypeName;

        /// <summary>
        /// The amount of variables on this component.
        /// </summary>
        public int Count => variables.Count;
        bool ICollection<Variable>.IsReadOnly => true;
        public Variable this[int index]
        {
            get => variables[index];
            set => variables[index] = value;
        }

        public Component(string fullTypeName, IList<Variable> variables = null)
        {
            this.fullTypeName = fullTypeName;
            this.variables = new List<Variable>();

            if (variables is not null)
            {
                this.variables.AddRange(variables);
            }
        }

        /// <summary>
        /// Adds a new variable with this name and value.
        /// If a variable with this name already exists, it will just set the new value.
        /// </summary>
        public void Add(string name, object value)
        {
            if (value is string stringValue)
            {
                Add(new Variable(name, stringValue));
            }
            else
            {
                stringValue = Conversion.ToJsonToken(value)?.ToString();
                Add(new Variable(name, stringValue));
            }
        }

        /// <summary>
        /// Adds a new variable with this name and value.
        /// If a variable with this name already exists, it will just set the new value.
        /// </summary>
        public void Add(Variable variable)
        {
            int variablesCount = variables.Count;
            for (int i = variablesCount - 1; i >= 0; i--)
            {
                if (variables[i].Name == variable.Name)
                {
                    variables[i] = variable;
                    return;
                }
            }

            variables.Add(variable);
        }

        /// <summary>
        /// Returns true if this variable exists on this component.
        /// </summary>
        public bool Contains(string name) => IndexOf(name) != -1;

        public int IndexOf(string name)
        {
            int variablesCount = variables.Count;
            for (int i = variablesCount - 1; i >= 0; i--)
            {
                Variable variable = variables[i];
                if (variable.Name == name)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Clears all variables off of this component.
        /// </summary>
        public void Clear() => variables.Clear();

        /// <summary>
        /// Returns the value from a variable with this name.
        /// </summary>
        public object Get(string name, Type type)
        {
            bool isString = type == typeof(string);
            int variablesCount = variables.Count;
            for (int i = variablesCount - 1; i >= 0; i--)
            {
                Variable variable = variables[i];
                if (variable.Name == name)
                {
                    if (isString)
                    {
                        return variable.Value;
                    }
                    else
                    {
                        return Conversion.ConvertFromJson(variable.Value, type);
                    }
                }
            }

            return default;
        }

        /// <summary>
        /// Returns the value from a variable with this name.
        /// </summary>
        public T Get<T>(string name) => (T)Get(name, typeof(T));

        /// <summary>
        /// Returns the raw value from a variable with this name.
        /// </summary>
        public string GetRaw(string name)
        {
            int variablesCount = variables.Count;
            for (int i = variablesCount - 1; i >= 0; i--)
            {
                Variable variable = variables[i];
                if (variable.Name == name)
                {
                    return variable.Value;
                }
            }

            return null;
        }

        int IList<Variable>.IndexOf(Variable item) => variables.IndexOf(item);
        void IList<Variable>.Insert(int index, Variable item) => variables.Insert(index, item);
        void IList<Variable>.RemoveAt(int index) => variables.RemoveAt(index);
        bool ICollection<Variable>.Contains(Variable item) => variables.Contains(item);
        void ICollection<Variable>.CopyTo(Variable[] array, int arrayIndex) => variables.CopyTo(array, arrayIndex);
        bool ICollection<Variable>.Remove(Variable item) => variables.Remove(item);
        IEnumerator<Variable> IEnumerable<Variable>.GetEnumerator() => variables.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)variables;
    }
}