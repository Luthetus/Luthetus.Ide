namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Keys;

public record DotNetProjectKey(Guid Guid)
{
    public static readonly DotNetProjectKey Empty = new DotNetProjectKey(Guid.Empty);

    public static DotNetProjectKey NewProjectKey()
    {
        return new DotNetProjectKey(Guid.NewGuid());
    }
}

