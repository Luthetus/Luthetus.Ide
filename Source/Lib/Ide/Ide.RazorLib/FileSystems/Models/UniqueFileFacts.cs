﻿using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.FileSystems.Models;

public static class UniqueFileFacts
{
    public const string Properties = "Properties";
    public const string WwwRoot = "wwwroot";

	private static readonly List<string> _empty = new();

	/// <summary>
	/// If rendering a .csproj file pass in <see cref="ExtensionNoPeriodFacts.C_SHARP_PROJECT"/>
	///
	/// Then perhaps the returning array would contain { "Properties", "wwwroot" } as they are unique files
	/// with this context.
	/// </summary>
	/// <returns></returns>
	public static List<string> GetUniqueFilesByContainerFileExtension(string extensionNoPeriod)
    {
        return extensionNoPeriod switch
        {
            ExtensionNoPeriodFacts.C_SHARP_PROJECT => new() { Properties, WwwRoot },
            _ => _empty
		};
    }
}