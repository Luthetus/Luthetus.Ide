namespace Luthetus.TextEditor.Tests.Basis.Virtualizations.Models;

public record VirtualizationEntryTests<T>( // Wraps the item the consumer of the component wants to render
    int Index, // The index of the item to render
    T Item, // the item itself
    double? WidthInPixels,
    double? HeightInPixels,
    double? LeftInPixels,
    double? TopInPixels);