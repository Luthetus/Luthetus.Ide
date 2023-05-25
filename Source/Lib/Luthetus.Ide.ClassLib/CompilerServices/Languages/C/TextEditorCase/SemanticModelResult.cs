using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.C.ParserCase;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.C.TextEditorCase;

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