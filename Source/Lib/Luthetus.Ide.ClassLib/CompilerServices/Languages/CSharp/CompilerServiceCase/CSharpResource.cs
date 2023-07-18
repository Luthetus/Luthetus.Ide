using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Model;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.CompilerServiceCase;

public class CSharpResource
{
    public CSharpResource(
        TextEditorModelKey modelKey,
        ResourceUri resourceUri,
        CSharpCompilerService cSharpCompilerService)
    {
        ModelKey = modelKey;
        ResourceUri = resourceUri;
        CSharpCompilerService = cSharpCompilerService;
    }

    public TextEditorModelKey ModelKey { get; }
    public ResourceUri ResourceUri { get; }
    public CSharpCompilerService CSharpCompilerService { get; }
    public CompilationUnit? CompilationUnit { get; internal set; }
    public ImmutableArray<ISyntaxToken>? SyntaxTokens { get; internal set; }

    public ImmutableArray<TextEditorTextSpan> SyntacticTextSpans => GetSyntacticTextSpans();

    private ImmutableArray<TextEditorTextSpan> GetSyntacticTextSpans()
    {
        if (SyntaxTokens is null)
            return ImmutableArray<TextEditorTextSpan>.Empty;

        return SyntaxTokens.Value.Select(st => st.TextSpan).ToImmutableArray();
    }

    /// <returns>
    /// The <see cref="ISyntaxNode"/>
    /// which represents the resource in the compilation result.
    /// </returns>
    public async Task GetRootSyntaxNodeAsync()
    {
        //CSharpCompilerService.Compilation.RootSyntaxNode;
    }
}
