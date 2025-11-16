using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kumu.Kulitan.Backend
{
    public class GenericServiceConverter<T> : JsonConverter<T>
    {
        private readonly IDictionary<string, string> fieldMapping;

        public GenericServiceConverter(IDictionary<string, string> fieldMapping)
        {
            this.fieldMapping = fieldMapping;
        }

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            var jObject = new JObject();
            var objType = typeof(T);

            foreach (var field in objType.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var fieldName = field.Name;
                var fieldValue = field.GetValue(value);

                if (fieldMapping.TryGetValue(fieldName, out var newFieldName))
                {
                    fieldName = newFieldName;
                }

                if (field.FieldType.IsArray)
                {
                    var jArray = new JArray();
                    var list = field.GetValue(value) as Array;

                    foreach (var item in list)
                    {
                        var fieldObject = new JObject();

                        foreach (var itemField in item.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                        {
                            var itemFieldName = itemField.Name;
                            var itemFieldValue = itemField.GetValue(item) ?? "";

                            if (fieldMapping.TryGetValue(itemFieldName, out var newItemFieldName))
                            {
                                itemFieldName = newItemFieldName;
                            }

                            fieldObject.Add(itemFieldName, JToken.FromObject(itemFieldValue, serializer));
                        }

                        jArray.Add(fieldObject);
                    }

                    jObject.Add(fieldName, JToken.FromObject(jArray, serializer));
                }
                else
                {
                    jObject.Add(fieldName, JToken.FromObject(fieldValue, serializer));
                }
            }
            
            jObject.WriteTo(writer);
        }

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var reverseMapping = fieldMapping
                   .ToDictionary(a => a.Value, a => a.Key);

            if (objectType.IsArray)
            {
                var elementType = objectType.GetElementType();
                var obj = (object)null;

                var list = new List<object>();

                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.StartObject:
                            obj = Activator.CreateInstance(elementType);
                            list.Add(obj);
                            continue;

                        case JsonToken.PropertyName:
                            var propName = reader.Value != null ? reader.Value.ToString() : "";

                            if (!reverseMapping.TryGetValue(propName, out var fieldName))
                            {
                                fieldName = propName;
                            }

                            SetFieldValue(reader, fieldName, obj, elementType);
                            break;
                    }
                }

                var arr = Array.CreateInstance(elementType, list.Count);
                list.ToArray().CopyTo(arr, 0);
                return (T)(object)arr;
            }
            else
            {
                var elementType = objectType;
                var resultObj = Activator.CreateInstance(elementType);

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndObject)
                    {
                        break;
                    }

                    if (reader.TokenType != JsonToken.PropertyName)
                    {
                        continue;
                    }

                    var propName = reader.Value != null ? reader.Value.ToString() : "";

                    if (!reverseMapping.TryGetValue(propName, out var fieldName))
                    {
                        fieldName = propName;
                    }

                    SetFieldValue(reader, fieldName, resultObj, elementType);
                }

                return (T)resultObj;
            }
        }

        private void SetFieldValue(JsonReader reader, string fieldName, object fieldObj, Type elementType)
        {
            var fieldInfo = elementType.GetField(fieldName);

            if (fieldInfo == null)
            {
                return;
            }

            reader.Read();
            var type = Nullable.GetUnderlyingType(fieldInfo.FieldType) ?? fieldInfo.FieldType;
            var fieldValue = reader.Value != null ? Convert.ChangeType(reader.Value, type) : null;
            fieldInfo.SetValue(fieldObj, fieldValue);
        }
    }
}
