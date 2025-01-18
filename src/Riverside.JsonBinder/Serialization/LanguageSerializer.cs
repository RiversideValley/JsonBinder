using System.Text.Json.Nodes;

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

		// Split the input into words using non-alphanumeric characters as delimiters.
		var words = input.Split(new[] { '_', '-', ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);

		// Capitalize the first letter of each word and join them.
		return string.Concat(words.Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));
	}
}
