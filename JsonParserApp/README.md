# JsonParserApp

JsonParserApp is a custom JSON parser written in C# from scratch. It's a lightweight and efficient library for working with JSON data in .NET applications.

## Features

*   **Parse JSON strings:** Convert JSON strings into a structured, in-memory representation.
*   **Supports all JSON data types:** Handles JSON objects, arrays, strings, numbers, booleans, and null values.
*   **Extensible design:** Built with a strategy pattern, making it easy to extend with new functionalities.
*   **No external dependencies:** The parser is self-contained and does not rely on any third-party libraries.

## Getting Started

To get started with the JsonParserApp, you'll need the .NET SDK installed on your machine.

### Running the application

1.  Clone the repository:
    ```bash
    git clone <repository-url>
    ```
2.  Navigate to the project directory:
    ```bash
    cd JsonParserApp
    ```
3.  Run the application:
    ```bash
    dotnet run --project src/JsonParserApp/JsonParserApp.csproj
    ```

This will execute the `Program.cs` file, which demonstrates a simple use case of the parser.

## Usage

Here's how you can use the `JsonParser` in your own code:

```csharp
using JsonParserApp.Parser;

string json = @"{ ""name"": ""John Doe"", ""age"": 30, ""isStudent"": false, ""courses"": [""C#"", ""ASP.NET""], ""address"": null }";

var parser = new JsonParser(json);
var jsonElement = parser.Parse();

// You can now work with the parsed JSON element
Console.WriteLine(jsonElement.ToJson());
```

## Project Structure

The project is organized into the following directories:

*   **src/JsonParserApp:** The main project containing the JSON parser implementation.
    *   **Builder:** Contains the `JsonBuilder` class for constructing JSON elements.
    *   **Elements:** Defines the different types of JSON elements (e.g., `JsonObject`, `JsonArray`).
    *   **Factory:** Includes the `JsonElementFactory` for creating JSON elements.
    *   **Parser:** Contains the core `JsonParser` and `JsonTokenizer` classes.
    *   **Strategies:** Implements the strategy pattern for parsing different JSON value types.
    *   **Utils:** Provides utility classes, such as custom exceptions.
*   **src/JsonParserApp.Tests:** Contains unit tests for the parser.

## Running Tests

To run the tests, navigate to the `JsonParserApp` directory and run the following command:

```bash
dotnet test
```
