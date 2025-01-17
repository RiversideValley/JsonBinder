# 🧩 JSON to Classes Converter

## 🌟 Overview

The JSON to Classes Converter is a command-line utility that generates language-specific class representations from JSON input. It supports multiple programming languages and allows users to select one or more target languages for the output.

## ✨ Features

- Converts JSON into class representations for:
  - C#, Python, Java, JavaScript, TypeScript, PHP, Ruby, and Swift.
- Multi-language selection for simultaneous generation.
- User-friendly interface with detailed error handling.
- 📖 Help menu for quick guidance.

## 🛠️ Usage

### 📚 Using the Library
To use the library in your .NET 9 project, include the `NoobNotFound.Json2Any` namespace and call the static method `JsonClassConverter.ConvertTo`:

```csharp
using NoobNotFound.Json2Any;

string json = "{ \"Name\": \"John\", \"Age\": 30 }";
string result = JsonClassConverter.ConvertTo(json, Language.CSharp);
Console.WriteLine(result);
```

This method takes two parameters:
1. `json`: A string containing the JSON input.
2. `language`: An enum specifying the target language.

Supported languages include:
- `Language.CSharp`
- `Language.Python`
- `Language.Java`
- `Language.JavaScript`
- `Language.TypeScript`
- `Language.PHP`
- `Language.Ruby`
- `Language.Swift`

You can use the output directly in your projects or customize it as needed.

### 🚀 Running the Test Program

To see the library in action:

1. Compile the solution using .NET 9.
2. Run the `TestProgram` project.
3. Use the interactive menu to input JSON and select target languages.

The program will display the generated classes for the chosen languages in an organized format.

- ❌ Invalid JSON displays an error in **red** and returns to the main menu.

## 🔧 Development

### 📋 Requirements

- .NET 9.0 or later.
- Basic understanding of JSON and object-oriented programming.

### ⚖️ License

This project, including the library `NoobNotFound.Json2Any`, is licensed under the **GNU General Public License v3.0**. You are free to use, modify, and distribute the software, provided you adhere to the terms of the license.

### 📝 Note

***Entirely crafted by AI magic.***  
*GPT4o & Claude 3.5 Sonnet.*

