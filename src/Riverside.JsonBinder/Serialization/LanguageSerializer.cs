using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace Riverside.JsonBinder.Serialization;

/// <summary>
/// Abstract base class for language-specific configurations for generating classes from JSON.
/// </summary>
public abstract class LanguageSerializer
{
	/// <summary>
	/// Generates classes from the provided JSON node.
	/// </summary>
	/// <param name="node">The JSON node to convert.</param>
	/// <param name="className">The name of the root class.</param>
	/// <returns>A string containing the generated classes.</returns>
	public abstract string GenerateClasses(JsonNode node, string className);

	/// <summary>
	/// Gets the type for a given JSON node.
	/// </summary>
	/// <param name="node">The JSON node to evaluate.</param>
	/// <param name="propertyName">The name of the property.</param>
	/// <returns>The type as a string.</returns>
	public abstract string GetType(JsonNode? node, string propertyName);
	public string ToPascalCase(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}

		// Use a regular expression to split by non-alphanumeric delimiters and case transitions.
		var words = System.Text.RegularExpressions.Regex
			.Split(input, @"(?<!^)(?=[A-Z])|[_\-\.\s]+")
			.Where(word => !string.IsNullOrEmpty(word));

		// Capitalize the first letter of each word and join them.
		return string.Concat(words.Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));
	}

	public string GetValueType(JsonValue jsonValue, SerializableLanguage language)
	{
		if (language.Equals(SerializableLanguage.CSharp))
			return jsonValue switch
			{
				JsonValue value when value.TryGetValue<int>(out _) => "int",
				JsonValue value when value.TryGetValue<double>(out _) => "double",
				JsonValue value when value.TryGetValue<string>(out _) => "string",
				JsonValue value when value.TryGetValue<bool>(out _) => "bool",
				_ => "object"
			};
		else if (language.Equals(SerializableLanguage.JavaScript) || language.Equals(SerializableLanguage.TypeScript))
			return jsonValue switch
			{
				JsonValue value when value.TryGetValue<int>(out _) => "number",
				JsonValue value when value.TryGetValue<double>(out _) => "number",
				JsonValue value when value.TryGetValue<string>(out _) => "string",
				JsonValue value when value.TryGetValue<bool>(out _) => "boolean",
				_ => "any"
			};
		else if (language.Equals(SerializableLanguage.Java))
			return jsonValue switch
			{
				JsonValue value when value.TryGetValue<int>(out _) => "int",
				JsonValue value when value.TryGetValue<double>(out _) => "double",
				JsonValue value when value.TryGetValue<string>(out _) => "String",
				JsonValue value when value.TryGetValue<bool>(out _) => "boolean",
				_ => "Object"
			};
		else if (language.Equals(SerializableLanguage.PHP))
			return jsonValue switch
			{
				JsonValue value when value.TryGetValue<int>(out _) => "int",
				JsonValue value when value.TryGetValue<double>(out _) => "float",
				JsonValue value when value.TryGetValue<string>(out _) => "string",
				JsonValue value when value.TryGetValue<bool>(out _) => "bool",
				_ => "mixed"  // PHP 8+ type hint
			};
		else if (language.Equals(SerializableLanguage.Python))
			return jsonValue switch
			{
				JsonValue value when value.TryGetValue<int>(out _) => "int",
				JsonValue value when value.TryGetValue<double>(out _) => "float",
				JsonValue value when value.TryGetValue<string>(out _) => "str",
				JsonValue value when value.TryGetValue<bool>(out _) => "bool",
				_ => "object"
			};
		else if(language.Equals(SerializableLanguage.Ruby))
			return jsonValue switch
			{
				JsonValue value when value.TryGetValue<int>(out _) => "Integer",
				JsonValue value when value.TryGetValue<double>(out _) => "Float",
				JsonValue value when value.TryGetValue<string>(out _) => "String",
				JsonValue value when value.TryGetValue<bool>(out _) => "T::Boolean",
				_ => "T.untyped"
			};
		else if (language.Equals(SerializableLanguage.Swift))
			return jsonValue switch
			{
				JsonValue value when value.TryGetValue<int>(out _) => "Int",
				JsonValue value when value.TryGetValue<double>(out _) => "Double",
				JsonValue value when value.TryGetValue<string>(out _) => "String",
				JsonValue value when value.TryGetValue<bool>(out _) => "Bool",
				_ => "Any"
			};

		return "unknown";
	}
}
