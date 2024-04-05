using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;

public static class TextEditorKeymapTerminalFacts
{
    public static readonly KeymapLayer DefaultLayer = new KeymapLayer(
        new Key<KeymapLayer>(Guid.Parse("3732d948-a12b-4dad-8c47-5df750a52332")),
        "Default Layer",
        "default-layer");

    public static readonly KeymapLayer HasSelectionLayer = new KeymapLayer(
        new Key<KeymapLayer>(Guid.Parse("bfa794cb-5d5e-41a4-be95-b62b18143850")),
        "If Has Selection",
        "if-has-selection");
}
