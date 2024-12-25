using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public static class UtilityApi
{
    public static bool IsContextualKeywordSyntaxKind(SyntaxKind syntaxKind)
    {
        return IsContextualKeywordSyntaxKind(syntaxKind.ToString());
    }
    
    public static bool IsContextualKeywordSyntaxKind(string syntaxKindString)
    {
        return syntaxKindString.EndsWith("ContextualKeyword");
    }

    public static bool IsKeywordSyntaxKind(SyntaxKind syntaxKind)
    {
        return syntaxKind.ToString().EndsWith("Keyword");
    }

    /// <summary>
    /// The keywords: 'string', 'bool' 'int' and etc... are keywords, but identify a type.
    /// </summary>
    public static bool IsTypeIdentifierKeywordSyntaxKind(SyntaxKind syntaxKind)
    {
        switch (syntaxKind)
        {
            case SyntaxKind.BoolTokenKeyword:
            case SyntaxKind.ByteTokenKeyword:
            case SyntaxKind.CharTokenKeyword:
            case SyntaxKind.DecimalTokenKeyword:
            case SyntaxKind.DelegateTokenKeyword:
            case SyntaxKind.DoubleTokenKeyword:
            case SyntaxKind.EnumTokenKeyword:
            case SyntaxKind.FloatTokenKeyword:
            case SyntaxKind.IntTokenKeyword:
            case SyntaxKind.LongTokenKeyword:
            case SyntaxKind.NullTokenKeyword:
            case SyntaxKind.ObjectTokenKeyword:
            case SyntaxKind.SbyteTokenKeyword:
            case SyntaxKind.ShortTokenKeyword:
            case SyntaxKind.StringTokenKeyword:
            case SyntaxKind.UintTokenKeyword:
            case SyntaxKind.UlongTokenKeyword:
            case SyntaxKind.UshortTokenKeyword:
            case SyntaxKind.VoidTokenKeyword:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// TODO: What is described in this summary has not yet been implemented.
    /// The text 'var' can be an identifier to any syntax.
    /// For example, 'var' can be the name of a class, a variable, etc...
    /// Therefore, this method will check if the <see cref="SyntaxKind"/> is
    /// <see cref="SyntaxKind.VarTokenContextualKeyword"/> AND check if
    /// any 'var' identified definitions are in scope.
    /// </summary>
    public static bool IsVarContextualKeyword(CSharpCompilationUnit compilationUnit, SyntaxKind syntaxKind)
    {
        if (syntaxKind != SyntaxKind.VarTokenContextualKeyword)
            return false;

        return true;
    }

    public static bool IsBinaryOperatorSyntaxKind(SyntaxKind syntaxKind)
    {
        switch (syntaxKind)
        {
            case SyntaxKind.PlusToken:
            case SyntaxKind.MinusToken:
            case SyntaxKind.StarToken:
            case SyntaxKind.DivisionToken:
            case SyntaxKind.MemberAccessToken:
            case SyntaxKind.CloseAngleBracketToken:
            case SyntaxKind.CloseAngleBracketEqualsToken:
            case SyntaxKind.OpenAngleBracketToken:
            case SyntaxKind.OpenAngleBracketEqualsToken:
            case SyntaxKind.BangEqualsToken:
            case SyntaxKind.EqualsToken:
            case SyntaxKind.EqualsEqualsToken:
            case SyntaxKind.AmpersandToken:
            case SyntaxKind.AmpersandAmpersandToken:
            case SyntaxKind.PipeToken:
            case SyntaxKind.PipePipeToken:
            case SyntaxKind.QuestionMarkQuestionMarkToken:
                return true;
            default:
                return false;
        }
    }

    public static bool IsUnaryOperatorSyntaxKind(SyntaxKind syntaxKind)
    {
        switch (syntaxKind)
        {
            case SyntaxKind.PlusPlusToken:
            case SyntaxKind.MinusMinusToken:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// public, private, protected, internal, ....
    /// </summary>
    public static bool IsAccessibilitySyntaxKind(SyntaxKind syntaxKind)
    {
        switch (syntaxKind)
        {
            case SyntaxKind.PublicTokenKeyword:
            case SyntaxKind.PrivateTokenKeyword:
            case SyntaxKind.ProtectedTokenKeyword:
            case SyntaxKind.InternalTokenKeyword:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// The value '0' is returned if the provided <see cref="SyntaxKind"/> was not an operator.
    /// </summary>
    public static int GetOperatorPrecedence(SyntaxKind syntaxKind)
    {
        switch (syntaxKind)
        {
            case SyntaxKind.PlusToken:
            case SyntaxKind.MinusToken:
                return 1;
            case SyntaxKind.StarToken:
            case SyntaxKind.DivisionToken:
                return 2;
            case SyntaxKind.ParenthesizedExpressionNode:
                return 3;
            default:
                return 0;
        }
    }

    /// <summary>
    /// If the provided <see cref="KeywordToken"/> does not map to a <see cref="StorageModifierKind"/>,
    /// then null is returned.
    /// </summary>
    public static StorageModifierKind? GetStorageModifierKindFromToken(ISyntaxToken consumedToken)
    {
        switch (consumedToken.TextSpan.GetText())
        {
            case "enum":
                return StorageModifierKind.Enum;
            case "struct":
                return StorageModifierKind.Struct;
            case "class":
                return StorageModifierKind.Class;
            case "interface":
                return StorageModifierKind.Interface;
            case "record":
                return StorageModifierKind.Record;
            default:
                return null;
        }
    }
    
    /// <summary>
    /// If the provided <see cref="KeywordToken"/> does not map to a <see cref="AccessModifierKind"/>,
    /// then null is returned.
    /// </summary>
    public static AccessModifierKind? GetAccessModifierKindFromToken(ISyntaxToken consumedToken)
    {
        switch (consumedToken.TextSpan.GetText())
        {
            case "public":
                return AccessModifierKind.Public;
            case "protected":
                return AccessModifierKind.Protected;
            case "internal":
                return AccessModifierKind.Internal;
            case "private":
                return AccessModifierKind.Private;
            default:
                return null;
        }
    }
    
    public static bool IsConvertibleToTypeClauseNode(SyntaxKind syntaxKind)
    {
    	return syntaxKind == SyntaxKind.TypeClauseNode ||
    		   syntaxKind == SyntaxKind.IdentifierToken ||
    		   IsTypeIdentifierKeywordSyntaxKind(syntaxKind) ||
    		   IsContextualKeywordSyntaxKind(syntaxKind);
    }
    
    public static TypeClauseNode ConvertToTypeClauseNode(ISyntax syntax, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	if (syntax.SyntaxKind == SyntaxKind.TypeClauseNode)
    	{
    		return (TypeClauseNode)syntax;
    	}
    	else if (syntax.SyntaxKind == SyntaxKind.IdentifierToken)
    	{
    		return new TypeClauseNode(
	    		(IdentifierToken)syntax,
		        null,
		        null);
    	}
	    else if (IsTypeIdentifierKeywordSyntaxKind(syntax.SyntaxKind))
	    {
	    	return new TypeClauseNode(
	    		(KeywordToken)syntax,
		        null,
		        null);
	    }
	    else if (IsContextualKeywordSyntaxKind(syntax.SyntaxKind))
	    {
	    	return new TypeClauseNode(
	    		(KeywordContextualToken)syntax,
		        null,
		        null);
	    }
	    else
	    {
	    	// 'parserModel.TokenWalker.Current.TextSpan' isn't necessarily the syntax passed to this method.
	    	// TODO: But getting a TextSpan from a general type such as 'ISyntax' is a pain.
	    	//
	    	parserModel.DiagnosticBag.ReportTodoException(
	    		parserModel.TokenWalker.Current.TextSpan,
	    		$"The {nameof(SyntaxKind)}: {syntax.SyntaxKind}, is not convertible to a {nameof(TypeClauseNode)}. Invoke {nameof(IsConvertibleToTypeClauseNode)} and check the result, before invoking {nameof(ConvertToTypeClauseNode)}.");
	    	
	    	// TODO: Returning null when it can't be converted is a bad idea (the method return isn't documented as nullable).
	    	return null;
	    }
    }
    
    public static bool IsConvertibleToIdentifierToken(SyntaxKind syntaxKind)
    {
    	return syntaxKind == SyntaxKind.IdentifierToken ||
    		   IsContextualKeywordSyntaxKind(syntaxKind);
    }
    
    public static IdentifierToken ConvertToIdentifierToken(ISyntax syntax, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	if (syntax.SyntaxKind == SyntaxKind.IdentifierToken)
    	{
    		return (IdentifierToken)syntax;
    	}
	    else if (IsContextualKeywordSyntaxKind(syntax.SyntaxKind))
	    {
	    	var keywordContextualToken = (KeywordContextualToken)syntax;
	    	return new IdentifierToken(keywordContextualToken.TextSpan);
	    }
	    else
	    {
	    	// 'parserModel.TokenWalker.Current.TextSpan' isn't necessarily the syntax passed to this method.
	    	// TODO: But getting a TextSpan from a general type such as 'ISyntax' is a pain.
	    	//
	    	parserModel.DiagnosticBag.ReportTodoException(
	    		parserModel.TokenWalker.Current.TextSpan,
	    		$"The {nameof(SyntaxKind)}: {syntax.SyntaxKind}, is not convertible to a {nameof(IdentifierToken)}. Invoke {nameof(IsConvertibleToIdentifierToken)} and check the result, before invoking {nameof(ConvertToIdentifierToken)}.");
	    		
	    	// TODO: Returning default when it can't be converted might be a fine idea? It isn't as bad as returning null.
	    	return default;
	    }
    }
}