using System.Text.Json.Nodes;

namespace Riverside.JsonBinder.Configs;

public class JavaScriptConfig : LanguageConfig
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
            var classDef = $"class {className} {{\n    constructor() {{";
            foreach (var property in obj)
            {
                classDef += $"\n        this.{property.Key} = null;";
            }
            classDef += "\n    }\n}";
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
            string elementType = "any";
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
            classes.Add($"class {className} {{\n    constructor() {{\n        this.items = [];\n    }}\n}}");
        }
    }

    public override string GetType(JsonNode? node, string propertyName)
    {
        return node switch
        {
            JsonObject => "object",
            JsonArray => "array",
            JsonValue value when value.TryGetValue<int>(out _) => "number",
            JsonValue value when value.TryGetValue<double>(out _) => "number",
            JsonValue value when value.TryGetValue<string>(out _) => "string",
            JsonValue value when value.TryGetValue<bool>(out _) => "boolean",
            _ => "any"
        };
    }
}