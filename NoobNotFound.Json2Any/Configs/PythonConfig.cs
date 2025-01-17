using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace NoobNotFound.Json2Any.Configs;


public class PythonConfig : LanguageConfig
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