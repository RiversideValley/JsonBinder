using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Text.Json;

namespace Riverside.JsonBinder.Console;

/// <summary>
/// Program class for the JSON Binder console application.
/// </summary>
public class Program
{
	private const string CHECKBOX_CHECKED = "[x]";
	private const string CHECKBOX_UNCHECKED = "[ ]";

	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	/// <param name="args">The command-line arguments.</param>
	/// <returns>The exit code of the application.</returns>
	public static int Main(string[] args)
	{
		if (args.Length == 0)
		{
			RunInteractiveMode();
			return 0;
		}

		var rootCommand = new RootCommand("JSON to Classes Converter")
		{
			new Argument<string>("json", "The JSON string to convert"),
			new Option<string[]>(["--lang", "--languages"], "Comma-separated list of target languages")
		};

		rootCommand.Handler = CommandHandler.Create<string, string[]>(ConvertJsonToClasses);
		return rootCommand.InvokeAsync(args).Result;
	}

	/// <summary>
	/// Runs the interactive console mode of the application.
	/// </summary>
	private static void RunInteractiveMode()
	{
		while (true)
		{
			DisplayMainMenu();
			string choice = System.Console.ReadLine()?.Trim();

			switch (choice)
			{
				case "1":
					ConvertJsonToClassesInteractive();
					break;
				case "2":
					DisplayHelp();
					break;
				case "3":
					if (ConfirmExit())
						return;
					break;
				default:
					DisplayRedMessage("Invalid option. Please select a valid menu option.\n");
					break;
			}
		}
	}

	/// <summary>
	/// Displays the main menu of the application.
	/// </summary>
	private static void DisplayMainMenu()
	{
		System.Console.Clear();
		System.Console.WriteLine("=========================================");
		System.Console.WriteLine("      JSON to Classes Converter");
		System.Console.WriteLine("=========================================");
		System.Console.WriteLine("1. Convert JSON to Classes");
		System.Console.WriteLine("2. Help");
		System.Console.WriteLine("3. Exit");
		System.Console.Write("\nSelect an option: ");
	}

	/// <summary>
	/// Handles the interactive JSON to classes conversion process.
	/// </summary>
	private static void ConvertJsonToClassesInteractive()
	{
		System.Console.Clear();
		System.Console.WriteLine("=========================================");
		System.Console.WriteLine("      Convert JSON to Classes");
		System.Console.WriteLine("=========================================");

		System.Console.WriteLine("Enter your JSON (or type 'back' to return to the main menu):");
		string? json = System.Console.ReadLine();

		if (string.IsNullOrWhiteSpace(json) || string.Equals(json, "back", StringComparison.OrdinalIgnoreCase))
			return;

		var selectedLanguages = SelectLanguagesInteractive();
		if (selectedLanguages.Count == 0)
		{
			DisplayRedMessage("\nNo languages selected. Please try again.\n");
			PressAnyKey();
			return;
		}

		System.Console.Clear();
		System.Console.WriteLine("=========================================");
		System.Console.WriteLine("      Generating Classes");
		System.Console.WriteLine("=========================================");

		foreach (var language in selectedLanguages)
		{
			try
			{
				string result = JsonSerializer.ConvertTo(json, language);
				System.Console.ForegroundColor = ConsoleColor.Green;
				System.Console.WriteLine($"\n{language} Classes:\n");
				System.Console.ResetColor();
				System.Console.WriteLine(result);
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

		PressAnyKey();
	}

	/// <summary>
	/// Displays an interactive language selection interface and returns the selected languages.
	/// </summary>
	/// <returns>A list of selected languages.</returns>
	private static List<SerializableLanguage> SelectLanguagesInteractive()
	{
		var languages = Enum.GetValues<SerializableLanguage>();
		var selected = new HashSet<SerializableLanguage>();
		var currentIndex = 0;

		while (true)
		{
			System.Console.Clear();
			System.Console.WriteLine("\nSelect target languages (use arrow keys to navigate, space to select, enter to confirm):");
			System.Console.WriteLine("Press 'ESC' to cancel selection\n");

			for (int i = 0; i < languages.Length; i++)
			{
				var language = languages[i];
				if (i == currentIndex)
					System.Console.Write("> ");
				else
					System.Console.Write("  ");

				System.Console.WriteLine($"{(selected.Contains(language) ? CHECKBOX_CHECKED : CHECKBOX_UNCHECKED)} {language}");
			}

			var key = System.Console.ReadKey(true);
			switch (key.Key)
			{
				case ConsoleKey.UpArrow:
					currentIndex = (currentIndex - 1 + languages.Length) % languages.Length;
					break;
				case ConsoleKey.DownArrow:
					currentIndex = (currentIndex + 1) % languages.Length;
					break;
				case ConsoleKey.Spacebar:
					var currentLanguage = languages[currentIndex];
					if (selected.Contains(currentLanguage))
						selected.Remove(currentLanguage);
					else
						selected.Add(currentLanguage);
					break;
				case ConsoleKey.Enter:
					return selected.ToList();
				case ConsoleKey.Escape:
					return new List<SerializableLanguage>();
			}
		}
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
					string result = JsonSerializer.ConvertTo(json, selectedLanguage);
					System.Console.WriteLine($"\n{selectedLanguage} Classes:\n");
					System.Console.WriteLine(result);
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
	/// Displays the help menu.
	/// </summary>
	private static void DisplayHelp()
	{
		System.Console.Clear();
		System.Console.WriteLine("=========================================");
		System.Console.WriteLine("               Help Menu");
		System.Console.WriteLine("=========================================");
		System.Console.WriteLine("Interactive Mode:");
		System.Console.WriteLine("1. Input a valid JSON string to generate classes.");
		System.Console.WriteLine("2. Use arrow keys to navigate language options.");
		System.Console.WriteLine("3. Press SPACE to select/deselect languages.");
		System.Console.WriteLine("4. Press ENTER to confirm selections.");
		System.Console.WriteLine("\nCommand Line Mode:");
		System.Console.WriteLine("Usage: jsonbinder <json> --lang <languages>");
		System.Console.WriteLine("Example: jsonbinder '{\"name\":\"test\"}' --lang csharp,typescript");
		System.Console.WriteLine("\nPress any key to return to the main menu...");
		System.Console.ReadKey();
	}

	/// <summary>
	/// Confirms if the user wants to exit the application.
	/// </summary>
	/// <returns>True if the user confirms exit, false otherwise.</returns>
	private static bool ConfirmExit()
	{
		System.Console.Clear();
		System.Console.Write("Are you sure you want to exit? (y/n): ");
		string? confirmation = System.Console.ReadLine()?.Trim().ToLower();
		return confirmation == "y" || confirmation == "yes";
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

	/// <summary>
	/// Displays a "Press any key to continue" message and waits for input.
	/// </summary>
	private static void PressAnyKey()
	{
		System.Console.WriteLine("\nPress any key to continue...");
		System.Console.ReadKey();
	}
}