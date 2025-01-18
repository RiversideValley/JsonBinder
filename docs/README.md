# 🧩  `Riverside.JsonBinder`

`Riverside.JsonBinder` is a .NET based library that generates language-specific class representations from JSON input. It supports multiple programming languages.

## ✨ Features

- Converts JSON into class representations for:
  - C#, Python, Java, JavaScript, TypeScript, PHP, Ruby, and Swift.
- Multi-language selection for simultaneous generation.
- User-friendly interface with detailed error handling.

## 🛠️ Usage

### 📚 Using the Library

To use the library in your .NET project, include the `Riverside.JsonBinder` namespace and call the static method `JsonSerializer.ConvertTo`:

```csharp
using Riverside.JsonBinder;

string json = "{ \"Name\": \"John\", \"Age\": 30 }";
string result = JsonSerializer.ConvertTo(json, SerializableLanguage.CSharp);
Console.WriteLine(result);
```

This method takes two parameters:

1. `json`: A string containing the JSON input.
2. `language`: An enum specifying the target SerializableLanguage

Supported languages include:

- `SerializableLanguage.CSharp`
- `SerializableLanguage.Python`
- `SerializableLanguage.Java`
- `SerializableLanguage.JavaScript`
- `SerializableLanguage.TypeScript`
- `SerializableLanguage.PHP`
- `SerializableLanguage.Ruby`
- `SerializableLanguage.Swift`

You can use the output directly in your projects or customize it as needed.

## 🚀 Running the Test Program

To see the library in action:

1. Compile the solution using your chosen TFM.
2. Run the `Riverside.JsonBinder.Console` project.
3. Use the interactive menu to input JSON and select target languages.

The program will display the generated classes for the chosen languages in an organized format.

- ❌ Invalid JSON displays an error in **red** and returns to the main menu.

### 📋 Requirements

- Any version of .NET
- Basic understanding of JSON and object-oriented programming.

## ⚖️ License

This project, including the library `Riverside.JsonBinder`, is licensed under the **MIT License**. You are free to use, modify, and distribute the software, provided you adhere to the terms of the license.

---
  
***Crafted with the help of AI magic.***  
*GPT4o & Claude 3.5 Sonnet.*
