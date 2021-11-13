using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Omnius.Core.Utils;

internal sealed class TypedJsonHelper
{
    public static async ValueTask<T?> ReadFileAsync<T>(string path)
    {
        using var stream = new FileStream(path, FileMode.Open);
        return await ReadStreamAsync<T>(stream);
    }

    public static async ValueTask<T?> ReadStreamAsync<T>(Stream stream)
    {
        using (var streamReader = new StreamReader(stream, new UTF8Encoding(false)))
        using (var jsonTextReader = new JsonTextReader(streamReader))
        {
            var serializer = new JsonSerializer();
            serializer.MissingMemberHandling = MissingMemberHandling.Ignore;
            serializer.TypeNameHandling = TypeNameHandling.All;
            serializer.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            serializer.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter());
            serializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            serializer.ContractResolver = new CustomContractResolver();

            return serializer.Deserialize<T>(jsonTextReader);
        }
    }

    public static async ValueTask WriteFileAsync<T>(string path, T value, bool writeIndented = false)
    {
        using var stream = new FileStream(path, FileMode.Create);
        await WriteStreamAsync<T>(stream, value, writeIndented);
    }

    public static async ValueTask WriteStreamAsync<T>(Stream stream, T value, bool writeIndented = false)
    {
        using (var streamWriter = new StreamWriter(stream, new UTF8Encoding(false)))
        using (var jsonTextWriter = new JsonTextWriter(streamWriter))
        {
            var serializer = new JsonSerializer();
            serializer.Formatting = writeIndented ? Formatting.Indented : Formatting.None;
            serializer.TypeNameHandling = TypeNameHandling.All;
            serializer.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            serializer.Converters.Insert(0, new PrimitiveJsonConverter());
            serializer.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter());
            serializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            serializer.ContractResolver = new CustomContractResolver();

            serializer.Serialize(jsonTextWriter, value);
        }
    }

    private sealed class CustomContractResolver : DefaultContractResolver
    {
        protected override JsonContract CreateContract(Type objectType)
        {
            if (objectType.GetTypeInfo().GetInterfaces().Any(type => type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
            {
                return this.CreateArrayContract(objectType);
            }

            if (objectType.GetTypeInfo().CustomAttributes.Any(n => n.AttributeType == typeof(DataContractAttribute)))
            {
                var objectContract = this.CreateObjectContract(objectType);
                objectContract.DefaultCreatorNonPublic = false;
                objectContract.DefaultCreator = () => FormatterServices.GetUninitializedObject(objectType);

                return objectContract;
            }

            return base.CreateContract(objectType);
        }
    }

    // https://stackoverflow.com/questions/25007001/json-net-does-not-preserve-primitive-type-information-in-lists-or-dictionaries-o
    private sealed class PrimitiveJsonConverter : JsonConverter
    {
        public PrimitiveJsonConverter() { }

        public override bool CanRead { get { return false; } }

        public override bool CanConvert(Type objectType)
        {
            return objectType.GetTypeInfo().IsPrimitive;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            switch (serializer.TypeNameHandling)
            {
                case TypeNameHandling.All:
                    writer.WriteStartObject();
                    writer.WritePropertyName("$type", false);

                    switch (serializer.TypeNameAssemblyFormatHandling)
                    {
                        case TypeNameAssemblyFormatHandling.Full:
                            writer.WriteValue(value!.GetType().AssemblyQualifiedName);
                            break;
                        default:
                            writer.WriteValue(value!.GetType().FullName);
                            break;
                    }

                    writer.WritePropertyName("$value", false);
                    writer.WriteValue(value);
                    writer.WriteEndObject();
                    break;
                default:
                    writer.WriteValue(value);
                    break;
            }
        }
    }
}