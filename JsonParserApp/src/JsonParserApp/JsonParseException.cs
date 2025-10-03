using System;

namespace JsonParserApp
{
    public class JsonParseException : Exception
    {
        public int Line { get; }
        public int Column { get; }

        public JsonParseException(string message, int line = -1, int column = -1, Exception? inner = null)
            : base($"{message}{(line >= 0 ? $" (at {line}:{column})" : "")}", inner)
        {
            Line = line;
            Column = column;
        }
    }
}
