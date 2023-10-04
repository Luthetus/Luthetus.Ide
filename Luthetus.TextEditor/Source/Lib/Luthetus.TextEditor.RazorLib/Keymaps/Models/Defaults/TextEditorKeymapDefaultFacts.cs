using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;

public static class TextEditorKeymapDefaultFacts
{
    public static readonly KeymapLayer DefaultLayer = new KeymapLayer(
        new Key<KeymapLayer>(Guid.Parse("d0ac9354-6671-44fd-b281-e652a6aa1f56")),
        "Default Layer",
        "default-layer");

    public static readonly KeymapLayer HasSelectionLayer = new KeymapLayer(
        new Key<KeymapLayer>(Guid.Parse("3ac23ee9-ea25-4b8a-bed4-f10367ad095e")),
        "If Has Selection",
        "if-has-selection");
}
