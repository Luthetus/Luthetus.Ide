namespace Luthetus.Extensions.DotNet.CommandLines.Models;

public class DiagnosticTextSpan
{
	public DiagnosticTextSpan(
		int startInclusiveIndex,
		int endExclusiveIndex)
	{
		StartInclusiveIndex = startInclusiveIndex;
		EndExclusiveIndex = endExclusiveIndex;
	}

	public int StartInclusiveIndex { get; }
	public int EndExclusiveIndex { get; }
}
