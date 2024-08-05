namespace Luthetus.Extensions.DotNet.CommandLines.Models;

/// <summary>Used in the method <see cref="ParseOutputEntireDotNetRun"/></summary>
public class DiagnosticLine
{
	private string _textShort;

	// <summary>The entire line of text itself</summary>
	public int StartInclusiveIndex { get; set; }
	// <summary>The entire line of text itself</summary>
	public int EndExclusiveIndex { get; set; }
	// <summary>The entire line of text itself</summary>
	public string Text { get; set; }
	public DiagnosticLineKind DiagnosticLineKind { get; set; } = DiagnosticLineKind.Error;
	
	public string TextShort => _textShort ??= Text
		.Replace(FilePathTextSpan.Text, string.Empty)
		.Replace(ProjectTextSpan.Text, string.Empty);
	
	public DiagnosticTextSpan? FilePathTextSpan { get; set; }
	public DiagnosticTextSpan? LineAndColumnIndicesTextSpan { get; set; }
	public DiagnosticTextSpan? DiagnosticKindTextSpan { get; set; }
	public DiagnosticTextSpan? DiagnosticCodeTextSpan { get; set; }
	public DiagnosticTextSpan? MessageTextSpan { get; set; }
	public DiagnosticTextSpan? ProjectTextSpan { get; set; }
	
	public bool IsValid => 
		FilePathTextSpan is not null &&
		LineAndColumnIndicesTextSpan is not null &&
		DiagnosticKindTextSpan is not null &&
		DiagnosticCodeTextSpan is not null &&
		MessageTextSpan is not null &&
		ProjectTextSpan is not null;
}
