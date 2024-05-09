using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Virtualizations.Models;

public record VirtualizationResult<T> : IVirtualizationResultWithoutTypeMask
{
    public VirtualizationResult(
        ImmutableArray<VirtualizationEntry<T>> entries,
        VirtualizationBoundary leftVirtualizationBoundary,
        VirtualizationBoundary rightVirtualizationBoundary,
        VirtualizationBoundary topVirtualizationBoundary,
        VirtualizationBoundary bottomVirtualizationBoundary,
        TextEditorDimensions textEditorMeasurements,
        CharAndLineMeasurements charAndRowMeasurements)
    {
        EntryList = entries;
        LeftVirtualizationBoundary = leftVirtualizationBoundary;
        RightVirtualizationBoundary = rightVirtualizationBoundary;
        TopVirtualizationBoundary = topVirtualizationBoundary;
        BottomVirtualizationBoundary = bottomVirtualizationBoundary;
        TextEditorMeasurements = textEditorMeasurements;
        CharAndLineMeasurements = charAndRowMeasurements;
    }

    public static VirtualizationResult<List<RichCharacter>> GetEmptyRichCharacters() => new(
        ImmutableArray<VirtualizationEntry<List<RichCharacter>>>.Empty,
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new TextEditorDimensions(0, 0, 0, 0, 0, 0, 0),
        new CharAndLineMeasurements(0, 0));

    public ImmutableArray<VirtualizationEntry<T>> EntryList { get; init; }
    public VirtualizationBoundary LeftVirtualizationBoundary { get; init; }
    public VirtualizationBoundary RightVirtualizationBoundary { get; init; }
    public VirtualizationBoundary TopVirtualizationBoundary { get; init; }
    public VirtualizationBoundary BottomVirtualizationBoundary { get; init; }
    public TextEditorDimensions TextEditorMeasurements { get; init; }
    public CharAndLineMeasurements CharAndLineMeasurements { get; set; }
}