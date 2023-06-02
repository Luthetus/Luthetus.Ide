namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Keys;

public record DotNetSolutionKey(Guid Guid)
{
    public static readonly DotNetSolutionKey Empty = new DotNetSolutionKey(Guid.Empty);

    public static DotNetSolutionKey NewSolutionKey()
    {
        return new DotNetSolutionKey(Guid.NewGuid());
    }
}

