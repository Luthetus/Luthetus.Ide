using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.ReadOnlys;

public static class TextEditorKeymapReadOnlyFacts
{
    public static readonly KeymapLayer DefaultLayer = new KeymapLayer(
        new Key<KeymapLayer>(Guid.Parse("f3675ef2-2bb5-4db5-a79c-2e19292e54a6")),
        "Default Layer",
        "default-layer");

    public static readonly KeymapLayer HasSelectionLayer = new KeymapLayer(
        new Key<KeymapLayer>(Guid.Parse("5e0fc69c-b4b5-4547-b051-cb97abd69c12")),
        "If Has Selection",
        "if-has-selection");
}
