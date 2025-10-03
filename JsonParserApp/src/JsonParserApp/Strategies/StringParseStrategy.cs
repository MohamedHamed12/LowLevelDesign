using JsonParserApp.Elements;
using JsonParserApp.Factory;

namespace JsonParserApp.Parser.Strategies
{
    public class StringParseStrategy : IParseStrategy
    {
        public IJsonElement Parse(JsonParser parser)
        {
            parser.Expect('"');
            int start = parser.Position;
            while (parser.Peek() != '"') parser.Advance();
            string value = parser.Input[start..parser.Position];
            parser.Expect('"');
            return JsonElementFactory.CreateString(value);
        }
    }
}
