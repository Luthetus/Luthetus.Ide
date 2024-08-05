namespace Luthetus.Extensions.DotNet.CommandLines.Models;

public class DiagnosticTextSpan
{
	public DiagnosticTextSpan(
		int startInclusiveIndex,
		int endExclusiveIndex,
		string sourceText)
	{
		StartInclusiveIndex = startInclusiveIndex;
		EndExclusiveIndex = endExclusiveIndex;
		
		Text = sourceText.Substring(
			StartInclusiveIndex,
			EndExclusiveIndex - StartInclusiveIndex);
	}

	public int StartInclusiveIndex { get; }
	public int EndExclusiveIndex { get; }
	public string Text { get; }
}
