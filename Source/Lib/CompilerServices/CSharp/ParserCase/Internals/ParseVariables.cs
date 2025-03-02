using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public static class ParseVariables
{
    /// <summary>Function invocation which uses the 'out' keyword.</summary>
    public static IVariableDeclarationNode? HandleVariableDeclarationExpression(
        TypeClauseNode consumedTypeClauseNode,
        SyntaxToken consumedIdentifierToken,
        VariableKind variableKind,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserComputation parserComputation)
    {
    	IVariableDeclarationNode variableDeclarationNode;

		variableDeclarationNode = new VariableDeclarationNode(
	        consumedTypeClauseNode,
	        consumedIdentifierToken,
	        variableKind,
	        false);

        compilationUnit.Binder.BindVariableDeclarationNode(variableDeclarationNode, compilationUnit);
        parserComputation.CurrentCodeBlockBuilder.ChildList.Add(variableDeclarationNode);
        return variableDeclarationNode;
    }
}
