namespace JsonParserApp.Elements
{
    public class JsonNull : IJsonElement
    {
        public string ToJson() => "null";
    }
}