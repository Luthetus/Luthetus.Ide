using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Examples:<br/>
/// 
/// public T Clone&lt;T&gt;(T item) where T : class<br/>
/// {<br/>
/// &#9;return item;<br/>
/// }<br/>
/// 
/// public T Clone&lt;T&gt;(T item) where T : class => item;<br/>
/// </summary>
public sealed record ConstraintNode : ISyntaxNode
{
    public ConstraintNode(ImmutableArray<ISyntaxToken> innerTokens)
    {
        InnerTokens = innerTokens;

        var children = new List<ISyntax>();
        children.AddRange(InnerTokens);

        ChildList = children.ToImmutableArray();
    }

    /// <summary>
    /// TODO: For now, just grab all tokens and put them in an array...
    /// ...In the future parse the tokens. (2023-10-19)
    /// </summary>
    public ImmutableArray<ISyntaxToken> InnerTokens { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ConstraintNode;
}