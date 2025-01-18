using System.Text.Json.Nodes;

namespace Riverside.JsonBinder.Serialization;

/// <summary>
/// Abstract base class for language-specific configurations for generating classes from JSON.
/// </summary>
public abstract class LanguageSerializer
{
	/// <summary>
	/// Generates classes from the provided JSON node.
	/// </summary>
	/// <param name="node">The JSON node to convert.</param>
	/// <param name="className">The name of the root class.</param>
	/// <returns>A string containing the generated classes.</returns>
	public abstract string GenerateClasses(JsonNode node, string className);

	/// <summary>
	/// Gets the type for a given JSON node.
	/// </summary>
	/// <param name="node">The JSON node to evaluate.</param>
	/// <param name="propertyName">The name of the property.</param>
	/// <returns>The type as a string.</returns>
	public abstract string GetType(JsonNode? node, string propertyName);
}
