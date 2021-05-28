using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Popcron.SceneStaging
{
    public class Conversion
    {
        private static StringBuilder stringBuilder = new StringBuilder();
        private static JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            ContractResolver = new CustomResolver()
        };

        /// <summary>
        /// Convers this object to a raw string value.
        /// </summary>
        public static JToken ToJsonToken(object value)
        {
            if (value is null)
            {
                return null;
            }
            else
            {
                Type type = value.GetType();
                TypeType typeType = StageUtils.GetTypeType(type);
                if (IsCollection(type))
                {
                    JArray jsonArray = new JArray();
                    IEnumerator enumerator = (value as IEnumerable).GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        object element = enumerator.Current;
                        JToken content = ToJsonToken(element);
                        jsonArray.Add(content);
                    }

                    return jsonArray;
                }
                else
                {
                    if (value is Object unityObject)
                    {
                        string path = ReferencesDatabase.GetPath(unityObject);
                        return path;
                    }
                    else if (typeType == TypeType.Enum)
                    {
                        return value.ToString();
                    }
                    else if (typeType == TypeType.Class)
                    {
                        try
                        {
                            return JsonConvert.SerializeObject(value, settings);
                        }
                        catch
                        {
                            Debug.LogError($"error when converting {value}:{type} to string");
                            return null;
                        }
                    }
                    else
                    {
                        if (type == typeof(byte))
                        {
                            return (byte)value;
                        }
                        else if (type == typeof(sbyte))
                        {
                            return (sbyte)value;
                        }
                        else if (type == typeof(short))
                        {
                            return (short)value;
                        }
                        else if (type == typeof(ushort))
                        {
                            return (ushort)value;
                        }
                        else if (type == typeof(int))
                        {
                            return (int)value;
                        }
                        else if (type == typeof(uint))
                        {
                            return (uint)value;
                        }
                        else if (type == typeof(long))
                        {
                            return (long)value;
                        }
                        else if (type == typeof(ulong))
                        {
                            return (ulong)value;
                        }
                        else if (type == typeof(float))
                        {
                            return (float)value;
                        }
                        else if (type == typeof(double))
                        {
                            return (double)value;
                        }
                        else if (type == typeof(decimal))
                        {
                            return (decimal)value;
                        }
                        else if (type == typeof(bool))
                        {
                            return (bool)value;
                        }
                        else if (type == typeof(string))
                        {
                            return (string)value;
                        }
                        else if (value is Vector2 vector2)
                        {
                            stringBuilder.Clear();
                            stringBuilder.Append(vector2.x);
                            stringBuilder.Append(',');
                            stringBuilder.Append(vector2.y);
                            return stringBuilder.ToString();
                        }
                        else if (value is Vector3 vector3)
                        {
                            stringBuilder.Clear();
                            stringBuilder.Append(vector3.x);
                            stringBuilder.Append(',');
                            stringBuilder.Append(vector3.y);
                            stringBuilder.Append(',');
                            stringBuilder.Append(vector3.z);
                            return stringBuilder.ToString();
                        }
                        else if (value is Vector4 vector4)
                        {
                            stringBuilder.Clear();
                            stringBuilder.Append(vector4.x);
                            stringBuilder.Append(',');
                            stringBuilder.Append(vector4.y);
                            stringBuilder.Append(',');
                            stringBuilder.Append(vector4.z);
                            stringBuilder.Append(',');
                            stringBuilder.Append(vector4.w);
                            return stringBuilder.ToString();
                        }
                        else if (value is Vector2Int vector2Int)
                        {
                            stringBuilder.Clear();
                            stringBuilder.Append(vector2Int.x);
                            stringBuilder.Append(',');
                            stringBuilder.Append(vector2Int.y);
                            return stringBuilder.ToString();
                        }
                        else if (value is Vector3Int vector3Int)
                        {
                            stringBuilder.Clear();
                            stringBuilder.Append(vector3Int.x);
                            stringBuilder.Append(',');
                            stringBuilder.Append(vector3Int.y);
                            stringBuilder.Append(',');
                            stringBuilder.Append(vector3Int.z);
                            return stringBuilder.ToString();
                        }
                        else if (value is Quaternion quaternion)
                        {
                            stringBuilder.Clear();
                            stringBuilder.Append(quaternion.x);
                            stringBuilder.Append(',');
                            stringBuilder.Append(quaternion.y);
                            stringBuilder.Append(',');
                            stringBuilder.Append(quaternion.z);
                            stringBuilder.Append(',');
                            stringBuilder.Append(quaternion.w);
                            return stringBuilder.ToString();
                        }
                        else if (value is Bounds bounds)
                        {
                            stringBuilder.Clear();
                            stringBuilder.Append(ToJsonToken(bounds.min));
                            stringBuilder.Append(',');
                            stringBuilder.Append(ToJsonToken(bounds.max));
                            return stringBuilder.ToString();
                        }
                        else if (value is BoundsInt boundsInt)
                        {
                            stringBuilder.Clear();
                            stringBuilder.Append(ToJsonToken(boundsInt.min));
                            stringBuilder.Append(',');
                            stringBuilder.Append(ToJsonToken(boundsInt.max));
                            return stringBuilder.ToString();
                        }
                        else if (value is Color color)
                        {
                            stringBuilder.Clear();
                            stringBuilder.Append(color.r);
                            stringBuilder.Append(',');
                            stringBuilder.Append(color.g);
                            stringBuilder.Append(',');
                            stringBuilder.Append(color.b);
                            stringBuilder.Append(',');
                            stringBuilder.Append(color.a);
                            return stringBuilder.ToString();
                        }
                        else if (value is Color32 color32)
                        {
                            stringBuilder.Clear();
                            stringBuilder.Append(color32.r);
                            stringBuilder.Append(',');
                            stringBuilder.Append(color32.g);
                            stringBuilder.Append(',');
                            stringBuilder.Append(color32.b);
                            stringBuilder.Append(',');
                            stringBuilder.Append(color32.a);
                            return stringBuilder.ToString();
                        }
                        else if (value is LayerMask layerMask)
                        {
                            return (int)layerMask;
                        }
                        else
                        {
                            try
                            {
                                return JsonConvert.SerializeObject(value, settings);
                            }
                            catch
                            {
                                Debug.LogError($"error when converting {value}:{type} to string");
                                return value.ToString();
                            }
                        }
                    }
                }
            }
        }

        private static T[] ExtractParts<T>(string raw)
        {
            //remove garbage characters
            int length = raw.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                char c = raw[i];
                if (c == ' ' || c == '(' || c == ')')
                {
                    raw = raw.Remove(i, 1);
                }
            }

            string[] rawParts = raw.Split(',');
            T[] parts = new T[rawParts.Length];
            length = rawParts.Length;
            for (int i = 0; i < length; i++)
            {
                parts[i] = ConvertFromJson<T>(rawParts[i]);
            }

            return parts;
        }

        /// <summary>
        /// Converts this raw json string to the provided type.
        /// </summary>
        public static object ConvertFromJson(string raw, Type type)
        {
            if (type is null)
            {
                return null;
            }

            TypeType typeType = StageUtils.GetTypeType(type);
            if (typeType == TypeType.Enum)
            {
                if (int.TryParse(raw, out int index))
                {
                    Array options = Enum.GetValues(type);
                    if (index > 0 && options.Length > index)
                    {
                        return options.GetValue(index);
                    }
                }

                try
                {
                    object enumValue = Enum.Parse(type, raw, true);
                    if (enumValue is not null)
                    {
                        return enumValue;
                    }
                }
                catch { }
            }
            else
            {
                if (typeof(Object).IsAssignableFrom(type))
                {
                    return ReferencesDatabase.Get(type, raw);
                }
                else if (type == typeof(byte))
                {
                    return byte.Parse(raw);
                }
                else if (type == typeof(sbyte))
                {
                    return sbyte.Parse(raw);
                }
                else if (type == typeof(short))
                {
                    return short.Parse(raw);
                }
                else if (type == typeof(ushort))
                {
                    return ushort.Parse(raw);
                }
                else if (type == typeof(int))
                {
                    return int.Parse(raw);
                }
                else if (type == typeof(uint))
                {
                    return uint.Parse(raw);
                }
                else if (type == typeof(long))
                {
                    return long.Parse(raw);
                }
                else if (type == typeof(ulong))
                {
                    return ulong.Parse(raw);
                }
                else if (type == typeof(float))
                {
                    return float.Parse(raw);
                }
                else if (type == typeof(double))
                {
                    return double.Parse(raw);
                }
                else if (type == typeof(decimal))
                {
                    return decimal.Parse(raw);
                }
                else if (type == typeof(bool))
                {
                    return bool.Parse(raw);
                }
                else if (type == typeof(string))
                {
                    return raw;
                }
                else if (type == typeof(Vector2))
                {
                    float[] floats = ExtractParts<float>(raw);
                    return new Vector2(floats[0], floats[1]);
                }
                else if (type == typeof(Vector3))
                {
                    float[] floats = ExtractParts<float>(raw);
                    return new Vector3(floats[0], floats[1], floats[2]);
                }
                else if (type == typeof(Vector4))
                {
                    float[] floats = ExtractParts<float>(raw);
                    return new Vector4(floats[0], floats[1], floats[2], floats[3]);
                }
                else if (type == typeof(Vector2Int))
                {
                    int[] ints = ExtractParts<int>(raw);
                    return new Vector2Int(ints[0], ints[1]);
                }
                else if (type == typeof(Vector3Int))
                {
                    int[] ints = ExtractParts<int>(raw);
                    return new Vector3Int(ints[0], ints[1], ints[2]);
                }
                else if (type == typeof(Quaternion))
                {
                    float[] floats = ExtractParts<float>(raw);
                    return new Quaternion(floats[0], floats[1], floats[2], floats[3]);
                }
                else if (type == typeof(Bounds))
                {
                    float[] floats = ExtractParts<float>(raw);
                    Vector3 min = new Vector3(floats[0], floats[1], floats[2]);
                    Vector3 max = new Vector3(floats[3], floats[4], floats[5]);
                    return new Bounds((max + min) * 0.5f, max - min);
                }
                else if (type == typeof(BoundsInt))
                {
                    int[] ints = ExtractParts<int>(raw);
                    Vector3Int min = new Vector3Int(ints[0], ints[1], ints[2]);
                    Vector3Int max = new Vector3Int(ints[3], ints[4], ints[5]);
                    return new BoundsInt((max + min) / 2, max - min);
                }
                else if (type == typeof(Color))
                {
                    float[] floats = ExtractParts<float>(raw);
                    return new Color(floats[0], floats[1], floats[2], floats[3]);
                }
                else if (type == typeof(Color32))
                {
                    byte[] bytes = ExtractParts<byte>(raw);
                    return new Color32(bytes[0], bytes[1], bytes[2], bytes[3]);
                }
                else if (type == typeof(LayerMask))
                {
                    return (LayerMask)int.Parse(raw);
                }

                try
                {
                    object jsonToken = JsonConvert.DeserializeObject(raw, settings);
                    if (jsonToken is JArray jsonArray)
                    {
                        int count = jsonArray.Count;
                        if (type.IsArray)
                        {
                            Type elementType = type.GetElementType();
                            Array array = Array.CreateInstance(elementType, count);
                            for (int i = 0; i < count; i++)
                            {
                                JToken jsonElement = jsonArray[i];
                                object arrayElement = ConvertFromJson(jsonElement.ToString(), elementType);
                                array.SetValue(arrayElement, i);
                            }

                            return array;
                        }
                        else
                        {
                            Type elementType = type.GetGenericArguments()[0];
                            Type genericType = typeof(List<>);
                            Type listType = genericType.MakeGenericType(elementType);
                            IList list = Activator.CreateInstance(listType) as IList;
                            for (int i = 0; i < count; i++)
                            {
                                JToken jsonElement = jsonArray[i];
                                object arrayElement = ConvertFromJson(jsonElement.ToString(), elementType);
                                list.Add(arrayElement);
                            }

                            return list;
                        }
                    }
                    else
                    {
                        object result = JsonConvert.DeserializeObject(raw, type, settings);
                        return result;
                    }
                }
                catch
                {
                    Debug.LogError($"error converting {raw} into {type}");
                }
            }

            return default;
        }

        private static bool IsCollection(Type type)
        {
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Converts this stage to a json string.
        /// </summary>
        public static string ToJson(Stage stage, bool prettyPrint)
        {
            JObject stageJson = new JObject();
            stageJson.Add("version", stage.Version);
            stageJson.Add("id", stage.ID);
            stageJson.Add("name", stage.DisplayName);

            //fill in the props
            JArray propsArray = new JArray();
            int propsCount = stage.Props.Count;
            for (int p = 0; p < propsCount; p++)
            {
                Prop prop = stage.Props[p];
                JObject propJson = new JObject();
                propJson.Add("id", prop.ID);
                propJson.Add("parent", prop.Parent);

                //fill in the components of the prop
                JArray compArray = new JArray();
                int componentsCount = prop.Count;
                for (int c = 0; c < componentsCount; c++)
                {
                    Component comp = prop[c];
                    JObject compJson = new JObject();
                    compJson.Add("$type", comp.FullTypeName);

                    //fill in the variables for this component
                    int variablesCount = comp.Count;
                    for (int v = 0; v < variablesCount; v++)
                    {
                        Variable variable = comp[v];
                        try
                        {
                            compJson.Add(variable.Name, variable.Value);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"tried adding {variable.Name} from {comp.Type} but couldnt\n{e}");
                        }
                    }

                    compArray.Add(compJson);
                }

                propJson.Add("components", compArray);
                propsArray.Add(propJson);
            }

            stageJson.Add("props", propsArray);

            Formatting formatting = Formatting.None;
            if (prettyPrint)
            {
                formatting = Formatting.Indented;
            }

            return stageJson.ToString(formatting);
        }

        /// <summary>
        /// Converts this json string into a stage.
        /// </summary>
        public static async Task<Stage> FromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            try
            {
                using (StringReader stringReader = new StringReader(json))
                {
                    using (JsonReader reader = new JsonTextReader(stringReader))
                    {
                        JObject jobject = await JObject.LoadAsync(reader);
                        string mapId = jobject["id"]?.ToString();
                        string mapName = jobject["name"]?.ToString() ?? jobject["displayName"]?.ToString();
                        Stage map = new Stage(mapName, mapId);

                        //add the props
                        if ((jobject["props"] ?? jobject["objects"]) is JArray props)
                        {
                            foreach (JToken prop in props)
                            {
                                int propId = int.Parse(prop["id"]?.ToString() ?? "-1");
                                int propParent = int.Parse(prop["parent"]?.ToString() ?? "-1");
                                Prop propAdded = map.AddProp(propId, propParent);

                                //add the components too
                                if (prop["components"] is JArray components)
                                {
                                    for (int c = 0; c < components.Count; c++)
                                    {
                                        JToken component = components[c];
                                        JToken componentType = component["$type"];
                                        if (componentType is not null)
                                        {
                                            Component comp = new Component(componentType.ToString());
                                            JEnumerable<JProperty> properties = component.Children<JProperty>();
                                            foreach (JProperty property in properties)
                                            {
                                                if (property.Name != "$type")
                                                {
                                                    comp.Set(property.Name, property.Value?.ToString());
                                                }
                                            }

                                            propAdded.AddComponent(comp);
                                        }
                                    }
                                }
                            }
                        }

                        return map;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        /// <summary>
        /// Converts this raw json string to the provided type.
        /// </summary>
        public static T ConvertFromJson<T>(string raw) => (T)ConvertFromJson(raw, typeof(T));
    }
}