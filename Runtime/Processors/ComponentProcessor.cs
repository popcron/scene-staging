using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Popcron.SceneStaging
{
    public abstract class ComponentProcessor
    {
        private static List<ComponentProcessor> matchingProcessors = new List<ComponentProcessor>();
        private static Dictionary<string, ComponentProcessor> typeToProcessor = new Dictionary<string, ComponentProcessor>();
        private static ReadOnlyCollection<ComponentProcessor> all;

        /// <summary>
        /// List of all component processors.
        /// </summary>
        public static ReadOnlyCollection<ComponentProcessor> All
        {
            get
            {
                if (all is null)
                {
                    FindAllProcessors();
                }

                return all;
            }
        }

        /// <summary>
        /// The map that this processor is running against.
        /// </summary>
        public Stage Stage { get; set; }

        /// <summary>
        /// Returns true if this component processor is meant to parse for this type.
        /// </summary>
        protected abstract bool IsMatch(Type type);

        /// <summary>
        /// Saves the variables from the Unity component onto the prop component.
        /// </summary>
        public abstract void SaveComponent(Component component, Object unityComponent);

        /// <summary>
        /// Loads the variables and injects them into this Unity component.
        /// </summary>
        public abstract void LoadComponent(Component component, Object unityComponent);

        /// <summary>
        /// Returns the variables from this component.
        /// </summary>
        public IList<Variable> SaveComponent(Object unityComponent)
        {
            Component temp = new Component("");
            SaveComponent(temp, unityComponent);
            return temp;
        }

        /// <summary>
        /// Returns an existing component or creates one from this game object.
        /// </summary>
        public virtual Object GetOrAddComponent(Component component, GameObject gameObject)
        {
            if (component.Type != null)
            {
                if (component.Type == typeof(GameObject))
                {
                    return gameObject;
                }

                Object unityComponent = gameObject.GetComponent(component.Type);
                if (!unityComponent)
                {
                    unityComponent = gameObject.AddComponent(component.Type);
                }

                return unityComponent;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a processor that is meant to process this kind of component.
        /// </summary>
        public static ComponentProcessor<T> Get<T>() where T : Object => Get(typeof(T)) as ComponentProcessor<T>;

        /// <summary>
        /// Returns a processor that is meant to process this kind of component.
        /// </summary>
        public static ComponentProcessor Get(Type type)
        {
            if (type is null)
            {
                return null;
            }

            string typeFullName = type.FullName;
            return Get(typeFullName);
        }

        /// <summary>
        /// Returns a processor that is meant to process this kind of component.
        /// </summary>
        public static ComponentProcessor Get(string typeFullName)
        {
            if (string.IsNullOrEmpty(typeFullName))
            {
                return null;
            }

            Type type = StageUtils.GetType(typeFullName);
            if (type is null)
            {
                Debug.LogError($"No type found for {typeFullName}");
                return null;
            }

            if (typeToProcessor.TryGetValue(typeFullName, out ComponentProcessor typeProcessor))
            {
                return typeProcessor;
            }
            else
            {
                matchingProcessors.Clear();
                int allCount = All.Count;
                for (int i = 0; i < allCount; i++)
                {
                    ComponentProcessor processor = All[i];
                    if (processor.IsMatch(type))
                    {
                        matchingProcessors.Add(processor);
                    }
                }

                int matchingCount = matchingProcessors.Count;
                if (matchingCount == 1)
                {
                    //only 1 matching so return it
                    typeProcessor = matchingProcessors[0];
                }
                else if (matchingCount > 1)
                {
                    //more than 1 found so return the first that isnt a generic processor
                    for (int i = 0; i < matchingCount; i++)
                    {
                        ComponentProcessor processor = matchingProcessors[i];
                        if (!(processor is GenericProcessor))
                        {
                            typeProcessor = processor;
                            break;
                        }
                    }
                }

                typeToProcessor[typeFullName] = typeProcessor;
                return typeProcessor;
            }
        }

        private static void FindAllProcessors()
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("Looking for all component processors");
            }

            List<ComponentProcessor> processors = new List<ComponentProcessor>();
            List<Type> processorTypes = new List<Type>();
            foreach (Type type in StageUtils.GetAllAssignableFrom<ComponentProcessor>())
            {
                if (!type.IsAbstract)
                {
                    ComponentProcessor processor = Activator.CreateInstance(type) as ComponentProcessor;
                    if (processor != null)
                    {
                        processors.Add(processor);
                        processorTypes.Add(type);
                    }
                }
            }

            //ensure the builtin processors are included
            if (!processorTypes.Contains(typeof(GenericProcessor)))
            {
                processors.Add(new GenericProcessor());
            }

            if (!processorTypes.Contains(typeof(GameObjectProcessor)))
            {
                processors.Add(new GameObjectProcessor());
            }

            if (!processorTypes.Contains(typeof(TransformProcessor)))
            {
                processors.Add(new TransformProcessor());
            }

            all = processors.AsReadOnly();
            foreach (ComponentProcessor processor in all)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.Log($"Loaded component processor {processor.GetType().Name}");
                }
            }
        }
    }

    public abstract class ComponentProcessor<T> : ComponentProcessor where T : Object
    {
        protected sealed override bool IsMatch(Type type)
        {
            if (type is null)
            {
                return false;
            }

            return type == typeof(T);
        }

        public sealed override void SaveComponent(Component component, Object unityComponent)
        {
            T c = unityComponent as T;
            if (component != null && c)
            {
                try
                {
                    Save(component, c);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        public sealed override void LoadComponent(Component component, Object unityComponent)
        {
            T c = unityComponent as T;
            if (component != null && c)
            {
                try
                {
                    Load(component, c);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        protected abstract void Save(Component mapObject, T component);
        protected abstract void Load(Component mapObject, T component);
    }
}