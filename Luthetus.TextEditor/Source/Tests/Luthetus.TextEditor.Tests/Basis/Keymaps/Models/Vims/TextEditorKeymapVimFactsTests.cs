using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;

public static class TextEditorKeymapVimFacts
{
    public static readonly KeymapLayer InsertLayer = new KeymapLayer(
        new Key<KeymapLayer>(Guid.Parse("2a7956f0-757d-4a8d-93b9-1a3478d9dce4")),
        "Insert Layer",
        "insert-layer");

    public static readonly KeymapLayer NormalLayer = new KeymapLayer(
        new Key<KeymapLayer>(Guid.Parse("ce806519-7ebb-45aa-b91f-22a1c9799d84")),
        "Normal Layer",
        "normal-layer");

    public static readonly KeymapLayer VisualLayer = new KeymapLayer(
        new Key<KeymapLayer>(Guid.Parse("62558819-be8a-43bf-9d0c-fa8ff2e6f715")),
        "Visual Layer",
        "visual-layer");

    public static readonly KeymapLayer VisualLineLayer = new KeymapLayer(
        new Key<KeymapLayer>(Guid.Parse("d1cc07e7-3def-4633-8935-dfb81e7891ee")),
        "Visual Line Layer",
        "visual-line-layer");

    public static readonly KeymapLayer CommandLayer = new KeymapLayer(
        new Key<KeymapLayer>(Guid.Parse("fb98c6d4-2c57-4ca2-b470-a4c2d06ac1ce")),
        "Command Layer",
        "command-layer");
}