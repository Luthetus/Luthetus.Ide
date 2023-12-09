namespace Luthetus.TextEditor.Tests.Basis.Edits.Models;

public record EditBlockTests(
    TextEditKind TextEditKind,
    string DisplayName,
    string ContentSnapshot,
    string? OtherTextEditKindIdentifier);