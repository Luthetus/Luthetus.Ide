using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

/// <summary>
/// The <see cref="CSharpBinder"/> is being instantiated, then re-used many times
/// foreach C# resource. This allows the files to know of eachother but,
/// some data should only last for the length of a particular resource being parsed.
/// Opposed to the lifetime of the <see cref="CSharpBinder"/> instance.
///
/// The binder sessions will now be used specifically to parse a single ResourceUri.
/// (pseudo code: 'parser.Parse(resourceUri)') creates a BinderSession for the resourceUri.
/// |
/// The IBinder needs to have a Dictionary that maps from a resourceUri to an IBinderSession.
/// |
/// The Binder's data can only be accessed via a thread safe mechanism.
/// |
/// After a file is parsed, replace the item in the dictionary for the resourceUri with the most recent
/// IBinderSession for that resource uri.
/// |
/// But what about partial types that span many files?
/// Uh I don't know. But I think this will illuminate the final answer which
/// will support partial types. (2024-10-13).
/// </summary>
public interface IBinderSession
{
    public IBinder Binder { get; }
    public Key<IScope> CurrentScopeKey { get; set; }
    public NamespaceStatementNode CurrentNamespaceStatementNode { get; set; }
    public List<UsingStatementNode> CurrentUsingStatementNodeList { get; set; }
    public ResourceUri ResourceUri { get; set; }
    
    public IScope GetScope(ResourceUri resourceUri, Key<IScope> scopeKey);
    public IScope GetScopeCurrent();
}
