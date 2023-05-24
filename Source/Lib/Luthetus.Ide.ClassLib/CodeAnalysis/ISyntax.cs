using Luthetus.Ide.ClassLib.CodeAnalysis.CSharp.Syntax;

namespace Luthetus.Ide.ClassLib.CodeAnalysis;

public interface ISyntax
{
    public SyntaxKind SyntaxKind { get; }
}
