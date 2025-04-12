using System.Text;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.BinderCase;
using Luthetus.CompilerServices.Razor.CompilerServiceCase;
using Luthetus.CompilerServices.Razor.Facts;
using Luthetus.CompilerServices.Xml.Html;
using Luthetus.CompilerServices.Xml.Html.Decoration;
using Luthetus.CompilerServices.Xml.Html.Facts;
using Luthetus.CompilerServices.Xml.Html.InjectedLanguage;
using Luthetus.CompilerServices.Xml.Html.SyntaxActors;
using Luthetus.CompilerServices.Xml.Html.SyntaxObjects;

namespace Luthetus.CompilerServices.Razor;

public class RazorSyntaxTree
{
    public readonly string _codebehindClassIdentifier;
    public readonly string _codebehindRenderFunctionIdentifier;
    private readonly StringBuilder _codebehindClassBuilder;
    private readonly StringBuilder _codebehindRenderFunctionBuilder;
    private readonly List<AdhocTextInsertion> _codebehindClassInsertions = new();
    private readonly List<AdhocTextInsertion> _codebehindRenderFunctionInsertions = new();
    /// <summary>
    /// This <see cref="MarkupResourceUri"/> is for the '.razor.cs' file itself.
    /// If no .razor.cs files exist, then one will be created behind the scenes.
    /// </summary>
    private readonly ResourceUri _codebehindResourceUri;
    private readonly RazorCompilerService _razorCompilerService;
    private readonly CSharpCompilerService _cSharpCompilerService;
    private readonly IEnvironmentProvider _environmentProvider;

    public RazorSyntaxTree(
        ResourceUri resourceUri,
        RazorCompilerService razorCompilerService,
        CSharpCompilerService cSharpCompilerService,
        IEnvironmentProvider environmentProvider)
    {
        MarkupResourceUri = resourceUri;
        _razorCompilerService = razorCompilerService;
        _cSharpCompilerService = cSharpCompilerService;
        _environmentProvider = environmentProvider;

        var absolutePath = _environmentProvider.AbsolutePathFactory(resourceUri.Value, false);

        _codebehindClassIdentifier = absolutePath.NameNoExtension;
        _codebehindRenderFunctionIdentifier = "__LUTHETUS_RENDER_FUNCTION__";

        _codebehindClassBuilder = new($"public class {_codebehindClassIdentifier}\n{{");
        _codebehindRenderFunctionBuilder = new($"public void {_codebehindRenderFunctionIdentifier}()\n\t{{");

        _codebehindResourceUri = new ResourceUri($"{resourceUri.Value}.cs");
    }

    /// <summary>
    /// This <see cref="MarkupResourceUri"/> is for the '.razor' file itself.
    /// </summary>
    public ResourceUri MarkupResourceUri { get; }
    public SemanticResultRazor? SemanticResultRazor { get; private set; }

    /// <summary>
    /// The <see cref="ParseCodebehind"/> method combines the
    /// <see cref="_codebehindClassInsertions"/> and the
    /// <see cref="_codebehindRenderFunctionInsertions"/>
    /// into an 'adhoc' class so it can be parsed with the C# parser.
    /// </summary>
    public void ParseCodebehind()
    {
        StringWalker? stringWalker = null;

        if (_codebehindClassInsertions.Any())
            stringWalker = _codebehindClassInsertions.First().StringWalker;
        else if (_codebehindRenderFunctionInsertions.Any())
            stringWalker = _codebehindRenderFunctionInsertions.First().StringWalker;

        if (stringWalker is null)
            return;

        _codebehindClassBuilder.Append("\n\t");

        var renderFunctionAdhocTextInsertion = AdhocTextInsertion.PerformInsertion(
            _codebehindRenderFunctionBuilder.ToString(),
            0,
            _codebehindClassBuilder,
            stringWalker);

        foreach (var renderFunctionInsertion in _codebehindRenderFunctionInsertions)
        {
            renderFunctionInsertion.InsertionStartingIndexInclusive +=
                renderFunctionAdhocTextInsertion.InsertionStartingIndexInclusive;
        }

        _codebehindClassBuilder.Append("\n\t}\n}");

        var classContents = _codebehindClassBuilder.ToString();

		var cSharpBinder = new CSharpBinder();
		
        var compilationUnit = new CSharpCompilationUnit(_codebehindResourceUri);
			
		var lexerOutput = CSharpLexer.Lex(_codebehindResourceUri, classContents);
		
		cSharpBinder.StartCompilationUnit(_codebehindResourceUri);
		
		CSharpParser.Parse(compilationUnit, cSharpBinder, ref lexerOutput);
        
        SemanticResultRazor = new SemanticResultRazor(
            compilationUnit,
            cSharpBinder,
            _codebehindClassInsertions,
            _codebehindRenderFunctionInsertions,
            renderFunctionAdhocTextInsertion,
            classContents);
    }

    /// <summary>currentCharacterIn:<br/> -<see cref="InjectedLanguageDefinition.TransitionSubstring"/><br/></summary>
    public List<IHtmlSyntaxNode> ParseInjectedLanguageFragment(
        StringWalker stringWalker,
        List<TextEditorDiagnostic> diagnosticList,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        // current character is '@'
        _ = stringWalker.ReadCharacter();

        if (WhitespaceFacts.ALL_LIST.Contains(stringWalker.CurrentCharacter))
        {
            /*diagnosticBag.ReportRazorWhitespaceImmediatelyFollowingTransitionCharacterIsUnexpected(
                new TextEditorTextSpan(
                    stringWalker.PositionIndex,
                    stringWalker.PositionIndex + 1,
                    (byte)HtmlDecorationKind.None,
                    stringWalker.ResourceUri,
                    stringWalker.SourceText));*/

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
                    diagnosticList,
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
                    diagnosticList,
                    injectedLanguageDefinition,
                    foundString);
            }
        }

        if (stringWalker.CurrentCharacter == RazorFacts.COMMENT_START)
        {
            return ReadComment(
                stringWalker,
                diagnosticList,
                injectedLanguageDefinition);
        }

        if (stringWalker.CurrentCharacter == RazorFacts.SINGLE_LINE_TEXT_OUTPUT_WITHOUT_ADDING_HTML_ELEMENT)
        {
            return ReadSingleLineTextOutputWithoutAddingHtmlElement(
                stringWalker,
                diagnosticList,
                injectedLanguageDefinition);
        }

        if (stringWalker.CurrentCharacter == RazorFacts.CODE_BLOCK_START)
        {
            return ReadCodeBlock(
                stringWalker,
                diagnosticList,
                injectedLanguageDefinition,
                false);
        }

        // TODO: Check for invalid expressions
        return ReadInlineExpression(
            stringWalker,
            diagnosticList,
            injectedLanguageDefinition);
    }

    public void ParseTagName(
        StringWalker stringWalker,
        List<TextEditorDiagnostic> diagnosticList,
        InjectedLanguageDefinition injectedLanguageDefinition,
        TextEditorTextSpan textSpan)
    {
        var allTypeDefinitions = _cSharpCompilerService.__CSharpBinder.AllTypeDefinitions;

        var text = textSpan.GetText();
        
        var matchingKvps = allTypeDefinitions.Where(x => x.Key == text);

        foreach (var kvp in matchingKvps)
        {
        	var typeDefinitionNode = kvp.Value;
        	
            if (typeDefinitionNode is not null && typeDefinitionNode.InheritedTypeReference != TypeFacts.NotApplicable.ToTypeReference())
            {
                var inheritanceIdentifierText = typeDefinitionNode
                    .InheritedTypeReference.TypeIdentifierToken.TextSpan.GetText();

                if (inheritanceIdentifierText != "ComponentBase")
                    continue;

                var compilerServiceResource = _razorCompilerService.GetResource(textSpan.ResourceUri);

                if (compilerServiceResource is null)
                    continue;

                var razorResource = (RazorResource)compilerServiceResource;

                razorResource.HtmlSymbols.Add(new Symbol(
                	SyntaxKind.InjectedLanguageComponentSymbol,
            		symbolId: 0,
                	textSpan));
            }
        }
    }

    public static AttributeNameNode ParseAttributeName(
        StringWalker stringWalker,
        List<TextEditorDiagnostic> diagnosticList,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        var startingPositionIndex = stringWalker.PositionIndex;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (WhitespaceFacts.ALL_LIST.Contains(stringWalker.CurrentCharacter) ||
                HtmlFacts.SEPARATOR_FOR_ATTRIBUTE_NAME_AND_ATTRIBUTE_VALUE == stringWalker.CurrentCharacter ||
                stringWalker.PeekForSubstringRange(HtmlFacts.OPEN_TAG_ENDING_OPTIONS, out var matchedOn))
            {
                break;
            }
        }

        var attributeNameTextSpan = new TextEditorTextSpan(
            startingPositionIndex,
            stringWalker.PositionIndex,
            (byte)HtmlDecorationKind.InjectedLanguageFragment,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        return new AttributeNameNode(attributeNameTextSpan);
    }

    public static AttributeValueNode ParseAttributeValue(
        StringWalker stringWalker,
        List<TextEditorDiagnostic> diagnosticList,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        throw new NotImplementedException();
    }

    /// <summary> The @code{...} section must be wrapped in an adhoc class definition so that Roslyn can syntax highlight methods. <br/><br/> The @{...} code blocks must be wrapped in an adhoc method.</summary>
    private List<IHtmlSyntaxNode> ReadCodeBlock(
        StringWalker stringWalker,
        List<TextEditorDiagnostic> diagnosticList,
        InjectedLanguageDefinition injectedLanguageDefinition,
        bool isClassLevelCodeBlock)
    {
        var injectedLanguageFragmentSyntaxes = new List<IHtmlSyntaxNode>();

        var startingPositionIndex = stringWalker.PositionIndex;

        // Syntax highlight the CODE_BLOCK_START as a razor keyword specifically
        {
            injectedLanguageFragmentSyntaxes.Add(
                new InjectedLanguageFragmentNode(
                    Array.Empty<IHtmlSyntax>(),
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
                        diagnosticList,
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

                if (stringWalker.PeekForSubstring(singleLineTextOutputText))
                {
                    var positionIndexPriorReadingLine = stringWalker.PositionIndex;

                    var injectedLanguageFragmentSyntaxStartingPositionIndex =
                        stringWalker.PositionIndex;

                    // Track text span of the "@" sign
                    injectedLanguageFragmentSyntaxes.Add(
                        new InjectedLanguageFragmentNode(
                            Array.Empty<IHtmlSyntax>(),
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
                        diagnosticList,
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
                                Array.Empty<IHtmlSyntax>(),
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
                                positionIndexOffset,
                                stringWalker));
                    }
                    else
                    {
                        injectedLanguageFragmentSyntaxes.AddRange(
                            ParseCSharpWithAdhocMethodWrapping(
                                cSharpText,
                                positionIndexOffset,
                                stringWalker));
                    }

                    break;
                }
            }
        }

        return injectedLanguageFragmentSyntaxes;
    }

    private List<IHtmlSyntaxNode> ReadInlineExpression(
        StringWalker stringWalker,
        List<TextEditorDiagnostic> diagnosticList,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        if (stringWalker.CurrentCharacter == RazorFacts.EXPLICIT_EXPRESSION_START)
        {
            return ReadExplicitInlineExpression(
                stringWalker,
                diagnosticList,
                injectedLanguageDefinition);
        }

        return ReadImplicitInlineExpression(
            stringWalker,
            diagnosticList,
            injectedLanguageDefinition);
    }

    /// <summary>Example: @(myVariable)</summary>
    private List<IHtmlSyntaxNode> ReadExplicitInlineExpression(
        StringWalker stringWalker,
        List<TextEditorDiagnostic> diagnosticList,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        var injectedLanguageFragmentSyntaxes = new List<IHtmlSyntaxNode>();

        var startingPositionIndex = stringWalker.PositionIndex;

        // Syntax highlight the EXPLICIT_EXPRESSION_START as a razor keyword specifically
        {
            injectedLanguageFragmentSyntaxes.Add(
                new InjectedLanguageFragmentNode(
                    Array.Empty<IHtmlSyntax>(),
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
                                Array.Empty<IHtmlSyntax>(),
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
        List<TextEditorDiagnostic> diagnosticList,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        var injectedLanguageFragmentSyntaxes = new List<IHtmlSyntaxNode>();
        var entryPositionIndex = stringWalker.PositionIndex;

        while (!stringWalker.IsEof)
        {
            if (!char.IsLetterOrDigit(stringWalker.CurrentCharacter) &&
                stringWalker.CurrentCharacter != '_')
            {
                break;
            }

            _ = stringWalker.ReadCharacter();
        }

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        ParseCSharpWithAdhocMethodWrapping(
            textSpan.GetText(),
            entryPositionIndex,
            stringWalker);

        // TODO: Syntax highlighting
        return injectedLanguageFragmentSyntaxes;
    }

    /// <summary>Example: @if (true) { ... } else { ... }</summary>
    private List<IHtmlSyntaxNode> ReadCSharpRazorKeyword(
        StringWalker stringWalker,
        List<TextEditorDiagnostic> diagnosticList,
        InjectedLanguageDefinition injectedLanguageDefinition,
        string matchedOn)
    {
        var injectedLanguageFragmentSyntaxes = new List<IHtmlSyntaxNode>();

        // Syntax highlight the keyword as a razor keyword specifically
        {
            injectedLanguageFragmentSyntaxes.Add(
                new InjectedLanguageFragmentNode(
                    Array.Empty<IHtmlSyntax>(),
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
                            diagnosticList,
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
                            diagnosticList,
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
                            diagnosticList,
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
                            diagnosticList,
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
                            diagnosticList,
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
                            diagnosticList,
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
                            diagnosticList,
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
                            diagnosticList,
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
                               diagnosticList,
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
                            diagnosticList,
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
                            diagnosticList,
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
                            diagnosticList,
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
                            diagnosticList,
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
                            diagnosticList,
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
        List<TextEditorDiagnostic> diagnosticList,
        InjectedLanguageDefinition injectedLanguageDefinition,
        string matchedOn)
    {
        var injectedLanguageFragmentSyntaxes = new List<IHtmlSyntaxNode>();

        // Syntax highlight the keyword as a razor keyword specifically
        {
            injectedLanguageFragmentSyntaxes.Add(
                new InjectedLanguageFragmentNode(
                    Array.Empty<IHtmlSyntax>(),
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
                            diagnosticList,
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
        List<TextEditorDiagnostic> diagnosticList,
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
                Array.Empty<IHtmlSyntax>(),
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
            Array.Empty<IHtmlSyntax>(),
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
                Array.Empty<IHtmlSyntax>(),
                commentEndTextSpan);

            injectedLanguageFragmentSyntaxes.Add(commentEndSyntax);
        }

        return injectedLanguageFragmentSyntaxes;
    }

    /// <summary>Example: @* This is a razor comment *@</summary>
    private List<IHtmlSyntaxNode> ReadSingleLineTextOutputWithoutAddingHtmlElement(
        StringWalker stringWalker,
        List<TextEditorDiagnostic> diagnosticList,
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
                Array.Empty<IHtmlSyntax>(),
                commentStartTextSpan);

            injectedLanguageFragmentSyntaxes.Add(commentStartSyntax);
        }

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (WhitespaceFacts.LINE_ENDING_CHARACTER_LIST.Contains(stringWalker.CurrentCharacter))
            {
                break;
            }
        }

        return injectedLanguageFragmentSyntaxes;
    }

    private bool TryReadCodeBlock(
        StringWalker stringWalker,
        List<TextEditorDiagnostic> diagnosticList,
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
                    diagnosticList,
                    injectedLanguageDefinition,
                    isClassLevelCodeBlock);

                tagSyntaxes = codeBlockTagSyntaxes;
                return true;
            }

            if (WhitespaceFacts.ALL_LIST.Contains(stringWalker.CurrentCharacter))
                continue;

            /*diagnosticBag.ReportRazorCodeBlockWasExpectedToFollowRazorKeyword(
                RazorFacts.TRANSITION_SUBSTRING,
                keywordText,
                new TextEditorTextSpan(
                    stringWalker.PositionIndex,
                    stringWalker.PositionIndex + 1,
                    (byte)HtmlDecorationKind.None,
                    stringWalker.ResourceUri,
                    stringWalker.SourceText));*/

            break;
        }

        tagSyntaxes = null;
        return false;
    }

    private bool TryReadExplicitInlineExpression(
        StringWalker stringWalker,
        List<TextEditorDiagnostic> diagnosticList,
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
                    diagnosticList,
                    injectedLanguageDefinition);

                tagSyntaxes = explicitExpressionTagSyntaxes;
                return true;
            }

            if (WhitespaceFacts.ALL_LIST.Contains(stringWalker.CurrentCharacter))
                continue;

            /*diagnosticBag.ReportRazorExplicitExpressionPredicateWasExpected(
                RazorFacts.TRANSITION_SUBSTRING,
                keywordText,
                new TextEditorTextSpan(
                    stringWalker.PositionIndex,
                    stringWalker.PositionIndex + 1,
                    (byte)HtmlDecorationKind.None,
                    stringWalker.ResourceUri,
                    stringWalker.SourceText));*/

            break;
        }

        tagSyntaxes = null;
        return false;
    }

    private bool TryReadElseIf(
        StringWalker stringWalker,
        List<TextEditorDiagnostic> diagnosticList,
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

            if (stringWalker.PeekForSubstring(elseIfKeywordCombo))
            {
                // Syntax highlight the keyword as a razor keyword specifically
                {
                    tagSyntaxes.Add(
                        new InjectedLanguageFragmentNode(
                            Array.Empty<IHtmlSyntax>(),
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
                        diagnosticList,
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
                        diagnosticList,
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

            if (WhitespaceFacts.ALL_LIST.Contains(stringWalker.CurrentCharacter))
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
        List<TextEditorDiagnostic> diagnosticList,
        InjectedLanguageDefinition injectedLanguageDefinition,
        string keywordText,
        out List<IHtmlSyntaxNode>? tagSyntaxes)
    {
        tagSyntaxes = new List<IHtmlSyntaxNode>();

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (stringWalker.PeekForSubstring(CSharpRazorKeywords.ELSE_KEYWORD))
            {
                // Syntax highlight the keyword as a razor keyword specifically
                {
                    tagSyntaxes.Add(
                        new InjectedLanguageFragmentNode(
                            Array.Empty<IHtmlSyntax>(),
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
                        diagnosticList,
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

            if (WhitespaceFacts.ALL_LIST.Contains(stringWalker.CurrentCharacter))
                continue;

            break;
        }

        tagSyntaxes = null;
        return false;
    }

    private bool TryReadWhileOfDoWhile(
        StringWalker stringWalker,
        List<TextEditorDiagnostic> diagnosticList,
        InjectedLanguageDefinition injectedLanguageDefinition,
        string keywordText,
        out List<IHtmlSyntaxNode>? tagSyntaxes)
    {
        tagSyntaxes = new List<IHtmlSyntaxNode>();

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (stringWalker.PeekForSubstring(CSharpRazorKeywords.WHILE_KEYWORD))
            {
                // Syntax highlight the keyword as a razor keyword specifically
                {
                    tagSyntaxes.Add(
                        new InjectedLanguageFragmentNode(
                            Array.Empty<IHtmlSyntax>(),
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
                        diagnosticList,
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

            if (WhitespaceFacts.ALL_LIST.Contains(stringWalker.CurrentCharacter))
                continue;

            break;
        }

        tagSyntaxes = null;
        return false;
    }

    private List<IHtmlSyntaxNode> ParseCSharpWithAdhocClassWrapping(
        string cSharpText,
        int sourceTextStartingIndexInclusive,
        StringWalker stringWalker)
    {
        // Testing something
        {
            var adhocTextInsertion = AdhocTextInsertion.PerformInsertion(
                cSharpText,
                sourceTextStartingIndexInclusive,
                _codebehindClassBuilder,
                stringWalker);

            _codebehindClassInsertions.Add(adhocTextInsertion);
        }

        var classTemplateOpening = "public class Aaa{";

        var injectedLanguageString = classTemplateOpening +
                                     cSharpText;

        return ParseCSharp(
            injectedLanguageString,
            classTemplateOpening.Length,
            sourceTextStartingIndexInclusive);
    }

    private List<IHtmlSyntaxNode> ParseCSharpWithAdhocMethodWrapping(
        string cSharpText,
        int sourceTextStartingIndexInclusive,
        StringWalker stringWalker)
    {
        // Testing something
        {
            var adhocTextInsertion = AdhocTextInsertion.PerformInsertion(
                cSharpText,
                sourceTextStartingIndexInclusive,
                _codebehindRenderFunctionBuilder,
                stringWalker);

            _codebehindRenderFunctionInsertions.Add(adhocTextInsertion);
        }

        var classTemplateOpening = "public class Aaa{public void Bbb(){";

        var injectedLanguageString = classTemplateOpening +
                                     cSharpText;

        return ParseCSharp(
            injectedLanguageString,
            classTemplateOpening.Length,
            sourceTextStartingIndexInclusive);
    }

    /// <summary> If Lexing C# from a razor code block one must either use <see cref="ParseCSharpWithAdhocClassWrapping"/> for an @code{} section or <see cref="ParseCSharpWithAdhocMethodWrapping"/> for a basic @{} block</summary>
    private List<IHtmlSyntaxNode> ParseCSharp(
        string cSharpText,
        int adhocTemplateOpeningLength,
        int offsetPositionIndex)
    {
        var injectedLanguageFragmentSyntaxes = new List<IHtmlSyntaxNode>();

        var lexerOutput = CSharpLexer.Lex(ResourceUri.Empty, cSharpText);

        foreach (var lexedTokenTextSpan in lexerOutput.SyntaxTokenList.Select(x => x.TextSpan).Union(lexerOutput.MiscTextSpanList))
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
                            Array.Empty<IHtmlSyntax>(),
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
                            Array.Empty<IHtmlSyntax>(),
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
                            Array.Empty<IHtmlSyntax>(),
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
                            Array.Empty<IHtmlSyntax>(),
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
                            Array.Empty<IHtmlSyntax>(),
                            razorCommentTextSpan));

                        break;
                    }
            }
        }

        return injectedLanguageFragmentSyntaxes;
    }
}