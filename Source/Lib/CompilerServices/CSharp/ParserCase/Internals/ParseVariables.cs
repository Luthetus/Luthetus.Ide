using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public static class ParseVariables
{
    /// <summary>Function invocation which uses the 'out' keyword.</summary>
    public static VariableDeclarationNode? HandleVariableDeclarationExpression(
        TypeClauseNode consumedTypeClauseNode,
        SyntaxToken consumedIdentifierToken,
        VariableKind variableKind,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserModel parserModel)
    {
    	VariableDeclarationNode variableDeclarationNode;

		variableDeclarationNode = new VariableDeclarationNode(
	        new TypeReference(consumedTypeClauseNode),
	        consumedIdentifierToken,
	        variableKind,
	        false);

        parserModel.Binder.BindVariableDeclarationNode(variableDeclarationNode, compilationUnit, ref parserModel);
        parserModel.CurrentCodeBlockBuilder.ChildList.Add(variableDeclarationNode);
        return variableDeclarationNode;
    }
}
