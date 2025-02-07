using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

public record struct NamespaceGroup
{
    public NamespaceGroup(
        string namespaceString,
        List<NamespaceStatementNode> namespaceStatementNodeList)
    {
        NamespaceString = namespaceString;
        NamespaceStatementNodeList = namespaceStatementNodeList;
    }

    public string NamespaceString { get; }
    public List<NamespaceStatementNode> NamespaceStatementNodeList { get; }

    public bool ConstructorWasInvoked => NamespaceStatementNodeList is not null;

    /// <summary>
    /// <see cref="GetTopLevelTypeDefinitionNodes"/> provides a collection
    /// which contains all top level type definitions of the namespace.
    /// <br/><br/>
    /// This is to say that, any type definitions which are nested, would not
    /// be in this collection.
    /// </summary>
    public IEnumerable<TypeDefinitionNode> GetTopLevelTypeDefinitionNodes()
    {
        return NamespaceStatementNodeList
            .SelectMany(x => x.GetTopLevelTypeDefinitionNodes());
    }
}