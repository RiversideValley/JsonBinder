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
			var classDef = $"class {className}";
			var properties = new List<string>();
			var typeSignatures = new List<string>();

			// Add type signatures using Ruby 3 syntax
			foreach (var property in obj)
			{
				var propType = GetType(property.Value, ToPascalCase(property.Key));
				typeSignatures.Add($"    sig {{ returns({propType}).nilable }}");
				properties.Add($"    attr_accessor :{property.Key}");
			}

			// Add initialize method
			var initMethod = "    def initialize\n";
			foreach (var property in obj)
			{
				initMethod += $"        @{property.Key} = nil\n";
			}
			initMethod += "    end";

			classDef += "\n    extend T::Sig\n\n";
			classDef += string.Join("\n", typeSignatures) + "\n\n";
			classDef += string.Join("\n", properties) + "\n\n";
			classDef += initMethod + "\nend";

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
	/// Gets the Ruby type for a given JSON node.
	/// </summary>
	/// <param name="node">The JSON node to evaluate.</param>
	/// <param name="propertyName">The name of the property.</param>
	/// <returns>The Ruby type as a string.</returns>
	public override string GetType(JsonNode? node, string propertyName)
	{
		return node switch
		{
			JsonObject => propertyName,
			JsonArray => $"T::Array[{propertyName}Item]",
			JsonValue value when value.TryGetValue<int>(out _) => "Integer",
			JsonValue value when value.TryGetValue<double>(out _) => "Float",
			JsonValue value when value.TryGetValue<string>(out _) => "String",
			JsonValue value when value.TryGetValue<bool>(out _) => "T::Boolean",
			_ => "T.untyped"
		};
	}
}
