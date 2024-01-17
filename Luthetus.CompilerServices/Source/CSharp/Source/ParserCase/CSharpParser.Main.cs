using Luthetus.CompilerServices.Lang.CSharp.BinderCase;
using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase;

/// <summary>
/// Summary: Start in <em>'CSharpParser.LoopApi.cs'</em>, then go to
/// <em>'CSharpParser.ParseApi.cs'</em>, and ultimately go to <em>'CSharpParser.HandleApi.cs'</em>.
/// </summary>
public partial class CSharpParser : IParser
{
    private readonly TokenWalker _tokenWalker;
    private readonly CodeBlockBuilder _globalCodeBlockBuilder;
    private readonly LuthetusDiagnosticBag _diagnosticBag = new();
    private readonly GeneralApi _general;
    private readonly SpecificApi _specific;
    private readonly UtilityApi _utility;
    private readonly Stack<ISyntax> _expressionStack = new();

    public CSharpParser(CSharpLexer lexer)
    {
        Lexer = lexer;
        _tokenWalker = new TokenWalker(lexer.SyntaxTokens, _diagnosticBag);
        Binder = new CSharpBinder();

        _globalCodeBlockBuilder = new(null, null);
        _currentCodeBlockBuilder = _globalCodeBlockBuilder;

        _general = new(this);
        _specific = new(this);
        _utility = new(this);
    }

    /// <summary>If a file scoped namespace is found, then set this field, so that prior to finishing the parser constructs the namespace node.</summary>
    private Action<CodeBlockNode>? _finalizeNamespaceFileScopeCodeBlockNodeAction;
    private ISyntaxNode? _nodeRecent;
    private CodeBlockBuilder _currentCodeBlockBuilder;
    /// <summary>When parsing the body of a function this is used in order to keep the function definition node itself in the syntax tree immutable.<br/><br/>That is to say, this action would create the function definition node and then append it.</summary>
    private Stack<Action<CodeBlockNode>> _finalizeCodeBlockNodeActionStack = new();

    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList => _diagnosticBag.ToImmutableArray();
    public CSharpBinder Binder { get; private set; }
    public CSharpLexer Lexer { get; }

    /// <summary>This method is used when parsing many files as a single compilation. The first binder instance would be passed to the following parsers. The resourceUri is passed in so if a file is parsed for a second time, the previous symbols can be deleted so they do not duplicate.</summary>
    public CompilationUnit Parse(
        CSharpBinder previousBinder,
        ResourceUri resourceUri)
    {
        Binder = previousBinder;
        Binder.CurrentResourceUri = resourceUri;
        Binder.ClearStateByResourceUri(resourceUri);

        return Parse();
    }

    public CompilationUnit Parse()
    {
        while (true)
        {
            var consumedToken = _tokenWalker.Consume();

            switch (consumedToken.SyntaxKind)
            {
                case SyntaxKind.NumericLiteralToken:
                    _general.ParseNumericLiteralToken((NumericLiteralToken)consumedToken);
                    break;
                case SyntaxKind.StringLiteralToken:
                    _general.ParseStringLiteralToken((StringLiteralToken)consumedToken);
                    break;
                case SyntaxKind.PlusToken:
                    _general.ParsePlusToken((PlusToken)consumedToken);
                    break;
                case SyntaxKind.PlusPlusToken:
                    _general.ParsePlusPlusToken((PlusPlusToken)consumedToken);
                    break;
                case SyntaxKind.MinusToken:
                    _general.ParseMinusToken((MinusToken)consumedToken);
                    break;
                case SyntaxKind.StarToken:
                    _general.ParseStarToken((StarToken)consumedToken);
                    break;
                case SyntaxKind.PreprocessorDirectiveToken:
                    _general.ParsePreprocessorDirectiveToken((PreprocessorDirectiveToken)consumedToken);
                    break;
                case SyntaxKind.CommentSingleLineToken:
                    // Do not parse comments.
                    break;
                case SyntaxKind.IdentifierToken:
                    _general.ParseIdentifierToken((IdentifierToken)consumedToken);
                    break;
                case SyntaxKind.OpenBraceToken:
                    _general.ParseOpenBraceToken((OpenBraceToken)consumedToken);
                    break;
                case SyntaxKind.CloseBraceToken:
                    _general.ParseCloseBraceToken((CloseBraceToken)consumedToken);
                    break;
                case SyntaxKind.OpenParenthesisToken:
                    _general.ParseOpenParenthesisToken((OpenParenthesisToken)consumedToken);
                    break;
                case SyntaxKind.CloseParenthesisToken:
                    _general.ParseCloseParenthesisToken((CloseParenthesisToken)consumedToken);
                    break;
                case SyntaxKind.OpenAngleBracketToken:
                    _general.ParseOpenAngleBracketToken((OpenAngleBracketToken)consumedToken);
                    break;
                case SyntaxKind.CloseAngleBracketToken:
                    _general.ParseCloseAngleBracketToken((CloseAngleBracketToken)consumedToken);
                    break;
                case SyntaxKind.OpenSquareBracketToken:
                    _general.ParseOpenSquareBracketToken((OpenSquareBracketToken)consumedToken);
                    break;
                case SyntaxKind.CloseSquareBracketToken:
                    _general.ParseCloseSquareBracketToken((CloseSquareBracketToken)consumedToken);
                    break;
                case SyntaxKind.DollarSignToken:
                    _general.ParseDollarSignToken((DollarSignToken)consumedToken);
                    break;
                case SyntaxKind.ColonToken:
                    _general.ParseColonToken((ColonToken)consumedToken);
                    break;
                case SyntaxKind.MemberAccessToken:
                    _general.ParseMemberAccessToken((MemberAccessToken)consumedToken);
                    break;
                case SyntaxKind.StatementDelimiterToken:
                    _general.ParseStatementDelimiterToken((StatementDelimiterToken)consumedToken);
                    break;
                case SyntaxKind.EndOfFileToken:
                    if (_nodeRecent is IExpressionNode)
                    {
                        _currentCodeBlockBuilder.ChildList.Add(_nodeRecent);
                    }
                    else if (_nodeRecent is AmbiguousIdentifierNode)
                    {
                        _currentCodeBlockBuilder.ChildList.Add(_nodeRecent);
                    }
                    break;
                default:
                    if (_utility.IsContextualKeywordSyntaxKind(consumedToken.SyntaxKind))
                        _general.ParseKeywordContextualToken((KeywordContextualToken)consumedToken);
                    else if (_utility.IsKeywordSyntaxKind(consumedToken.SyntaxKind))
                        _general.ParseKeywordToken((KeywordToken)consumedToken);
                    break;
            }

            if (consumedToken.SyntaxKind == SyntaxKind.EndOfFileToken)
                break;
        }

        if (_finalizeNamespaceFileScopeCodeBlockNodeAction is not null &&
            _currentCodeBlockBuilder.Parent is not null)
        {
            // The current token here would be the EOF token.
            Binder.DisposeBoundScope(_tokenWalker.Current.TextSpan);

            _finalizeNamespaceFileScopeCodeBlockNodeAction.Invoke(
                _currentCodeBlockBuilder.Build());

            _currentCodeBlockBuilder = _currentCodeBlockBuilder.Parent;
        }

        var topLevelStatementsCodeBlock = _currentCodeBlockBuilder.Build(
            DiagnosticsList
                .Union(Binder.DiagnosticsList)
                .Union(Lexer.DiagnosticList)
                .ToImmutableArray());

        return new CompilationUnit(
            topLevelStatementsCodeBlock,
            Lexer,
            this,
            Binder);
    }
}