namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public record struct NamespaceAndTypeIdentifiers(string NamespaceIdentifier, string TypeIdentifier)
{
    private const char MEMBER_ACCESS_TEXT = '.';

    public string FullTypeName => NamespaceIdentifier + MEMBER_ACCESS_TEXT + TypeIdentifier;
}
