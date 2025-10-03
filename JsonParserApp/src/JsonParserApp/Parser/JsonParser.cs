using JsonParserApp.Elements;
using JsonParserApp.Parser.Strategies;

namespace JsonParserApp.Parser
{
    public class JsonParser
    {
        public string Input { get; }
        public int Position { get; private set; }

        public JsonParser(string input)
        {
            Input = input.Trim();
            Position = 0;
        }

        public IJsonElement Parse()
        {
            SkipWhitespace();
            char c = Peek();

            IParseStrategy strategy = c switch
            {
                '{' => new ObjectParseStrategy(),
                '[' => new ArrayParseStrategy(),
                '"' => new StringParseStrategy(),
                '-' or >= '0' and <= '9' => new NumberParseStrategy(),
                _ when Input[Position..].StartsWith("true") ||
                       Input[Position..].StartsWith("false") => new BooleanParseStrategy(),
                _ when Input[Position..].StartsWith("null") => new NullParseStrategy(),
                _ => throw new Exception($"Unexpected token at {Position}")
            };

            return strategy.Parse(this);
        }

        // Helpers
        public char Peek() => Input[Position];
        public void Advance() => Position++;
        public void Expect(char c)
        {
            if (Input[Position] != c)
                throw new Exception($"Expected '{c}' at {Position}");
            Position++;
        }
        public void SkipWhitespace()
        {
            while (Position < Input.Length && char.IsWhiteSpace(Input[Position]))
                Position++;
        }
        public bool Match(string keyword)
        {
            if (Input[Position..].StartsWith(keyword))
            {
                Position += keyword.Length;
                return true;
            }
            return false;
        }
    }
}
