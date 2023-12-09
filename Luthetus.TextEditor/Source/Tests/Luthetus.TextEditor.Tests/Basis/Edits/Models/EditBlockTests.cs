namespace Luthetus.TextEditor.RazorLib.Edits.Models;

public record EditBlockTests(
    TextEditKind TextEditKind,
    string DisplayName,
    string ContentSnapshot,
    string? OtherTextEditKindIdentifier);