using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.CSharp.BinderCase;

/// <summary>
/// The <see cref="CSharpBinder"/> is being instantiated, then re-used many times
/// foreach C# resource. This allows the files to know of eachother but,
/// some data should only last for the length of a particular resource being parsed.
/// Opposed to the lifetime of the <see cref="CSharpBinder"/> instance.
/// </summary>
public class BinderSession
{
    public BinderSession(
        ResourceUri resourceUri,
        CSharpBoundScope globalScope,
        NamespaceStatementNode topLevelNamespaceStatementNode,
        CSharpBinder binder)
    {
        Binder = binder;

        CurrentScope = globalScope;
        CurrentNamespaceStatementNode = topLevelNamespaceStatementNode;
        CurrentUsingStatementNodeList = new();


    }

    public CSharpBinder Binder { get; }

    public CSharpBoundScope CurrentScope { get; set; }
    public NamespaceStatementNode CurrentNamespaceStatementNode { get; set; }
    public List<UsingStatementNode> CurrentUsingStatementNodeList { get; set; }
    public ResourceUri? CurrentResourceUri { get; set; }
}
