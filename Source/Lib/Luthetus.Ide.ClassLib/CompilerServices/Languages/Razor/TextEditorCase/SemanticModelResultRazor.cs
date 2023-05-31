using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.Razor.TextEditorCase;

public class SemanticModelResultRazor
{
    public SemanticModelResultRazor(
        Lexer lexer,
        Parser parser,
        CompilationUnit compilationUnit,
        List<AdhocTextInsertion> adhocClassInsertions,
        List<AdhocTextInsertion> adhocRenderFunctionInsertions,
        AdhocTextInsertion renderFunctionAdhocTextInsertion)
    {
        Lexer = lexer;
        Parser = parser;
        CompilationUnit = compilationUnit;
        AdhocClassInsertions = adhocClassInsertions;
        AdhocRenderFunctionInsertions = adhocRenderFunctionInsertions;
        RenderFunctionAdhocTextInsertion = renderFunctionAdhocTextInsertion;
    }

    public Lexer Lexer { get; }
    public Parser Parser { get; }
    public CompilationUnit CompilationUnit { get; }
    public List<AdhocTextInsertion> AdhocClassInsertions { get; }
    public List<AdhocTextInsertion> AdhocRenderFunctionInsertions { get; }
    public AdhocTextInsertion RenderFunctionAdhocTextInsertion { get; }
}