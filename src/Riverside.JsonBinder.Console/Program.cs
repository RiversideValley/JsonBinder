using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Text.Json;

namespace Riverside.JsonBinder.Console;

/// <summary>
/// Program class for the JSON Binder console application.
/// </summary>
/// <remarks>
/// This was originally made by AI but refactored using the powerful <see cref="System.CommandLine"/> library.
/// </remarks>
public class Program
{
	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	/// <param name="args">The command-line arguments.</param>
	/// <returns>The exit code of the application.</returns>
	public static int Main(string[] args)
	{
		var rootCommand = new RootCommand("JSON to Classes Converter");

		var convertCommand = new Command("convert", "Convert JSON to Classes")
		{
			new Option<string>("--json", "The JSON string to convert"),
			new Option<string[]>(["--lang", "--languages"], "Comma-separated list of target languages")
		};
		convertCommand.Handler = CommandHandler.Create<string, string[]>(ConvertJsonToClasses);

		var helpCommand = new Command("help", "Display help information")
		{
			Handler = CommandHandler.Create(DisplayHelp)
		};

		rootCommand.AddCommand(convertCommand);
		rootCommand.AddCommand(helpCommand);

		return rootCommand.InvokeAsync(args).Result;
	}

	/// <summary>
	/// Converts the provided JSON string to classes in the specified languages.
	/// </summary>
	/// <param name="json">The JSON string to convert.</param>
	/// <param name="languages">The target languages for the conversion.</param>
	private static void ConvertJsonToClasses(string json, string[] languages)
	{
		if (string.IsNullOrWhiteSpace(json))
		{
			DisplayRedMessage("No JSON provided. Please try again.\n");
			return;
		}

		if (languages == null || languages.Length == 0)
		{
			DisplayRedMessage("No languages selected. Please try again.\n");
			return;
		}

		foreach (string choice in languages)
		{
			if (Enum.TryParse<SerializableLanguage>(choice.Trim(), true, out var selectedLanguage))
			{
				try
				{
					string result = JsonClassConverter.ConvertTo(json, selectedLanguage);
					System.Console.ForegroundColor = ConsoleColor.White;
					System.Console.WriteLine(result);
					System.Console.ResetColor();
				}
				catch (JsonException ex)
				{
					DisplayError("Invalid JSON format.", ex.Message);
				}
				catch (Exception ex)
				{
					DisplayError("An unexpected error occurred.", ex.Message);
				}
			}
			else
			{
				DisplayRedMessage($"\nInvalid language choice: {choice.Trim()}\n");
			}
		}
	}

	/// <summary>
	/// Displays the help information.
	/// </summary>
	private static void DisplayHelp()
	{
		System.Console.Clear();
		System.Console.WriteLine("=========================================");
		System.Console.WriteLine("               Help Menu");
		System.Console.WriteLine("=========================================");
		System.Console.WriteLine("1. Input a valid JSON string to generate classes.");
		System.Console.WriteLine("2. Select one or more target languages by entering their corresponding names, separated by commas.");
		System.Console.WriteLine("3. Supported languages include C#, Python, Java, JavaScript, TypeScript, PHP, Ruby, and Swift.");
		System.Console.WriteLine("4. If an error occurs, ensure your JSON is valid and formatted correctly.");
		System.Console.WriteLine("\nPress any key to return to the main menu...");
		System.Console.ReadKey();
	}

	/// <summary>
	/// Displays an error message with the specified title and details.
	/// </summary>
	/// <param name="title">The title of the error.</param>
	/// <param name="details">The details of the error.</param>
	private static void DisplayError(string title, string details)
	{
		DisplayRedMessage($"\nError: {title}", false);
		DisplayRedMessage($"Details: {details}\n");
	}

	/// <summary>
	/// Displays a message in red color.
	/// </summary>
	/// <param name="message">The message to display.</param>
	/// <param name="colorResets">Indicates whether to reset the color after displaying the message.</param>
	private static void DisplayRedMessage(string message, bool? colorResets = true)
	{
		System.Console.ForegroundColor = ConsoleColor.Red;
		System.Console.WriteLine(message);
		if (colorResets is true)
			System.Console.ResetColor();
	}
}
