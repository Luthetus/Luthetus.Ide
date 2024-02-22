using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;

/// <summary>
/// (2023-07-23): The <see cref="ILuthCompilerServiceResource"/>
/// implementation count is growing rapidly.
/// <br/><br/>
/// I'm not sure if I'll need such fine grained implementations of
/// <see cref="ILuthCompilerServiceResource"/> in the long run.
/// <br/><br/>
/// But, for now, a separate <see cref="ILuthCompilerServiceResource"/>
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
public class DotNetSolutionResource : ILuthCompilerServiceResource
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
    public CompilationUnit? CompilationUnit { get; set; }
    public ImmutableArray<ISyntaxToken>? SyntaxTokenList { get; set; }
    public DotNetSolutionModel? DotNetSolutionModel { get; set; }

    ILuthCompilerService ILuthCompilerServiceResource.CompilerService => DotNetSolutionCompilerService;
    ImmutableArray<ISyntaxToken> ILuthCompilerServiceResource.SyntaxTokenList { get; set; } = ImmutableArray<ISyntaxToken>.Empty;

    /// <summary>
    /// TODO: It might be a useful optimization to evaluate these linq expressions
    /// and store the result as to not re-evaluate the linq expression over and over.
    /// </summary>
    public ImmutableArray<TextEditorTextSpan> GetTokenTextSpans()
    {
        var localSyntaxTokens = SyntaxTokenList;

        if (localSyntaxTokens is null)
            return ImmutableArray<TextEditorTextSpan>.Empty;

        return localSyntaxTokens.Value.Select(st => st.TextSpan).ToImmutableArray();
    }

    /// <summary>
    /// TODO: It might be a useful optimization to evaluate these linq expressions
    /// and store the result as to not re-evaluate the linq expression over and over.
    /// </summary>
    public ImmutableArray<ITextEditorSymbol> GetSymbols()
    {
        var localCompilationUnit = CompilationUnit;

        if (localCompilationUnit?.Binder is null)
            return ImmutableArray<ITextEditorSymbol>.Empty;

        return localCompilationUnit.Binder.SymbolsList;
    }

    public ImmutableArray<TextEditorDiagnostic> GetDiagnostics()
    {
        return ImmutableArray<TextEditorDiagnostic>.Empty;
    }
}