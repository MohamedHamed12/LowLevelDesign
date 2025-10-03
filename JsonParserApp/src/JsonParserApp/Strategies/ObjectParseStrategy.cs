using JsonParserApp.Elements;
using JsonParserApp.Factory;

namespace JsonParserApp.Parser.Strategies
{
    public class ObjectParseStrategy : IParseStrategy
    {
        public IJsonElement Parse(JsonParser parser)
        {
            var obj = new JsonObject();
            parser.Expect('{');
            parser.SkipWhitespace();

            while (parser.Peek() != '}')
            {
                var key = ((JsonString)new StringParseStrategy().Parse(parser)).Value;
                parser.SkipWhitespace();
                parser.Expect(':');
                parser.SkipWhitespace();

                var value = parser.Parse(); // recursive
                obj.Put(key, value);

                parser.SkipWhitespace();
                if (parser.Peek() == ',')
                {
                    parser.Advance();
                    parser.SkipWhitespace();
                }
            }

            parser.Expect('}');
            return obj;
        }
    }
}
