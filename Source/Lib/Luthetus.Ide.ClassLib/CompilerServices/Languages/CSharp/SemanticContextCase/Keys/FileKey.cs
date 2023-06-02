namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Keys;

public record FileKey(Guid Guid)
{
    public static readonly FileKey Empty = new FileKey(Guid.Empty);

    public static FileKey NewFileKey()
    {
        return new FileKey(Guid.NewGuid());
    }
}

