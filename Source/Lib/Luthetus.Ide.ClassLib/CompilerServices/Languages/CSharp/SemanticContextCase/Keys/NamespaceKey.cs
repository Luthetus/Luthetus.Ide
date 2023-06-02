namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Keys;

public record NamespaceKey(Guid Guid)
{
    public static readonly NamespaceKey Empty = new NamespaceKey(Guid.Empty);

    public static NamespaceKey NewNamespaceKey()
    {
        return new NamespaceKey(Guid.NewGuid());
    }
}

