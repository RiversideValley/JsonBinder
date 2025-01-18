using System.Text.Json.Nodes;

namespace Riverside.JsonBinder.Serialization;

/// <summary>
/// Configuration for generating Ruby classes from JSON.
/// </summary>
public class RubySerializer : LanguageSerializer
{
	/// <summary>
	/// Generates Ruby classes from the provided JSON node.
	/// </summary>
	/// <param name="node">The JSON node to convert.</param>
	/// <param name="className">The name of the root class.</param>
	/// <returns>A string containing the generated Ruby classes.</returns>
	public override string GenerateClasses(JsonNode node, string className)
	{
		var classes = new List<string>();
		ProcessNode(node, className, classes);
		return string.Join("\n\n", classes);
	}

	/// <summary>
	/// Processes a JSON node and generates the corresponding Ruby class definition.
	/// </summary>
	/// <param name="node">The JSON node to process.</param>
	/// <param name="className">The name of the class to generate.</param>
	/// <param name="classes">The list of generated class definitions.</param>
	private void ProcessNode(JsonNode node, string className, List<string> classes)
	{
		if (node is JsonObject obj)
		{
			var classDef = $"class {className}\n    attr_accessor ";
			classDef += string.Join(", ", obj.Select(property => $":{property.Key}"));
			classDef += "\nend";
			classes.Add(classDef);

			foreach (var property in obj)
			{
				if (property.Value is JsonObject || property.Value is JsonArray)
				{
					ProcessNode(property.Value, property.Key, classes);
				}
			}
		}
		else if (node is JsonArray array)
		{
			string elementType = "Object";
			if (array.Count > 0)
			{
				var firstElement = array[0];
				elementType = GetType(firstElement, className);
				if (firstElement is JsonObject || firstElement is JsonArray)
				{
					ProcessNode(firstElement, className + "Item", classes);
					elementType = className + "Item";
				}
			}
			classes.Add($"class {className}\n    attr_accessor :items\n\n    def initialize\n        @items = []\n    end\nend");
		}
	}

	/// <summary>
	/// Gets the Ruby type for a given JSON node.
	/// </summary>
	/// <param name="node">The JSON node to evaluate.</param>
	/// <param name="propertyName">The name of the property.</param>
	/// <returns>The Ruby type as a string.</returns>
	public override string GetType(JsonNode? node, string propertyName)
	{
		return node switch
		{
			JsonObject => "Hash",
			JsonArray => "Array",
			JsonValue value when value.TryGetValue<int>(out _) => "Integer",
			JsonValue value when value.TryGetValue<double>(out _) => "Float",
			JsonValue value when value.TryGetValue<string>(out _) => "String",
			JsonValue value when value.TryGetValue<bool>(out _) => "Boolean",
			_ => "Object"
		};
	}
}
