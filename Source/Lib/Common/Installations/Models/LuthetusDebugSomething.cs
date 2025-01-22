using System.Text;

namespace Luthetus.Common.RazorLib.Installations.Models;

/// <summary>
/// Statically track the constructor invocations to make sense of possible optimizations.
/// (this file likely can be deleted if you see this in the future (2024-12-10))
/// </summary>
public static class LuthetusDebugSomething
{
	public static int AncestorDirectory_ConstructorCount { get; set; }
	
	public static string CreateText()
	{
		var builder = new StringBuilder();
		
		builder.AppendLine();
		builder.AppendLine($"{nameof(AncestorDirectory_ConstructorCount)}: {AncestorDirectory_ConstructorCount:N0}");
		builder.AppendLine();
		
		return builder.ToString();
	}
}