using System.Text.Json.Nodes;

namespace Riverside.JsonBinder.Configs;

public class JavaConfig : LanguageConfig
{
	public override string GenerateClasses(JsonNode node, string className)
	{
		var classes = new List<string>();
		ProcessNode(node, className, classes);
		return string.Join("\n\n", classes);
	}

	private void ProcessNode(JsonNode node, string className, List<string> classes)
	{
		if (node is JsonObject obj)
		{
			var classDef = $"public class {className} {{";
			foreach (var property in obj)
			{
				var propType = GetType(property.Value, property.Key);
				classDef += $"\n    private {propType} {property.Key};";
				classDef += $"\n    public {propType} get{property.Key}() {{ return {property.Key}; }}";
				classDef += $"\n    public void set{property.Key}({propType} {property.Key}) {{ this.{property.Key} = {property.Key}; }}";
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
			classes.Add($"public class {className} {{\n    private List<{elementType}> items;\n    public List<{elementType}> getItems() {{ return items; }}\n    public void setItems(List<{elementType}> items) {{ this.items = items; }}\n}}");
		}
	}

	public override string GetType(JsonNode? node, string propertyName)
	{
		return node switch
		{
			JsonObject => propertyName,
			JsonArray => $"List<{propertyName}>",
			JsonValue value when value.TryGetValue<int>(out _) => "int",
			JsonValue value when value.TryGetValue<double>(out _) => "double",
			JsonValue value when value.TryGetValue<string>(out _) => "String",
			JsonValue value when value.TryGetValue<bool>(out _) => "boolean",
			_ => "Object"
		};
	}
}