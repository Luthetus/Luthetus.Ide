using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Measurements.Models;
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
        ElementMeasurementsInPixels elementMeasurementsInPixels,
        CharacterWidthAndRowHeight characterWidthAndRowHeight)
    {
        EntryBag = entries;
        LeftVirtualizationBoundary = leftVirtualizationBoundary;
        RightVirtualizationBoundary = rightVirtualizationBoundary;
        TopVirtualizationBoundary = topVirtualizationBoundary;
        BottomVirtualizationBoundary = bottomVirtualizationBoundary;
        ElementMeasurementsInPixels = elementMeasurementsInPixels;
        CharacterWidthAndRowHeight = characterWidthAndRowHeight;
    }

    public static VirtualizationResult<List<RichCharacter>> GetEmptyRichCharacters() => new(
        ImmutableArray<VirtualizationEntry<List<RichCharacter>>>.Empty,
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new ElementMeasurementsInPixels(0, 0, 0, 0, 0, 0, 0, CancellationToken.None),
        new CharacterWidthAndRowHeight(0, 0));

    public ImmutableArray<VirtualizationEntry<T>> EntryBag { get; init; }
    public VirtualizationBoundary LeftVirtualizationBoundary { get; init; }
    public VirtualizationBoundary RightVirtualizationBoundary { get; init; }
    public VirtualizationBoundary TopVirtualizationBoundary { get; init; }
    public VirtualizationBoundary BottomVirtualizationBoundary { get; init; }
    public ElementMeasurementsInPixels ElementMeasurementsInPixels { get; init; }
    public CharacterWidthAndRowHeight CharacterWidthAndRowHeight { get; set; }
}