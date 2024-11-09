using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.CompilerServices.CSharp.Facts;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public static class ParseVariables
{
    public static void HandleVariableReference(
        IdentifierToken consumedIdentifierToken,
        CSharpParserModel model)
    {
    }

	/// <summary>Function invocation which uses the 'out' keyword.</summary>
    public static IVariableDeclarationNode? HandleVariableDeclarationExpression(
        TypeClauseNode consumedTypeClauseNode,
        IdentifierToken consumedIdentifierToken,
        VariableKind variableKind,
        IParserModel model)
    {
    	Console.WriteLine("HandleVariableDeclarationExpression");
    
		IVariableDeclarationNode variableDeclarationNode;

		if (variableKind == VariableKind.Local || variableKind == VariableKind.Closure)
		{
			Console.WriteLine("if (variableKind == VariableKind.Local || variableKind == VariableKind.Closure)");
			
			variableDeclarationNode = new VariableDeclarationNode(
	            consumedTypeClauseNode,
	            consumedIdentifierToken,
	            variableKind,
	            false);
		}
		else if (variableKind == VariableKind.Field)
		{
			variableDeclarationNode = new FieldDefinitionNode(
	            consumedTypeClauseNode,
	            consumedIdentifierToken,
	            variableKind,
	            false);
		}
		else if (variableKind == VariableKind.Property)
		{
			variableDeclarationNode = new PropertyDefinitionNode(
	            consumedTypeClauseNode,
	            consumedIdentifierToken,
	            variableKind,
	            false,
	            model.CurrentCodeBlockBuilder.CodeBlockOwner);
		}
		else
		{
			model.DiagnosticBag.ReportTodoException(consumedIdentifierToken.TextSpan, $"The {nameof(VariableKind)}: {variableKind} was not recognized.");
			return null;
		}

        model.Binder.BindVariableDeclarationNode(variableDeclarationNode, model);
        model.CurrentCodeBlockBuilder.ChildList.Add(variableDeclarationNode);
        return variableDeclarationNode;
    }
    
    /// <summary>
    /// TODO: This method should return the 'VariableDeclarationNode?' just the same as <see cref="HandleVariableDeclarationExpression"/>
    /// </summary>
    public static void HandleVariableDeclarationStatement(
        TypeClauseNode consumedTypeClauseNode,
        IdentifierToken consumedIdentifierToken,
        VariableKind variableKind,
        IParserModel model)
    {
    	Console.WriteLine("HandleVariableDeclarationStatement");
    
		var variableDeclarationNode = HandleVariableDeclarationExpression(
			consumedTypeClauseNode,
	        consumedIdentifierToken,
	        variableKind,
	        model);
	        
	    if (variableDeclarationNode is null)
	    	return;
	    	
	    // if (variableKind == VariableKind.Local || variableKind == VariableKind.Closure)

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
        {
            if (model.TokenWalker.Peek(1).SyntaxKind == SyntaxKind.CloseAngleBracketToken)
            {
                HandlePropertyExpression(
                    variableDeclarationNode,
                    (EqualsToken)model.TokenWalker.Consume(),
                    (CloseAngleBracketToken)model.TokenWalker.Consume(),
                    (CSharpParserModel)model);
            }
            else
            {
                // Variable initialization occurs here.
                HandleVariableAssignment(
                    consumedIdentifierToken,
                    (EqualsToken)model.TokenWalker.Consume(),
                    (CSharpParserModel)model);
            }
        }
        else
        {
            if (variableDeclarationNode.TypeClauseNode.TypeIdentifierToken.SyntaxKind ==
                SyntaxKind.VarTokenContextualKeyword)
            {
                model.DiagnosticBag.ReportImplicitlyTypedVariablesMustBeInitialized(
                    consumedIdentifierToken.TextSpan);
            }
        }

        if (variableKind == VariableKind.Property &&
            model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
        {
            HandlePropertyDeclaration(
                variableDeclarationNode,
                (OpenBraceToken)model.TokenWalker.Consume(),
                (CSharpParserModel)model);
        }
        else
        {
            _ = model.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
        }
    }

    public static void HandlePropertyDeclaration(
        IVariableDeclarationNode consumedVariableDeclarationNode,
        OpenBraceToken consumedOpenBraceToken,
        CSharpParserModel model)
    {
    }

    public static void HandlePropertyExpression(
        IVariableDeclarationNode consumedVariableDeclarationNode,
        EqualsToken consumedEqualsToken,
        CloseAngleBracketToken consumedCloseAngleBracketToken,
        CSharpParserModel model)
    {
    }

    public static void HandleVariableAssignment(
        IdentifierToken consumedIdentifierToken,
        EqualsToken consumedEqualsToken,
        CSharpParserModel model)
    {
    }
}
