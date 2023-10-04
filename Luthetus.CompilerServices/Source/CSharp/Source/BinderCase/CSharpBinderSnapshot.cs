using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.CSharp.BinderCase;

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
        ImmutableDictionary<string, NamespaceStatementNode> boundNamespaceStatementNodes,
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
    public ImmutableDictionary<string, NamespaceStatementNode> BoundNamespaceStatementNodes { get; } = null!;
    public ImmutableArray<ISymbol> Symbols { get; }
    public Dictionary<string, SymbolDefinition> SymbolDefinitions { get; } = null!;
    public ImmutableArray<BoundScope> BoundScopes { get; }
    public ImmutableArray<TextEditorDiagnostic> Diagnostics { get; }
}