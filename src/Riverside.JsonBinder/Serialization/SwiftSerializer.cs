using System.Text.Json.Nodes;

namespace Riverside.JsonBinder.Serialization;

/// <summary>
/// Configuration for generating Swift structs from JSON.
/// </summary>
public class SwiftSerializer : LanguageSerializer
{
	/// <summary>
	/// Generates Swift structs from the provided JSON node.
	/// </summary>
	/// <param name="node">The JSON node to convert.</param>
	/// <param name="className">The name of the root struct.</param>
	/// <returns>A string containing the generated Swift structs.</returns>
	public override string GenerateClasses(JsonNode node, string className)
	{
		var classes = new List<string>();
		ProcessNode(node, className, classes);
		return string.Join("\n\n", classes);
	}

	/// <summary>
	/// Processes a JSON node and generates the corresponding Swift struct definition.
	/// </summary>
	/// <param name="node">The JSON node to process.</param>
	/// <param name="className">The name of the struct to generate.</param>
	/// <param name="classes">The list of generated struct definitions.</param>

	private void ProcessNode(JsonNode node, string className, List<string> classes)
	{
		if (node is JsonObject obj)
		{
			var classDef = $"struct {className}: Codable {{";
			var properties = new List<string>();

			foreach (var property in obj)
			{
				var propType = GetType(property.Value, ToPascalCase(property.Key));
				properties.Add($"    var {property.Key}: {propType}?");
			}

			classDef += "\n" + string.Join("\n", properties);
			classDef += "\n}";

			classes.Add(classDef);

			foreach (var property in obj)
			{
				if (property.Value is JsonObject || property.Value is JsonArray)
				{
					ProcessNode(property.Value, ToPascalCase(property.Key), classes);
				}
			}
		}
		else if (node is JsonArray array && array.Count > 0)
		{
			var firstElement = array[0];
			if (firstElement is JsonObject || firstElement is JsonArray)
			{
				ProcessNode(firstElement, className + "Item", classes);
			}
		}
	}

	/// <summary>
	/// Gets the Swift type for a given JSON node.
	/// </summary>
	/// <param name="node">The JSON node to evaluate.</param>
	/// <param name="propertyName">The name of the property.</param>
	/// <returns>The Swift type as a string.</returns>

	public override string GetType(JsonNode? node, string propertyName)
	{
		return node switch
		{
			JsonObject => propertyName,
			JsonArray => $"[{propertyName}Item]",
			JsonValue value when value.TryGetValue<int>(out _) => "Int",
			JsonValue value when value.TryGetValue<double>(out _) => "Double",
			JsonValue value when value.TryGetValue<string>(out _) => "String",
			JsonValue value when value.TryGetValue<bool>(out _) => "Bool",
			_ => "Any"
		};
	}
}
