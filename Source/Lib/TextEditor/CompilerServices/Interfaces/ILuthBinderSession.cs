using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

/// <summary>
/// The <see cref="CSharpBinder"/> is being instantiated, then re-used many times
/// foreach C# resource. This allows the files to know of eachother but,
/// some data should only last for the length of a particular resource being parsed.
/// Opposed to the lifetime of the <see cref="CSharpBinder"/> instance.
/// </summary>
public interface ILuthBinderSession
{
    public ILuthBinder Binder { get; }
    public IBoundScope CurrentScope { get; set; }
    public NamespaceStatementNode CurrentNamespaceStatementNode { get; set; }
    public List<UsingStatementNode> CurrentUsingStatementNodeList { get; set; }
    public ResourceUri? CurrentResourceUri { get; set; }
}
