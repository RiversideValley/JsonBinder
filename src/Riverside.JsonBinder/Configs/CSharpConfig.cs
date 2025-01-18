using System.Text.Json.Nodes;

namespace Riverside.JsonBinder.Configs;

public class CSharpConfig : LanguageConfig
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
            var classDef = $"public class {className}\n{{";
            foreach (var property in obj)
            {
                var propType = GetType(property.Value, property.Key);
                classDef += $"\n    public {propType} {property.Key} {{ get; set; }}";
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
            string elementType = "object";
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
            classes.Add($"public class {className}\n{{\n    public List<{elementType}> Items {{ get; set; }} = new List<{elementType}>();\n}}");
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
            JsonValue value when value.TryGetValue<string>(out _) => "string",
            JsonValue value when value.TryGetValue<bool>(out _) => "bool",
            _ => "object"
        };
    }
}
