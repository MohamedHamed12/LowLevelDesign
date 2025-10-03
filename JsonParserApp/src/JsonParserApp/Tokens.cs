namespace JsonParserApp.Tokens
{
    public enum JsonTokenType
    {
        LeftBrace, RightBrace,
        LeftBracket, RightBracket,
        Colon, Comma,
        String, Number, True, False, Null,
        EOF
    }

    public readonly struct JsonToken
    {
        public JsonToken(JsonTokenType type, string? lexeme, int line, int col)
        {
            Type = type;
            Lexeme = lexeme;
            Line = line;
            Column = col;
        }
        public JsonTokenType Type { get; }
        public string? Lexeme { get; }
        public int Line { get; }
        public int Column { get; }
        public override string ToString() => $"{Type}('{Lexeme}') @{Line}:{Column}";
    }
}
