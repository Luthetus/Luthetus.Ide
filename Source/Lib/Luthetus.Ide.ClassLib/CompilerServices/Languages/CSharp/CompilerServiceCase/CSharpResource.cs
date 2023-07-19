using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
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
    public ImmutableArray<ITextEditorSymbol> Symbols => GetSymbols();

    /// <summary>
    /// TODO: When setting the <see cref="CompilationUnit"/> it might be a useful
    /// optimization to evaluate these linq expressions and store the result
    /// as to not re-evaluate the linq expression over and over.
    /// </summary>
    private ImmutableArray<TextEditorTextSpan> GetSyntacticTextSpans()
    {
        if (SyntaxTokens is null)
            return ImmutableArray<TextEditorTextSpan>.Empty;

        return SyntaxTokens.Value.Select(st => st.TextSpan).ToImmutableArray();
    }
    
    /// <summary>
    /// TODO: When setting the <see cref="CompilationUnit"/> it might be a useful
    /// optimization to evaluate these linq expressions and store the result
    /// as to not re-evaluate the linq expression over and over.
    /// </summary>
    private ImmutableArray<ITextEditorSymbol> GetSymbols()
    {
        if (CompilationUnit is null)
            return ImmutableArray<ITextEditorSymbol>.Empty;

        return CompilationUnit.Binder.Symbols;
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
