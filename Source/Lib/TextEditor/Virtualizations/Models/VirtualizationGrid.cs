using Luthetus.TextEditor.RazorLib.Characters.Models;

namespace Luthetus.TextEditor.RazorLib.Virtualizations.Models;

/// <summary>
/// This type is used to render the text editor's text on the UI.
///
/// A single List of 'VirtualizationEntry' is allocated each time
/// the UI is calculated.
///
/// Each 'VirtualizationEntry' is a representation of each line of text to display.
///
/// The text to display for each line is not stored in the 'VirtualizationEntry'.
///
/// The 'VirtualizationEntry' is only describing the position index span
/// of characters that should be rendered by mapping from the 'TextEditorModel'.
///
/// The 'VirtualizationBoundary's will ensure that the horizontal
/// and vertical scrollbars stay consistent, regardless of how much
/// text is "virtually" not being rendered.
/// </summary>
public record VirtualizationGrid
{
	public static VirtualizationGrid Empty { get; } = new(
        Array.Empty<VirtualizationEntry>(),
        new(),
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0));

    public VirtualizationGrid(
        VirtualizationEntry[] entries,
        List<RichCharacter> flatList,
        VirtualizationBoundary leftVirtualizationBoundary,
        VirtualizationBoundary rightVirtualizationBoundary,
        VirtualizationBoundary topVirtualizationBoundary,
        VirtualizationBoundary bottomVirtualizationBoundary)
    {
        EntryList = entries;
        FlatList = flatList;
        LeftVirtualizationBoundary = leftVirtualizationBoundary;
        RightVirtualizationBoundary = rightVirtualizationBoundary;
        TopVirtualizationBoundary = topVirtualizationBoundary;
        BottomVirtualizationBoundary = bottomVirtualizationBoundary;
    }

    public VirtualizationEntry[] EntryList { get; init; }
    
    /// <summary>
    /// Take all the text that is to be rendered and put it in a single List.
    ///
    /// (this being opposed to having a List<List<RichCharacter>>)
    ///
    /// Each 'VirtualizationEntry' in 'EntryList' contains the indices
    /// from this flattened list that represent that line.
    /// </summary>
    public List<RichCharacter> FlatList { get; set; }
    
    public VirtualizationBoundary LeftVirtualizationBoundary { get; init; }
    public VirtualizationBoundary RightVirtualizationBoundary { get; init; }
    public VirtualizationBoundary TopVirtualizationBoundary { get; init; }
    public VirtualizationBoundary BottomVirtualizationBoundary { get; init; }
}