using System.Collections.Generic;

namespace Popcron.SceneStaging
{
    public struct PrefabInformation
    {
        /// <summary>
        /// Path to the prefab in the project.
        /// </summary>
        public string Path
        {
            get => GetRaw("path");
            set => Add("path", value);
        }

        private List<Variable> variables;

        public PrefabInformation(Component component)
        {
            variables = new List<Variable>();
            Read(component);
        }

        /// <summary>
        /// Reads the values from this component.
        /// </summary>
        public void Read(Component component)
        {
            variables.AddRange(component);
        }

        /// <summary>
        /// Writes the values into this component.
        /// </summary>
        public void Write(Component component)
        {
            int variablesCount = variables.Count;
            for (int i = 0; i < variablesCount; i++)
            {
                Variable variable = variables[i];
                component.Add(variable);
            }
        }

        private string GetRaw(string name)
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

        /// <summary>
        /// Adds a new variable with this name and value.
        /// If a variable with this name already exists, it will just set the new value.
        /// </summary>
        public void Add(string name, object value)
        {
            string stringValue = value as string;
            if (stringValue is null)
            {
                stringValue = Conversion.ToJsonToken(value)?.ToString();
            }

            int variablesCount = variables.Count;
            for (int i = variablesCount - 1; i >= 0; i--)
            {
                if (variables[i].Name == name)
                {
                    variables[i] = new Variable(name, stringValue);
                    return;
                }
            }

            variables.Add(new Variable(name, stringValue));
        }
    }
}