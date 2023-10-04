namespace Luthetus.TextEditor.RazorLib.Edits.Models;

public enum TextEditKind
{
    None,
    InitialState,
    Other,
    Insertion,
    Deletion,
    ForcePersistEditBlock
}