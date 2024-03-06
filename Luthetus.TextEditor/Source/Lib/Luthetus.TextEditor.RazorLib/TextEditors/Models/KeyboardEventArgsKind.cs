namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays;

public enum KeyboardEventArgsKind
{
    None,

    /// <summary>
    /// TODO: Are movements not just commands?
    /// </summary>
    Movement,
    ContextMenu,
    Command,
    Text,
    Other
}
