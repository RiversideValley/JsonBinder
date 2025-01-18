﻿using System.Text.Json.Nodes;

namespace Riverside.JsonBinder.Configs;

/// <summary>
/// Configuration for generating PHP classes from JSON.
/// </summary>
public class PHPConfig : LanguageConfig
{
	/// <summary>
	/// Generates PHP classes from the provided JSON node.
	/// </summary>
	/// <param name="node">The JSON node to convert.</param>
	/// <param name="className">The name of the root class.</param>
	/// <returns>A string containing the generated PHP classes.</returns>
	public override string GenerateClasses(JsonNode node, string className)
	{
		var classes = new List<string>();
		ProcessNode(node, className, classes);
		return string.Join("\n\n", classes);
	}

	/// <summary>
	/// Processes a JSON node and generates the corresponding PHP class definition.
	/// </summary>
	/// <param name="node">The JSON node to process.</param>
	/// <param name="className">The name of the class to generate.</param>
	/// <param name="classes">The list of generated class definitions.</param>
	private void ProcessNode(JsonNode node, string className, List<string> classes)
	{
		if (node is JsonObject obj)
		{
			var classDef = $"class {className} {{\n";
			foreach (var property in obj)
			{
				classDef += $"\n    public ${property.Key};";
			}
			classDef += "\n}";
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
			string elementType = "mixed";
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
			classes.Add($"class {className} {{\n    public array ${className} = [];\n}}");
		}
	}

	/// <summary>
	/// Gets the PHP type for a given JSON node.
	/// </summary>
	/// <param name="node">The JSON node to evaluate.</param>
	/// <param name="propertyName">The name of the property.</param>
	/// <returns>The PHP type as a string.</returns>
	public override string GetType(JsonNode? node, string propertyName)
	{
		return node switch
		{
			JsonObject => "object",
			JsonArray => "array",
			JsonValue value when value.TryGetValue<int>(out _) => "int",
			JsonValue value when value.TryGetValue<double>(out _) => "float",
			JsonValue value when value.TryGetValue<string>(out _) => "string",
			JsonValue value when value.TryGetValue<bool>(out _) => "bool",
			_ => "mixed"
		};
	}
}
