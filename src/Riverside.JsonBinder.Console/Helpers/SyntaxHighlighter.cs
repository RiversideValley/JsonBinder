using System.Text.RegularExpressions;

namespace Riverside.JsonBinder.Console.Helpers;

public static class SyntaxHighlighter
{
	private static readonly Dictionary<SerializableLanguage, List<SyntaxRule>> SyntaxRulesByLanguage = new()
	{
		{
				SerializableLanguage.CSharp, new List<SyntaxRule>
				{
					// Core keywords
					new(@"\b(abstract|as|base|break|case|catch|checked|class|const|continue|default|delegate|do|else|enum|event|explicit|extern|finally|fixed|for|foreach|goto|if|implicit|in|interface|internal|is|lock|namespace|new|operator|out|override|params|private|protected|public|readonly|ref|return|sealed|sizeof|stackalloc|static|switch|this|throw|try|typeof|unchecked|unsafe|using|virtual|volatile|while|get|set)\b", ConsoleColor.Red),
					
					// Types
					new(@"\b(bool|byte|char|decimal|double|float|int|long|object|sbyte|short|string|uint|ulong|ushort|void|var)\b", ConsoleColor.White),
					
					// Type definitions (classes, interfaces, etc.)
					new(@"\b[A-Z][a-zA-Z0-9_]*\b", ConsoleColor.Gray),
					
					// Properties and methods
					new(@"\b[a-z][a-zA-Z0-9_]*(?=\s*[{<])", ConsoleColor.Magenta),
					
					// Generic type parameters
					new(@"<[^>]+>", ConsoleColor.DarkYellow),
					
					// Numbers
					new(@"\b\d+\b", ConsoleColor.Blue),
					
					// Strings
					new(@"""[^""\\]*(?:\\.[^""\\]*)*""", ConsoleColor.Cyan),
				}
			},
			{
				SerializableLanguage.Python, new List<SyntaxRule>
				{
					// Keywords
					new(@"\b(and|as|assert|async|await|break|class|continue|def|del|elif|else|except|finally|for|from|global|if|import|in|is|lambda|nonlocal|not|or|pass|raise|return|try|while|with|yield)\b", ConsoleColor.Red),
					
					// Built-in types and values
					new(@"\b(True|False|None|bool|int|float|str|list|dict|set|tuple)\b", ConsoleColor.White),
					
					// Class names
					new(@"(?<=\bclass\s+)\w+", ConsoleColor.Gray),
					
					// Function definitions
					new(@"(?<=\bdef\s+)\w+", ConsoleColor.Magenta),
					
					// Decorators
					new(@"@\w+", ConsoleColor.DarkYellow),
					
					// Numbers
					new(@"\b\d+\b", ConsoleColor.Blue),
					
					// Strings
					new(@"""[^""\\]*(?:\\.[^""\\]*)*""|'[^'\\]*(?:\\.[^'\\]*)*'", ConsoleColor.Cyan),
				}
			},
			{
				SerializableLanguage.JavaScript, new List<SyntaxRule>
				{
					// Keywords
					new(@"\b(async|await|break|case|catch|class|const|continue|debugger|default|delete|do|else|export|extends|finally|for|function|if|import|in|instanceof|let|new|of|return|super|switch|this|throw|try|typeof|var|void|while|with|yield)\b", ConsoleColor.Red),
					
					// Built-in objects and types
					new(@"\b(Array|Boolean|Date|Error|Function|JSON|Math|Number|Object|RegExp|String|Promise|Map|Set|Symbol|BigInt)\b", ConsoleColor.White),
					
					// Class names and custom types
					new(@"\b[A-Z][a-zA-Z0-9_]*\b", ConsoleColor.Gray),
					
					// Function declarations
					new(@"(?<=\bfunction\s+)\w+", ConsoleColor.Magenta),
					
					// Arrow functions
					new(@"=>\s*{", ConsoleColor.DarkYellow),
					
					// Numbers
					new(@"\b\d+(\.\d+)?\b", ConsoleColor.Blue),
					
					// Strings
					new(@"""[^""\\]*(?:\\.[^""\\]*)*""|'[^'\\]*(?:\\.[^'\\]*)*'|`[^`\\]*(?:\\.[^`\\]*)*`", ConsoleColor.Cyan),
				}
			},
			{
				SerializableLanguage.TypeScript, new List<SyntaxRule>
				{
					// TypeScript-specific keywords
					new(@"\b(abstract|as|asserts|async|await|break|case|catch|class|const|continue|debugger|declare|default|delete|do|else|enum|export|extends|finally|for|from|function|if|implements|import|in|infer|instanceof|interface|is|keyof|let|module|namespace|new|null|of|return|super|switch|this|throw|try|type|typeof|undefined|unique|unknown|var|void|while|with|yield)\b", ConsoleColor.Red),
					
					// Types and type operators
					new(@"\b(any|boolean|number|string|symbol|never|object|bigint|readonly|private|protected|public|static)\b", ConsoleColor.White),
					
					// Classes and interfaces
					new(@"\b[A-Z][a-zA-Z0-9_]*\b", ConsoleColor.Gray),
					
					// Generics
					new(@"<[^>]+>", ConsoleColor.Magenta),
					
					// Decorators
					new(@"@\w+", ConsoleColor.DarkYellow),
					
					// Numbers
					new(@"\b\d+(\.\d+)?\b", ConsoleColor.Blue),
					
					// Strings
					new(@"""[^""\\]*(?:\\.[^""\\]*)*""|'[^'\\]*(?:\\.[^'\\]*)*'|`[^`\\]*(?:\\.[^`\\]*)*`", ConsoleColor.Cyan),
				}
			},
			{
				SerializableLanguage.Java, new List<SyntaxRule>
				{
					// Keywords
					new(@"\b(abstract|assert|break|case|catch|class|const|continue|default|do|else|enum|extends|final|finally|for|if|implements|import|instanceof|interface|native|new|package|private|protected|public|return|static|strictfp|super|switch|synchronized|this|throw|throws|transient|try|volatile|while)\b", ConsoleColor.Red),
					
					// Types
					new(@"\b(boolean|byte|char|double|float|int|long|short|void|var)\b", ConsoleColor.White),
					
					// Class names and custom types
					new(@"\b[A-Z][a-zA-Z0-9_]*\b", ConsoleColor.Gray),
					
					// Method declarations
					new(@"(?<=\b\w+\s+)\w+(?=\s*\()", ConsoleColor.Magenta),
					
					// Generics
					new(@"<[^>]+>", ConsoleColor.DarkYellow),
					
					// Annotations
					new(@"@\w+", ConsoleColor.DarkYellow),
					
					// Numbers
					new(@"\b\d+(\.\d+)?[dDfFlL]?\b", ConsoleColor.Blue),
					
					// Strings
					new(@"""[^""\\]*(?:\\.[^""\\]*)*""", ConsoleColor.Cyan),
				}
			},
			{
				SerializableLanguage.PHP, new List<SyntaxRule>
				{
					// Keywords
					new(@"\b(abstract|and|array|as|break|callable|case|catch|class|clone|const|continue|declare|default|die|do|echo|else|elseif|empty|enddeclare|endfor|endforeach|endif|endswitch|endwhile|eval|exit|extends|final|finally|fn|for|foreach|function|global|goto|if|implements|include|include_once|instanceof|insteadof|interface|isset|list|match|namespace|new|or|print|private|protected|public|require|require_once|return|static|switch|throw|trait|try|unset|use|var|while|yield|yield from)\b", ConsoleColor.Red),
					
					// Types and type hints
					new(@"\b(bool|boolean|int|integer|float|double|string|array|object|callable|iterable|void|null|mixed|never|true|false)\b", ConsoleColor.White),
					
					// Class names and namespaces
					new(@"\b[A-Z][a-zA-Z0-9_]*\b", ConsoleColor.Gray),
					
					// Function declarations
					new(@"(?<=\bfunction\s+)\w+", ConsoleColor.Magenta),
					
					// Variables
					new(@"\$\w+", ConsoleColor.DarkYellow),
					
					// Numbers
					new(@"\b\d+(\.\d+)?\b", ConsoleColor.Blue),
					
					// Strings
					new(@"""[^""\\]*(?:\\.[^""\\]*)*""|'[^'\\]*(?:\\.[^'\\]*)*'", ConsoleColor.Cyan),
				}
			},
			{
				SerializableLanguage.Ruby, new List<SyntaxRule>
				{
					// Keywords
					new(@"\b(alias|and|begin|break|case|class|def|defined\?|do|else|elsif|end|ensure|false|for|if|in|module|next|nil|not|or|redo|rescue|retry|return|self|super|then|true|undef|unless|until|when|while|yield)\b", ConsoleColor.Red),
					
					// Built-in classes and modules
					new(@"\b(Array|Hash|String|Integer|Float|Symbol|Range|Regexp|Time|File|Dir|Class|Module|Enumerable|Comparable|Kernel|Process|Thread)\b", ConsoleColor.White),
					
					// Class and module names
					new(@"\b[A-Z][a-zA-Z0-9_]*\b", ConsoleColor.Gray),
					
					// Method declarations
					new(@"(?<=\bdef\s+)\w+", ConsoleColor.Magenta),
					
					// Symbols
					new(@":\w+", ConsoleColor.DarkYellow),
					
					// Instance variables
					new(@"@{1,2}\w+", ConsoleColor.DarkYellow),
					
					// Numbers
					new(@"\b\d+(\.\d+)?(e[+-]?\d+)?\b", ConsoleColor.Blue),
					
					// Strings
					new(@"""[^""\\]*(?:\\.[^""\\]*)*""|'[^'\\]*(?:\\.[^'\\]*)*'|%[qQ]?\{[^}]*\}|%[qQ]?\[[^\]]*\]|%[qQ]?\([^)]*\)|%[qQ]?<[^>]*>", ConsoleColor.Cyan),
				}
			},
			{
				SerializableLanguage.Swift, new List<SyntaxRule>
				{
					// Keywords
					new(@"\b(associatedtype|class|deinit|enum|extension|fileprivate|func|import|init|inout|internal|let|open|operator|private|precedencegroup|protocol|public|rethrows|static|struct|subscript|typealias|var|break|case|continue|default|defer|do|else|fallthrough|for|guard|if|in|repeat|return|switch|where|while|as|Any|catch|false|is|nil|super|self|Self|throw|throws|true|try)\b", ConsoleColor.Red),
					
					// Types
					new(@"\b(Int|Double|Float|Bool|String|Character|Optional|Array|Dictionary|Set|Result|Error)\b", ConsoleColor.White),
					
					// Type names and protocols
					new(@"\b[A-Z][a-zA-Z0-9_]*\b", ConsoleColor.Gray),
					
					// Function declarations
					new(@"(?<=\bfunc\s+)\w+", ConsoleColor.Magenta),
					
					// Property wrappers and attributes
					new(@"@\w+", ConsoleColor.DarkYellow),
					
					// Generics
					new(@"<[^>]+>", ConsoleColor.DarkYellow),
					
					// Numbers
					new(@"\b\d+(\.\d+)?([eE][+-]?\d+)?\b", ConsoleColor.Blue),
					
					// Strings
					new(@"""[^""\\]*(?:\\.[^""\\]*)*""", ConsoleColor.Cyan),
				}
			}
	};

	public static void DisplayCodeWithColors(string code, SerializableLanguage language)
	{
		if (!SyntaxRulesByLanguage.ContainsKey(language))
		{
			System.Console.WriteLine(code);
			return;
		}

		var rules = SyntaxRulesByLanguage[language];
		var spans = new List<(int start, int end, ConsoleColor color)>();

		// Collect all matches
		foreach (var rule in rules)
		{
			var matches = Regex.Matches(code, rule.Pattern, RegexOptions.Compiled);
			foreach (Match match in matches)
			{
				spans.Add((match.Index, match.Index + match.Length, rule.Color));
			}
		}

		// Sort spans by start position and handle overlaps
		spans.Sort((a, b) => a.start.CompareTo(b.start));
		var mergedSpans = new List<(int start, int end, ConsoleColor color)>();

		for (int i = 0; i < spans.Count; i++)
		{
			var current = spans[i];
			bool overlap = false;

			// Check if this span overlaps with any previous span
			for (int j = 0; j < mergedSpans.Count; j++)
			{
				if (current.start < mergedSpans[j].end && current.end > mergedSpans[j].start)
				{
					overlap = true;
					break;
				}
			}

			if (!overlap)
			{
				mergedSpans.Add(current);
			}
		}

		// Print with colors
		int currentPos = 0;
		foreach (var span in mergedSpans)
		{
			// Print text before the colored span
			if (currentPos < span.start)
			{
				System.Console.ResetColor();
				System.Console.Write(code.Substring(currentPos, span.start - currentPos));
			}

			// Print the colored span
			System.Console.ForegroundColor = span.color;
			System.Console.Write(code.Substring(span.start, span.end - span.start));
			currentPos = span.end;
		}

		// Print any remaining text
		if (currentPos < code.Length)
		{
			System.Console.ResetColor();
			System.Console.Write(code.Substring(currentPos));
		}

		System.Console.ResetColor();
		System.Console.WriteLine();
	}
}
