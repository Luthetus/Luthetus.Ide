using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.CompilerServices.CSharp.Facts;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public static class ParseTokens
{
    public static void ParsePreprocessorDirectiveToken(
        PreprocessorDirectiveToken consumedPreprocessorDirectiveToken,
        CSharpParserModel model)
    {
        var consumedToken = model.TokenWalker.Consume();
    }

    public static void ParseIdentifierToken(CSharpParserModel model)
    {
    	var identifierToken = (IdentifierToken)model.TokenWalker.Consume();
    }

    public static void ParsePlusToken(
        PlusToken consumedPlusToken,
        CSharpParserModel model)
    {
    }

    public static void ParsePlusPlusToken(
        PlusPlusToken consumedPlusPlusToken,
        CSharpParserModel model)
    {
    }

    public static void ParseMinusToken(
        MinusToken consumedMinusToken,
        CSharpParserModel model)
    {
    }

    public static void ParseStarToken(
        StarToken consumedStarToken,
        CSharpParserModel model)
    {
    }

    public static void ParseDollarSignToken(
        DollarSignToken consumedDollarSignToken,
        CSharpParserModel model)
    {
    }
    
    public static void ParseAtToken(
        AtToken consumedAtToken,
        CSharpParserModel model)
    {
    }

    public static void ParseColonToken(CSharpParserModel model)
    {
    	var colonToken = (ColonToken)model.TokenWalker.Consume();
    
        if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.TypeDefinitionNode)
        {
            var typeDefinitionNode = (TypeDefinitionNode)model.SyntaxStack.Pop();
            var inheritedTypeClauseNode = model.TokenWalker.MatchTypeClauseNode(model);

            model.Binder.BindTypeClauseNode(inheritedTypeClauseNode, model);

			typeDefinitionNode.SetInheritedTypeClauseNode(inheritedTypeClauseNode);

            model.SyntaxStack.Push(typeDefinitionNode);
            model.CurrentCodeBlockBuilder.PendingChild = typeDefinitionNode;
        }
        else
        {
            model.DiagnosticBag.ReportTodoException(colonToken.TextSpan, "Colon is in unexpected place.");
        }
    }

    public static void ParseOpenBraceToken(
        OpenBraceToken consumedOpenBraceToken,
        CSharpParserModel model)
    {
    }

    public static void ParseCloseBraceToken(
        CloseBraceToken consumedCloseBraceToken,
        CSharpParserModel model)
    {
    }

    public static void ParseOpenParenthesisToken(
        OpenParenthesisToken consumedOpenParenthesisToken,
        CSharpParserModel model)
    {
    }

    public static void ParseCloseParenthesisToken(
        CloseParenthesisToken consumedCloseParenthesisToken,
        CSharpParserModel model)
    {
    }

    public static void ParseOpenAngleBracketToken(
        OpenAngleBracketToken consumedOpenAngleBracketToken,
        CSharpParserModel model)
    {
    }

    public static void ParseCloseAngleBracketToken(
        CloseAngleBracketToken consumedCloseAngleBracketToken,
        CSharpParserModel model)
    {
    }

    public static void ParseOpenSquareBracketToken(
        OpenSquareBracketToken consumedOpenSquareBracketToken,
        CSharpParserModel model)
    {
    }

    public static void ParseCloseSquareBracketToken(
        CloseSquareBracketToken consumedCloseSquareBracketToken,
        CSharpParserModel model)
    {
    }

    public static void ParseEqualsToken(
        EqualsToken consumedEqualsToken,
        CSharpParserModel model)
    {
    }

    public static void ParseMemberAccessToken(
        MemberAccessToken consumedMemberAccessToken,
        CSharpParserModel model)
    {
    }

    public static void ParseStatementDelimiterToken(
        StatementDelimiterToken consumedStatementDelimiterToken,
        CSharpParserModel model)
    {
    }

    public static void ParseKeywordToken(CSharpParserModel model)
    {
        // 'return', 'if', 'get', etc...
        switch (model.TokenWalker.Current.SyntaxKind)
        {
            case SyntaxKind.AsTokenKeyword:
                ParseDefaultKeywords.HandleAsTokenKeyword(model);
                break;
            case SyntaxKind.BaseTokenKeyword:
                ParseDefaultKeywords.HandleBaseTokenKeyword(model);
                break;
            case SyntaxKind.BoolTokenKeyword:
                ParseDefaultKeywords.HandleBoolTokenKeyword(model);
                break;
            case SyntaxKind.BreakTokenKeyword:
                ParseDefaultKeywords.HandleBreakTokenKeyword(model);
                break;
            case SyntaxKind.ByteTokenKeyword:
                ParseDefaultKeywords.HandleByteTokenKeyword(model);
                break;
            case SyntaxKind.CaseTokenKeyword:
                ParseDefaultKeywords.HandleCaseTokenKeyword(model);
                break;
            case SyntaxKind.CatchTokenKeyword:
                ParseDefaultKeywords.HandleCatchTokenKeyword(model);
                break;
            case SyntaxKind.CharTokenKeyword:
                ParseDefaultKeywords.HandleCharTokenKeyword(model);
                break;
            case SyntaxKind.CheckedTokenKeyword:
                ParseDefaultKeywords.HandleCheckedTokenKeyword(model);
                break;
            case SyntaxKind.ConstTokenKeyword:
                ParseDefaultKeywords.HandleConstTokenKeyword(model);
                break;
            case SyntaxKind.ContinueTokenKeyword:
                ParseDefaultKeywords.HandleContinueTokenKeyword(model);
                break;
            case SyntaxKind.DecimalTokenKeyword:
                ParseDefaultKeywords.HandleDecimalTokenKeyword(model);
                break;
            case SyntaxKind.DefaultTokenKeyword:
                ParseDefaultKeywords.HandleDefaultTokenKeyword(model);
                break;
            case SyntaxKind.DelegateTokenKeyword:
                ParseDefaultKeywords.HandleDelegateTokenKeyword(model);
                break;
            case SyntaxKind.DoTokenKeyword:
                ParseDefaultKeywords.HandleDoTokenKeyword(model);
                break;
            case SyntaxKind.DoubleTokenKeyword:
                ParseDefaultKeywords.HandleDoubleTokenKeyword(model);
                break;
            case SyntaxKind.ElseTokenKeyword:
                ParseDefaultKeywords.HandleElseTokenKeyword(model);
                break;
            case SyntaxKind.EnumTokenKeyword:
                ParseDefaultKeywords.HandleEnumTokenKeyword(model);
                break;
            case SyntaxKind.EventTokenKeyword:
                ParseDefaultKeywords.HandleEventTokenKeyword(model);
                break;
            case SyntaxKind.ExplicitTokenKeyword:
                ParseDefaultKeywords.HandleExplicitTokenKeyword(model);
                break;
            case SyntaxKind.ExternTokenKeyword:
                ParseDefaultKeywords.HandleExternTokenKeyword(model);
                break;
            case SyntaxKind.FalseTokenKeyword:
                ParseDefaultKeywords.HandleFalseTokenKeyword(model);
                break;
            case SyntaxKind.FinallyTokenKeyword:
                ParseDefaultKeywords.HandleFinallyTokenKeyword(model);
                break;
            case SyntaxKind.FixedTokenKeyword:
                ParseDefaultKeywords.HandleFixedTokenKeyword(model);
                break;
            case SyntaxKind.FloatTokenKeyword:
                ParseDefaultKeywords.HandleFloatTokenKeyword(model);
                break;
            case SyntaxKind.ForTokenKeyword:
                ParseDefaultKeywords.HandleForTokenKeyword(model);
                break;
            case SyntaxKind.ForeachTokenKeyword:
                ParseDefaultKeywords.HandleForeachTokenKeyword(model);
                break;
            case SyntaxKind.GotoTokenKeyword:
                ParseDefaultKeywords.HandleGotoTokenKeyword(model);
                break;
            case SyntaxKind.ImplicitTokenKeyword:
                ParseDefaultKeywords.HandleImplicitTokenKeyword(model);
                break;
            case SyntaxKind.InTokenKeyword:
                ParseDefaultKeywords.HandleInTokenKeyword(model);
                break;
            case SyntaxKind.IntTokenKeyword:
                ParseDefaultKeywords.HandleIntTokenKeyword(model);
                break;
            case SyntaxKind.IsTokenKeyword:
                ParseDefaultKeywords.HandleIsTokenKeyword(model);
                break;
            case SyntaxKind.LockTokenKeyword:
                ParseDefaultKeywords.HandleLockTokenKeyword(model);
                break;
            case SyntaxKind.LongTokenKeyword:
                ParseDefaultKeywords.HandleLongTokenKeyword(model);
                break;
            case SyntaxKind.NullTokenKeyword:
                ParseDefaultKeywords.HandleNullTokenKeyword(model);
                break;
            case SyntaxKind.ObjectTokenKeyword:
                ParseDefaultKeywords.HandleObjectTokenKeyword(model);
                break;
            case SyntaxKind.OperatorTokenKeyword:
                ParseDefaultKeywords.HandleOperatorTokenKeyword(model);
                break;
            case SyntaxKind.OutTokenKeyword:
                ParseDefaultKeywords.HandleOutTokenKeyword(model);
                break;
            case SyntaxKind.ParamsTokenKeyword:
                ParseDefaultKeywords.HandleParamsTokenKeyword(model);
                break;
            case SyntaxKind.ProtectedTokenKeyword:
                ParseDefaultKeywords.HandleProtectedTokenKeyword(model);
                break;
            case SyntaxKind.ReadonlyTokenKeyword:
                ParseDefaultKeywords.HandleReadonlyTokenKeyword(model);
                break;
            case SyntaxKind.RefTokenKeyword:
                ParseDefaultKeywords.HandleRefTokenKeyword(model);
                break;
            case SyntaxKind.SbyteTokenKeyword:
                ParseDefaultKeywords.HandleSbyteTokenKeyword(model);
                break;
            case SyntaxKind.ShortTokenKeyword:
                ParseDefaultKeywords.HandleShortTokenKeyword(model);
                break;
            case SyntaxKind.SizeofTokenKeyword:
                ParseDefaultKeywords.HandleSizeofTokenKeyword(model);
                break;
            case SyntaxKind.StackallocTokenKeyword:
                ParseDefaultKeywords.HandleStackallocTokenKeyword(model);
                break;
            case SyntaxKind.StringTokenKeyword:
                ParseDefaultKeywords.HandleStringTokenKeyword(model);
                break;
            case SyntaxKind.StructTokenKeyword:
                ParseDefaultKeywords.HandleStructTokenKeyword(model);
                break;
            case SyntaxKind.SwitchTokenKeyword:
                ParseDefaultKeywords.HandleSwitchTokenKeyword(model);
                break;
            case SyntaxKind.ThisTokenKeyword:
                ParseDefaultKeywords.HandleThisTokenKeyword(model);
                break;
            case SyntaxKind.ThrowTokenKeyword:
                ParseDefaultKeywords.HandleThrowTokenKeyword(model);
                break;
            case SyntaxKind.TrueTokenKeyword:
                ParseDefaultKeywords.HandleTrueTokenKeyword(model);
                break;
            case SyntaxKind.TryTokenKeyword:
                ParseDefaultKeywords.HandleTryTokenKeyword(model);
                break;
            case SyntaxKind.TypeofTokenKeyword:
                ParseDefaultKeywords.HandleTypeofTokenKeyword(model);
                break;
            case SyntaxKind.UintTokenKeyword:
                ParseDefaultKeywords.HandleUintTokenKeyword(model);
                break;
            case SyntaxKind.UlongTokenKeyword:
                ParseDefaultKeywords.HandleUlongTokenKeyword(model);
                break;
            case SyntaxKind.UncheckedTokenKeyword:
                ParseDefaultKeywords.HandleUncheckedTokenKeyword(model);
                break;
            case SyntaxKind.UnsafeTokenKeyword:
                ParseDefaultKeywords.HandleUnsafeTokenKeyword(model);
                break;
            case SyntaxKind.UshortTokenKeyword:
                ParseDefaultKeywords.HandleUshortTokenKeyword(model);
                break;
            case SyntaxKind.VoidTokenKeyword:
                ParseDefaultKeywords.HandleVoidTokenKeyword(model);
                break;
            case SyntaxKind.VolatileTokenKeyword:
                ParseDefaultKeywords.HandleVolatileTokenKeyword(model);
                break;
            case SyntaxKind.WhileTokenKeyword:
                ParseDefaultKeywords.HandleWhileTokenKeyword(model);
                break;
            case SyntaxKind.UnrecognizedTokenKeyword:
                ParseDefaultKeywords.HandleUnrecognizedTokenKeyword(model);
                break;
            case SyntaxKind.ReturnTokenKeyword:
                ParseDefaultKeywords.HandleReturnTokenKeyword(model);
                break;
            case SyntaxKind.NamespaceTokenKeyword:
                ParseDefaultKeywords.HandleNamespaceTokenKeyword(model);
                break;
            case SyntaxKind.ClassTokenKeyword:
                ParseDefaultKeywords.HandleClassTokenKeyword(model);
                break;
            case SyntaxKind.InterfaceTokenKeyword:
                ParseDefaultKeywords.HandleInterfaceTokenKeyword(model);
                break;
            case SyntaxKind.UsingTokenKeyword:
                ParseDefaultKeywords.HandleUsingTokenKeyword(model);
                break;
            case SyntaxKind.PublicTokenKeyword:
                ParseDefaultKeywords.HandlePublicTokenKeyword(model);
                break;
            case SyntaxKind.InternalTokenKeyword:
                ParseDefaultKeywords.HandleInternalTokenKeyword(model);
                break;
            case SyntaxKind.PrivateTokenKeyword:
                ParseDefaultKeywords.HandlePrivateTokenKeyword(model);
                break;
            case SyntaxKind.StaticTokenKeyword:
                ParseDefaultKeywords.HandleStaticTokenKeyword(model);
                break;
            case SyntaxKind.OverrideTokenKeyword:
                ParseDefaultKeywords.HandleOverrideTokenKeyword(model);
                break;
            case SyntaxKind.VirtualTokenKeyword:
                ParseDefaultKeywords.HandleVirtualTokenKeyword(model);
                break;
            case SyntaxKind.AbstractTokenKeyword:
                ParseDefaultKeywords.HandleAbstractTokenKeyword(model);
                break;
            case SyntaxKind.SealedTokenKeyword:
                ParseDefaultKeywords.HandleSealedTokenKeyword(model);
                break;
            case SyntaxKind.IfTokenKeyword:
                ParseDefaultKeywords.HandleIfTokenKeyword(model);
                break;
            case SyntaxKind.NewTokenKeyword:
                ParseDefaultKeywords.HandleNewTokenKeyword(model);
                break;
            default:
                ParseDefaultKeywords.HandleDefault(model);
                break;
        }
    }

    public static void ParseKeywordContextualToken(CSharpParserModel model)
    {
        switch (model.TokenWalker.Current.SyntaxKind)
        {
            case SyntaxKind.VarTokenContextualKeyword:
                ParseContextualKeywords.HandleVarTokenContextualKeyword(model);
                break;
            case SyntaxKind.PartialTokenContextualKeyword:
                ParseContextualKeywords.HandlePartialTokenContextualKeyword(model);
                break;
            case SyntaxKind.AddTokenContextualKeyword:
                ParseContextualKeywords.HandleAddTokenContextualKeyword(model);
                break;
            case SyntaxKind.AndTokenContextualKeyword:
                ParseContextualKeywords.HandleAndTokenContextualKeyword(model);
                break;
            case SyntaxKind.AliasTokenContextualKeyword:
                ParseContextualKeywords.HandleAliasTokenContextualKeyword(model);
                break;
            case SyntaxKind.AscendingTokenContextualKeyword:
                ParseContextualKeywords.HandleAscendingTokenContextualKeyword(model);
                break;
            case SyntaxKind.ArgsTokenContextualKeyword:
                ParseContextualKeywords.HandleArgsTokenContextualKeyword(model);
                break;
            case SyntaxKind.AsyncTokenContextualKeyword:
                ParseContextualKeywords.HandleAsyncTokenContextualKeyword(model);
                break;
            case SyntaxKind.AwaitTokenContextualKeyword:
                ParseContextualKeywords.HandleAwaitTokenContextualKeyword(model);
                break;
            case SyntaxKind.ByTokenContextualKeyword:
                ParseContextualKeywords.HandleByTokenContextualKeyword(model);
                break;
            case SyntaxKind.DescendingTokenContextualKeyword:
                ParseContextualKeywords.HandleDescendingTokenContextualKeyword(model);
                break;
            case SyntaxKind.DynamicTokenContextualKeyword:
                ParseContextualKeywords.HandleDynamicTokenContextualKeyword(model);
                break;
            case SyntaxKind.EqualsTokenContextualKeyword:
                ParseContextualKeywords.HandleEqualsTokenContextualKeyword(model);
                break;
            case SyntaxKind.FileTokenContextualKeyword:
                ParseContextualKeywords.HandleFileTokenContextualKeyword(model);
                break;
            case SyntaxKind.FromTokenContextualKeyword:
                ParseContextualKeywords.HandleFromTokenContextualKeyword(model);
                break;
            case SyntaxKind.GetTokenContextualKeyword:
                ParseContextualKeywords.HandleGetTokenContextualKeyword(model);
                break;
            case SyntaxKind.GlobalTokenContextualKeyword:
                ParseContextualKeywords.HandleGlobalTokenContextualKeyword(model);
                break;
            case SyntaxKind.GroupTokenContextualKeyword:
                ParseContextualKeywords.HandleGroupTokenContextualKeyword(model);
                break;
            case SyntaxKind.InitTokenContextualKeyword:
                ParseContextualKeywords.HandleInitTokenContextualKeyword(model);
                break;
            case SyntaxKind.IntoTokenContextualKeyword:
                ParseContextualKeywords.HandleIntoTokenContextualKeyword(model);
                break;
            case SyntaxKind.JoinTokenContextualKeyword:
                ParseContextualKeywords.HandleJoinTokenContextualKeyword(model);
                break;
            case SyntaxKind.LetTokenContextualKeyword:
                ParseContextualKeywords.HandleLetTokenContextualKeyword(model);
                break;
            case SyntaxKind.ManagedTokenContextualKeyword:
                ParseContextualKeywords.HandleManagedTokenContextualKeyword(model);
                break;
            case SyntaxKind.NameofTokenContextualKeyword:
                ParseContextualKeywords.HandleNameofTokenContextualKeyword(model);
                break;
            case SyntaxKind.NintTokenContextualKeyword:
                ParseContextualKeywords.HandleNintTokenContextualKeyword(model);
                break;
            case SyntaxKind.NotTokenContextualKeyword:
                ParseContextualKeywords.HandleNotTokenContextualKeyword(model);
                break;
            case SyntaxKind.NotnullTokenContextualKeyword:
                ParseContextualKeywords.HandleNotnullTokenContextualKeyword(model);
                break;
            case SyntaxKind.NuintTokenContextualKeyword:
                ParseContextualKeywords.HandleNuintTokenContextualKeyword(model);
                break;
            case SyntaxKind.OnTokenContextualKeyword:
                ParseContextualKeywords.HandleOnTokenContextualKeyword(model);
                break;
            case SyntaxKind.OrTokenContextualKeyword:
                ParseContextualKeywords.HandleOrTokenContextualKeyword(model);
                break;
            case SyntaxKind.OrderbyTokenContextualKeyword:
                ParseContextualKeywords.HandleOrderbyTokenContextualKeyword(model);
                break;
            case SyntaxKind.RecordTokenContextualKeyword:
                ParseContextualKeywords.HandleRecordTokenContextualKeyword(model);
                break;
            case SyntaxKind.RemoveTokenContextualKeyword:
                ParseContextualKeywords.HandleRemoveTokenContextualKeyword(model);
                break;
            case SyntaxKind.RequiredTokenContextualKeyword:
                ParseContextualKeywords.HandleRequiredTokenContextualKeyword(model);
                break;
            case SyntaxKind.ScopedTokenContextualKeyword:
                ParseContextualKeywords.HandleScopedTokenContextualKeyword(model);
                break;
            case SyntaxKind.SelectTokenContextualKeyword:
                ParseContextualKeywords.HandleSelectTokenContextualKeyword(model);
                break;
            case SyntaxKind.SetTokenContextualKeyword:
                ParseContextualKeywords.HandleSetTokenContextualKeyword(model);
                break;
            case SyntaxKind.UnmanagedTokenContextualKeyword:
                ParseContextualKeywords.HandleUnmanagedTokenContextualKeyword(model);
                break;
            case SyntaxKind.ValueTokenContextualKeyword:
                ParseContextualKeywords.HandleValueTokenContextualKeyword(model);
                break;
            case SyntaxKind.WhenTokenContextualKeyword:
                ParseContextualKeywords.HandleWhenTokenContextualKeyword(model);
                break;
            case SyntaxKind.WhereTokenContextualKeyword:
                ParseContextualKeywords.HandleWhereTokenContextualKeyword(model);
                break;
            case SyntaxKind.WithTokenContextualKeyword:
                ParseContextualKeywords.HandleWithTokenContextualKeyword(model);
                break;
            case SyntaxKind.YieldTokenContextualKeyword:
                ParseContextualKeywords.HandleYieldTokenContextualKeyword(model);
                break;
            case SyntaxKind.UnrecognizedTokenContextualKeyword:
                ParseContextualKeywords.HandleUnrecognizedTokenContextualKeyword(model);
                break;
            default:
            	model.DiagnosticBag.ReportTodoException(model.TokenWalker.Current.TextSpan, $"Implement the {model.TokenWalker.Current.SyntaxKind.ToString()} contextual keyword.");
            	break;
        }
    }
}