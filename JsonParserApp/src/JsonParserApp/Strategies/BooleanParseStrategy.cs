using JsonParserApp.Elements;
using JsonParserApp.Factory;

namespace JsonParserApp.Parser.Strategies
{
    public class BooleanParseStrategy : IParseStrategy
    {
        public const string TRUE_KEYWORD = "true";
        public const string FALSE_KEYWORD = "false";

        public IJsonElement Parse(JsonParser parser)
        {
            if (parser.Match("true"))
                return JsonElementFactory.CreateBoolean(true);
            if (parser.Match("false"))
                return JsonElementFactory.CreateBoolean(false);
            throw new Exception("Invalid boolean value");
        }
    }
}
