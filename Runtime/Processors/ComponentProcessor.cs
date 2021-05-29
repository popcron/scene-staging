using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor.Callbacks;
#endif

namespace Popcron.SceneStaging
{
    public abstract class ComponentProcessor
    {
        private static List<ComponentProcessor> matchingProcessors = new List<ComponentProcessor>();
        private static Dictionary<Type, ComponentProcessor> typeToProcessor = new Dictionary<Type, ComponentProcessor>();
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
                    List<ComponentProcessor> allList = new List<ComponentProcessor>();
                    foreach (Type type in StageUtils.GetAllAssignableFrom<ComponentProcessor>())
                    {
                        if (!type.IsAbstract)
                        {
                            allList.Add(Activator.CreateInstance(type) as ComponentProcessor);
                        }
                    }

                    all = allList.AsReadOnly();
                }

                return all;
            }
        }

        /// <summary>
        /// The map that this processor is running against.
        /// </summary>
        public Stage Stage { get; set; }

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
        public virtual Object GetComponent(Component component, GameObject gameObject)
        {
            if (component.Type != null)
            {
                if (component.Type.Equals(typeof(GameObject)))
                {
                    return gameObject;
                }

                Object unityComponent = gameObject.GetComponent(component.Type);
                if (unityComponent)
                {
                    return unityComponent;
                }
                else
                {
                    return gameObject.AddComponent(component.Type);
                }
            }
            else
            {
                return null;
            }
        }

#if UNITY_EDITOR
        [DidReloadScripts]
#endif
        private static void Initialize()
        {
            int allCount = All.Count;
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

            if (typeToProcessor.TryGetValue(type, out ComponentProcessor typeProcessor))
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

                typeToProcessor[type] = typeProcessor;
                return typeProcessor;
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