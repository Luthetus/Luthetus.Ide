using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels.Internals;

public interface IHasTextEditorMetadata
{
    public int CharacterCount { get; }
    /// <summary>
	/// If there is a mixture of<br/>-Carriage Return<br/>-Linefeed<br/>-CRLF<br/>
	/// Then this will be null.<br/><br/>
	/// If there are no line endings then this will be null.
	/// </summary>
    public RowEndingKind? OnlyRowEndingKind { get; }
    /// <summary>
    /// Provides exact position index of a tab character
    /// </summary>
    public IReadOnlyList<int> TabList { get; }
    /// <summary>
    /// To get the ending position of RowIndex _rowEndingPositions[RowIndex]<br /><br />
    /// _rowEndingPositions returns the start of the NEXT row
    /// </summary>
    public IReadOnlyList<RowEnding> RowEndingList { get; }
    public IReadOnlyList<(RowEndingKind rowEndingKind, int count)> RowEndingKindCountList { get; }
    public int RowCount { get; }
    public int DocumentLength { get; }
}