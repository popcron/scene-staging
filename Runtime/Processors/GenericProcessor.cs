using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Popcron.SceneStaging
{
    public class GenericProcessor : ComponentProcessor
    {
        private static StringBuilder stringBuilder = new StringBuilder();

        protected override bool IsMatch(Type type)
        {
            return typeof(MonoBehaviour).IsAssignableFrom(type);
        }

        public override void SaveComponent(Component component, Object unityComponent)
        {
            if (unityComponent is MonoBehaviour monoBehaviour)
            {
                FieldInfo[] fields = StageUtils.GetFields(monoBehaviour.GetType());
                int fieldsLength = fields.Length;
                for (int i = 0; i < fieldsLength; i++)
                {
                    FieldInfo field = fields[i];
                    Type fieldType = field.FieldType;
                    object value = field.GetValue(monoBehaviour);
                    string pathToPrefab = null;
                    if (value is Object valueObject)
                    {
                        pathToPrefab = ReferencesDatabase.GetPath(valueObject);
                    }

                    if (!string.IsNullOrEmpty(pathToPrefab))
                    {
                        string fullTypeName = fieldType.FullName;
                        stringBuilder.Clear();
                        stringBuilder.Append("prefab:");
                        stringBuilder.Append(pathToPrefab);
                        stringBuilder.Append(':');
                        stringBuilder.Append(fullTypeName);
                        component.Set(field.Name, stringBuilder.ToString());
                    }
                    else
                    {
                        if (typeof(Transform).Equals(fieldType))
                        {
                            if (value is Transform transform && transform)
                            {
                                int? id = Stage.GetProp(transform)?.ID;
                                component.Set(field.Name, id?.ToString() ?? "");
                            }
                            else
                            {
                                component.Set(field.Name, "");
                            }
                        }
                        else if (typeof(Behaviour).IsAssignableFrom(fieldType))
                        {
                            if (value is Behaviour behaviour && behaviour)
                            {
                                Prop prop = Stage.GetProp(behaviour.gameObject);
                                if (prop != null)
                                {
                                    int id = prop.ID;
                                    Behaviour[] components = behaviour.gameObject.GetComponents<Behaviour>();
                                    int componentIndex = Array.IndexOf(components, behaviour);
                                    stringBuilder.Clear();
                                    stringBuilder.Append(id);
                                    stringBuilder.Append(':');
                                    stringBuilder.Append(componentIndex);
                                    component.Set(field.Name, stringBuilder.ToString());
                                }
                                else
                                {
                                    component.Set(field.Name, "");
                                }
                            }
                            else
                            {
                                component.Set(field.Name, "");
                            }
                        }
                        else
                        {
                            component.Set(field.Name, value);
                        }
                    }
                }
            }
        }

        private async void AssignEventually(FieldInfo field, MonoBehaviour mb, GameObject gameObject, int index, int timeout = 500)
        {
            int frame = 0;
            while (frame < timeout)
            {
                Behaviour[] behaviours = gameObject.GetComponents<Behaviour>();
                if (behaviours.Length > 0 && index < behaviours.Length)
                {
                    field.SetValue(mb, behaviours[index]);
                    return;
                }

                await Task.Delay(1);
                frame++;
            }
        }

        public override void LoadComponent(Component component, Object unityComponent)
        {
            if (unityComponent is MonoBehaviour mb)
            {
                FieldInfo[] fields = StageUtils.GetFields(mb.GetType());
                int fieldsLength = fields.Length;
                for (int i = fieldsLength - 1; i >= 0; i--)
                {
                    FieldInfo field = fields[i];
                    Type fieldType = field.FieldType;
                    if (typeof(Object).IsAssignableFrom(fieldType))
                    {
                        if (component.Contains(field.Name))
                        {
                            string value = component.GetRaw(field.Name);
                            if (int.TryParse(value, out int transformId))
                            {
                                Prop prop = Stage.GetProp(transformId);
                                field.SetValue(mb, prop.Transform);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(value))
                                {
                                    string[] splits = value.Split(':');
                                    if (splits[0] == "prefab")
                                    {
                                        string pathToPrefab = splits[1];
                                        string fullTypeName = splits[2];
                                        Type type = StageUtils.GetType(fullTypeName);
                                        if (type != null)
                                        {
                                            Object prefab = ReferencesDatabase.Get(type, pathToPrefab);
                                            field.SetValue(mb, prefab);
                                        }
                                        else
                                        {
                                            Debug.LogError($"couldnt find type with name {fullTypeName} when assigning a value to field {field.Name}");
                                            field.SetValue(mb, null);
                                        }
                                    }
                                    else
                                    {
                                        int id = Conversion.ConvertFromJson<int>(splits[0]);
                                        int index = Conversion.ConvertFromJson<int>(splits[1]);
                                        Prop prop = Stage.GetProp(id);
                                        AssignEventually(field, mb, prop.GameObject, index);
                                    }
                                }
                                else
                                {
                                    field.SetValue(mb, null);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (component.Contains(field.Name))
                        {
                            object value = component.Get(field.Name, fieldType);
                            field.SetValue(mb, value);
                        }
                    }
                }
            }
        }
    }
}