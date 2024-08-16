using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

public interface ISyntaxNode : ISyntax
{
    public ImmutableArray<ISyntax> ChildList { get; }
    
    /// <summary>
    /// I am working on Quick Actions / Refactoring context menu,
    /// and I don't see a better way of handling "getting a node's parent".
    ///
    /// I deliberated on somehow tracking the nodes as I traveled.
    /// But is this constant mental overhead of tracking nodes even worth it?
    /// 
    /// I only consider tracking the nodes to avoid
    /// a reference type property on all of the syntax nodes.
    ///
    /// I suppose just keep in mind this extra memory allocated here,
    /// but since its just a reference it is expected to be negligible.
    ///
    /// I have this as nullable because it entails going through all the old
    /// code and ensuring this property is getting set.
    ///
    /// Once all the old code is setting this property though it should
    /// have the nullability removed (2024-08-16).
    /// </summary>
    public ISyntaxNode? Parent { get; }
}