using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Omnius.Core.Base.Serialization;

public record TypedJsonSerializerOptions
{
    public bool WriteIndented { get; set; } = false;
}

public sealed class TypedJsonHelper
{
    public static T? ReadFile<T>(string path, TypedJsonSerializerOptions? options = null)
    {
        using var stream = new FileStream(path, FileMode.Open);
        return ReadStream<T>(stream, options);
    }

    public static T? ReadStream<T>(Stream stream, TypedJsonSerializerOptions? options = null)
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

    public static void WriteFile<T>(string path, T value, TypedJsonSerializerOptions? options = null)
    {
        using var stream = new FileStream(path, FileMode.Create);
        WriteStream<T>(stream, value, options);
    }

    public static void WriteStream<T>(Stream stream, T value, TypedJsonSerializerOptions? options = null)
    {
        using (var streamWriter = new StreamWriter(stream, new UTF8Encoding(false)))
        using (var jsonTextWriter = new JsonTextWriter(streamWriter))
        {
            var serializer = new JsonSerializer();
            serializer.Formatting = options?.WriteIndented ?? false ? Formatting.Indented : Formatting.None;
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
                objectContract.DefaultCreator = () => RuntimeHelpers.GetUninitializedObject(objectType);

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
