using System.Linq;
using System.Text.Json.Nodes;

namespace Riverside.JsonBinder.Serialization;

/// <summary>
/// Configuration for generating Python classes from JSON.
/// </summary>
public class PythonSerializer : LanguageSerializer
{
	/// <summary>
	/// Generates Python classes from the provided JSON node.
	/// </summary>
	/// <param name="node">The JSON node to convert.</param>
	/// <param name="className">The name of the root class.</param>
	/// <returns>A string containing the generated Python classes.</returns>
	public override string GenerateClasses(JsonNode node, string className)
	{
		var classes = new List<string>();
		ProcessNode(node, className, classes);
		return string.Join("\n\n", classes);
	}

	/// <summary>
	/// Processes a JSON node and generates the corresponding Python class definition.
	/// </summary>
	/// <param name="node">The JSON node to process.</param>
	/// <param name="className">The name of the class to generate.</param>
	/// <param name="classes">The list of generated class definitions.</param>
	/// 

	private void ProcessNode(JsonNode node, string className, List<string> classes)
	{
		if (node is JsonObject obj)
		{
			var classDef = $"class {className}:";
			var initDef = "    def __init__(self):";
			var annotations = new List<string>();

			foreach (var property in obj)
			{
				var propName = property.Key.ToLower();
				var propType = GetType(property.Value, className);
				annotations.Add($"    {propName}: Optional[{propType}]");
				initDef += $"\n        self.{propName}: Optional[{propType}] = None";
			}

			classes.Add($"{classDef}\n{string.Join("\n", annotations)}\n\n{initDef}");

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
	/// Gets the Python type for a given JSON node.
	/// </summary>
	/// <param name="node">The JSON node to evaluate.</param>
	/// <param name="propertyName">The name of the property.</param>
	/// <returns>The Python type as a string.</returns>
	public override string GetType(JsonNode? node, string propertyName)
	{
		return node switch
		{
			JsonObject => propertyName,
			JsonArray => $"List[{propertyName}Item]",
			JsonValue value when value.TryGetValue<int>(out _) => "int",
			JsonValue value when value.TryGetValue<double>(out _) => "float",
			JsonValue value when value.TryGetValue<string>(out _) => "str",
			JsonValue value when value.TryGetValue<bool>(out _) => "bool",
			_ => "object"
		};
	}
}
