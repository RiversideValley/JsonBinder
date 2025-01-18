using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Riverside.JsonBinder.Serialization;
using System.Reflection;
using System.Text.Json;

namespace Riverside.JsonBinder.Tests;

[TestClass]
public class LanguageSerializerTests
{
	private static readonly Dictionary<SerializableLanguage, Type> SerializerTypes = new()
	{
		{ SerializableLanguage.CSharp, typeof(CSharpSerializer) },
		{ SerializableLanguage.Python, typeof(PythonSerializer) },
		{ SerializableLanguage.Java, typeof(JavaSerializer) },
		{ SerializableLanguage.JavaScript, typeof(JavaScriptSerializer) },
		{ SerializableLanguage.TypeScript, typeof(TypeScriptSerializer) },
		{ SerializableLanguage.PHP, typeof(PHPSerializer) },
		{ SerializableLanguage.Ruby, typeof(RubySerializer) },
		{ SerializableLanguage.Swift, typeof(SwiftSerializer) }
	};

	[TestMethod]
	[DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
	public void GenerateClasses_ValidJson_ReturnsExpectedResult(SerializableLanguage language, string json, string expected)
	{
		// Arrange
		var serializer = (LanguageSerializer)Activator.CreateInstance(SerializerTypes[language]);

		// Act
		var result = serializer.GenerateClasses(JsonNode.Parse(json), "Root");

		// Assert
		Assert.AreEqual(expected.Normalize(), result.Normalize());
	}

	[TestMethod]
	[DynamicData(nameof(GetInvalidJsonTestData), DynamicDataSourceType.Method)]
	public void GenerateClasses_InvalidJson_ThrowsException(SerializableLanguage language, string invalidJson)
	{
		// Arrange
		var serializer = (LanguageSerializer)Activator.CreateInstance(SerializerTypes[language]);

		// Act
		try
		{
			serializer.GenerateClasses(JsonNode.Parse(invalidJson), "Root");
		}
		catch (Exception ex)
		{
			// Assert
			Assert.AreEqual("'i' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 0.", ex.Message);
		}
	}

	public static IEnumerable<object[]> GetTestData()
	{
		yield return new object[] { SerializableLanguage.CSharp, "{\"name\":\"John\"}", "public class Root\n{\n    public string name { get; set; }\n}" };
		yield return new object[] { SerializableLanguage.Python, "{\"name\":\"John\"}", "class Root:\n    def __init__(self):\n        self.name: str = None" };
		yield return new object[] { SerializableLanguage.Java, "{\"name\":\"John\"}", "public class Root {\n    private String name;\n    public String getname() { return name; }\n    public void setname(String name) { this.name = name; }\n}" };
		// Add more test data for other languages as needed
	}

	public static IEnumerable<object[]> GetInvalidJsonTestData()
	{
		yield return new object[] { SerializableLanguage.CSharp, "invalid json" };
		yield return new object[] { SerializableLanguage.Python, "invalid json" };
		yield return new object[] { SerializableLanguage.Java, "invalid json" };
		// Add more invalid JSON test data for other languages as needed
	}
}
