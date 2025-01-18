using Riverside.JsonBinder.Configs;
using System.Text.Json.Nodes;

namespace Riverside.JsonBinder;
public class JsonClassConverter
{
	private static readonly Dictionary<Language, LanguageConfig> LanguageConfigs = new()
	{
		{ Language.CSharp, new CSharpConfig() },
		{ Language.Python, new PythonConfig() },
		{ Language.Java, new JavaConfig() },
		{ Language.JavaScript, new JavaScriptConfig() },
		{ Language.TypeScript, new TypeScriptConfig() },
		{ Language.PHP, new PHPConfig() },
		{ Language.Ruby, new RubyConfig() },
		{ Language.Swift, new SwiftConfig() }
	};

	public static string ConvertTo(string json, Language language)
	{
		try
		{
			var jsonNode = JsonNode.Parse(json);
			if (jsonNode == null)
			{
				throw new ArgumentException("Invalid JSON input.");
			}

			if (!LanguageConfigs.TryGetValue(language, out var config))
			{
				throw new NotSupportedException("Language not supported.");
			}

			if (jsonNode is JsonArray)
			{
				jsonNode = new JsonObject { ["Items"] = jsonNode };
			}

			return config.GenerateClasses(jsonNode, "Root");
		}
		catch
		{
			throw;
		}
	}
}