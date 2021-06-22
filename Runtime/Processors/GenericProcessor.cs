using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityComponent = UnityEngine.Component;

namespace Popcron.SceneStaging
{
    public class GenericProcessor : ComponentProcessor
    {
        private static StringBuilder stringBuilder = new StringBuilder();

        protected override bool IsMatch(Type type)
        {
            return typeof(UnityComponent).IsAssignableFrom(type);
        }

        public override void SaveComponent(Component component, Object unityObject)
        {
            if (unityObject is UnityComponent unityComponent)
            {
                MemberInfo[] members = StageUtils.GetMembers(unityComponent.GetType());
                int membersLength = members.Length;
                for (int i = 0; i < membersLength; i++)
                {
                    MemberInfo member = members[i];
                    FieldInfo field = member as FieldInfo;
                    PropertyInfo property = member as PropertyInfo;
                    Type fieldType = null;
                    object value = null;
                    if (field != null)
                    {
                        fieldType = field.FieldType;
                        value = field.GetValue(unityComponent);
                    }
                    else if (property != null)
                    {
                        if (property.GetMethod is null)
                        {
                            continue;
                        }

                        fieldType = property.PropertyType;
                        value = property.GetValue(unityComponent);
                    }

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
                        component.Set(member.Name, stringBuilder.ToString());
                    }
                    else
                    {
                        if (typeof(Transform).Equals(fieldType))
                        {
                            if (value is Transform transform && transform)
                            {
                                int? id = Stage.GetProp(transform)?.ID;
                                component.Set(member.Name, id?.ToString() ?? "");
                            }
                            else
                            {
                                component.Set(member.Name, "");
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
                                    component.Set(member.Name, stringBuilder.ToString());
                                }
                                else
                                {
                                    component.Set(member.Name, "");
                                }
                            }
                            else
                            {
                                component.Set(member.Name, "");
                            }
                        }
                        else
                        {
                            component.Set(member.Name, value);
                        }
                    }
                }
            }
        }

        private static async void AssignEventually(FieldInfo field, PropertyInfo property, UnityComponent unityComponent, GameObject gameObject, int index, int timeout = 500)
        {
            int frame = 0;
            while (frame < timeout)
            {
                Behaviour[] behaviours = gameObject.GetComponents<Behaviour>();
                if (behaviours.Length > 0 && index < behaviours.Length)
                {
                    SetValue(field, property, unityComponent, behaviours[index]);
                    return;
                }

                await Task.Delay(1);
                frame++;
            }
        }

        public override void LoadComponent(Component component, Object unityObject)
        {
            if (unityObject is UnityComponent unityComponent)
            {
                MemberInfo[] members = StageUtils.GetMembers(unityComponent.GetType());
                int membersLength = members.Length;
                for (int i = membersLength - 1; i >= 0; i--)
                {
                    MemberInfo member = members[i];
                    string name = member.Name;
                    FieldInfo field = member as FieldInfo;
                    PropertyInfo property = member as PropertyInfo;
                    Type fieldType = null;
                    if (field != null)
                    {
                        fieldType = field.FieldType;
                    }
                    else if (property != null)
                    {
                        if (property.SetMethod is null)
                        {
                            continue;
                        }

                        fieldType = property.PropertyType;
                    }

                    if (typeof(Object).IsAssignableFrom(fieldType))
                    {
                        if (component.Contains(name))
                        {
                            string value = component.GetRaw(name);
                            if (int.TryParse(value, out int transformId))
                            {
                                Prop prop = Stage.GetProp(transformId);
                                SetValue(field, property, unityComponent, prop.Transform);
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
                                            SetValue(field, property, unityComponent, prefab);
                                        }
                                        else
                                        {
                                            Debug.LogError($"couldnt find type with name {fullTypeName} when assigning a value to field {name}");
                                            SetValue(field, property, unityComponent, null);
                                        }
                                    }
                                    else
                                    {
                                        int id = Conversion.ConvertFromJson<int>(splits[0]);
                                        int index = Conversion.ConvertFromJson<int>(splits[1]);
                                        Prop prop = Stage.GetProp(id);
                                        AssignEventually(field, property, unityComponent, prop.GameObject, index);
                                    }
                                }
                                else
                                {
                                    SetValue(field, property, unityComponent, null);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (component.Contains(name))
                        {
                            object value = component.Get(name, fieldType);
                            SetValue(field, property, unityComponent, value);
                        }
                    }
                }
            }
        }

        private static void SetValue(FieldInfo field, PropertyInfo property, object obj, object value)
        {
            if (field != null)
            {
                field.SetValue(obj, value);
            }
            else if (property != null)
            {
                property.SetValue(obj, value);
            }
        }
    }
}