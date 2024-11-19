using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

public interface ISyntaxNode : ISyntax
{
    public ISyntax[] GetChildList();
    
    /// <summary>
    /// TODO: What is the cost of adding an instance method with regards to memory/speed versus...
    ///       ...a static class that has a static method with a switch statement for all the SyntaxKind?
    /// </summary>
    public int GetStartInclusiveIndex();
    /// <summary>
    /// TODO: What is the cost of adding an instance method with regards to memory/speed versus...
    ///       ...a static class that has a static method with a switch statement for all the SyntaxKind?
    /// </summary>
    public int GetEndExclusiveIndex();
}