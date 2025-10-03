using JsonParserApp.Elements;

namespace JsonParserApp
{
    public interface IJsonDeserializer<T>
    {
        T Deserialize(IJsonElement element);
    }
}
