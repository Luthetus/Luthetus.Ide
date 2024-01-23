using Luthetus.CompilerServices.Lang.CSharp.BinderCase;
using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase.Internals;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase;

public class CSharpParser : IParser
{
    public CSharpParser(CSharpLexer lexer)
    {
        Lexer = lexer;
        Binder = new CSharpBinder();
        Binder.CurrentResourceUri = lexer.ResourceUri;
    }

    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList { get; private set; } = ImmutableArray<TextEditorDiagnostic>.Empty;
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
        var globalCodeBlockBuilder = new CodeBlockBuilder(null, null);
        var currentCodeBlockBuilder = globalCodeBlockBuilder;
        var diagnosticBag = new LuthetusDiagnosticBag();

        var model = new ParserModel(
            Binder,
            new TokenWalker(Lexer.SyntaxTokens, diagnosticBag),
            new Stack<ISyntax>(),
            diagnosticBag,
            globalCodeBlockBuilder,
            currentCodeBlockBuilder,
            null,
            new Stack<Action<CodeBlockNode>>());

        while (true)
        {
            var token = model.TokenWalker.Consume();

            switch (token.SyntaxKind)
            {
                case SyntaxKind.NumericLiteralToken:
                    ParseTokens.ParseNumericLiteralToken((NumericLiteralToken)token, model);
                    break;
                case SyntaxKind.StringLiteralToken:
                    ParseTokens.ParseStringLiteralToken((StringLiteralToken)token, model);
                    break;
                case SyntaxKind.PlusToken:
                    ParseTokens.ParsePlusToken((PlusToken)token, model);
                    break;
                case SyntaxKind.PlusPlusToken:
                    ParseTokens.ParsePlusPlusToken((PlusPlusToken)token, model);
                    break;
                case SyntaxKind.MinusToken:
                    ParseTokens.ParseMinusToken((MinusToken)token, model);
                    break;
                case SyntaxKind.StarToken:
                    ParseTokens.ParseStarToken((StarToken)token, model);
                    break;
                case SyntaxKind.PreprocessorDirectiveToken:
                    ParseTokens.ParsePreprocessorDirectiveToken((PreprocessorDirectiveToken)token, model);
                    break;
                case SyntaxKind.CommentSingleLineToken:
                    // Do not parse comments.
                    break;
                case SyntaxKind.IdentifierToken:
                    ParseTokens.ParseIdentifierToken((IdentifierToken)token, model);
                    break;
                case SyntaxKind.OpenBraceToken:
                    ParseTokens.ParseOpenBraceToken((OpenBraceToken)token, model);
                    break;
                case SyntaxKind.CloseBraceToken:
                    ParseTokens.ParseCloseBraceToken((CloseBraceToken)token, model);
                    break;
                case SyntaxKind.OpenParenthesisToken:
                    ParseTokens.ParseOpenParenthesisToken((OpenParenthesisToken)token, model);
                    break;
                case SyntaxKind.CloseParenthesisToken:
                    ParseTokens.ParseCloseParenthesisToken((CloseParenthesisToken)token, model);
                    break;
                case SyntaxKind.OpenAngleBracketToken:
                    ParseTokens.ParseOpenAngleBracketToken((OpenAngleBracketToken)token, model);
                    break;
                case SyntaxKind.CloseAngleBracketToken:
                    ParseTokens.ParseCloseAngleBracketToken((CloseAngleBracketToken)token, model);
                    break;
                case SyntaxKind.OpenSquareBracketToken:
                    ParseTokens.ParseOpenSquareBracketToken((OpenSquareBracketToken)token, model);
                    break;
                case SyntaxKind.CloseSquareBracketToken:
                    ParseTokens.ParseCloseSquareBracketToken((CloseSquareBracketToken)token, model);
                    break;
                case SyntaxKind.DollarSignToken:
                    ParseTokens.ParseDollarSignToken((DollarSignToken)token, model);
                    break;
                case SyntaxKind.ColonToken:
                    ParseTokens.ParseColonToken((ColonToken)token, model);
                    break;
                case SyntaxKind.MemberAccessToken:
                    ParseTokens.ParseMemberAccessToken((MemberAccessToken)token, model);
                    break;
                case SyntaxKind.EqualsToken:
                    ParseTokens.ParseEqualsToken((EqualsToken)token, model);
                    break;
                case SyntaxKind.StatementDelimiterToken:
                    ParseTokens.ParseStatementDelimiterToken((StatementDelimiterToken)token, model);
                    break;
                case SyntaxKind.EndOfFileToken:
                    if (model.SyntaxStack.TryPeek(out var syntax) &&
                        syntax is EndOfFileToken)
                    {
                        _ = model.SyntaxStack.Pop();
                    }

                    if (model.SyntaxStack.TryPop(out var notUsedSyntax))
                    {
                        if (notUsedSyntax is IExpressionNode ||
                            notUsedSyntax is AmbiguousIdentifierNode)
                        {
                            model.CurrentCodeBlockBuilder.ChildList.Add(notUsedSyntax);
                        }
                    }
                    break;
                default:
                    if (UtilityApi.IsContextualKeywordSyntaxKind(token.SyntaxKind))
                        ParseTokens.ParseKeywordContextualToken((KeywordContextualToken)token, model);
                    else if (UtilityApi.IsKeywordSyntaxKind(token.SyntaxKind))
                        ParseTokens.ParseKeywordToken((KeywordToken)token, model);
                    break;
            }

            if (token.SyntaxKind == SyntaxKind.EndOfFileToken)
                break;
        }

        if (model.FinalizeNamespaceFileScopeCodeBlockNodeAction is not null &&
            model.CurrentCodeBlockBuilder.Parent is not null)
        {
            // The current token here would be the EOF token.
            Binder.DisposeBoundScope(model.TokenWalker.Current.TextSpan);

            model.FinalizeNamespaceFileScopeCodeBlockNodeAction.Invoke(
                model.CurrentCodeBlockBuilder.Build());

            model.CurrentCodeBlockBuilder = model.CurrentCodeBlockBuilder.Parent;
        }

        DiagnosticsList = DiagnosticsList.AddRange(model.DiagnosticBag.ToImmutableArray());

        var topLevelStatementsCodeBlock = model.CurrentCodeBlockBuilder.Build(
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