using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public class ParseFunctions
{
    public static void HandleFunctionInvocation(
        IdentifierToken consumedIdentifierToken,
        GenericParametersListingNode? genericParametersListingNode,
        CSharpParserModel model)
    {
        // TODO: (2023-06-04) I believe this if block will run for '<' mathematical operator.

        HandleFunctionParameters(
            (OpenParenthesisToken)model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken),
            model);

        var functionParametersListingNode = (FunctionParametersListingNode)model.SyntaxStack.Pop();

        var functionInvocationNode = new FunctionInvocationNode(
            consumedIdentifierToken,
            null,
            genericParametersListingNode,
            functionParametersListingNode,
            Facts.CSharpFacts.Types.Void.ToTypeClause());

        model.Binder.BindFunctionInvocationNode(functionInvocationNode, model);
        model.CurrentCodeBlockBuilder.ChildList.Add(functionInvocationNode);
    }

    public static void HandleFunctionDefinition(
        IdentifierToken consumedIdentifierToken,
        TypeClauseNode consumedTypeClauseNode,
        GenericArgumentsListingNode? consumedGenericArgumentsListingNode,
        CSharpParserModel model)
    {
        if (model.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenParenthesisToken)
            return;

        HandleFunctionArguments(
            (OpenParenthesisToken)model.TokenWalker.Consume(),
            model);

        var functionArgumentsListingNode = (FunctionArgumentsListingNode)model.SyntaxStack.Pop();

        var functionDefinitionNode = new FunctionDefinitionNode(
            AccessModifierKind.Public,
            consumedTypeClauseNode,
            consumedIdentifierToken,
            consumedGenericArgumentsListingNode,
            functionArgumentsListingNode,
            null,
            null);

        model.Binder.BindFunctionDefinitionNode(functionDefinitionNode, model);
        model.SyntaxStack.Push(functionDefinitionNode);

        if (model.CurrentCodeBlockBuilder.CodeBlockOwner is TypeDefinitionNode typeDefinitionNode &&
            typeDefinitionNode.IsInterface)
        {
            // TODO: Would method constraints break this code? "public T Aaa<T>() where T : OtherClass"
            var statementDelimiterToken = model.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);

            // TODO: Fabricating an OpenBraceToken in order to not duplicate the logic within 'ParseOpenBraceToken(...)' seems silly. This likely should be changed
            ParseTokens.ParseOpenBraceToken(
                new OpenBraceToken(statementDelimiterToken.TextSpan)
                {
                    IsFabricated = true
                },
                model);

            // TODO: Fabricating a CloseBraceToken in order to not duplicate the logic within 'ParseOpenBraceToken(...)' seems silly. This likely should be changed
            ParseTokens.ParseCloseBraceToken(
                new CloseBraceToken(statementDelimiterToken.TextSpan)
                {
                    IsFabricated = true
                },
                model);
        }
    }

    public static void HandleConstructorDefinition(
        IdentifierToken consumedIdentifierToken,
        CSharpParserModel model)
    {
        HandleFunctionArguments(
            (OpenParenthesisToken)model.TokenWalker.Consume(),
            model);

        var functionArgumentsListingNode = (FunctionArgumentsListingNode)model.SyntaxStack.Pop();

        if (model.CurrentCodeBlockBuilder.CodeBlockOwner is not TypeDefinitionNode typeDefinitionNode)
        {
            model.DiagnosticBag.ReportConstructorsNeedToBeWithinTypeDefinition(consumedIdentifierToken.TextSpan);
            typeDefinitionNode = Facts.CSharpFacts.Types.Void;
        }

        var typeClauseNode = new TypeClauseNode(
            typeDefinitionNode.TypeIdentifierToken,
            null,
            null);

        var constructorDefinitionNode = new ConstructorDefinitionNode(
            typeClauseNode,
            consumedIdentifierToken,
            null,
            functionArgumentsListingNode,
            null,
            null);

        model.Binder.BindConstructorDefinitionIdentifierToken(consumedIdentifierToken, model);
        model.SyntaxStack.Push(constructorDefinitionNode);

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.ColonToken)
        {
            // Constructor invokes some other constructor as well

            while (!model.TokenWalker.IsEof)
            {
                if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
                    model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
                {
                    break;
                }

                _ = model.TokenWalker.Consume();
            }
        }
    }

    public static void HandleConstructorInvocation(CSharpParserModel model)
    {
        var newKeywordToken = model.TokenWalker.Consume();

        TypeClauseNode typeClauseNode;

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
            typeClauseNode = Facts.CSharpFacts.Types.Var.ToTypeClause();
        else
            typeClauseNode = model.TokenWalker.MatchTypeClauseNode(model);

        var functionParametersListingNode = (FunctionParametersListingNode?)null;
        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
        {
            HandleFunctionParameters(
                (OpenParenthesisToken)model.TokenWalker.Consume(),
                model);

            functionParametersListingNode = (FunctionParametersListingNode)model.SyntaxStack.Pop();
        }

        var objectInitializationParametersListingNode = (ObjectInitializationParametersListingNode?)null;
        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
        {
            HandleObjectInitialization(
                (OpenBraceToken)model.TokenWalker.Consume(),
                model);

            objectInitializationParametersListingNode = (ObjectInitializationParametersListingNode)model.SyntaxStack.Pop();
        }

        var constructorInvocationExpressionNode = new ConstructorInvocationExpressionNode(
            (KeywordToken)newKeywordToken,
            typeClauseNode,
            functionParametersListingNode,
            objectInitializationParametersListingNode);

        model.SyntaxStack.Push(constructorInvocationExpressionNode);
    }

    public static void HandleObjectInitialization(OpenBraceToken consumedOpenBraceToken, CSharpParserModel model)
    {
        if (SyntaxKind.CloseBraceToken == model.TokenWalker.Peek(0).SyntaxKind)
        {
            model.SyntaxStack.Push(new ObjectInitializationParametersListingNode(
                consumedOpenBraceToken,
                ImmutableArray<ObjectInitializationParameterEntryNode>.Empty,
                (CloseBraceToken)model.TokenWalker.Consume()));

            return;
        }

        var mutableObjectInitializationParametersListing = new List<ObjectInitializationParameterEntryNode>();

        while (true)
        {
            var propertyIdentifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);
            if (propertyIdentifierToken.IsFabricated)
                break;
            
            var equalsToken = (EqualsToken)model.TokenWalker.Match(SyntaxKind.EqualsToken);
            if (equalsToken.IsFabricated)
                break;

            ParseOthers.HandleExpression(
                    null,
                    null,
                    null,
                    null,
                    null,
                    new[]
                    {
                        new ExpressionDelimiter(null, SyntaxKind.CommaToken, null, null),
                        new ExpressionDelimiter(
                            null,
                            SyntaxKind.CloseBraceToken,
                            null,
                            null)
                    },
                    model);

            var expressionNode = (IExpressionNode)model.SyntaxStack.Pop();

            // TODO: Make a PropertySymbol
            //
            // var variableReferenceNode = new VariableReferenceNode(
            //     propertyIdentifierToken,
            //     variableIdentifierToken,
            //     VariableKind.Local,
            //     false);
            //
            //variableReferenceNode = model.Binder.BindVariableReferenceNode(variableReferenceNode);

            var objectInitializationParameterEntryNode = new ObjectInitializationParameterEntryNode(
                propertyIdentifierToken,
                equalsToken,
                expressionNode);

            mutableObjectInitializationParametersListing.Add(objectInitializationParameterEntryNode);

            if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
            {
                var commaToken = (CommaToken)model.TokenWalker.Consume();
                // TODO: Track comma tokens?
                //
                // functionArgumentListing.Add(commaToken);
            }
            else
            {
                break;
            }
        }

        var closeBraceToken = (CloseBraceToken)model.TokenWalker.Match(SyntaxKind.CloseBraceToken);

        model.SyntaxStack.Push(new ObjectInitializationParametersListingNode(
            consumedOpenBraceToken,
            mutableObjectInitializationParametersListing.ToImmutableArray(),
            closeBraceToken));
    }

    /// <summary>
    /// As of (2023-08-04), when one enters this method,
    /// they have just finished parsing the function's identifier.
    /// <br/><br/>
    /// As a result, this method takes in an ImmutableArray of <see cref="FunctionDefinitionNode"/>
    /// where each entry is a function overload.
    /// <br/><br/>
    /// It has yet to be determined, which overload one is referring to.
    /// This method must compare the actual text's parameter types
    /// again the available overloads and determine which to use specifically.
    /// </summary>
    public static void HandleFunctionReferences(
        IdentifierToken consumedIdentifierToken,
        ImmutableArray<FunctionDefinitionNode> functionDefinitionNodes,
        CSharpParserModel model)
    {
        var concatenatedGetTextResults = string.Join(
            '\n',
            functionDefinitionNodes.Select(fd => fd.ConstructTextSpanRecursively().GetText()));

        if (SyntaxKind.OpenParenthesisToken == model.TokenWalker.Current.SyntaxKind)
        {
            HandleFunctionParameters(
                (OpenParenthesisToken)model.TokenWalker.Consume(),
                model);

            var functionParametersListingNode = (FunctionParametersListingNode)model.SyntaxStack.Pop();
            FunctionDefinitionNode? matchingOverload = null;

            foreach (var functionDefinitionNode in functionDefinitionNodes)
            {
                if (functionParametersListingNode.SatisfiesArguments(
                        functionDefinitionNode.FunctionArgumentsListingNode))
                {
                    matchingOverload = functionDefinitionNode;
                    break;
                }
            }

            if (matchingOverload is null)
            {
                model.DiagnosticBag.ReportTodoException(
                    consumedIdentifierToken.TextSpan,
                    "TODO: Handle case where none of the function overloads match the input.");
            }

            // TODO: Don't assume GenericParametersListingNode to be null
            var functionInvocationNode = new FunctionInvocationNode(
                consumedIdentifierToken,
                matchingOverload,
                null,
                functionParametersListingNode,
                matchingOverload?.ReturnTypeClauseNode ?? Facts.CSharpFacts.Types.Void.ToTypeClause());

            model.Binder.BindFunctionInvocationNode(functionInvocationNode, model);

            if (SyntaxKind.StatementDelimiterToken == model.TokenWalker.Current.SyntaxKind)
            {
                _ = model.TokenWalker.Consume();
                model.CurrentCodeBlockBuilder.ChildList.Add(functionInvocationNode);
            }
            else
            {
                model.SyntaxStack.Push(functionInvocationNode);
            }
        }
    }

    /// <summary>Use this method for function invocation, whereas <see cref="HandleFunctionArguments"/> should be used for function definition.</summary>
    public static void HandleFunctionParameters(
        OpenParenthesisToken consumedOpenParenthesisToken,
        CSharpParserModel model)
    {
        if (SyntaxKind.CloseParenthesisToken == model.TokenWalker.Current.SyntaxKind)
        {
            model.SyntaxStack.Push(new FunctionParametersListingNode(
                consumedOpenParenthesisToken,
                ImmutableArray<FunctionParameterEntryNode>.Empty,
                (CloseParenthesisToken)model.TokenWalker.Consume()));

            return;
        }

        var mutableFunctionParametersListing = new List<FunctionParameterEntryNode>();

        while (true)
        {
            var hasOutKeyword = false;
            var hasInKeyword = false;
            var hasRefKeyword = false;

            // Check for keywords: { 'out', 'in', 'ref', }
            {
                // TODO: Erroneously putting an assortment of the keywords: { 'out', 'in', 'ref', }
                if (SyntaxKind.OutTokenKeyword == model.TokenWalker.Current.SyntaxKind)
                {
                    _ = model.TokenWalker.Consume();
                    hasOutKeyword = true;

                    if (model.TokenWalker.Peek(1).SyntaxKind == SyntaxKind.IdentifierToken)
                    {
                        var outVariableTypeClause = model.TokenWalker.MatchTypeClauseNode(model);
                        var outVariableIdentifier = (IdentifierToken)model.TokenWalker.Peek(0);

                        ParseVariables.HandleVariableDeclaration(
                            outVariableTypeClause,
                            outVariableIdentifier,
                            VariableKind.Local,
                            model);
                    }
                }
                else if (SyntaxKind.InTokenKeyword == model.TokenWalker.Current.SyntaxKind)
                {
                    _ = model.TokenWalker.Consume();
                    hasInKeyword = true;
                }
                else if (SyntaxKind.RefTokenKeyword == model.TokenWalker.Current.SyntaxKind)
                {
                    _ = model.TokenWalker.Consume();
                    hasRefKeyword = true;
                }
            }

            ParseOthers.HandleExpression(
                    null,
                    null,
                    null,
                    null,
                    null,
                    new[]
                    {
                        new ExpressionDelimiter(null, SyntaxKind.CommaToken, null, null),
                        new ExpressionDelimiter(
                            SyntaxKind.OpenParenthesisToken,
                            SyntaxKind.CloseParenthesisToken,
                            null,
                            null)
                    },
                    model);

            var expression = (IExpressionNode)model.SyntaxStack.Pop();

            var functionParameterEntryNode = new FunctionParameterEntryNode(
                expression,
                hasOutKeyword,
                hasInKeyword,
                hasRefKeyword);

            mutableFunctionParametersListing.Add(functionParameterEntryNode);

            if (expression.IsFabricated && SyntaxKind.CommaToken != model.TokenWalker.Current.SyntaxKind)
            {
                break;
            }

            if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
            {
                var commaToken = (CommaToken)model.TokenWalker.Consume();
                // TODO: Track comma tokens?
            }
            else
            {
                break;
            }
        }

        var closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);

        model.SyntaxStack.Push(new FunctionParametersListingNode(
            consumedOpenParenthesisToken,
            mutableFunctionParametersListing.ToImmutableArray(),
            closeParenthesisToken));
    }

    /// <summary>Use this method for function definition, whereas <see cref="HandleFunctionParameters"/> should be used for function invocation.</summary>
    public static void HandleFunctionArguments(
        OpenParenthesisToken consumedOpenParenthesisToken,
        CSharpParserModel model)
    {
        if (SyntaxKind.CloseParenthesisToken == model.TokenWalker.Peek(0).SyntaxKind)
        {
            model.SyntaxStack.Push(new FunctionArgumentsListingNode(
                consumedOpenParenthesisToken,
                ImmutableArray<FunctionArgumentEntryNode>.Empty,
                (CloseParenthesisToken)model.TokenWalker.Consume()));

            return;
        }

        var mutableFunctionArgumentListing = new List<FunctionArgumentEntryNode>();

        while (true)
        {
            var hasOutKeyword = false;
            var hasInKeyword = false;
            var hasRefKeyword = false;

            // Check for keywords: { 'out', 'in', 'ref', }
            {
                // TODO: Erroneously putting an assortment of the keywords: { 'out', 'in', 'ref', }
                if (SyntaxKind.OutTokenKeyword == model.TokenWalker.Current.SyntaxKind)
                {
                    _ = model.TokenWalker.Consume();
                    hasOutKeyword = true;
                }
                else if (SyntaxKind.InTokenKeyword == model.TokenWalker.Current.SyntaxKind)
                {
                    _ = model.TokenWalker.Consume();
                    hasInKeyword = true;
                }
                else if (SyntaxKind.RefTokenKeyword == model.TokenWalker.Current.SyntaxKind)
                {
                    _ = model.TokenWalker.Consume();
                    hasRefKeyword = true;
                }
            }

            // TypeClause
            var typeClauseNode = model.TokenWalker.MatchTypeClauseNode(model);

            if (typeClauseNode.IsFabricated)
                break;

            // Identifier
            var variableIdentifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);

            if (variableIdentifierToken.IsFabricated)
                break;

            var variableDeclarationStatementNode = new VariableDeclarationNode(
                typeClauseNode,
                variableIdentifierToken,
                VariableKind.Local,
                false
            );

            model.Binder.BindVariableDeclarationNode(variableDeclarationStatementNode, model);

            var functionArgumentEntryNode = new FunctionArgumentEntryNode(
                variableDeclarationStatementNode,
                false,
                hasOutKeyword,
                hasInKeyword,
                hasRefKeyword);

            if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
            {
                var equalsToken = (EqualsToken)model.TokenWalker.Consume();

                var compileTimeConstantAbleSyntaxKinds = new[]
                {
                    SyntaxKind.StringLiteralToken,
                    SyntaxKind.NumericLiteralToken,
                };

                var compileTimeConstantToken = model.TokenWalker.MatchRange(
                    compileTimeConstantAbleSyntaxKinds,
                    SyntaxKind.BadToken);

                functionArgumentEntryNode = model.Binder.BindFunctionOptionalArgument(
                    functionArgumentEntryNode,
                    compileTimeConstantToken,
                    hasOutKeyword,
                    hasInKeyword,
                    hasRefKeyword,
                    model);
            }

            mutableFunctionArgumentListing.Add(functionArgumentEntryNode);

            if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
            {
                var commaToken = (CommaToken)model.TokenWalker.Consume();
                // TODO: Track comma tokens?
                //
                // functionArgumentListing.Add(commaToken);
            }
            else
            {
                break;
            }
        }

        var closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);

        model.SyntaxStack.Push(new FunctionArgumentsListingNode(
            consumedOpenParenthesisToken,
            mutableFunctionArgumentListing.ToImmutableArray(),
            closeParenthesisToken));
    }
}
