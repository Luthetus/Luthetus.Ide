using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Symbols;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Lexing;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.BinderCase;

/// <summary>
/// The <see cref="CSharpBinder"/> as of (2023-07-19) is being written
/// as though it is re-used everytime a file need be parsed.
/// <br/><br/>
/// The <see cref="CSharpBinderSnapshot"/> will make an immutable state
/// of the <see cref="CSharpBinder"/> so the result of various parses can be saved.
/// </summary>
public class CSharpBinderSnapshot
{
    public CSharpBinderSnapshot(
        ResourceUri resourceUri,
        ImmutableDictionary<string, BoundNamespaceStatementNode> boundNamespaceStatementNodes,
        ImmutableArray<ISymbol> symbols,
        Dictionary<string, SymbolDefinition> symbolDefinitions,
        ImmutableArray<BoundScope> boundScopes,
        ImmutableArray<TextEditorDiagnostic> diagnostics)
    {
        ResourceUri = resourceUri;
        BoundNamespaceStatementNodes = boundNamespaceStatementNodes;
        Symbols = symbols;
        SymbolDefinitions = symbolDefinitions;
        BoundScopes = boundScopes;
        Diagnostics = diagnostics;
    }

    public ResourceUri? ResourceUri { get; }
    public ImmutableDictionary<string, BoundNamespaceStatementNode> BoundNamespaceStatementNodes { get; } = null!;
    public ImmutableArray<ISymbol> Symbols { get; }
    public Dictionary<string, SymbolDefinition> SymbolDefinitions { get; } = null!;
    public ImmutableArray<BoundScope> BoundScopes { get; }
    public ImmutableArray<TextEditorDiagnostic> Diagnostics { get; }
}
