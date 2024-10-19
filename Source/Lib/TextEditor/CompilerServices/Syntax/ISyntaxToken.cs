using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

public interface ISyntaxToken : ISyntax
{
    public TextEditorTextSpan TextSpan { get; }
    
    /// <summary>
    /// If the struct instance is default, this will be false.
    /// Otherwise, it will be true.
    /// </summary>
    public bool ConstructorWasInvoked { get; }
}