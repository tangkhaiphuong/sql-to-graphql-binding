using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Contoso.Text.Json
{
    public class ObjectToInferredTypesConverter : JsonConverter<object>
    {
        public override object Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.False:
                    return false;
                case JsonTokenType.Number when reader.TryGetInt32(out int i):
                    return i;
                case JsonTokenType.Number when reader.TryGetInt64(out long l):
                    return l;
                case JsonTokenType.Number:
                    return reader.GetDouble();
                case JsonTokenType.String:
                    return reader.GetString();
                case JsonTokenType.StartArray:
                    {
                        var array = new List<object>();
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                            array.Add(Read(ref reader, typeToConvert, options));
                        return array;
                    }
                case JsonTokenType.StartObject:
                    {
                        var dictionary = new Dictionary<string, object>();
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                        {
                            if (reader.TokenType == JsonTokenType.PropertyName)
                            {
                                var propertyName = reader.GetString();
                                if (!reader.Read()) break;
                                dictionary.Add(propertyName, Read(ref reader, typeToConvert, options));
                            }
                            else
                                break;
                        }
                        return dictionary;
                    }
                default:
                    return JsonDocument.ParseValue(ref reader).RootElement.Clone();
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            object objectToWrite,
            JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, objectToWrite, objectToWrite.GetType(), options);
    }
}
