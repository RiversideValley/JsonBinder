using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Riverside.JsonBinder.Serialization;

/// <summary>
/// Configuration for generating C# classes from JSON.
/// </summary>
public class CSharpSerializer : LanguageSerializer
{
	/// <summary>
	/// Generates C# classes from the provided JSON node.
	/// </summary>
	/// <param name="node">The JSON node to convert.</param>
	/// <param name="className">The name of the root class.</param>
	/// <returns>A string containing the generated C# classes.</returns>
	public override string GenerateClasses(JsonNode node, string className)
	{
		var classes = new List<string>();
		ProcessNode(node, className, classes);
		return string.Join("\n\n", classes);
	}

	/// <summary>
	/// Processes a JSON node and generates the corresponding C# class definition.
	/// </summary>
	/// <param name="node">The JSON node to process.</param>
	/// <param name="className">The name of the class to generate.</param>
	/// <param name="classes">The list of generated class definitions.</param>
	private void ProcessNode(JsonNode node, string className, List<string> classes)
	{
		if (node is JsonObject obj)
		{
			var classDef = $"public class {className}\n{{";
			foreach (var property in obj)
			{
				var propType = GetType(property.Value, className + '_' + ToPascalCase(property.Key));
				classDef += $"\n\n    [JsonPropertyName(\"{property.Key}\")]";
				classDef += $"\n    public {propType} {ToPascalCase(property.Key)} {{ get; set; }}";
			}
			classDef += "\n}";
			classes.Add(classDef);

			foreach (var property in obj)
			{
				if (property.Value is JsonObject)
				{
					ProcessNode(property.Value, className + '_' + ToPascalCase(property.Key), classes);
				}
				else if (property.Value is JsonArray array && array.Count > 0 && array[0] is JsonObject)
				{
					// Only process arrays of objects
					ProcessNode(array[0], className + '_' + ToPascalCase(property.Key + "Item"), classes);
				}
			}
		}
	}

	/// <summary>
	/// Gets the C# type for a given JSON node.
	/// </summary>
	/// <param name="node">The JSON node to evaluate.</param>
	/// <param name="propertyName">The name of the property.</param>
	/// <returns>The C# type as a string.</returns>
	public override string GetType(JsonNode? node, string propertyName)
	{
		if (node is JsonArray array && array.Count > 0)
		{
			var firstElement = array[0];
			if (firstElement is JsonValue val)
			{
				return $"List<{GetValueType(val, SerializableLanguage.CSharp)}>";
			}
			// Handle complex object arrays
			return $"List<{propertyName}Item>";
		}
		else if (node is JsonObject)
			return propertyName;
		else if (node is JsonValue val)
			return GetValueType(val, SerializableLanguage.CSharp);

		return "object";
	}
}
