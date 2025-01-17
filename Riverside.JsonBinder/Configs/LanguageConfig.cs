using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Riverside.JsonBinder.Configs;

public abstract class LanguageConfig
{
    public abstract string GenerateClasses(JsonNode node, string className);
    public abstract string GetType(JsonNode? node, string propertyName);
}
