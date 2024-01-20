using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.CompilerServices.Lang.CSharp.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase;

internal static class SyntaxApi
{
    public static void HandleStaticClassIdentifier(ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        model.TokenWalker.Backtrack();

        var typeClauseNode = UtilityApi.MatchTypeClause(model);

        model.SyntaxStack.Push(typeClauseNode);
    }

    public static void HandleUndefinedTypeOrNamespaceReference(ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        var identifierReferenceNode = new AmbiguousIdentifierNode(
            identifierToken);

        model.Binder.BindTypeIdentifier(identifierToken);

        model.DiagnosticBag.ReportUndefinedTypeOrNamespace(
            identifierToken.TextSpan,
            identifierToken.TextSpan.GetText());

        model.SyntaxStack.Push(identifierReferenceNode);
    }

    public static void HandleVariableReference(ParserModel model)
    {
        var variableDeclarationStatementNode = (VariableDeclarationNode)model.SyntaxStack.Pop();
        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        var variableReferenceNode = new VariableReferenceNode(
            identifierToken,
            variableDeclarationStatementNode);

        model.Binder.BindVariableReferenceNode(variableReferenceNode);

        model.SyntaxStack.Push(variableReferenceNode);
    }

    public static void HandleFunctionInvocation(
        GenericParametersListingNode? genericParametersListingNode,
        ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        // TODO: (2023-06-04) I believe this if block will run for '<' mathematical operator.

        model.SyntaxStack.Push((OpenParenthesisToken)model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken));
        HandleFunctionParameters(model);

        var functionParametersListingNode = (FunctionParametersListingNode)model.SyntaxStack.Pop();

        var functionInvocationNode = new FunctionInvocationNode(
            identifierToken,
            null,
            genericParametersListingNode,
            functionParametersListingNode);

        model.Binder.BindFunctionInvocationNode(functionInvocationNode);
        model.CurrentCodeBlockBuilder.ChildList.Add(functionInvocationNode);
        model.SyntaxStack.Push(new EmptyNode());
    }

    public static void HandleVariableDeclaration(
        TypeClauseNode typeClauseNode,
        IdentifierToken identifierToken,
        VariableKind variableKind,
        ParserModel model)
    {
        /*
         * Issues:
         *     -Should Handle...(...) methods return the node?
         *         -If one returns the node then they must only ever 
         *             modify the current code block builder one time.
         *         -Yet, HandleVariableDeclaration(...) wants to proceed to invoke
         *             HandleVariableAssignment if an EqualsToken is found.
         *         -This would result in two nodes needing to be added
         *             to the current code block builder.
         *         -Futhermore, HandleVariableDeclaration(...) adds to the current code block
         *             builder prior to checking if the variable is a property.
         *             - A few if statements would fix this but that seems to start creating a mess.
         */

        var variableDeclarationNode = new VariableDeclarationNode(
            typeClauseNode,
            identifierToken,
            variableKind,
            false);

        model.Binder.BindVariableDeclarationStatementNode(variableDeclarationNode);
        model.CurrentCodeBlockBuilder.ChildList.Add(variableDeclarationNode);

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
        {
            if (model.TokenWalker.Peek(1).SyntaxKind == SyntaxKind.CloseAngleBracketToken)
            {
                model.SyntaxStack.Push(variableDeclarationNode);
                HandlePropertyExpression(model);
            }
            else
            {
                // Variable initialization occurs here.
                model.SyntaxStack.Push(identifierToken);
                HandleVariableAssignment(model);
            }
        }

        if (variableKind == VariableKind.Property &&
            model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
        {
            model.SyntaxStack.Push(variableDeclarationNode);
            model.SyntaxStack.Push((OpenBraceToken)model.TokenWalker.Consume());
            HandlePropertyDeclaration(model);
        }
        else
        {
            _ = model.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
        }

        model.SyntaxStack.Push(new EmptyNode());
    }

    public static void HandlePropertyDeclaration(ParserModel model)
    {
        var openBraceToken = (OpenBraceToken)model.SyntaxStack.Pop();
        var variableDeclarationNode = (VariableDeclarationNode)model.SyntaxStack.Pop();

        while (!model.TokenWalker.IsEof)
        {
            var token = model.TokenWalker.Consume();

            if (UtilityApi.IsAccessibilitySyntaxKind(token.SyntaxKind))
            {
                model.DiagnosticBag.ReportTodoException(token.TextSpan, "TODO: Implement accessibility modifiers for properties.");
                continue;
            }
            else if (token.SyntaxKind == SyntaxKind.GetTokenContextualKeyword)
            {
                variableDeclarationNode.HasGetter = true;

                if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
                {
                    _ = model.TokenWalker.Consume();
                    variableDeclarationNode.GetterIsAutoImplemented = true;
                }
                else if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
                {
                    // TODO: Parse getter body
                }
            }
            else if (token.SyntaxKind == SyntaxKind.SetTokenContextualKeyword)
            {
                variableDeclarationNode.HasSetter = true;

                if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
                {
                    _ = model.TokenWalker.Consume();
                    variableDeclarationNode.SetterIsAutoImplemented = true;
                }
                else if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
                {
                    // TODO: Parse setter body
                }
            }
            else if (token.SyntaxKind == SyntaxKind.CloseBraceToken)
            {
                break;
            }
            else
            {
                // TODO: Remove this else block if it is uneccessary
                model.DiagnosticBag.ReportTodoException(token.TextSpan, "TODO: Implement parsing for this property syntax.");
                continue;
            }
        }

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
        {
            // Property initialization occurs here.
            model.SyntaxStack.Push(variableDeclarationNode.IdentifierToken);
            HandleVariableAssignment(model);
        }
    }

    public static void HandlePropertyExpression(ParserModel model)
    {
        var variableDeclarationNode = (VariableDeclarationNode)model.SyntaxStack.Pop();
        var equalsToken = (EqualsToken)model.TokenWalker.Consume();
        var closeAngleBracketToken = (CloseAngleBracketToken)model.TokenWalker.Consume();

        HandleExpression(
            null,
            null,
            null,
            null,
            null,
            null,
            model);

        variableDeclarationNode.HasGetter = true;
    }

    public static void HandleFunctionDefinition(ParserModel model)
    {
        GenericArgumentsListingNode? genericArgumentsListingNode =
            model.SyntaxStack.Peek() is GenericArgumentsListingNode temporaryNode
                ? temporaryNode
                : null;

        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();
        var typeClauseNode = (TypeClauseNode)model.SyntaxStack.Pop();

        if (model.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenParenthesisToken)
            return;

        model.SyntaxStack.Push(model.TokenWalker.Consume());
        HandleFunctionArguments(model);

        var functionArgumentsListingNode = (FunctionArgumentsListingNode)model.SyntaxStack.Pop();

        var functionDefinitionNode = new FunctionDefinitionNode(
            typeClauseNode,
            identifierToken,
            genericArgumentsListingNode,
            functionArgumentsListingNode,
            null,
            null);

        model.Binder.BindFunctionDefinitionNode(functionDefinitionNode);

        model.SyntaxStack.Push(functionDefinitionNode);

        if (model.CurrentCodeBlockBuilder.CodeBlockOwner is TypeDefinitionNode typeDefinitionNode &&
            typeDefinitionNode.IsInterface)
        {
            // TODO: Would method constraints break this code? "public T Aaa<T>() where T : OtherClass"
            var statementDelimiterToken = model.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);

            // TODO: Fabricating an OpenBraceToken in order to not duplicate the logic within 'ParseOpenBraceToken(...)' seems silly. This likely should be changed
            model.SyntaxStack.Push(new OpenBraceToken(statementDelimiterToken.TextSpan)
            {
                IsFabricated = true
            });
            TokenApi.ParseOpenBraceToken(model);

            // TODO: Fabricating a CloseBraceToken in order to not duplicate the logic within 'ParseOpenBraceToken(...)' seems silly. This likely should be changed
            model.SyntaxStack.Push(new CloseBraceToken(statementDelimiterToken.TextSpan)
            {
                IsFabricated = true
            });
            TokenApi.ParseCloseBraceToken(model);
        }
    }

    public static void HandleConstructorDefinition(ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        GenericArgumentsListingNode? genericArgumentsListingNode = null;

        model.SyntaxStack.Push((OpenParenthesisToken)model.TokenWalker.Consume());
        HandleFunctionArguments(model);

        var functionArgumentsListingNode = (FunctionArgumentsListingNode)model.SyntaxStack.Pop();

        if (model.CurrentCodeBlockBuilder.CodeBlockOwner is not TypeDefinitionNode typeDefinitionNode)
        {
            model.DiagnosticBag.ReportConstructorsNeedToBeWithinTypeDefinition(identifierToken.TextSpan);
            typeDefinitionNode = CSharpFacts.Types.Void;
        }

        var typeClauseNode = new TypeClauseNode(
            typeDefinitionNode.TypeIdentifier,
            null,
            null);

        var constructorDefinitionNode = new ConstructorDefinitionNode(
            typeClauseNode,
            identifierToken,
            genericArgumentsListingNode,
            functionArgumentsListingNode,
            null,
            null);

        model.Binder.BindConstructorDefinitionIdentifierToken(identifierToken);

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

    public static void HandleNamespaceReference(ParserModel model)
    {
        var namespaceGroupNode = (NamespaceGroupNode)model.SyntaxStack.Pop();
        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        model.Binder.BindNamespaceReference(identifierToken);

        if (SyntaxKind.MemberAccessToken == model.TokenWalker.Current.SyntaxKind)
        {
            var memberAccessToken = model.TokenWalker.Consume();

            var memberIdentifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);

            if (memberIdentifierToken.IsFabricated)
            {
                model.DiagnosticBag.ReportUnexpectedToken(
                    model.TokenWalker.Current.TextSpan,
                    model.TokenWalker.Current.SyntaxKind.ToString(),
                    SyntaxKind.IdentifierToken.ToString());
            }

            // Check all the TypeDefinitionNodes that are in the namespace
            var typeDefinitionNodes = namespaceGroupNode.GetTopLevelTypeDefinitionNodes();

            var typeDefinitionNode = typeDefinitionNodes.SingleOrDefault(td =>
                td.TypeIdentifier.TextSpan.GetText() == memberIdentifierToken.TextSpan.GetText());

            if (typeDefinitionNode is null)
            {
                model.DiagnosticBag.ReportNotDefinedInContext(
                    model.TokenWalker.Current.TextSpan,
                    identifierToken.TextSpan.GetText());
            }
            else
            {
                model.SyntaxStack.Push(memberIdentifierToken);
                model.SyntaxStack.Push(typeDefinitionNode);
                HandleTypeReference(model);
            }
        }
        else
        {
            // TODO: (2023-05-28) Report an error diagnostic for 'namespaces are not statements'. Something like this I'm not sure.
            model.TokenWalker.Consume();
        }
    }

    public static void HandleTypeReference(ParserModel model)
    {
        var typeDefinitionNode = (TypeDefinitionNode)model.SyntaxStack.Pop();
        var memberIdentifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        model.Binder.BindTypeIdentifier(memberIdentifierToken);

        var memberAccessToken = (MemberAccessToken)model.TokenWalker.Match(SyntaxKind.MemberAccessToken);

        if (memberAccessToken.IsFabricated)
            throw new NotImplementedException("Implement a static class being member accessed, but the statement ends there --- it is incomplete.");

        var identifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);

        if (identifierToken.IsFabricated)
            throw new NotImplementedException("Implement a static class being member accessed, but the statement ends there --- it is incomplete.");

        var matchingFunctionDefinitionNodes = typeDefinitionNode
            .GetFunctionDefinitionNodes()
            .Where(fd =>
                fd.FunctionIdentifier.TextSpan.GetText() == identifierToken.TextSpan.GetText())
            .ToImmutableArray();

        model.SyntaxStack.Push(identifierToken);

        HandleFunctionReferences(matchingFunctionDefinitionNodes, model);
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
        ImmutableArray<FunctionDefinitionNode> functionDefinitionNodes,
        ParserModel model)
    {
        var functionInvocationIdentifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        var concatenatedGetTextResults = string.Join(
            '\n',
            functionDefinitionNodes.Select(fd => fd.GetTextRecursively()));

        // (2023-08-03) The input 'System.Console.WriteLine();' had 18 matches.

        if (SyntaxKind.OpenParenthesisToken == model.TokenWalker.Current.SyntaxKind)
        {
            model.SyntaxStack.Push((OpenParenthesisToken)model.TokenWalker.Consume());
            HandleFunctionParameters(model);

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
                    functionInvocationIdentifierToken.TextSpan,
                    "TODO: Handle case where none of the function overloads match the input.");
            }

            // TODO: Don't assume GenericParametersListingNode to be null
            var functionInvocationNode = new FunctionInvocationNode(
                functionInvocationIdentifierToken,
                matchingOverload,
                null,
                functionParametersListingNode);

            model.Binder.BindFunctionInvocationNode(functionInvocationNode);

            if (SyntaxKind.StatementDelimiterToken == model.TokenWalker.Current.SyntaxKind)
            {
                _ = model.TokenWalker.Consume();

                model.SyntaxStack.Push(new EmptyNode());
                model.CurrentCodeBlockBuilder.ChildList.Add(functionInvocationNode);
            }
            else
            {
                model.SyntaxStack.Push(functionInvocationNode);
            }
        }
    }

    public static void HandleVariableAssignment(ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        // Move past the EqualsToken
        var equalsToken = (EqualsToken)model.TokenWalker.Consume();

        HandleExpression(
            null,
            null,
            null,
            null,
            null,
            null,
            model);

        var rightHandExpression = (IExpressionNode)model.SyntaxStack.Pop();

        var variableAssignmentExpressionNode = new VariableAssignmentExpressionNode(
            identifierToken,
            equalsToken,
            rightHandExpression);

        model.Binder.BindVariableAssignmentExpressionNode(variableAssignmentExpressionNode);

        model.CurrentCodeBlockBuilder.ChildList.Add(variableAssignmentExpressionNode);
    }

    /// <summary>TODO: Correctly parse object initialization. For now, just skip over it when parsing.</summary>
    public static void HandleObjectInitialization(ParserModel model)
    {
        var openBraceToken = (OpenBraceToken)model.SyntaxStack.Pop();

        ISyntaxToken shouldBeCloseBraceToken = new BadToken(openBraceToken.TextSpan);

        while (!model.TokenWalker.IsEof)
        {
            shouldBeCloseBraceToken = model.TokenWalker.Consume();

            if (SyntaxKind.EndOfFileToken == shouldBeCloseBraceToken.SyntaxKind ||
                SyntaxKind.CloseBraceToken == shouldBeCloseBraceToken.SyntaxKind)
            {
                break;
            }
        }

        if (SyntaxKind.CloseBraceToken != shouldBeCloseBraceToken.SyntaxKind)
            shouldBeCloseBraceToken = model.TokenWalker.Match(SyntaxKind.CloseBraceToken);

        model.SyntaxStack.Push(new ObjectInitializationNode(
            openBraceToken,
            (CloseBraceToken)shouldBeCloseBraceToken));
    }

    public static void HandleNamespaceIdentifier(ParserModel model)
    {
        var combineNamespaceIdentifierIntoOne = new List<ISyntaxToken>();

        while (!model.TokenWalker.IsEof)
        {
            if (combineNamespaceIdentifierIntoOne.Count % 2 == 0)
            {
                var matchedToken = model.TokenWalker.Match(SyntaxKind.IdentifierToken);
                combineNamespaceIdentifierIntoOne.Add(matchedToken);

                if (matchedToken.IsFabricated)
                    break;
            }
            else
            {
                if (SyntaxKind.MemberAccessToken == model.TokenWalker.Current.SyntaxKind)
                    combineNamespaceIdentifierIntoOne.Add(model.TokenWalker.Consume());
                else
                    break;
            }
        }

        var identifierTextSpan = combineNamespaceIdentifierIntoOne.First().TextSpan with
        {
            EndingIndexExclusive = combineNamespaceIdentifierIntoOne.Last().TextSpan.EndingIndexExclusive
        };

        model.SyntaxStack.Push(new IdentifierToken(identifierTextSpan));
    }

    /// <summary>
    /// This method is used for generic type usage such as, 'var words = new List&lt;string&gt;;'
    /// </summary>
    public static void HandleGenericParameters(ParserModel model)
    {
        var openAngleBracketToken = (OpenAngleBracketToken)model.SyntaxStack.Pop();

        if (SyntaxKind.CloseAngleBracketToken == model.TokenWalker.Current.SyntaxKind)
        {
            model.SyntaxStack.Push(new GenericParametersListingNode(
                openAngleBracketToken,
                ImmutableArray<GenericParameterEntryNode>.Empty,
                (CloseAngleBracketToken)model.TokenWalker.Consume()));
            
            return;
        }

        var mutableGenericParametersListing = new List<GenericParameterEntryNode>();

        while (true)
        {
            // TypeClause
            var typeClauseNode = UtilityApi.MatchTypeClause(model);

            if (typeClauseNode.IsFabricated)
                break;

            var genericParameterEntryNode = new GenericParameterEntryNode(typeClauseNode);

            mutableGenericParametersListing.Add(genericParameterEntryNode);

            if (SyntaxKind.CommaToken == model.TokenWalker.Current.SyntaxKind)
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

        var closeAngleBracketToken = (CloseAngleBracketToken)model.TokenWalker.Match(SyntaxKind.CloseAngleBracketToken);

        model.SyntaxStack.Push(new GenericParametersListingNode(
            openAngleBracketToken,
            mutableGenericParametersListing.ToImmutableArray(),
            closeAngleBracketToken));
    }

    /// <summary>
    /// This method is used for generic type definition such as, 'class List&lt;T&gt; { ... }'
    /// </summary>
    public static void HandleGenericArguments(ParserModel model)
    {
        var openAngleBracketToken = (OpenAngleBracketToken)model.SyntaxStack.Pop();

        if (SyntaxKind.CloseAngleBracketToken == model.TokenWalker.Current.SyntaxKind)
        {
            model.SyntaxStack.Push(new GenericArgumentsListingNode(
                openAngleBracketToken,
                ImmutableArray<GenericArgumentEntryNode>.Empty,
                (CloseAngleBracketToken)model.TokenWalker.Consume()));

            return;
        }

        var mutableGenericArgumentsListing = new List<GenericArgumentEntryNode>();

        while (true)
        {
            // TypeClause
            var typeClauseNode = UtilityApi.MatchTypeClause(model);

            if (typeClauseNode.IsFabricated)
                break;

            var genericArgumentEntryNode = new GenericArgumentEntryNode(typeClauseNode);

            mutableGenericArgumentsListing.Add(genericArgumentEntryNode);

            if (SyntaxKind.CommaToken == model.TokenWalker.Current.SyntaxKind)
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

        var closeAngleBracketToken = (CloseAngleBracketToken)model.TokenWalker.Match(SyntaxKind.CloseAngleBracketToken);

        model.SyntaxStack.Push(new GenericArgumentsListingNode(
            openAngleBracketToken,
            mutableGenericArgumentsListing.ToImmutableArray(),
            closeAngleBracketToken));
    }

    /// <summary>Use this method for function invocation, whereas <see cref="HandleFunctionArguments"/> should be used for function definition.</summary>
    public static void HandleFunctionParameters(ParserModel model)
    {
        var openParenthesisToken = (OpenParenthesisToken)model.SyntaxStack.Pop();

        if (SyntaxKind.CloseParenthesisToken == model.TokenWalker.Current.SyntaxKind)
        {
            model.SyntaxStack.Push(new FunctionParametersListingNode(
                openParenthesisToken,
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
                        var outVariableTypeClause = UtilityApi.MatchTypeClause(model);
                        var outVariableIdentifier = (IdentifierToken)model.TokenWalker.Peek(0);

                        HandleVariableDeclaration(
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

            IExpressionNode expression;

            if (SyntaxKind.IdentifierToken == model.TokenWalker.Current.SyntaxKind)
            {
                var variableIdentifierToken = (IdentifierToken)model.TokenWalker.Consume();

                if (!model.Binder.TryGetVariableDeclarationHierarchically(
                        variableIdentifierToken.TextSpan.GetText(),
                        out var variableDeclarationNode)
                    || variableDeclarationNode is null)
                {
                    variableDeclarationNode = new(
                        CSharpFacts.Types.Void.ToTypeClause(),
                        variableIdentifierToken,
                        VariableKind.Local,
                        false)
                    {
                        IsFabricated = true
                    };

                    model.Binder.BindVariableDeclarationStatementNode(variableDeclarationNode);
                }

                var variableReferenceNode = new VariableReferenceNode(
                    variableIdentifierToken,
                    variableDeclarationNode);

                variableReferenceNode = model.Binder.BindVariableReferenceNode(variableReferenceNode);

                expression = variableReferenceNode;
            }
            else
            {
                HandleExpression(
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

                expression = (IExpressionNode)model.SyntaxStack.Pop();
            }

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

            if (SyntaxKind.CommaToken == model.TokenWalker.Current.SyntaxKind)
            {
                _ = model.TokenWalker.Consume();
                // TODO: Track comma tokens?
                //
                // mutableFunctionParametersListing.Add(_cSharpParser._tokenWalker.Consume());
            }
            else if (SyntaxKind.CloseParenthesisToken == model.TokenWalker.Current.SyntaxKind)
            {
                _ = model.TokenWalker.Consume();
                break;
            }
        }

        // The expression logic will consume the CloseParenthesisToken, therefore backtrack once before matching
        _ = model.TokenWalker.Backtrack();
        var closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);

        model.SyntaxStack.Push(new FunctionParametersListingNode(
            openParenthesisToken,
            mutableFunctionParametersListing.ToImmutableArray(),
            closeParenthesisToken));
    }

    /// <summary>Use this method for function definition, whereas <see cref="HandleFunctionParameters"/> should be used for function invocation.</summary>
    public static void HandleFunctionArguments(ParserModel model)
    {
        var openParenthesisToken = (OpenParenthesisToken)model.SyntaxStack.Pop();

        if (SyntaxKind.CloseParenthesisToken == model.TokenWalker.Peek(0).SyntaxKind)
        {
            model.SyntaxStack.Push(new FunctionArgumentsListingNode(
                openParenthesisToken,
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
            var typeClauseNode = UtilityApi.MatchTypeClause(model);

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

            model.Binder.BindVariableDeclarationStatementNode(variableDeclarationStatementNode);

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
                    hasRefKeyword);
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
            openParenthesisToken,
            mutableFunctionArgumentListing.ToImmutableArray(),
            closeParenthesisToken));
    }

    /// <summary>TODO: Correctly implement this method. For now going to skip until the attribute closing square bracket.</summary>
    public static void HandleAttribute(ParserModel model)
    {
        var openSquareBracketToken = (OpenSquareBracketToken)model.SyntaxStack.Pop();

        ISyntaxToken tokenCurrent;
        var innerTokens = new List<ISyntaxToken>();

        while (true)
        {
            tokenCurrent = model.TokenWalker.Consume();

            if (tokenCurrent.SyntaxKind == SyntaxKind.EndOfFileToken ||
                tokenCurrent.SyntaxKind == SyntaxKind.CloseSquareBracketToken)
            {
                break;
            }
            else
            {
                innerTokens.Add(tokenCurrent);
            }
        }

        if (tokenCurrent.SyntaxKind == SyntaxKind.CloseSquareBracketToken)
        {
            model.SyntaxStack.Push(model.Binder.BindAttributeNode(
                openSquareBracketToken,
                innerTokens,
                (CloseSquareBracketToken)tokenCurrent));
        }
    }

    public static void HandleExpression(
        IExpressionNode? topMostExpressionNode,
        IExpressionNode? previousInvocationExpressionNode,
        IExpressionNode? leftExpressionNode,
        ISyntaxToken? operatorToken,
        IExpressionNode? rightExpressionNode,
        ExpressionDelimiter[]? extraExpressionDeliminaters, ParserModel model)
    {
        while (!model.TokenWalker.IsEof)
        {
            var tokenCurrent = model.TokenWalker.Consume();

            if (tokenCurrent.SyntaxKind == SyntaxKind.EndOfFileToken || tokenCurrent.SyntaxKind == SyntaxKind.StatementDelimiterToken)
            {
                model.TokenWalker.Backtrack();
                break;
            }

            ExpressionDelimiter? closeExtraExpressionDelimiterEncountered =
                extraExpressionDeliminaters?.FirstOrDefault(x => x.CloseSyntaxKind == tokenCurrent.SyntaxKind);

            if (closeExtraExpressionDelimiterEncountered is not null)
            {
                if (tokenCurrent.SyntaxKind == SyntaxKind.CloseParenthesisToken)
                {
                    if (closeExtraExpressionDelimiterEncountered?.OpenSyntaxToken is not null)
                    {
                        ParenthesizedExpressionNode parenthesizedExpression;

                        if (previousInvocationExpressionNode is not null)
                        {
                            parenthesizedExpression = new ParenthesizedExpressionNode(
                                (OpenParenthesisToken)closeExtraExpressionDelimiterEncountered.OpenSyntaxToken,
                                previousInvocationExpressionNode,
                                (CloseParenthesisToken)tokenCurrent);
                        }
                        else
                        {
                            parenthesizedExpression = new ParenthesizedExpressionNode(
                                (OpenParenthesisToken)closeExtraExpressionDelimiterEncountered.OpenSyntaxToken,
                                new EmptyExpressionNode(CSharpFacts.Types.Void.ToTypeClause()),
                                (CloseParenthesisToken)tokenCurrent);
                        }

                        model.SyntaxStack.Push(parenthesizedExpression);
                        return;
                    }
                    else
                    {
                        // If one provides 'CloseParenthesisToken' as a closing delimiter,
                        // but does not provide the corresponding open delimiter (it is null)
                        // then a function invocation started the initial invocation
                        // of this method.
                        model.TokenWalker.Backtrack();
                        break;
                    }
                }
                else if (tokenCurrent.SyntaxKind == SyntaxKind.CommaToken)
                {
                    model.TokenWalker.Backtrack();
                    break;
                }
            }

            switch (tokenCurrent.SyntaxKind)
            {
                case SyntaxKind.TrueTokenKeyword:
                case SyntaxKind.FalseTokenKeyword:
                    var booleanLiteralExpressionNode = new LiteralExpressionNode(tokenCurrent, CSharpFacts.Types.Bool.ToTypeClause());

                    previousInvocationExpressionNode = booleanLiteralExpressionNode;

                    if (topMostExpressionNode is null)
                        topMostExpressionNode = booleanLiteralExpressionNode;
                    else if (leftExpressionNode is null)
                        leftExpressionNode = booleanLiteralExpressionNode;
                    else if (rightExpressionNode is null)
                        rightExpressionNode = booleanLiteralExpressionNode;
                    else
                        throw new ApplicationException("TODO: Why would this occur?");

                    break;
                case SyntaxKind.NumericLiteralToken:
                    var numericLiteralExpressionNode = new LiteralExpressionNode(tokenCurrent, CSharpFacts.Types.Int.ToTypeClause());

                    previousInvocationExpressionNode = numericLiteralExpressionNode;

                    if (topMostExpressionNode is null)
                        topMostExpressionNode = numericLiteralExpressionNode;
                    else if (leftExpressionNode is null)
                        leftExpressionNode = numericLiteralExpressionNode;
                    else if (rightExpressionNode is null)
                        rightExpressionNode = numericLiteralExpressionNode;
                    else
                        throw new ApplicationException("TODO: Why would this occur?");

                    break;
                case SyntaxKind.StringLiteralToken:
                    var stringLiteralExpressionNode = new LiteralExpressionNode(tokenCurrent, CSharpFacts.Types.String.ToTypeClause());

                    previousInvocationExpressionNode = stringLiteralExpressionNode;

                    if (topMostExpressionNode is null)
                        topMostExpressionNode = stringLiteralExpressionNode;
                    else if (leftExpressionNode is null)
                        leftExpressionNode = stringLiteralExpressionNode;
                    else if (rightExpressionNode is null)
                        rightExpressionNode = stringLiteralExpressionNode;
                    else
                        throw new ApplicationException("TODO: Why would this occur?");

                    break;
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.StarToken:
                case SyntaxKind.DivisionToken:
                    if (leftExpressionNode is null && previousInvocationExpressionNode is not null)
                        leftExpressionNode = previousInvocationExpressionNode;

                    if (previousInvocationExpressionNode is BinaryExpressionNode previousBinaryExpressionNode)
                    {
                        var previousOperatorPrecedence = UtilityApi.GetOperatorPrecedence(previousBinaryExpressionNode.BinaryOperatorNode.OperatorToken.SyntaxKind);
                        var currentOperatorPrecedence = UtilityApi.GetOperatorPrecedence(tokenCurrent.SyntaxKind);

                        if (currentOperatorPrecedence > previousOperatorPrecedence)
                        {
                            // Take the right node from the previous expression.
                            // Make it the new expression's left node.
                            //
                            // Then replace the previous expression's right node with the
                            // newly formed expression.

                            HandleExpression(
                                topMostExpressionNode,
                                null,
                                previousBinaryExpressionNode.RightExpressionNode,
                                tokenCurrent,
                                null,
                                extraExpressionDeliminaters,
                                model);

                            var modifiedRightExpressionNode = (IExpressionNode)model.SyntaxStack.Pop();

                            topMostExpressionNode = new BinaryExpressionNode(
                                previousBinaryExpressionNode.LeftExpressionNode,
                                previousBinaryExpressionNode.BinaryOperatorNode,
                                modifiedRightExpressionNode);
                        }
                    }

                    if (operatorToken is null)
                        operatorToken = tokenCurrent;
                    else
                        throw new ApplicationException("TODO: Why would this occur?");

                    break;
                case SyntaxKind.OpenParenthesisToken:
                    var copyExtraExpressionDeliminaters = new List<ExpressionDelimiter>(extraExpressionDeliminaters ?? Array.Empty<ExpressionDelimiter>());

                    copyExtraExpressionDeliminaters.Insert(0, new ExpressionDelimiter(
                        SyntaxKind.OpenParenthesisToken,
                        SyntaxKind.CloseParenthesisToken,
                        tokenCurrent,
                        null));

                    HandleExpression(
                        null,
                        null,
                        null,
                        null,
                        null,
                        copyExtraExpressionDeliminaters.ToArray(),
                        model);

                    var parenthesizedExpression = (IExpressionNode)model.SyntaxStack.Pop();

                    previousInvocationExpressionNode = parenthesizedExpression;

                    if (topMostExpressionNode is null)
                        topMostExpressionNode = parenthesizedExpression;
                    else if (leftExpressionNode is null)
                        leftExpressionNode = parenthesizedExpression;
                    else if (rightExpressionNode is null)
                        rightExpressionNode = parenthesizedExpression;
                    else
                        throw new ApplicationException("TODO: Why would this occur?");
                    break;
                default:
                    if (tokenCurrent.SyntaxKind == SyntaxKind.DollarSignToken)
                    {
                        // TODO: Convert DollarSignToken to a function signature...
                        // ...Then read in the parameters...
                        // ...Any function invocation logic also would be done here

                        model.Binder.BindStringInterpolationExpression((DollarSignToken)tokenCurrent);
                    }

                    break;
            }

            if (leftExpressionNode is not null && operatorToken is not null && rightExpressionNode is not null)
            {
                var binaryOperatorNode = model.Binder.BindBinaryOperatorNode(
                    leftExpressionNode,
                    operatorToken,
                    rightExpressionNode);

                var binaryExpressionNode = new BinaryExpressionNode(
                    leftExpressionNode,
                    binaryOperatorNode,
                    rightExpressionNode);

                topMostExpressionNode = binaryExpressionNode;
                previousInvocationExpressionNode = binaryExpressionNode;

                leftExpressionNode = null;
                operatorToken = null;
                rightExpressionNode = null;

                HandleExpression(
                    topMostExpressionNode,
                    previousInvocationExpressionNode,
                    leftExpressionNode,
                    operatorToken,
                    rightExpressionNode,
                    extraExpressionDeliminaters,
                    model);
                
                return;
            }
        }

        var fallbackExpressionNode = new LiteralExpressionNode(
            new EndOfFileToken(new(0, 0, (byte)GenericDecorationKind.None, new(string.Empty), string.Empty)),
            CSharpFacts.Types.Void.ToTypeClause());

        model.SyntaxStack.Push(topMostExpressionNode ?? fallbackExpressionNode);
    }

    public static void HandleAsTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleBaseTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleBoolTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleBreakTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleByteTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleCaseTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleCatchTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleCharTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleCheckedTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleConstTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleContinueTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleDecimalTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleDefaultTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleDelegateTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleDoTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleDoubleTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleElseTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleEnumTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleEventTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleExplicitTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleExternTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleFalseTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleFinallyTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleFixedTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleFloatTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleForTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleForeachTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleGotoTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleImplicitTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleInTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleIntTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleIsTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleLockTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleLongTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleNullTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleObjectTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleOperatorTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleOutTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleParamsTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleProtectedTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleReadonlyTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleRefTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleSbyteTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleShortTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleSizeofTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleStackallocTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleStringTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleStructTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleSwitchTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleThisTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleThrowTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleTrueTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleTryTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleTypeofTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleUintTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleUlongTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleUncheckedTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleUnsafeTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleUshortTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleVoidTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleVolatileTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleWhileTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleUnrecognizedTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleDefault(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        if (UtilityApi.IsTypeIdentifierKeywordSyntaxKind(keywordToken.SyntaxKind))
        {
            // One enters this conditional block with the 'keywordToken' having already been consumed.
            model.TokenWalker.Backtrack();

            var typeClauseNode = UtilityApi.MatchTypeClause(model);
            
            model.SyntaxStack.Push(typeClauseNode);
        }
        else
        {
            throw new NotImplementedException($"Implement the {keywordToken.SyntaxKind} keyword.");
        }
    }

    public static void HandleTypeIdentifierKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        if (UtilityApi.IsTypeIdentifierKeywordSyntaxKind(keywordToken.SyntaxKind))
        {
            // One enters this conditional block with the 'keywordToken' having already been consumed.
            model.TokenWalker.Backtrack();

            var typeClauseNode = UtilityApi.MatchTypeClause(model);

            if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.AttributeNode)
                typeClauseNode.AttributeNode = (AttributeNode)model.SyntaxStack.Pop();

            model.SyntaxStack.Push(typeClauseNode);
        }
        else
        {
            throw new NotImplementedException($"Implement the {keywordToken.SyntaxKind} keyword.");
        }
    }

    public static void HandleNewTokenKeyword(ParserModel model)
    {
        var typeClauseToken = UtilityApi.MatchTypeClause(model);

        if (model.TokenWalker.Peek(0).SyntaxKind == SyntaxKind.MemberAccessToken)
        {
            // "explicit namespace qualification" OR "nested class"
            throw new NotImplementedException();
        }

        // TODO: Fix _cSharpParser.model.Binder.TryGetClassReferenceHierarchically, it broke on (2023-07-26)
        //
        // _cSharpParser.model.Binder.TryGetClassReferenceHierarchically(typeClauseToken, null, out boundClassReferenceNode);

        // TODO: combine the logic for 'new()' without a type identifier and 'new List<int>()' with a type identifier. To start I am going to isolate them in their own if conditional blocks.
        if (typeClauseToken.IsFabricated)
        {
            // If "new()" LACKS a type identifier then the OpenParenthesisToken must be there. This is true even still for when there is object initialization OpenBraceToken. For new() the parenthesis are required.
            // valid inputs:
            //     new()
            //     new(){}
            //     new(...)
            //     new(...){}

            model.SyntaxStack.Push(model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken));
            HandleFunctionParameters(model);

            ObjectInitializationNode? boundObjectInitializationNode = null;

            if (model.TokenWalker.Peek(0).SyntaxKind == SyntaxKind.OpenBraceToken)
            {
                model.SyntaxStack.Push((OpenBraceToken)model.TokenWalker.Consume());

                HandleObjectInitialization(model);
                boundObjectInitializationNode = (ObjectInitializationNode?)model.SyntaxStack.Pop();
            }

            // TODO: Fix _cSharpParser.model.Binder.BindConstructorInvocationNode, it broke on (2023-07-26)
            //
            // var boundConstructorInvocationNode = _cSharpParser.model.Binder.BindConstructorInvocationNode(
            //     keywordToken,
            //     boundClassReferenceNode,
            //     boundFunctionArgumentsNode,
            //     boundObjectInitializationNode);
            //
            // _cSharpParser._currentCodeBlockBuilder.Children.Add(boundConstructorInvocationNode);
        }
        else
        {
            // If "new List<int>()" HAS a type identifier then the OpenParenthesisToken is optional, given that the object initializer syntax OpenBraceToken is found, and one wishes to invoke the parameterless constructor.
            // valid inputs:
            //     new List<int>()
            //     new List<int>(){}
            //     new List<int>{}
            //     new List<int>(...)
            //     new List<int>(...){}
            //     new string(...){}

            if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
            {
                model.SyntaxStack.Push((OpenAngleBracketToken)model.TokenWalker.Consume());
                HandleGenericArguments(model);
            }

            FunctionParametersListingNode? functionParametersListingNode = null;

            if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
            {
                model.SyntaxStack.Push((OpenParenthesisToken)model.TokenWalker.Consume());
                HandleFunctionParameters(model);

                functionParametersListingNode = (FunctionParametersListingNode?)model.SyntaxStack.Pop();
            }

            ObjectInitializationNode? boundObjectInitializationNode = null;

            if (model.TokenWalker.Peek(0).SyntaxKind == SyntaxKind.OpenBraceToken)
            {
                model.SyntaxStack.Push((OpenBraceToken)model.TokenWalker.Consume());
                HandleObjectInitialization(model);

                boundObjectInitializationNode = (ObjectInitializationNode?)model.SyntaxStack.Pop();
            }

            // TODO: Fix _cSharpParser.model.Binder.BindConstructorInvocationNode, it broke on (2023-07-26)
            //
            // var boundConstructorInvocationNode = _cSharpParser.model.Binder.BindConstructorInvocationNode(
            //     keywordToken,
            //     boundClassReferenceNode,
            //     functionParametersListingNode,
            //     boundObjectInitializationNode);
            //
            // _cSharpParser._currentCodeBlockBuilder.Children.Add(boundConstructorInvocationNode);
        }
    }

    public static void HandlePublicTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleInternalTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandlePrivateTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleStaticTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleOverrideTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleVirtualTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleAbstractTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleSealedTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleIfTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        var openParenthesisToken = model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);

        if (openParenthesisToken.IsFabricated)
            return;

        HandleExpression(
            null,
            null,
            null,
            null,
            null,
            new[]
            {
                new ExpressionDelimiter(
                    SyntaxKind.OpenParenthesisToken,
                    SyntaxKind.CloseParenthesisToken,
                    null,
                    null)
            },
            model);

        var expression = (IExpressionNode)model.SyntaxStack.Pop();

        var boundIfStatementNode = model.Binder.BindIfStatementNode(keywordToken, expression);
        model.SyntaxStack.Push(boundIfStatementNode);
    }

    public static void HandleUsingTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        HandleNamespaceIdentifier(model);

        var namespaceIdentifier = (IdentifierToken)model.SyntaxStack.Pop();

        var boundUsingStatementNode = model.Binder.BindUsingStatementNode(
            keywordToken,
            namespaceIdentifier);

        model.CurrentCodeBlockBuilder.ChildList.Add(boundUsingStatementNode);
        model.SyntaxStack.Push(boundUsingStatementNode);
    }

    public static void HandleInterfaceTokenKeyword(ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);

        GenericArgumentsListingNode? genericArgumentsListingNode = null;

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
        {
            model.SyntaxStack.Push((OpenAngleBracketToken)model.TokenWalker.Consume());
            HandleGenericArguments(model);

            genericArgumentsListingNode = (GenericArgumentsListingNode?)model.SyntaxStack.Pop();
        }

        var typeDefinitionNode = new TypeDefinitionNode(
            identifierToken,
            null,
            genericArgumentsListingNode,
            null,
            null)
        {
            IsInterface = true
        };

        model.Binder.BindTypeDefinitionNode(typeDefinitionNode);
        model.Binder.BindTypeIdentifier(identifierToken);
        model.SyntaxStack.Push(typeDefinitionNode);
    }

    public static void HandleClassTokenKeyword(ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);

        GenericArgumentsListingNode? genericArgumentsListingNode = null;

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
        {
            model.SyntaxStack.Push((OpenAngleBracketToken)model.TokenWalker.Consume());
            HandleGenericArguments(model);

            genericArgumentsListingNode = (GenericArgumentsListingNode?)model.SyntaxStack.Pop();
        }

        var typeDefinitionNode = new TypeDefinitionNode(
            identifierToken,
            null,
            genericArgumentsListingNode,
            null,
            null);

        model.Binder.BindTypeDefinitionNode(typeDefinitionNode);
        model.Binder.BindTypeIdentifier(identifierToken);
        model.SyntaxStack.Push(typeDefinitionNode);
    }

    public static void HandleNamespaceTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();

        HandleNamespaceIdentifier(model);
        var namespaceIdentifier = (IdentifierToken)model.SyntaxStack.Pop();

        if (model.FinalizeNamespaceFileScopeCodeBlockNodeAction is not null)
        {
            throw new NotImplementedException(
                "Need to add logic to report diagnostic when there is" +
                " already a file scoped namespace.");
        }

        var namespaceStatementNode = new NamespaceStatementNode(
            keywordToken,
            namespaceIdentifier,
            new CodeBlockNode(ImmutableArray<ISyntax>.Empty));

        model.SyntaxStack.Push(namespaceStatementNode);
    }

    public static void HandleReturnTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();

        HandleExpression(
            null,
            null,
            null,
            null,
            null,
            null,
            model);

        var returnExpression = (IExpressionNode)model.SyntaxStack.Pop();

        var boundReturnStatementNode = model.Binder.BindReturnStatementNode(
            keywordToken,
            returnExpression);

        model.CurrentCodeBlockBuilder.ChildList.Add(boundReturnStatementNode);

        model.SyntaxStack.Push(boundReturnStatementNode);
    }

    public static void HandleVarTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();

        // Check if previous statement is finished, and a new one is starting.
        // TODO: 'Peek(-2)' is horribly confusing. The reason for using -2 is that one consumed the 'var' keyword and moved their position forward by 1. So to read the token behind 'var' one must go back 2 tokens. It feels natural to put '-1' and then this evaluates to the wrong token. Should an expression bound property be made for 'Peek(-2)'?
        var previousToken = model.TokenWalker.Peek(-2);

        if (previousToken.SyntaxKind == SyntaxKind.StatementDelimiterToken ||
            previousToken.SyntaxKind == SyntaxKind.BadToken)
        {
            // Check if the next token is a second 'var keyword' or an IdentifierToken. Two IdentifierTokens is invalid, and therefore one can contextually take this 'var' as a keyword.
            bool nextTokenIsVarKeyword = SyntaxKind.VarTokenContextualKeyword == model.TokenWalker.Current.SyntaxKind;

            bool nextTokenIsIdentifierToken = SyntaxKind.IdentifierToken == model.TokenWalker.Current.SyntaxKind;

            if (nextTokenIsVarKeyword || nextTokenIsIdentifierToken)
            {
                var varKeyword = new TypeClauseNode(
                    contextualKeywordToken,
                    null,
                    null);

                model.SyntaxStack.Push(varKeyword);
            }
        }
        else
        {
            // Take 'var' as an identifier
            IdentifierToken varIdentifierToken = new(contextualKeywordToken.TextSpan);

            model.SyntaxStack.Push(varIdentifierToken);

            TokenApi.ParseIdentifierToken(model);
        }
    }

    public static void HandlePartialTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleAddTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleAndTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleAliasTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleAscendingTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleArgsTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleAsyncTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleAwaitTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleByTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleDescendingTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleDynamicTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleEqualsTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleFileTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleFromTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleGetTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleGlobalTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleGroupTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleInitTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleIntoTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleJoinTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleLetTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleManagedTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleNameofTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleNintTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleNotTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleNotnullTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleNuintTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleOnTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleOrTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleOrderbyTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleRecordTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleRemoveTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleRequiredTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleScopedTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleSelectTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleSetTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleUnmanagedTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleValueTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleWhenTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleWhereTokenContextualKeyword(ParserModel model)
    {
        var whereKeywordContextualToken = (KeywordContextualToken)model.SyntaxStack.Pop();

        if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.FunctionDefinitionNode)
        {
            var functionDefinitionNode = (FunctionDefinitionNode)model.SyntaxStack.Pop();

            /*
             Examples:

             public static T Clone<T>(T item) where T : class
             {
                 return item;
             }

             public static T Clone<T>(T item) where T : class => item;
            */

            // TODO: Implement generic constraints, until then just read until the generic...
            // ...constraint is finished.

            var constraintNodeInnerTokens = new List<ISyntaxToken>
            {
                whereKeywordContextualToken
            };

            while (!model.TokenWalker.IsEof)
            {
                if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
                    model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
                {
                    break;
                }

                constraintNodeInnerTokens.Add(model.TokenWalker.Consume());
            }

            var constraintNode = new ConstraintNode(constraintNodeInnerTokens.ToImmutableArray());

            model.SyntaxStack.Push(new FunctionDefinitionNode(
                functionDefinitionNode.ReturnTypeClauseNode,
                functionDefinitionNode.FunctionIdentifier,
                functionDefinitionNode.GenericArgumentsListingNode,
                functionDefinitionNode.FunctionArgumentsListingNode,
                functionDefinitionNode.FunctionBodyCodeBlockNode,
                constraintNode));
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public static void HandleWithTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleYieldTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleUnrecognizedTokenContextualKeyword(ParserModel model)
    {
        var contextualKeywordToken = (KeywordContextualToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }
}