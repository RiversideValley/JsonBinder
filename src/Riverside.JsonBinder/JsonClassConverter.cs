using Riverside.JsonBinder.Serialization;
using System.Text.Json.Nodes;

namespace Riverside.JsonBinder;

/// <summary>
/// Provides functionality to convert JSON to classes in various programming languages.
/// </summary>
public class JsonClassConverter
{
	/// <summary>
	/// A dictionary mapping supported languages to their respective configurations.
	/// </summary>
	private static readonly Dictionary<SerializableLanguage, LanguageSerializer> LanguageConfigs = new()
	{
		{ SerializableLanguage.CSharp, new CSharpSerializer() },
		{ SerializableLanguage.Python, new PythonSerializer() },
		{ SerializableLanguage.Java, new JavaSerializer() },
		{ SerializableLanguage.JavaScript, new JavaScriptSerializer() },
		{ SerializableLanguage.TypeScript, new TypeScriptSerializer() },
		{ SerializableLanguage.PHP, new PHPSerializer() },
		{ SerializableLanguage.Ruby, new RubySerializer() },
		{ SerializableLanguage.Swift, new SwiftSerializer() }
	};

	/// <summary>
	/// Converts the provided JSON string to classes in the specified language.
	/// </summary>
	/// <param name="json">The JSON string to convert.</param>
	/// <param name="language">The target language for the conversion.</param>
	/// <returns>A string containing the generated classes.</returns>
	/// <exception cref="ArgumentException">Thrown when the JSON input is invalid.</exception>
	/// <exception cref="NotSupportedException">Thrown when the specified language is not supported.</exception>
	public static string ConvertTo(string json, SerializableLanguage language)
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
