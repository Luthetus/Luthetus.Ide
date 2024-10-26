using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

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
    public ResourceUri ResourceUri { get; }
    public IBinder Binder { get; }
    public int CurrentScopeIndexKey { get; set; }
    public NamespaceStatementNode CurrentNamespaceStatementNode { get; set; }
    public List<UsingStatementNode> CurrentUsingStatementNodeList { get; set; }
    
    public DiagnosticBag DiagnosticBag { get; }
    public List<IScope> ScopeList { get; }
    /// <summary>
    /// Key is the name of the type, prefixed with the ScopeKey and '_' to separate the ScopeKey from the type.
    /// Given: public class MyClass { }
    /// Then: Key == new ScopeKeyAndIdentifierText(ScopeKey, "MyClass")
    /// </summary>
    public Dictionary<ScopeKeyAndIdentifierText, TypeDefinitionNode> ScopeTypeDefinitionMap { get; }
    /// <summary>
    /// Key is the name of the function, prefixed with the ScopeKey and '_' to separate the ScopeKey from the function.
    /// Given: public void MyMethod() { }
    /// Then: Key == new ScopeKeyAndIdentifierText(ScopeKey, "MyMethod")
    /// </summary>
    public Dictionary<ScopeKeyAndIdentifierText, FunctionDefinitionNode> ScopeFunctionDefinitionMap { get; }
    /// <summary>
    /// Key is the name of the variable, prefixed with the ScopeKey and '_' to separate the ScopeKey from the variable.
    /// Given: var myVariable = 2;
    /// Then: Key == new ScopeKeyAndIdentifierText(ScopeKey, "myVariable")
    /// </summary>
    public Dictionary<ScopeKeyAndIdentifierText, IVariableDeclarationNode> ScopeVariableDeclarationMap { get; }
    public Dictionary<int, TypeClauseNode> ScopeReturnTypeClauseNodeMap { get; }
    
    public int GetNextIndexKey();
}
