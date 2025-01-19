using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.JsonBinder.Console;

public class SyntaxRule(string pattern, ConsoleColor color)
{
	public string Pattern { get; } = pattern;
	public ConsoleColor Color { get; } = color;
}
