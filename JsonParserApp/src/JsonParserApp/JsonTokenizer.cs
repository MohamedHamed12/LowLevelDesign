using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JsonParserApp.Tokens;
namespace JsonParserApp
{
    // Tokenizer reads from a TextReader and yields JsonToken instances.
    public sealed class JsonTokenizer
    {
        private readonly TextReader _reader;
        private int _line = 1;
        private int _col = 1;
        private int _peek = -2; // -2 = not loaded; -1 = EOF

        public JsonTokenizer(string input) : this(new StringReader(input)) { }
        public JsonTokenizer(TextReader reader) { _reader = reader ?? throw new ArgumentNullException(nameof(reader)); }

        private int Peek()
        {
            if (_peek == -2) _peek = _reader.Read();
            return _peek;
        }
        private int Read()
        {
            int r = Peek();
            _peek = -2;
            if (r == -1) return -1;
            if (r == '\n') { _line++; _col = 1; } else { _col++; }
            return r;
        }

        private void SkipWhitespace()
        {
            while (true)
            {
                int c = Peek();
                if (c == -1) return;
                if (!char.IsWhiteSpace((char)c)) return;
                Read();
            }
        }

        public IEnumerable<JsonToken> Tokenize()
        {
            while (true)
            {
                SkipWhitespace();
                int c = Peek();
                int line = _line, col = _col;
                if (c == -1) { yield return new JsonToken(JsonTokenType.EOF, null, line, col); yield break; }

                switch ((char)c)
                {
                    case '{': Read(); yield return new JsonToken(JsonTokenType.LeftBrace, "{", line, col); break;
                    case '}': Read(); yield return new JsonToken(JsonTokenType.RightBrace, "}", line, col); break;
                    case '[': Read(); yield return new JsonToken(JsonTokenType.LeftBracket, "[", line, col); break;
                    case ']': Read(); yield return new JsonToken(JsonTokenType.RightBracket, "]", line, col); break;
                    case ':': Read(); yield return new JsonToken(JsonTokenType.Colon, ":", line, col); break;
                    case ',': Read(); yield return new JsonToken(JsonTokenType.Comma, ",", line, col); break;
                    case '"': yield return ReadString(line, col); break;
                    default:
                        if (c == '-' || char.IsDigit((char)c)) yield return ReadNumber(line, col);
                        else if (char.IsLetter((char)c)) yield return ReadLiteral(line, col);
                        else throw new JsonParseException($"Unexpected character '{(char)c}'", line, col);
                        break;
                }
            }
        }

        private JsonToken ReadString(int startLine, int startCol)
        {
            // consume opening quote
            Read();
            var sb = new StringBuilder();
            while (true)
            {
                int rc = Read();
                if (rc == -1) throw new JsonParseException("Unterminated string", startLine, startCol);
                char ch = (char)rc;
                if (ch == '"') break;
                if (ch == '\\')
                {
                    int esc = Read();
                    if (esc == -1) throw new JsonParseException("Unterminated escape", _line, _col);
                    switch ((char)esc)
                    {
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'u':
                            // read 4 hex digits
                            var hex = new char[4];
                            for (int i = 0; i < 4; i++)
                            {
                                int h = Read();
                                if (h == -1) throw new JsonParseException("Unterminated unicode escape", _line, _col);
                                hex[i] = (char)h;
                            }
                            string hexs = new string(hex);
                            if (!int.TryParse(hexs, System.Globalization.NumberStyles.HexNumber, null, out int code))
                                throw new JsonParseException($"Invalid unicode escape \\u{hexs}", startLine, startCol);
                            sb.Append(char.ConvertFromUtf32(code));
                            break;
                        default:
                            throw new JsonParseException($"Invalid escape sequence '\\{(char)esc}'", _line, _col);
                    }
                }
                else
                {
                    sb.Append(ch);
                }
            }
            return new JsonToken(JsonTokenType.String, sb.ToString(), startLine, startCol);
        }

        private JsonToken ReadNumber(int startLine, int startCol)
        {
            var sb = new StringBuilder();
            int c = Peek();
            if (c == '-') sb.Append((char)Read());
            // integer part
            if (!char.IsDigit((char)Peek())) throw new JsonParseException("Invalid number", startLine, startCol);
            while (char.IsDigit((char)Peek())) sb.Append((char)Read());
            // fraction
            if (Peek() == '.')
            {
                sb.Append((char)Read());
                if (!char.IsDigit((char)Peek())) throw new JsonParseException("Invalid number fraction", startLine, startCol);
                while (char.IsDigit((char)Peek())) sb.Append((char)Read());
            }
            // exponent
            if (Peek() == 'e' || Peek() == 'E')
            {
                sb.Append((char)Read());
                if (Peek() == '+' || Peek() == '-') sb.Append((char)Read());
                if (!char.IsDigit((char)Peek())) throw new JsonParseException("Invalid exponent in number", startLine, startCol);
                while (char.IsDigit((char)Peek())) sb.Append((char)Read());
            }
            return new JsonToken(JsonTokenType.Number, sb.ToString(), startLine, startCol);
        }

        private JsonToken ReadLiteral(int startLine, int startCol)
        {
            var sb = new StringBuilder();
            while (char.IsLetter((char)Peek())) sb.Append((char)Read());
            string s = sb.ToString();
            return s switch
            {
                "true" => new JsonToken(JsonTokenType.True, s, startLine, startCol),
                "false" => new JsonToken(JsonTokenType.False, s, startLine, startCol),
                "null" => new JsonToken(JsonTokenType.Null, s, startLine, startCol),
                _ => throw new JsonParseException($"Unexpected literal '{s}'", startLine, startCol)
            };
        }
    }
}
