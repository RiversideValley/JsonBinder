namespace Riverside.JsonBinder.Console.Helpers;

public static class ConsoleHelpers
{
	/// <summary>
	/// Displays an error message with the specified title and details.
	/// </summary>
	/// <param name="title">The title of the error.</param>
	/// <param name="details">The details of the error.</param>
	public static void DisplayError(string title, string details)
	{
		DisplayRedMessage($"\nError: {title}", false);
		DisplayRedMessage($"Details: {details}\n");
	}

	/// <summary>
	/// Displays a message in red color.
	/// </summary>
	/// <param name="message">The message to display.</param>
	/// <param name="colorResets">Indicates whether to reset the color after displaying the message.</param>
	public static void DisplayRedMessage(string message, bool? colorResets = true)
	{
		System.Console.ForegroundColor = ConsoleColor.Red;
		System.Console.WriteLine(message);
		if (colorResets is true)
			System.Console.ResetColor();
	}

	/// <summary>
	/// Displays a "Press any key to continue" message and waits for input.
	/// </summary>
	public static void PressAnyKey()
	{
		System.Console.WriteLine("\nPress any key to continue...");
		System.Console.ReadKey();
	}

	/// <summary>
	/// Confirms if the user wants to exit the application.
	/// </summary>
	/// <returns>True if the user confirms exit, false otherwise.</returns>
	public static bool ConfirmExit()
	{
		System.Console.Clear();
		System.Console.Write("Are you sure you want to exit? (y/n): ");
		string? confirmation = System.Console.ReadLine()?.Trim().ToLower();
		return confirmation == "y" || confirmation == "yes";
	}
}
