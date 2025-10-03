using JsonParserApp.Elements;
using JsonParserApp.Factory;

namespace JsonParserApp.Parser.Strategies
{
    public class ArrayParseStrategy : IParseStrategy
    {
        public IJsonElement Parse(JsonParser parser)
        {
            var arr = new JsonArray();
            parser.Expect('[');
            parser.SkipWhitespace();

            while (parser.Peek() != ']')
            {
                arr.Add(parser.Parse()); // recursive
                parser.SkipWhitespace();

                if (parser.Peek() == ',')
                {
                    parser.Advance();
                    parser.SkipWhitespace();
                }
            }

            parser.Expect(']');
            return arr;
        }
    }
}
