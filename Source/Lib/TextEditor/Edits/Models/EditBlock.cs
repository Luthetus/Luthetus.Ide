namespace Luthetus.TextEditor.RazorLib.Edits.Models;

public record EditBlock(
    TextEditKind TextEditKind,
    string DisplayName,
    string ContentSnapshot,
    string? OtherTextEditKindIdentifier);