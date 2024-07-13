using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

/// <summary>
/// <inheritdoc cref="IBinderSession"/>
/// </summary>
public class BinderSession : IBinderSession
{
    public BinderSession(
        ResourceUri resourceUri,
        IBoundScope globalScope,
        NamespaceStatementNode topLevelNamespaceStatementNode,
        IBinder binder)
    {
        Binder = binder;

        CurrentScope = globalScope;
        CurrentNamespaceStatementNode = topLevelNamespaceStatementNode;
        CurrentUsingStatementNodeList = new();


    }

    public IBinder Binder { get; }

    public IBoundScope CurrentScope { get; set; }
    public NamespaceStatementNode CurrentNamespaceStatementNode { get; set; }
    public List<UsingStatementNode> CurrentUsingStatementNodeList { get; set; }
    public ResourceUri? CurrentResourceUri { get; set; }
}
