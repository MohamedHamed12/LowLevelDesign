namespace JsonParserApp.Elements
{
    public class JsonArray : IJsonElement
    {
        private readonly List<IJsonElement> _elements = new();
        public void Add(IJsonElement element) => _elements.Add(element);
        public string ToJson() => "[" + string.Join(",", _elements.Select(e => e.ToJson())) + "]";
    }
}