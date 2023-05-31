using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Analysis.CSharp.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Analysis.Html.Decoration;
using Luthetus.TextEditor.RazorLib.Analysis.Html.InjectedLanguage;
using Luthetus.TextEditor.RazorLib.Analysis.Html.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.Html.SyntaxObjects;
using Luthetus.TextEditor.RazorLib.Analysis.Html;
using Luthetus.TextEditor.RazorLib.Analysis.Razor.Facts;
using Luthetus.TextEditor.RazorLib.Analysis;
using System.Text;
using Luthetus.TextEditor.RazorLib.Analysis.Html.Facts;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Symbols;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.Razor.TextEditorCase;

public class TEST_RazorSyntaxTree
{
    public const string ADHOC_CLASS_IDENTIFIER = "__CLASS_Aaa__";
    public const string ADHOC_FUNCTION_IDENTIFIER = "__RENDER_FUNCTION_Bbb__";

    private readonly StringBuilder _classBuilder = new($"public class {ADHOC_CLASS_IDENTIFIER}\n{{");
    private readonly StringBuilder _renderFunctionBuilder = new($"public void {ADHOC_FUNCTION_IDENTIFIER}()\n\t{{");

    /// <summary>Need to track the offset of these insertions so <see cref="_classBuilder"/> isn't working when it comes to directly appending to it.</summary>
    private readonly List<TEST_delayed_text_insertion> DelayedClassBuilder = new();
    /// <summary>Need to track the offset of these insertions so <see cref="_renderFunctionBuilder"/> isn't working when it comes to directly appending to it.</summary>
    private readonly List<TEST_delayed_text_insertion> DelayedRenderFunctionBuilder = new();

    private readonly ResourceUri AdhocResourceUri = new ResourceUri(ADHOC_CLASS_IDENTIFIER + ".cs");

#if DEBUG
    [Obsolete("Only available in DEBUG mode, to be viewed via the debugger")]
    private string ClassContents => _classBuilder.ToString();
    [Obsolete("Only available in DEBUG mode, to be viewed via the debugger")]
    private string RenderFunctionContents => _renderFunctionBuilder.ToString();
#endif

    public ImmutableArray<ISymbol> TEST_Finalize()
    {
        _classBuilder.Append("\n\t" + _renderFunctionBuilder.ToString() + "\n\t}");
        _classBuilder.Append("\n}");

        var classContents = _classBuilder.ToString();

        var lexer = new Lexer(
            AdhocResourceUri,
            classContents);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        var resultingSymbols = new List<ISymbol>();

        foreach (var originalSymbol in parser.Binder.Symbols)
        {
            var originalTextSpan = originalSymbol.TextSpan;

            var insertedTextBucket = DelayedClassBuilder
                .SingleOrDefault(x =>
                    originalSymbol.TextSpan.StartingIndexInclusive >= x.StartInsertionPositionIndexInclusive &&
                    originalSymbol.TextSpan.EndingIndexExclusive < x.EndInsertionPositionIndexExclusive);

            if (insertedTextBucket is null)
            {
                insertedTextBucket = DelayedRenderFunctionBuilder
                    .SingleOrDefault(x =>
                        originalSymbol.TextSpan.StartingIndexInclusive >= x.StartInsertionPositionIndexInclusive &&
                        originalSymbol.TextSpan.EndingIndexExclusive < x.EndInsertionPositionIndexExclusive);
            }

            if (insertedTextBucket is not null)
            {
                ISymbol? symbolToAdd = null;

                switch (originalSymbol.SyntaxKind)
                {
                    case SyntaxKind.TypeSymbol:
                        var startingIndexInclusive = insertedTextBucket.Offset + (originalTextSpan.EndingIndexExclusive - originalTextSpan.StartingIndexInclusive);

                        var shiftedTextSpan = originalTextSpan with 
                        { 
                            StartingIndexInclusive = startingIndexInclusive,
                            EndingIndexExclusive = originalTextSpan.EndingIndexExclusive - insertedTextBucket.Offset,
                        };

                        symbolToAdd = new TypeSymbol(shiftedTextSpan);
                        break;
                    case SyntaxKind.FunctionSymbol:
                        break;
                    case SyntaxKind.VariableSymbol:
                        break;
                }

                if (symbolToAdd is not null)
                    resultingSymbols.Add(symbolToAdd);
            }
        }

        return parser.Binder.Symbols
            .Select(s => 
            {
                

                return s;
            }).ToImmutableArray();
    }

    /// <summary>currentCharacterIn:<br/> -<see cref="InjectedLanguageDefinition.TransitionSubstring"/><br/></summary>
    public List<IHtmlSyntaxNode> ParseInjectedLanguageFragment(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        // current character is '@'
        _ = stringWalker.ReadCharacter();

        if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
        {
            textEditorHtmlDiagnosticBag.Report(
                TextEditorDiagnosticLevel.Error,
                "Whitespace immediately following the Razor transition character is unexpected.",
                new TextEditorTextSpan(
                    stringWalker.PositionIndex,
                    stringWalker.PositionIndex + 1,
                    (byte)HtmlDecorationKind.None,
                    stringWalker.ResourceUri,
                    stringWalker.SourceText));

            return new List<IHtmlSyntaxNode>();
        }

        // Check for both RazorKeywords and CSharpRazorKeywords
        {
            var nextWord = stringWalker.PeekNextWord();

            string? foundString = null;

            foreach (var razorKeyword in RazorKeywords.ALL)
            {
                if (razorKeyword == nextWord)
                {
                    foundString = razorKeyword;
                    break;
                }
            }

            if (foundString is not null)
            {
                return ReadRazorKeyword(
                    stringWalker,
                    textEditorHtmlDiagnosticBag,
                    injectedLanguageDefinition,
                    foundString);
            }

            foreach (var cSharpRazorKeyword in CSharpRazorKeywords.ALL)
            {
                if (cSharpRazorKeyword == nextWord)
                {
                    foundString = cSharpRazorKeyword;
                    break;
                }
            }

            if (foundString is not null)
            {
                return ReadCSharpRazorKeyword(
                    stringWalker,
                    textEditorHtmlDiagnosticBag,
                    injectedLanguageDefinition,
                    foundString);
            }
        }

        if (stringWalker.CurrentCharacter == RazorFacts.COMMENT_START)
        {
            return ReadComment(
                stringWalker,
                textEditorHtmlDiagnosticBag,
                injectedLanguageDefinition);
        }

        if (stringWalker.CurrentCharacter == RazorFacts.SINGLE_LINE_TEXT_OUTPUT_WITHOUT_ADDING_HTML_ELEMENT)
        {
            return ReadSingleLineTextOutputWithoutAddingHtmlElement(
                stringWalker,
                textEditorHtmlDiagnosticBag,
                injectedLanguageDefinition);
        }

        if (stringWalker.CurrentCharacter == RazorFacts.CODE_BLOCK_START)
        {
            return ReadCodeBlock(
                stringWalker,
                textEditorHtmlDiagnosticBag,
                injectedLanguageDefinition,
                false);
        }

        // TODO: Check for invalid expressions
        return ReadInlineExpression(
            stringWalker,
            textEditorHtmlDiagnosticBag,
            injectedLanguageDefinition);
    }

    /// <summary> The @code{...} section must be wrapped in an adhoc class definition so that Roslyn can syntax highlight methods. <br/><br/> The @{...} code blocks must be wrapped in an adhoc method.</summary>
    private List<IHtmlSyntaxNode> ReadCodeBlock(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition,
        bool isClassLevelCodeBlock)
    {
        var injectedLanguageFragmentSyntaxes = new List<IHtmlSyntaxNode>();

        var startingPositionIndex = stringWalker.PositionIndex;

        // Syntax highlight the CODE_BLOCK_START as a razor keyword specifically
        {
            injectedLanguageFragmentSyntaxes.Add(
                new InjectedLanguageFragmentNode(
                    ImmutableArray<IHtmlSyntax>.Empty,
                    new TextEditorTextSpan(
                        stringWalker.PositionIndex,
                        stringWalker.PositionIndex +
                        1,
                        (byte)HtmlDecorationKind.InjectedLanguageFragment,
                        stringWalker.ResourceUri,
                        stringWalker.SourceText)));
        }

        // Enters the while loop on the '{'
        var unmatchedCodeBlockStarts = 1;

        // While iterating through the text append any C# text to the cSharpBuilder afterwards pass it through Roslyn for the syntax highlighting and map the corresponding position indices.
        var cSharpBuilder = new StringBuilder();

        var positionIndexOffset = stringWalker.PositionIndex + 1;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (!isClassLevelCodeBlock)
            {
                if (stringWalker.CurrentCharacter == HtmlFacts.OPEN_TAG_BEGINNING)
                {
                    var positionIndexPriorToHtmlTag = stringWalker.PositionIndex;

                    var tagSyntax = HtmlSyntaxTree.HtmlSyntaxTreeStateMachine.ParseTag(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition);

                    injectedLanguageFragmentSyntaxes.Add(tagSyntax);

                    var necessaryWhitespacePadding =
                        stringWalker.PositionIndex -
                        positionIndexPriorToHtmlTag +
                        1;

                    for (int i = 0; i < necessaryWhitespacePadding; i++)
                    {
                        cSharpBuilder.Append(WhitespaceFacts.SPACE);
                    }

                    continue;
                }

                // '@:' is a single line version of '<text></text>' as of this comment | 2022-12-07
                var singleLineTextOutputText =
                    $"{RazorFacts.TRANSITION_SUBSTRING}{RazorFacts.SINGLE_LINE_TEXT_OUTPUT_WITHOUT_ADDING_HTML_ELEMENT}";

                if (stringWalker.CheckForSubstring(singleLineTextOutputText))
                {
                    var positionIndexPriorReadingLine = stringWalker.PositionIndex;

                    var injectedLanguageFragmentSyntaxStartingPositionIndex =
                        stringWalker.PositionIndex;

                    // Track text span of the "@" sign
                    injectedLanguageFragmentSyntaxes.Add(
                        new InjectedLanguageFragmentNode(
                            ImmutableArray<IHtmlSyntax>.Empty,
                            new TextEditorTextSpan(
                                injectedLanguageFragmentSyntaxStartingPositionIndex,
                                stringWalker.PositionIndex + 1,
                                (byte)HtmlDecorationKind.InjectedLanguageFragment,
                                stringWalker.ResourceUri,
                                stringWalker.SourceText)));

                    // Move beyond the "@" sign
                    _ = stringWalker.ReadCharacter();

                    injectedLanguageFragmentSyntaxes.AddRange(ReadSingleLineTextOutputWithoutAddingHtmlElement(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition));

                    var necessaryWhitespacePadding =
                        stringWalker.PositionIndex -
                        positionIndexPriorReadingLine +
                        1;

                    for (int i = 0; i < necessaryWhitespacePadding; i++)
                    {
                        cSharpBuilder.Append(WhitespaceFacts.SPACE);
                    }

                    continue;
                }
            }

            // Track all the C# text
            cSharpBuilder.Append(stringWalker.CurrentCharacter);

            if (stringWalker.CurrentCharacter == RazorFacts.CODE_BLOCK_START)
            {
                unmatchedCodeBlockStarts++;
                continue;
            }

            if (stringWalker.CurrentCharacter == RazorFacts.CODE_BLOCK_END)
            {
                unmatchedCodeBlockStarts--;

                if (unmatchedCodeBlockStarts == 0)
                {
                    // Syntax highlight the CODE_BLOCK_END as a razor keyword specifically
                    {
                        injectedLanguageFragmentSyntaxes.Add(
                            new InjectedLanguageFragmentNode(
                                ImmutableArray<IHtmlSyntax>.Empty,
                                new TextEditorTextSpan(
                                    stringWalker.PositionIndex,
                                    stringWalker.PositionIndex +
                                    1,
                                    (byte)HtmlDecorationKind.InjectedLanguageFragment,
                                    stringWalker.ResourceUri,
                                    stringWalker.SourceText)));
                    }

                    // A final '}' will be erroneously appended so remove that
                    cSharpBuilder.Remove(cSharpBuilder.Length - 1, 1);

                    var cSharpText = cSharpBuilder.ToString();

                    if (isClassLevelCodeBlock)
                    {
                        injectedLanguageFragmentSyntaxes.AddRange(
                            ParseCSharpWithAdhocClassWrapping(
                                cSharpText,
                                positionIndexOffset));
                    }
                    else
                    {
                        injectedLanguageFragmentSyntaxes.AddRange(
                            ParseCSharpWithAdhocMethodWrapping(
                                cSharpText,
                                positionIndexOffset));
                    }

                    break;
                }
            }
        }

        return injectedLanguageFragmentSyntaxes;
    }

    private List<IHtmlSyntaxNode> ReadInlineExpression(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        if (stringWalker.CurrentCharacter == RazorFacts.EXPLICIT_EXPRESSION_START)
        {
            return ReadExplicitInlineExpression(
                stringWalker,
                textEditorHtmlDiagnosticBag,
                injectedLanguageDefinition);
        }

        return ReadImplicitInlineExpression(
            stringWalker,
            textEditorHtmlDiagnosticBag,
            injectedLanguageDefinition);
    }

    /// <summary>Example: @(myVariable)</summary>
    private List<IHtmlSyntaxNode> ReadExplicitInlineExpression(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        var injectedLanguageFragmentSyntaxes = new List<IHtmlSyntaxNode>();

        var startingPositionIndex = stringWalker.PositionIndex;

        // Syntax highlight the EXPLICIT_EXPRESSION_START as a razor keyword specifically
        {
            injectedLanguageFragmentSyntaxes.Add(
                new InjectedLanguageFragmentNode(
                    ImmutableArray<IHtmlSyntax>.Empty,
                    new TextEditorTextSpan(
                        stringWalker.PositionIndex,
                        stringWalker.PositionIndex +
                        1,
                        (byte)HtmlDecorationKind.InjectedLanguageFragment,
                        stringWalker.ResourceUri,
                        stringWalker.SourceText)));
        }

        // Enters the while loop on the '('
        var unmatchedExplicitExpressionStarts = 1;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (stringWalker.CurrentCharacter == RazorFacts.EXPLICIT_EXPRESSION_START)
                unmatchedExplicitExpressionStarts++;

            if (stringWalker.CurrentCharacter == RazorFacts.EXPLICIT_EXPRESSION_END)
            {
                unmatchedExplicitExpressionStarts--;

                if (unmatchedExplicitExpressionStarts == 0)
                {
                    // Syntax highlight the EXPLICIT_EXPRESSION_END as a razor keyword specifically
                    {
                        injectedLanguageFragmentSyntaxes.Add(
                            new InjectedLanguageFragmentNode(
                                ImmutableArray<IHtmlSyntax>.Empty,
                                new TextEditorTextSpan(
                                    stringWalker.PositionIndex,
                                    stringWalker.PositionIndex +
                                    1,
                                    (byte)HtmlDecorationKind.InjectedLanguageFragment,
                                    stringWalker.ResourceUri,
                                    stringWalker.SourceText)));
                    }

                    break;
                }
            }
        }

        // TODO: Syntax highlighting
        return injectedLanguageFragmentSyntaxes;
    }

    /// <summary>Example: @myVariable</summary>
    private List<IHtmlSyntaxNode> ReadImplicitInlineExpression(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        return new List<IHtmlSyntaxNode>();
    }

    /// <summary>Example: @if (true) { ... } else { ... }</summary>
    private List<IHtmlSyntaxNode> ReadCSharpRazorKeyword(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition,
        string matchedOn)
    {
        var injectedLanguageFragmentSyntaxes = new List<IHtmlSyntaxNode>();

        // Syntax highlight the keyword as a razor keyword specifically
        {
            injectedLanguageFragmentSyntaxes.Add(
                new InjectedLanguageFragmentNode(
                    ImmutableArray<IHtmlSyntax>.Empty,
                    new TextEditorTextSpan(
                        stringWalker.PositionIndex,
                        stringWalker.PositionIndex +
                        matchedOn.Length,
                        (byte)HtmlDecorationKind.InjectedLanguageFragment,
                        stringWalker.ResourceUri,
                        stringWalker.SourceText)));

            _ = stringWalker
                .ReadRange(matchedOn.Length);
        }

        switch (matchedOn)
        {
            case CSharpRazorKeywords.CASE_KEYWORD:
                break;
            case CSharpRazorKeywords.DO_KEYWORD:
                {
                    // Necessary in the case where the do-while statement's code block immediately follows the 'do' text. Example: "@do{"
                    stringWalker.BacktrackCharacter();

                    if (!TryReadCodeBlock(
                            stringWalker,
                            textEditorHtmlDiagnosticBag,
                            injectedLanguageDefinition,
                            CSharpRazorKeywords.DO_KEYWORD,
                            out var codeBlockTagSyntaxes) ||
                        codeBlockTagSyntaxes is null)
                    {
                        break;
                    }

                    injectedLanguageFragmentSyntaxes.AddRange(codeBlockTagSyntaxes);

                    if (TryReadWhileOfDoWhile(
                            stringWalker,
                            textEditorHtmlDiagnosticBag,
                            injectedLanguageDefinition,
                            CSharpRazorKeywords.DO_KEYWORD,
                            out var whileOfDoWhileTagSyntaxes) &&
                        whileOfDoWhileTagSyntaxes is not null)
                    {
                        injectedLanguageFragmentSyntaxes.AddRange(whileOfDoWhileTagSyntaxes);
                    }

                    break;
                }
            case CSharpRazorKeywords.DEFAULT_KEYWORD:
                break;
            case CSharpRazorKeywords.FOR_KEYWORD:
                {
                    // Necessary in the case where the switch statement's predicate expression immediately follows the 'switch' text. Example: "@switch(predicate) {"
                    stringWalker.BacktrackCharacter();

                    if (!TryReadExplicitInlineExpression(
                            stringWalker,
                            textEditorHtmlDiagnosticBag,
                            injectedLanguageDefinition,
                            CSharpRazorKeywords.FOR_KEYWORD,
                            out var explicitExpressionTagSyntaxes) ||
                        explicitExpressionTagSyntaxes is null)
                    {
                        break;
                    }

                    injectedLanguageFragmentSyntaxes.AddRange(explicitExpressionTagSyntaxes);

                    if (TryReadCodeBlock(
                            stringWalker,
                            textEditorHtmlDiagnosticBag,
                            injectedLanguageDefinition,
                            CSharpRazorKeywords.FOR_KEYWORD,
                            out var codeBlockTagSyntaxes) &&
                        codeBlockTagSyntaxes is not null)
                    {
                        injectedLanguageFragmentSyntaxes.AddRange(codeBlockTagSyntaxes);
                    }

                    break;
                }
            case CSharpRazorKeywords.FOREACH_KEYWORD:
                {
                    // Necessary in the case where the foreach statement's predicate expression immediately follows the 'foreach' text. Example: "@foreach(predicate) {"
                    stringWalker.BacktrackCharacter();

                    if (!TryReadExplicitInlineExpression(
                            stringWalker,
                            textEditorHtmlDiagnosticBag,
                            injectedLanguageDefinition,
                            CSharpRazorKeywords.FOREACH_KEYWORD,
                            out var explicitExpressionTagSyntaxes) ||
                        explicitExpressionTagSyntaxes is null)
                    {
                        break;
                    }

                    injectedLanguageFragmentSyntaxes.AddRange(explicitExpressionTagSyntaxes);

                    if (TryReadCodeBlock(
                            stringWalker,
                            textEditorHtmlDiagnosticBag,
                            injectedLanguageDefinition,
                            CSharpRazorKeywords.FOREACH_KEYWORD,
                            out var codeBlockTagSyntaxes) &&
                        codeBlockTagSyntaxes is not null)
                    {
                        injectedLanguageFragmentSyntaxes.AddRange(codeBlockTagSyntaxes);
                    }

                    break;
                }
            case CSharpRazorKeywords.IF_KEYWORD:
                {
                    // Necessary in the case where the if statement's predicate expression immediately follows the 'if' text. Example: "@if(predicate) {"
                    stringWalker.BacktrackCharacter();

                    if (!TryReadExplicitInlineExpression(
                            stringWalker,
                            textEditorHtmlDiagnosticBag,
                            injectedLanguageDefinition,
                            CSharpRazorKeywords.IF_KEYWORD,
                            out var explicitExpressionTagSyntaxes) ||
                        explicitExpressionTagSyntaxes is null)
                    {
                        break;
                    }

                    injectedLanguageFragmentSyntaxes.AddRange(explicitExpressionTagSyntaxes);

                    if (TryReadCodeBlock(
                            stringWalker,
                            textEditorHtmlDiagnosticBag,
                            injectedLanguageDefinition,
                            CSharpRazorKeywords.IF_KEYWORD,
                            out var codeBlockTagSyntaxes) &&
                        codeBlockTagSyntaxes is not null)
                    {
                        injectedLanguageFragmentSyntaxes.AddRange(codeBlockTagSyntaxes);
                    }

                    var restorePositionIndexPriorToTryReadElseIf = stringWalker.PositionIndex;

                    while (TryReadElseIf(
                               stringWalker,
                               textEditorHtmlDiagnosticBag,
                               injectedLanguageDefinition,
                               CSharpRazorKeywords.IF_KEYWORD,
                               out var elseIfTagSyntaxes) &&
                           elseIfTagSyntaxes is not null)
                    {
                        injectedLanguageFragmentSyntaxes.AddRange(elseIfTagSyntaxes);
                        restorePositionIndexPriorToTryReadElseIf = stringWalker.PositionIndex;
                    }

                    if (restorePositionIndexPriorToTryReadElseIf != stringWalker.PositionIndex)
                    {
                        stringWalker.BacktrackRange(
                            stringWalker.PositionIndex - restorePositionIndexPriorToTryReadElseIf);
                    }

                    if (TryReadElse(
                            stringWalker,
                            textEditorHtmlDiagnosticBag,
                            injectedLanguageDefinition,
                            CSharpRazorKeywords.IF_KEYWORD,
                            out var elseTagSyntaxes) &&
                        elseTagSyntaxes is not null)
                    {
                        injectedLanguageFragmentSyntaxes.AddRange(elseTagSyntaxes);
                    }

                    break;
                }
            case CSharpRazorKeywords.ELSE_KEYWORD:
                break;
            case CSharpRazorKeywords.LOCK_KEYWORD:
                break;
            case CSharpRazorKeywords.SWITCH_KEYWORD:
                {
                    // Necessary in the case where the switch statement's predicate expression immediately follows the 'switch' text. Example: "@switch(predicate) {"
                    stringWalker.BacktrackCharacter();

                    if (!TryReadExplicitInlineExpression(
                            stringWalker,
                            textEditorHtmlDiagnosticBag,
                            injectedLanguageDefinition,
                            CSharpRazorKeywords.SWITCH_KEYWORD,
                            out var explicitExpressionTagSyntaxes) ||
                        explicitExpressionTagSyntaxes is null)
                    {
                        break;
                    }

                    injectedLanguageFragmentSyntaxes.AddRange(explicitExpressionTagSyntaxes);

                    if (TryReadCodeBlock(
                            stringWalker,
                            textEditorHtmlDiagnosticBag,
                            injectedLanguageDefinition,
                            CSharpRazorKeywords.SWITCH_KEYWORD,
                            out var codeBlockTagSyntaxes) &&
                        codeBlockTagSyntaxes is not null)
                    {
                        injectedLanguageFragmentSyntaxes.AddRange(codeBlockTagSyntaxes);
                    }

                    break;
                }
            case CSharpRazorKeywords.TRY_KEYWORD:
                break;
            case CSharpRazorKeywords.CATCH_KEYWORD:
                break;
            case CSharpRazorKeywords.FINALLY_KEYWORD:
                break;
            case CSharpRazorKeywords.USING_KEYWORD:
                break;
            case CSharpRazorKeywords.WHILE_KEYWORD:
                {
                    // Necessary in the case where the while statement's predicate expression immediately follows the 'while' text. Example: "@while(predicate) {"
                    stringWalker.BacktrackCharacter();

                    if (!TryReadExplicitInlineExpression(
                            stringWalker,
                            textEditorHtmlDiagnosticBag,
                            injectedLanguageDefinition,
                            CSharpRazorKeywords.IF_KEYWORD,
                            out var explicitExpressionTagSyntaxes) ||
                        explicitExpressionTagSyntaxes is null)
                    {
                        break;
                    }

                    injectedLanguageFragmentSyntaxes.AddRange(explicitExpressionTagSyntaxes);

                    if (TryReadCodeBlock(
                            stringWalker,
                            textEditorHtmlDiagnosticBag,
                            injectedLanguageDefinition,
                            CSharpRazorKeywords.IF_KEYWORD,
                            out var codeBlockTagSyntaxes) &&
                        codeBlockTagSyntaxes is not null)
                    {
                        injectedLanguageFragmentSyntaxes.AddRange(codeBlockTagSyntaxes);
                    }

                    break;
                }
        }

        return injectedLanguageFragmentSyntaxes;
    }

    /// <summary>Example: @page "/counter"</summary>
    private List<IHtmlSyntaxNode> ReadRazorKeyword(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition,
        string matchedOn)
    {
        var injectedLanguageFragmentSyntaxes = new List<IHtmlSyntaxNode>();

        // Syntax highlight the keyword as a razor keyword specifically
        {
            injectedLanguageFragmentSyntaxes.Add(
                new InjectedLanguageFragmentNode(
                    ImmutableArray<IHtmlSyntax>.Empty,
                    new TextEditorTextSpan(
                        stringWalker.PositionIndex,
                        stringWalker.PositionIndex +
                        matchedOn.Length,
                        (byte)HtmlDecorationKind.InjectedLanguageFragment,
                        stringWalker.ResourceUri,
                        stringWalker.SourceText)));

            _ = stringWalker
                .ReadRange(matchedOn.Length);
        }

        switch (matchedOn)
        {
            case RazorKeywords.PAGE_KEYWORD:
                {
                    // @page "/csharp"
                    break;
                }
            case RazorKeywords.NAMESPACE_KEYWORD:
                {
                    // @namespace Luthetus.TextEditor.Demo.ServerSide.Pages
                    break;
                }
            case RazorKeywords.CODE_KEYWORD:
            case RazorKeywords.FUNCTIONS_KEYWORD:
                {
                    // Necessary in the case where the code block immediately follows any keyword's text. Example: "@code{" 
                    stringWalker.BacktrackCharacter();

                    var keywordText = matchedOn == RazorKeywords.CODE_KEYWORD
                        ? RazorKeywords.CODE_KEYWORD
                        : RazorKeywords.FUNCTIONS_KEYWORD;

                    if (TryReadCodeBlock(
                            stringWalker,
                            textEditorHtmlDiagnosticBag,
                            injectedLanguageDefinition,
                            keywordText,
                            out var codeBlockTagSyntaxes) &&
                        codeBlockTagSyntaxes is not null)
                    {
                        return injectedLanguageFragmentSyntaxes
                            .Union(codeBlockTagSyntaxes)
                            .ToList();
                    }

                    break;
                }
            case RazorKeywords.INHERITS_KEYWORD:
                {
                    // @inherits IconBase
                    break;
                }
        }

        return injectedLanguageFragmentSyntaxes;
    }

    /// <summary>Example: @* This is a razor comment *@</summary>
    private List<IHtmlSyntaxNode> ReadComment(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        var injectedLanguageFragmentSyntaxes = new List<IHtmlSyntaxNode>();

        // Enters the while loop on the '*'

        // Syntax highlight the '*' the same color as a razor keyword
        {
            var commentStartTextSpan = new TextEditorTextSpan(
                stringWalker.PositionIndex,
                stringWalker.PositionIndex + 1,
                (byte)HtmlDecorationKind.InjectedLanguageFragment,
                stringWalker.ResourceUri,
                stringWalker.SourceText);

            var commentStartSyntax = new InjectedLanguageFragmentNode(
                ImmutableArray<IHtmlSyntax>.Empty,
                commentStartTextSpan);

            injectedLanguageFragmentSyntaxes.Add(commentStartSyntax);
        }

        // Do not syntax highlight the '*' as part of the comment
        var commentTextStartingPositionIndex = stringWalker.PositionIndex + 1;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (stringWalker.CurrentCharacter == RazorFacts.COMMENT_END &&
                stringWalker.NextCharacter.ToString() == RazorFacts.TRANSITION_SUBSTRING)
            {
                break;
            }
        }

        var commentValueTextSpan = new TextEditorTextSpan(
                commentTextStartingPositionIndex,
                stringWalker.PositionIndex,
                (byte)HtmlDecorationKind.Comment,
                stringWalker.ResourceUri,
                stringWalker.SourceText);

        var commentValueSyntax = new InjectedLanguageFragmentNode(
            ImmutableArray<IHtmlSyntax>.Empty,
            commentValueTextSpan);

        injectedLanguageFragmentSyntaxes.Add(commentValueSyntax);

        // Syntax highlight the '*' the same color as a razor keyword
        {
            var commentEndTextSpan = new TextEditorTextSpan(
                stringWalker.PositionIndex,
                stringWalker.PositionIndex + 1,
                (byte)HtmlDecorationKind.InjectedLanguageFragment,
                stringWalker.ResourceUri,
                stringWalker.SourceText);

            var commentEndSyntax = new InjectedLanguageFragmentNode(
                ImmutableArray<IHtmlSyntax>.Empty,
                commentEndTextSpan);

            injectedLanguageFragmentSyntaxes.Add(commentEndSyntax);
        }

        return injectedLanguageFragmentSyntaxes;
    }

    /// <summary>Example: @* This is a razor comment *@</summary>
    private List<IHtmlSyntaxNode> ReadSingleLineTextOutputWithoutAddingHtmlElement(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        var injectedLanguageFragmentSyntaxes = new List<IHtmlSyntaxNode>();

        // Enters the while loop on the ':'

        // Syntax highlight the ':' the same color as a razor keyword
        {
            var commentStartTextSpan = new TextEditorTextSpan(
                stringWalker.PositionIndex,
                stringWalker.PositionIndex + 1,
                (byte)HtmlDecorationKind.InjectedLanguageFragment,
                stringWalker.ResourceUri,
                stringWalker.SourceText);

            var commentStartSyntax = new InjectedLanguageFragmentNode(
                ImmutableArray<IHtmlSyntax>.Empty,
                commentStartTextSpan);

            injectedLanguageFragmentSyntaxes.Add(commentStartSyntax);
        }

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (WhitespaceFacts.LINE_ENDING_CHARACTERS.Contains(stringWalker.CurrentCharacter))
            {
                break;
            }
        }

        return injectedLanguageFragmentSyntaxes;
    }

    private bool TryReadCodeBlock(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition,
        string keywordText,
        out List<IHtmlSyntaxNode>? tagSyntaxes)
    {
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (stringWalker.CurrentCharacter == RazorFacts.CODE_BLOCK_START)
            {
                var isClassLevelCodeBlock =
                    keywordText == RazorKeywords.CODE_KEYWORD ||
                    keywordText == RazorKeywords.FUNCTIONS_KEYWORD;

                var codeBlockTagSyntaxes = ReadCodeBlock(
                    stringWalker,
                    textEditorHtmlDiagnosticBag,
                    injectedLanguageDefinition,
                    isClassLevelCodeBlock);

                tagSyntaxes = codeBlockTagSyntaxes;
                return true;
            }

            if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                continue;

            textEditorHtmlDiagnosticBag.Report(
                TextEditorDiagnosticLevel.Error,
                $"A code block was expected to follow the {RazorFacts.TRANSITION_SUBSTRING}{keywordText} razor keyword.",
                new TextEditorTextSpan(
                    stringWalker.PositionIndex,
                    stringWalker.PositionIndex + 1,
                    (byte)HtmlDecorationKind.None,
                    stringWalker.ResourceUri,
                    stringWalker.SourceText));

            break;
        }

        tagSyntaxes = null;
        return false;
    }

    private bool TryReadExplicitInlineExpression(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition,
        string keywordText,
        out List<IHtmlSyntaxNode>? tagSyntaxes)
    {
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (stringWalker.CurrentCharacter == RazorFacts.EXPLICIT_EXPRESSION_START)
            {
                var explicitExpressionTagSyntaxes = ReadExplicitInlineExpression(
                    stringWalker,
                    textEditorHtmlDiagnosticBag,
                    injectedLanguageDefinition);

                tagSyntaxes = explicitExpressionTagSyntaxes;
                return true;
            }

            if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                continue;

            textEditorHtmlDiagnosticBag.Report(
                TextEditorDiagnosticLevel.Error,
                $"An explicit expression predicate was expected to follow the {RazorFacts.TRANSITION_SUBSTRING}{keywordText} razor keyword.",
                new TextEditorTextSpan(
                    stringWalker.PositionIndex,
                    stringWalker.PositionIndex + 1,
                    (byte)HtmlDecorationKind.None,
                    stringWalker.ResourceUri,
                    stringWalker.SourceText));

            break;
        }

        tagSyntaxes = null;
        return false;
    }

    private bool TryReadElseIf(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition,
        string keywordText,
        out List<IHtmlSyntaxNode>? tagSyntaxes)
    {
        tagSyntaxes = new List<IHtmlSyntaxNode>();

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            var elseIfKeywordCombo =
                $"{CSharpRazorKeywords.ELSE_KEYWORD} {CSharpRazorKeywords.IF_KEYWORD}";

            if (stringWalker.CheckForSubstring(elseIfKeywordCombo))
            {
                // Syntax highlight the keyword as a razor keyword specifically
                {
                    tagSyntaxes.Add(
                        new InjectedLanguageFragmentNode(
                            ImmutableArray<IHtmlSyntax>.Empty,
                            new TextEditorTextSpan(
                                stringWalker.PositionIndex,
                                stringWalker.PositionIndex +
                                elseIfKeywordCombo.Length,
                                (byte)HtmlDecorationKind.InjectedLanguageFragment,
                                stringWalker.ResourceUri,
                                stringWalker.SourceText)));

                    // -1 is in the case that "else{" instead of a space between "else" and "{"
                    _ = stringWalker
                        .ReadRange(elseIfKeywordCombo.Length - 1);
                }

                if (!TryReadExplicitInlineExpression(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.IF_KEYWORD,
                        out var explicitExpressionTagSyntaxes) ||
                    explicitExpressionTagSyntaxes is null)
                {
                    break;
                }

                tagSyntaxes.AddRange(explicitExpressionTagSyntaxes);

                if (TryReadCodeBlock(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.ELSE_KEYWORD,
                        out var codeBlockTagSyntaxes) &&
                    codeBlockTagSyntaxes is not null)
                {
                    tagSyntaxes.AddRange(codeBlockTagSyntaxes);
                    return true;
                }

                break;
            }

            if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                continue;

            break;
        }

        tagSyntaxes = tagSyntaxes.Any()
            ? tagSyntaxes
            : null;

        return false;
    }

    private bool TryReadElse(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition,
        string keywordText,
        out List<IHtmlSyntaxNode>? tagSyntaxes)
    {
        tagSyntaxes = new List<IHtmlSyntaxNode>();

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (stringWalker.CheckForSubstring(CSharpRazorKeywords.ELSE_KEYWORD))
            {
                // Syntax highlight the keyword as a razor keyword specifically
                {
                    tagSyntaxes.Add(
                        new InjectedLanguageFragmentNode(
                            ImmutableArray<IHtmlSyntax>.Empty,
                            new TextEditorTextSpan(
                                stringWalker.PositionIndex,
                                stringWalker.PositionIndex +
                                CSharpRazorKeywords.ELSE_KEYWORD.Length,
                                (byte)HtmlDecorationKind.InjectedLanguageFragment,
                                stringWalker.ResourceUri,
                                stringWalker.SourceText)));

                    // -1 is in the case that "else{" instead of a space between "else" and "{"
                    _ = stringWalker
                        .ReadRange(CSharpRazorKeywords.ELSE_KEYWORD.Length - 1);
                }

                if (TryReadCodeBlock(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.ELSE_KEYWORD,
                        out var codeBlockTagSyntaxes) &&
                    codeBlockTagSyntaxes is not null)
                {
                    tagSyntaxes.AddRange(codeBlockTagSyntaxes);
                    return true;
                }

                break;
            }

            if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                continue;

            break;
        }

        tagSyntaxes = null;
        return false;
    }

    private bool TryReadWhileOfDoWhile(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition,
        string keywordText,
        out List<IHtmlSyntaxNode>? tagSyntaxes)
    {
        tagSyntaxes = new List<IHtmlSyntaxNode>();

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (stringWalker.CheckForSubstring(CSharpRazorKeywords.WHILE_KEYWORD))
            {
                // Syntax highlight the keyword as a razor keyword specifically
                {
                    tagSyntaxes.Add(
                        new InjectedLanguageFragmentNode(
                            ImmutableArray<IHtmlSyntax>.Empty,
                            new TextEditorTextSpan(
                                stringWalker.PositionIndex,
                                stringWalker.PositionIndex +
                                CSharpRazorKeywords.WHILE_KEYWORD.Length,
                                (byte)HtmlDecorationKind.InjectedLanguageFragment,
                                stringWalker.ResourceUri,
                                stringWalker.SourceText)));

                    // -1 is in the case that "while()" instead of a space between "while" and "("
                    _ = stringWalker
                        .ReadRange(CSharpRazorKeywords.WHILE_KEYWORD.Length - 1);
                }

                if (TryReadExplicitInlineExpression(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.ELSE_KEYWORD,
                        out var explicitExpressionTagSyntaxes) &&
                    explicitExpressionTagSyntaxes is not null)
                {
                    tagSyntaxes.AddRange(explicitExpressionTagSyntaxes);
                    return true;
                }

                break;
            }

            if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                continue;

            break;
        }

        tagSyntaxes = null;
        return false;
    }

    private List<IHtmlSyntaxNode> ParseCSharpWithAdhocClassWrapping(
        string cSharpText,
        int offsetPositionIndex)
    {
        // Testing something
        {
            var delayedTextInsertion = new TEST_delayed_text_insertion(
                cSharpText,
                offsetPositionIndex)
            {
                StartInsertionPositionIndexInclusive = _classBuilder.Length
            };

            _classBuilder.Append(cSharpText);

            delayedTextInsertion.EndInsertionPositionIndexExclusive = _classBuilder.Length;

            DelayedClassBuilder.Add(delayedTextInsertion);
        }

        var classTemplateOpening = "public class Aaa{";

        var injectedLanguageString = classTemplateOpening +
                                     cSharpText;

        return ParseCSharp(
            injectedLanguageString,
            classTemplateOpening.Length,
            offsetPositionIndex);
    }

    private List<IHtmlSyntaxNode> ParseCSharpWithAdhocMethodWrapping(
        string cSharpText,
        int offsetPositionIndex)
    {
        // Testing something
        {
            var delayedTextInsertion = new TEST_delayed_text_insertion(
                cSharpText,
                offsetPositionIndex)
            {
                StartInsertionPositionIndexInclusive = _renderFunctionBuilder.Length
            };

            _renderFunctionBuilder.Append(cSharpText);

            delayedTextInsertion.EndInsertionPositionIndexExclusive = _renderFunctionBuilder.Length;

            DelayedRenderFunctionBuilder.Add(delayedTextInsertion);
        }

        var classTemplateOpening = "public class Aaa{public void Bbb(){";

        var injectedLanguageString = classTemplateOpening +
                                     cSharpText;

        return ParseCSharp(
            injectedLanguageString,
            classTemplateOpening.Length,
            offsetPositionIndex);
    }

    /// <summary> If Lexing C# from a razor code block one must either use <see cref="ParseCSharpWithAdhocClassWrapping"/> for an @code{} section or <see cref="ParseCSharpWithAdhocMethodWrapping"/> for a basic @{} block</summary>
    private List<IHtmlSyntaxNode> ParseCSharp(
        string cSharpText,
        int adhocTemplateOpeningLength,
        int offsetPositionIndex)
    {
        var injectedLanguageFragmentSyntaxes = new List<IHtmlSyntaxNode>();

        var lexer = new TextEditorCSharpLexer(new ResourceUri(string.Empty));

        var lexedInjectedLanguage = lexer.Lex(
                cSharpText,
                RenderStateKey.NewRenderStateKey())
            .Result;

        foreach (var lexedTokenTextSpan in lexedInjectedLanguage)
        {
            var startingIndexInclusive = lexedTokenTextSpan.StartingIndexInclusive +
                                         offsetPositionIndex -
                                         adhocTemplateOpeningLength;

            var endingIndexExclusive = lexedTokenTextSpan.EndingIndexExclusive +
                                       offsetPositionIndex -
                                       adhocTemplateOpeningLength;

            // startingIndexInclusive < 0 means it was part of the class template that was prepended so roslyn would recognize methods
            if (lexedTokenTextSpan.StartingIndexInclusive - adhocTemplateOpeningLength
                < 0)
                continue;

            var cSharpDecorationKind = (GenericDecorationKind)lexedTokenTextSpan.DecorationByte;

            switch (cSharpDecorationKind)
            {
                case GenericDecorationKind.None:
                    break;
                case GenericDecorationKind.Function:
                    {
                        var razorMethodTextSpan = lexedTokenTextSpan with
                        {
                            DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageMethod,
                            StartingIndexInclusive = startingIndexInclusive,
                            EndingIndexExclusive = endingIndexExclusive,
                        };

                        injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentNode(
                            ImmutableArray<IHtmlSyntax>.Empty,
                            razorMethodTextSpan));

                        break;
                    }
                    // case CSharpDecorationKind.Type:
                    {
                        //     var razorTypeTextSpan = lexedTokenTextSpan with
                        //     {
                        //         DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageType,
                        //         StartingIndexInclusive = startingIndexInclusive,
                        //         EndingIndexExclusive = endingIndexExclusive,
                        //     };
                        //
                        //     injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                        //         ImmutableArray<IHtmlSyntax>.Empty,
                        //         string.Empty,
                        //         razorTypeTextSpan));
                        //
                        //     break;
                    }
                    // case CSharpDecorationKind.Parameter:
                    {
                        //     var razorVariableTextSpan = lexedTokenTextSpan with
                        //     {
                        //         DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageVariable,
                        //         StartingIndexInclusive = startingIndexInclusive,
                        //         EndingIndexExclusive = endingIndexExclusive,
                        //     };
                        //
                        //     injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                        //         ImmutableArray<IHtmlSyntax>.Empty,
                        //         string.Empty,
                        //         razorVariableTextSpan));
                        //
                        //     break;
                    }
                case GenericDecorationKind.StringLiteral:
                    {
                        var razorStringLiteralTextSpan = lexedTokenTextSpan with
                        {
                            DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageStringLiteral,
                            StartingIndexInclusive = startingIndexInclusive,
                            EndingIndexExclusive = endingIndexExclusive,
                        };

                        injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentNode(
                            ImmutableArray<IHtmlSyntax>.Empty,
                            razorStringLiteralTextSpan));

                        break;
                    }
                case GenericDecorationKind.Keyword:
                    {
                        var razorKeywordTextSpan = lexedTokenTextSpan with
                        {
                            DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageKeyword,
                            StartingIndexInclusive = startingIndexInclusive,
                            EndingIndexExclusive = endingIndexExclusive,
                        };

                        injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentNode(
                            ImmutableArray<IHtmlSyntax>.Empty,
                            razorKeywordTextSpan));

                        break;
                    }
                case GenericDecorationKind.CommentSingleLine:
                    {
                        var razorCommentTextSpan = lexedTokenTextSpan with
                        {
                            DecorationByte = (byte)HtmlDecorationKind.Comment,
                            StartingIndexInclusive = startingIndexInclusive,
                            EndingIndexExclusive = endingIndexExclusive,
                        };

                        injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentNode(
                            ImmutableArray<IHtmlSyntax>.Empty,
                            razorCommentTextSpan));

                        break;
                    }
                case GenericDecorationKind.CommentMultiLine:
                    {
                        var razorCommentTextSpan = lexedTokenTextSpan with
                        {
                            DecorationByte = (byte)HtmlDecorationKind.Comment,
                            StartingIndexInclusive = startingIndexInclusive,
                            EndingIndexExclusive = endingIndexExclusive,
                        };

                        injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentNode(
                            ImmutableArray<IHtmlSyntax>.Empty,
                            razorCommentTextSpan));

                        break;
                    }
            }
        }

        return injectedLanguageFragmentSyntaxes;
    }
}