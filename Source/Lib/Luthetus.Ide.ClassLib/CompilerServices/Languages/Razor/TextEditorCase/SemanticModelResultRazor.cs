using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Lexing;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using Luthetus.TextEditor.RazorLib.Model;

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

    public TextEditorTextSpan? MapAdhocCSharpTextSpanToSource(
        ResourceUri sourceResourceUri,
        string sourceText,
        TextEditorTextSpan textSpan)
    {
        var adhocTextInsertion = AdhocClassInsertions
                .SingleOrDefault(x =>
                    textSpan.StartingIndexInclusive >= x.InsertionStartingIndexInclusive &&
                    textSpan.EndingIndexExclusive <= x.InsertionEndingIndexExclusive);

        // TODO: Fix for spans that go 2 adhocTextInsertions worth of length?
        if (adhocTextInsertion is null)
        {
            adhocTextInsertion = AdhocRenderFunctionInsertions
                .SingleOrDefault(x =>
                    textSpan.StartingIndexInclusive >= x.InsertionStartingIndexInclusive &&
                    textSpan.EndingIndexExclusive <= x.InsertionEndingIndexExclusive);
        }

        if (adhocTextInsertion is null)
        {
            // Could not map the text span back to the source file  
            return null;
        }

        var symbolSourceTextStartingIndexInclusive =
                    adhocTextInsertion.SourceTextStartingIndexInclusive +
                    (textSpan.StartingIndexInclusive - adhocTextInsertion.InsertionStartingIndexInclusive);

        var symbolSourceTextEndingIndexExclusive =
            symbolSourceTextStartingIndexInclusive +
            (textSpan.EndingIndexExclusive - textSpan.StartingIndexInclusive);

        return textSpan with
        {
            ResourceUri = sourceResourceUri,
            SourceText = sourceText,
            StartingIndexInclusive = symbolSourceTextStartingIndexInclusive,
            EndingIndexExclusive = symbolSourceTextEndingIndexExclusive,
        };
    }
}