namespace JsonParserApp.Elements
{
     public class JsonNumber : IJsonElement
    {
        public double Value { get; }
        public JsonNumber(double value) => Value = value;
        public string ToJson() => Value.ToString();
    }
}