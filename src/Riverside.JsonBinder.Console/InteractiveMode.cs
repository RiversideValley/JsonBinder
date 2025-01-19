using Riverside.JsonBinder.Console.Helpers;
using System.Text.Json;

namespace Riverside.JsonBinder.Console;

public class InteractiveMode
{
	private const string CHECKBOX_CHECKED = "[x]";
	private const string CHECKBOX_UNCHECKED = "[ ]";

	/// <summary>
	/// Runs the interactive console mode of the application.
	/// </summary>
	public static void Initialize()
	{
		while (true)
		{
			DisplayMainMenu();
			string choice = System.Console.ReadLine()?.Trim();

			switch (choice)
			{
				case "1":
					ConvertJsonToClasses();
					break;
				case "2":
					DisplayHelp();
					break;
				case "3":
					if (ConsoleHelpers.ConfirmExit())
						return;
					break;
				default:
					ConsoleHelpers.DisplayRedMessage("Invalid option. Please select a valid menu option.\n");
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
	private static void ConvertJsonToClasses()
	{
		System.Console.Clear();
		System.Console.WriteLine("=========================================");
		System.Console.WriteLine("      Convert JSON to Classes");
		System.Console.WriteLine("=========================================");

		System.Console.WriteLine("Enter your JSON (or type 'back' to return to the main menu):");
		string? json = System.Console.ReadLine();

		if (string.IsNullOrWhiteSpace(json) || string.Equals(json, "back", StringComparison.OrdinalIgnoreCase))
			return;

		var selectedLanguages = SelectLanguages();
		if (selectedLanguages.Count == 0)
		{
			ConsoleHelpers.DisplayRedMessage("\nNo languages selected. Please try again.\n");
			ConsoleHelpers.PressAnyKey();
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
				System.Console.ForegroundColor = ConsoleColor.Blue;
				System.Console.WriteLine($"\n{language} Classes:\n");
				System.Console.ResetColor();

				// Use SyntaxHighlighter to display the code with colors
				SyntaxHighlighter.DisplayCodeWithColors(result, language);
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

		ConsoleHelpers.PressAnyKey();
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
	/// Displays an interactive language selection interface and returns the selected languages.
	/// </summary>
	/// <returns>A list of selected languages.</returns>
	private static List<SerializableLanguage> SelectLanguages()
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
}
