using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax.SyntaxNodes;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.C;

public class SemanticModelResult
{
    public SemanticModelResult(
        string text,
        ParserSession parserSession,
        CompilationUnit compilationUnit)
    {
        Text = text;
        ParserSession = parserSession;
        CompilationUnit = compilationUnit;
    }

    public string Text { get; }
    public ParserSession ParserSession { get; }
    public CompilationUnit CompilationUnit { get; }
}