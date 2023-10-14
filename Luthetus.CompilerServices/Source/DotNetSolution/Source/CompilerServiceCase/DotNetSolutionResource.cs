using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;

/// <summary>
/// (2023-07-23): The <see cref="ICompilerServiceResource"/>
/// implementation count is growing rapidly.
/// <br/><br/>
/// I'm not sure if I'll need such fine grained implementations of
/// <see cref="ICompilerServiceResource"/> in the long run.
/// <br/><br/>
/// But, for now, a separate <see cref="ICompilerServiceResource"/>
/// implementation for a .NET Solution, a C# project, a C# class, etc...
/// <br/><br/>
/// It all makes things a bit more clear while I figure things out.
/// <br/><br/>
/// In the long run I want to draw a dependency diagram.
/// So, at the top will be a given .NET Solution.
/// Then, arrows point down and out from the .NET Solution and
/// connect to all the various C# Projects.
/// C# Projects then point to the C# classes and etc...
/// </summary>
public class DotNetSolutionResource : ICompilerServiceResource
{
    public DotNetSolutionResource(
        ResourceUri resourceUri,
        DotNetSolutionCompilerService dotNetSolutionCompilerService)
    {
        ResourceUri = resourceUri;
        DotNetSolutionCompilerService = dotNetSolutionCompilerService;
    }

    public ResourceUri ResourceUri { get; }
    public DotNetSolutionCompilerService DotNetSolutionCompilerService { get; }
    public CompilationUnit? CompilationUnit { get; internal set; }
    public ImmutableArray<ISyntaxToken>? SyntaxTokenBag { get; internal set; }
    public DotNetSolutionModel? DotNetSolutionModel { get; set; }

    public ImmutableArray<TextEditorTextSpan> SyntacticTextSpanBag => GetSyntacticTextSpans();
    public ImmutableArray<ITextEditorSymbol> SymbolBag => GetSymbols();

    /// <summary>
    /// TODO: It might be a useful optimization to evaluate these linq expressions
    /// and store the result as to not re-evaluate the linq expression over and over.
    /// </summary>
    private ImmutableArray<TextEditorTextSpan> GetSyntacticTextSpans()
    {
        var localSyntaxTokens = SyntaxTokenBag;

        if (localSyntaxTokens is null)
            return ImmutableArray<TextEditorTextSpan>.Empty;

        return localSyntaxTokens.Value.Select(st => st.TextSpan).ToImmutableArray();
    }

    /// <summary>
    /// TODO: It might be a useful optimization to evaluate these linq expressions
    /// and store the result as to not re-evaluate the linq expression over and over.
    /// </summary>
    private ImmutableArray<ITextEditorSymbol> GetSymbols()
    {
        var localCompilationUnit = CompilationUnit;

        if (localCompilationUnit?.Binder is null)
            return ImmutableArray<ITextEditorSymbol>.Empty;

        return localCompilationUnit.Binder.SymbolsBag;
    }

    /// <returns>
    /// The <see cref="ISyntaxNode"/>
    /// which represents the resource in the compilation result.
    /// </returns>
    public Task GetRootSyntaxNodeAsync()
    {
        //DotNetCompilerService.Compilation.RootSyntaxNode;
        return Task.CompletedTask;
    }
}