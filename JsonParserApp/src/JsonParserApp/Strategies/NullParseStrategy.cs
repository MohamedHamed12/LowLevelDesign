using JsonParserApp.Elements;
using JsonParserApp.Factory;

namespace JsonParserApp.Parser.Strategies
{
    public class NullParseStrategy : IParseStrategy
    {
        public const string NULL_KEYWORD = "null";

        public IJsonElement Parse(JsonParser parser)
        {
            if (parser.Match("null"))
                return JsonElementFactory.CreateNull();
            throw new Exception("Invalid null value");
        }
    }
}
