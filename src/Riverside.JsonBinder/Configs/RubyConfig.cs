using System.Text.Json.Nodes;

namespace Riverside.JsonBinder.Configs;
public class RubyConfig : LanguageConfig
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
            var classDef = $"class {className}\n    attr_accessor ";
            classDef += string.Join(", ", obj.Select(property => $":{property.Key}"));
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