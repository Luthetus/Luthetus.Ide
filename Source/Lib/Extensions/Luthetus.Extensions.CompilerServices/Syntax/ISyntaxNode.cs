namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

public interface ISyntaxNode : ISyntax
{
    public IReadOnlyList<ISyntax> GetChildList();
}