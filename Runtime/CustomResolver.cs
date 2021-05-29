using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Popcron.SceneStaging
{
    public class CustomResolver : DefaultContractResolver
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            List<JsonProperty> jsonProperties = new List<JsonProperty>();
            FieldInfo[] fields = type.GetFields(Flags);
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                bool serialize = field.IsPublic;
                if (!serialize)
                {
                    if (field.GetCustomAttribute<SerializeField>() != null)
                    {
                        serialize = true;
                    }
                }

                if (serialize)
                {
                    if (field.GetCustomAttribute<NonSerializedAttribute>() != null)
                    {
                        continue;
                    }

                    JsonProperty jsonProperty = CreateProperty(field, memberSerialization);
                    jsonProperty.Writable = true;
                    jsonProperty.Readable = true;
                    jsonProperties.Add(jsonProperty);
                }
            }

            return jsonProperties;
        }
    }
}