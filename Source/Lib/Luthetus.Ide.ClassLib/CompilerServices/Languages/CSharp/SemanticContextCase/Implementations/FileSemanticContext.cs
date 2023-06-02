using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Implementations;

public sealed record FileSemanticContext
{
    public FileSemanticContext(ResourceUri fileResourceUri)
    {
        ResourceUri = fileResourceUri;
    }

    public ResourceUri ResourceUri { get; init; }
}