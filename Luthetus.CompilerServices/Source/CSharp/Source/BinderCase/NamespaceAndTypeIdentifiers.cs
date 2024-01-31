namespace Luthetus.CompilerServices.Lang.CSharp.BinderCase;

public record NamespaceAndTypeIdentifiers(string NamespaceIdentifier, string TypeIdentifier)
{
    private const char MEMBER_ACCESS_TEXT = '.';

    public string FullTypeName => NamespaceIdentifier + MEMBER_ACCESS_TEXT + TypeIdentifier;
}
