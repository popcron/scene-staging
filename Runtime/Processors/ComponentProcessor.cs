using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Popcron.SceneStaging
{
    public abstract class ComponentProcessor
    {
        private static List<ComponentProcessor> matchingProcessors = new List<ComponentProcessor>();
        private static List<ComponentProcessor> all = new List<ComponentProcessor>();

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

            string typeFullName = type.AssemblyQualifiedName;
            return Get(typeFullName);
        }

        /// <summary>
        /// Returns a processor that is meant to process this kind of component.
        /// </summary>
        public static ComponentProcessor Get(string assemblyQualifiedTypeName)
        {
            if (string.IsNullOrEmpty(assemblyQualifiedTypeName))
            {
                return null;
            }

            if (all.Count == 0)
            {
                LoadDefaultProcessors();
            }

            matchingProcessors.Clear();
            int allCount = all.Count;
            Type type = StageUtils.GetType(assemblyQualifiedTypeName);
            for (int i = 0; i < allCount; i++)
            {
                ComponentProcessor processor = all[i];
                if (processor.IsMatch(type))
                {
                    matchingProcessors.Add(processor);
                }
            }

            ComponentProcessor typeProcessor = null;
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

            return typeProcessor;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void LoadDefaultProcessors()
        {
            //ensure the builtin processors are included
            if (!IsRegistered<GenericProcessor>())
            {
                RegisterProcessor<GenericProcessor>();
            }

            if (!IsRegistered<GameObjectProcessor>())
            {
                RegisterProcessor<GameObjectProcessor>();
            }

            if (!IsRegistered<TransformProcessor>())
            {
                RegisterProcessor<TransformProcessor>();
            }
        }

        /// <summary>
        /// Returns true if this type of component processor is already registered.
        /// </summary>
        public static bool IsRegistered(Type type)
        {
            if (all is null)
            {
                return false;
            }

            int allCount = all.Count;
            for (int i = 0; i < allCount; i++)
            {
                ComponentProcessor existingProcessor = all[i];
                if (existingProcessor != null && existingProcessor.GetType() == type)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if this type of component processor is already registered.
        /// </summary>
        public static bool IsRegistered<T>() where T : ComponentProcessor => IsRegistered(typeof(T));

        /// <summary>
        /// Registers this component processor to a global static collection.
        /// </summary>
        public static void RegisterProcessor(Type type)
        {
            if (!IsRegistered(type))
            {
                if (all is null)
                {
                    all = new List<ComponentProcessor>();
                }

                ComponentProcessor processor = Activator.CreateInstance(type) as ComponentProcessor;
                all.Add(processor);
            }
        }

        /// <summary>
        /// Registers this component processor to a global static collection.
        /// </summary>
        public static void RegisterProcessor<T>() where T : ComponentProcessor => RegisterProcessor(typeof(T));
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