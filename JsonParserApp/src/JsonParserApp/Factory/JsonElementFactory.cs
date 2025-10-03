using JsonParserApp.Elements;

namespace JsonParserApp.Factory
{
    public static class JsonElementFactory
    {
        public static IJsonElement CreateString(string value) => new JsonString(value);
        public static IJsonElement CreateNumber(double value) => new JsonNumber(value);
        public static IJsonElement CreateBoolean(bool value) => new JsonBoolean(value);
        public static IJsonElement CreateNull() => new JsonNull();
        public static IJsonElement CreateArray() => new JsonArray();
        public static IJsonElement CreateObject() => new JsonObject();
    }
}
