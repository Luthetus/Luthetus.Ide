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

public partial class CSharpParser : IParser
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
            new CSharpBinder(),
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
            model.SyntaxStack.Push(token);

            switch (token.SyntaxKind)
            {
                case SyntaxKind.NumericLiteralToken:
                    TokenApi.ParseNumericLiteralToken(model);
                    break;
                case SyntaxKind.StringLiteralToken:
                    TokenApi.ParseStringLiteralToken(model);
                    break;
                case SyntaxKind.PlusToken:
                    TokenApi.ParsePlusToken(model);
                    break;
                case SyntaxKind.PlusPlusToken:
                    TokenApi.ParsePlusPlusToken(model);
                    break;
                case SyntaxKind.MinusToken:
                    TokenApi.ParseMinusToken(model);
                    break;
                case SyntaxKind.StarToken:
                    TokenApi.ParseStarToken(model);
                    break;
                case SyntaxKind.PreprocessorDirectiveToken:
                    TokenApi.ParsePreprocessorDirectiveToken(model);
                    break;
                case SyntaxKind.CommentSingleLineToken:
                    // Do not parse comments.
                    break;
                case SyntaxKind.IdentifierToken:
                    TokenApi.ParseIdentifierToken(model);
                    break;
                case SyntaxKind.OpenBraceToken:
                    TokenApi.ParseOpenBraceToken(model);
                    break;
                case SyntaxKind.CloseBraceToken:
                    TokenApi.ParseCloseBraceToken(model);
                    break;
                case SyntaxKind.OpenParenthesisToken:
                    TokenApi.ParseOpenParenthesisToken(model);
                    break;
                case SyntaxKind.CloseParenthesisToken:
                    TokenApi.ParseCloseParenthesisToken(model);
                    break;
                case SyntaxKind.OpenAngleBracketToken:
                    TokenApi.ParseOpenAngleBracketToken(model);
                    break;
                case SyntaxKind.CloseAngleBracketToken:
                    TokenApi.ParseCloseAngleBracketToken(model);
                    break;
                case SyntaxKind.OpenSquareBracketToken:
                    TokenApi.ParseOpenSquareBracketToken(model);
                    break;
                case SyntaxKind.CloseSquareBracketToken:
                    TokenApi.ParseCloseSquareBracketToken(model);
                    break;
                case SyntaxKind.DollarSignToken:
                    TokenApi.ParseDollarSignToken(model);
                    break;
                case SyntaxKind.ColonToken:
                    TokenApi.ParseColonToken(model);
                    break;
                case SyntaxKind.MemberAccessToken:
                    TokenApi.ParseMemberAccessToken(model);
                    break;
                case SyntaxKind.StatementDelimiterToken:
                    TokenApi.StatementDelimiterToken(model);
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
                        TokenApi.ParseKeywordContextualToken(model);
                    else if (UtilityApi.IsKeywordSyntaxKind(token.SyntaxKind))
                        TokenApi.ParseKeywordToken(model);
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