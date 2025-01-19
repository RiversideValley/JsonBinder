using Riverside.JsonBinder.Console.Helpers;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Text.Json;

namespace Riverside.JsonBinder.Console;

/// <summary>
/// Program class for the JSON Binder console application.
/// </summary>
public class Program
{
	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	/// <param name="args">The command-line arguments.</param>
	/// <returns>The exit code of the application.</returns>
	public static int Main(string[] args)
	{
		var rootCommand = new RootCommand("JSON to Classes Converter")
		{
			new Argument<string>("json", "The JSON string to convert"),
			new Option<string[]>(["--lang", "--languages"], "Comma-separated list of target languages")
		};

		var interactiveCommand = new Command("classic", "Enables the classic interactive Json2Any CLI mode")
		{
			Handler = CommandHandler.Create(InteractiveMode.Initialize)
		};

		rootCommand.AddCommand(interactiveCommand);

		rootCommand.Handler = CommandHandler.Create<string, string[]>(ConvertJsonToClasses);
		return rootCommand.InvokeAsync(args).Result;
	}

	/// <summary>
	/// Converts the provided JSON string to classes in the specified languages (CLI mode).
	/// </summary>
	/// <param name="json">The JSON string to convert.</param>
	/// <param name="languages">The target languages for the conversion.</param>
	private static void ConvertJsonToClasses(string json, string[] languages)
	{
		if (string.IsNullOrWhiteSpace(json))
		{
			ConsoleHelpers.DisplayRedMessage("No JSON provided. Please try again.\n");
			return;
		}

		if (languages == null || languages.Length == 0)
		{
			ConsoleHelpers.DisplayRedMessage("No languages selected. Please try again.\n");
			return;
		}

		foreach (string choice in languages)
		{
			if (Enum.TryParse<SerializableLanguage>(choice.Trim(), true, out var selectedLanguage))
			{
				try
				{
					string result = JsonSerializer.ConvertTo(json, selectedLanguage);
					System.Console.WriteLine($"\n{selectedLanguage} Classes:\n");
					SyntaxHighlighter.DisplayCodeWithColors(result, selectedLanguage);
				}
				catch (JsonException ex)
				{
					ConsoleHelpers.DisplayError("Invalid JSON format.", ex.Message);
				}
				catch (Exception ex)
				{
					ConsoleHelpers.DisplayError("An unexpected error occurred.", ex.Message);
				}
			}
			else
			{
				ConsoleHelpers.DisplayRedMessage($"\nInvalid language choice: {choice.Trim()}\n");
			}
		}
	}
}