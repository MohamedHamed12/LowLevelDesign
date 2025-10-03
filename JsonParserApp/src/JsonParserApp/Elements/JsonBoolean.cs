namespace JsonParserApp.Elements
{
       public class JsonBoolean : IJsonElement
    {
        public bool Value { get; }
        public JsonBoolean(bool value) => Value = value;
        public string ToJson() => Value.ToString().ToLower();
    }
}