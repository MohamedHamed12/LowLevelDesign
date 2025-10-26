# Json Parser App

This project is a C# application for parsing JSON data. It includes a tokenizer, parser, and a set of classes to represent JSON elements like objects, arrays, strings, numbers, booleans, and null.

## Features

- Tokenizes a JSON string into a stream of tokens.
- Parses the token stream to build a tree of JSON elements.
- Supports all standard JSON data types.
- Includes a simple console application to demonstrate the parser.
- Contains a test suite to verify the parser'''s correctness.

## How to Run

1.  Navigate to the `JsonParserApp` directory.
2.  Build the solution using `dotnet build`.
3.  Run the application using `dotnet run --project src/JsonParserApp/JsonParserApp.csproj`.
