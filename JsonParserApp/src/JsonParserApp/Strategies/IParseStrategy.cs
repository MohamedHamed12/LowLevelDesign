using JsonParserApp.Elements;

namespace JsonParserApp.Parser.Strategies
{
    public interface IParseStrategy
    {
        IJsonElement Parse(JsonParser parser);
    }
}
