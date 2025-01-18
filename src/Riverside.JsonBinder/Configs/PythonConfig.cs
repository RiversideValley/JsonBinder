﻿using System.Text.Json.Nodes;

namespace Riverside.JsonBinder.Configs;

/// <summary>
/// Configuration for generating Python classes from JSON.
/// </summary>
public class PythonConfig : LanguageConfig
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
	private void ProcessNode(JsonNode node, string className, List<string> classes)
	{
		if (node is JsonObject obj)
		{
			var classDef = $"class {className}:\n    def __init__(self):";
			foreach (var property in obj)
			{
				var propType = GetType(property.Value, property.Key);
				classDef += $"\n        self.{property.Key}: {propType} = None";
			}
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
			string elementType = "Any";
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
			classes.Add($"class {className}:\n    def __init__(self):\n        self.items: List[{elementType}] = []");
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
			JsonObject => "dict",
			JsonArray => "list",
			JsonValue value when value.TryGetValue<int>(out _) => "int",
			JsonValue value when value.TryGetValue<double>(out _) => "float",
			JsonValue value when value.TryGetValue<string>(out _) => "str",
			JsonValue value when value.TryGetValue<bool>(out _) => "bool",
			_ => "Any"
		};
	}
}
