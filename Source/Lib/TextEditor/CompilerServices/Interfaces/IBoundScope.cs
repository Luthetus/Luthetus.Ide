using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

/// <summary>
/// There should be <see cref="IBoundScope"/>,
/// but also 'IBoundScopeCode' which inherits <see cref="IBoundScope"/>.
///
/// The idea being, if one wanted to implement an <see cref ="IBinder"/>
/// for an HtmlCompilerService, maybe it is the case that their concept
/// of a scope differs from that of 'C#'. Perhaps the
/// <see cref="VariableDeclarationMap"/> would be "dead weight" to them.
///
/// This isn't to say many hierarchies of inheritance should exist to
/// specify further and further the kind of 'IBoundScope'.
///
/// Instead that, it might be beneficial to add just 1 level of interfaces
/// which inherit 'IBoundScope'. And then an enum that identifies their type.
///
/// A different idea for the same result, could be to
/// have 1 TypeDefinitionMap, and etc..., that all Binder instances
/// Add and remove to. Where the Key is the fully qualified name
/// of the definition?
///
/// Then, a compiler service could return null when getting their ScopeDefinitionMapper
/// to indicate that they aren't making use of this.
///
/// I like this idea a lot. The idea that every scope is going to
/// allocate 3 dictionaries, even if they use them, seems like insanity to me.
/// </summary>
public interface IBoundScope
{
    public int StartingIndexInclusive { get; }
    public int? EndingIndexExclusive { get; }
    public ResourceUri ResourceUri { get; }
    
    public IBoundScope? Parent { get; }
    /// <summary>
    /// Key is the name of the type.<br/><br/>
    /// Given: public class MyClass { }<br/>
    /// Then: Key == "MyClass"
    /// </summary>
    public Dictionary<string, TypeDefinitionNode> TypeDefinitionMap { get; init; }
    /// <summary>
    /// Key is the name of the function.<br/><br/>
    /// Given: public void MyMethod() { }<br/>
    /// Then: Key == "MyMethod"
    /// </summary>
    public Dictionary<string, FunctionDefinitionNode> FunctionDefinitionMap { get; init; }
    /// <summary>
    /// Key is the name of the variable.<br/><br/>
    /// Given: var myVariable = 2;<br/>
    /// Then: Key == 'myVariable' 
    /// </summary>
    public Dictionary<string, IVariableDeclarationNode> VariableDeclarationMap { get; init; }
}
