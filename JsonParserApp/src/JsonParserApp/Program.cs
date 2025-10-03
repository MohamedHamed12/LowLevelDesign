using JsonParserApp.Parser;

namespace JsonParserApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string json = "{ \"name\": \"Mohamed\", \"age\": 25, \"skills\": [\"C#\", \"Django\"] }";

            var parser = new JsonParser(json);
            var result = parser.Parse();

            Console.WriteLine(result.ToJson());
        }
    }
}
