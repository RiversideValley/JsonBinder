using System.Text.Json.Nodes;

namespace Riverside.JsonBinder.Configs;

public abstract class LanguageConfig
{
    public abstract string GenerateClasses(JsonNode node, string className);
    public abstract string GetType(JsonNode? node, string propertyName);
}
