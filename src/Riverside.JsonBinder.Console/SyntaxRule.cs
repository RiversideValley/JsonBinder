namespace Riverside.JsonBinder.Console;

public class SyntaxRule(string pattern, ConsoleColor color)
{
	public string Pattern { get; } = pattern;
	public ConsoleColor Color { get; } = color;
}
