
using System.Text.Json;
using Riverside.JsonBinder;

public class Program
{
    public static void Main()
    {
        while (true)
        {
            DisplayMainMenu();
            string choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    ConvertJsonToClasses();
                    break;
                case "2":
                    DisplayHelp();
                    break;
                case "3":
                    ExitApplication();
                    return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid option. Please select a valid menu option.\n");
                    Console.ResetColor();
                    break;
            }
        }
    }

    private static void DisplayMainMenu()
    {
        Console.Clear();
        Console.WriteLine("=========================================");
        Console.WriteLine("      JSON to Classes Converter");
        Console.WriteLine("=========================================");
        Console.WriteLine("1. Convert JSON to Classes");
        Console.WriteLine("2. Help");
        Console.WriteLine("3. Exit");
        Console.Write("Select an option: ");
    }

    private static void ConvertJsonToClasses()
    {
        Console.Clear();
        Console.WriteLine("=========================================");
        Console.WriteLine("      Convert JSON to Classes");
        Console.WriteLine("=========================================");

        Console.WriteLine("Enter your JSON (or type 'back' to return to the main menu):");
        string json = Console.ReadLine();

        if (string.Equals(json, "back", StringComparison.OrdinalIgnoreCase))
            return;

        Console.WriteLine("\nSelect target languages (comma-separated numbers):");
        foreach (var language in Enum.GetValues(typeof(Language)))
        {
            Console.WriteLine($"{(int)language}: {language}");
        }

        Console.Write("Enter your choices: ");
        string[] languageChoices = Console.ReadLine()?.Split(',');

        if (languageChoices == null || languageChoices.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nNo languages selected. Please try again.\n");
            Console.ResetColor();
            return;
        }

        Console.Clear();
        Console.WriteLine("=========================================");
        Console.WriteLine("      Generating Classes");
        Console.WriteLine("=========================================");

        foreach (string choice in languageChoices)
        {
            if (int.TryParse(choice.Trim(), out int languageChoice) && Enum.IsDefined(typeof(Language), languageChoice))
            {
                var selectedLanguage = (Language)languageChoice;
                try
                {
                    string result = JsonClassConverter.ConvertTo(json, selectedLanguage);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("========================================================");
                    Console.WriteLine($"\n{selectedLanguage} Classes:\n");
                    Console.WriteLine("========================================================");
                    Console.ResetColor();
                    Console.WriteLine(result);
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nInvalid language choice: {choice.Trim()}\n");
                Console.ResetColor();
            }
        }
        languageChoices = [];
        Console.WriteLine("\nPress any key to return to the main menu...");
        Console.ReadKey();
    }

    private static void DisplayHelp()
    {
        Console.Clear();
        Console.WriteLine("=========================================");
        Console.WriteLine("               Help Menu");
        Console.WriteLine("=========================================");
        Console.WriteLine("1. Input a valid JSON string to generate classes.");
        Console.WriteLine("2. Select one or more target languages by entering their corresponding numbers, separated by commas.");
        Console.WriteLine("3. Supported languages include C#, Python, Java, JavaScript, TypeScript, PHP, Ruby, and Swift.");
        Console.WriteLine("4. If an error occurs, ensure your JSON is valid and formatted correctly.");
        Console.WriteLine("\nPress any key to return to the main menu...");
        Console.ReadKey();
    }

    private static void ExitApplication()
    {
        Console.Clear();
        Console.Write("Are you sure you want to exit? (y/n): ");
        string confirmation = Console.ReadLine()?.Trim().ToLower();

        if (confirmation == "y" || confirmation == "yes")
        {
            Console.WriteLine("\nThank you for using the converter. Goodbye!");
            Environment.Exit(0);
        }
    }

    private static void DisplayError(string title, string details)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nError: {title}");
        Console.WriteLine($"Details: {details}\n");
        Console.ResetColor();
    }
}