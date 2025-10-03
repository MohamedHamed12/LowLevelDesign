namespace JsonParserApp.Elements
{
    public class JsonString : IJsonElement
    {
        public string Value { get; }
        public JsonString(string value)
        {
            Value = value;
        }
        public string ToJson()
        {
            return "\"" + Value + "\"";
        }
    }
}