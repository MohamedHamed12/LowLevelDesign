using JsonParserApp.Elements;
using JsonParserApp.Factory;

namespace JsonParserApp.Parser.Strategies
{
    public class NumberParseStrategy : IParseStrategy
    {
        public IJsonElement Parse(JsonParser parser)
        {
            int start = parser.Position;
            while (parser.Position < parser.Input.Length &&
                   (char.IsDigit(parser.Input[parser.Position]) ||
                    parser.Input[parser.Position] is '.' or '-'))
            {
                parser.Advance();
            }
            double value = double.Parse(parser.Input[start..parser.Position]);
            return JsonElementFactory.CreateNumber(value);
        }
    }
}
