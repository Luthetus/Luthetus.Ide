using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.CSharp.BinderCase;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.CompilerServices.Lang.CSharp.Facts;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase;

public partial class CSharpParser : IParser
{
    /// <summary>
    /// Language specific
    /// </summary>
    private class SpecificApi
    {
        private readonly CSharpParser _parser;

        public SpecificApi(CSharpParser cSharpParser)
        {
            _parser = cSharpParser;
        }

        /// <summary>TODO: I don't like this <see cref="TokenWalker"/> property. It points to a private field on a different object. But without this property things are incredibly verbose. I need to remember to come back to this and change how I get access to the object because this doesn't feel right.</summary>
        public TokenWalker TokenWalker => _parser._tokenWalker;

        /// <summary>TODO: I don't like this <see cref="Binder"/> property. It points to a private field on a different object. But without this property things are incredibly verbose. I need to remember to come back to this and change how I get access to the object because this doesn't feel right.</summary>
        public CSharpBinder Binder => _parser.Binder;

        /// <summary>TODO: I don't like this <see cref="Utility"/> property. It points to a private field on a different object. But without this property things are incredibly verbose. I need to remember to come back to this and change how I get access to the object because this doesn't feel right.</summary>
        public UtilityApi Utility => _parser._utility;

        /// <summary>TODO: I don't like this <see cref="DiagnosticBag"/> property. It points to a private field on a different object. But without this property things are incredibly verbose. I need to remember to come back to this and change how I get access to the object because this doesn't feel right.</summary>
        public LuthetusDiagnosticBag DiagnosticBag => _parser._diagnosticBag;

        /// <summary>TODO: I don't like this <see cref="General"/> property. It points to a private field on a different object. But without this property things are incredibly verbose. I need to remember to come back to this and change how I get access to the object because this doesn't feel right.</summary>
        public GeneralApi General => _parser._general;

        /// <summary>TODO: I don't like this <see cref="NodeRecent"/> property. It points to a private field on a different object. But without this property things are incredibly verbose. I need to remember to come back to this and change how I get access to the object because this doesn't feel right.</summary>
        public ISyntaxNode? NodeRecent 
        {
            get => _parser._nodeRecent;
            set => _parser._nodeRecent = value;
        }

        /// <summary>TODO: I don't like this <see cref="NodeRecent"/> property. It points to a private field on a different object. But without this property things are incredibly verbose. I need to remember to come back to this and change how I get access to the object because this doesn't feel right.</summary>
        public CodeBlockBuilder CurrentCodeBlockBuilder
        {
            get => _parser._currentCodeBlockBuilder;
            set => _parser._currentCodeBlockBuilder = value;
        }

        public TypeClauseNode HandleStaticClassIdentifier(IdentifierToken identifierToken)
        {
            TokenWalker.Backtrack();

            var typeClauseNode = Utility.MatchTypeClause();
            NodeRecent = typeClauseNode;

            return typeClauseNode;
        }

        public AmbiguousIdentifierNode HandleUndefinedTypeOrNamespaceReference(IdentifierToken identifierToken)
        {
            var identifierReferenceNode = new AmbiguousIdentifierNode(
                identifierToken);

            Binder.BindTypeIdentifier(identifierToken);

            DiagnosticBag.ReportUndefinedTypeOrNamespace(
                identifierToken.TextSpan,
                identifierToken.TextSpan.GetText());

            NodeRecent = identifierReferenceNode;
            return identifierReferenceNode;
        }

        public VariableReferenceNode HandleVariableReference(
            IdentifierToken identifierToken,
            VariableDeclarationStatementNode variableDeclarationStatementNode)
        {
            var variableReferenceNode = new VariableReferenceNode(
                identifierToken,
                variableDeclarationStatementNode);

            Binder.BindVariableReferenceNode(variableReferenceNode);

            NodeRecent = variableReferenceNode;
            return variableReferenceNode;
        }

        public FunctionInvocationNode HandleFunctionInvocation(IdentifierToken identifierToken)
        {
            // TODO: (2023-06-04) I believe this if block will run for '<' mathematical operator.

            GenericParametersListingNode? genericParametersListingNode = null;

            if (SyntaxKind.OpenAngleBracketToken == TokenWalker.Current.SyntaxKind)
            {
                var openAngleBracketToken = (OpenAngleBracketToken)TokenWalker.Consume();
                genericParametersListingNode = HandleGenericParameters(openAngleBracketToken);
            }

            var openParenthesisToken = (OpenParenthesisToken)TokenWalker.Match(SyntaxKind.OpenParenthesisToken);

            var functionParametersListingNode = HandleFunctionParameters(openParenthesisToken);

            var functionInvocationNode = new FunctionInvocationNode(
                identifierToken,
                null,
                genericParametersListingNode,
                functionParametersListingNode);

            Binder.BindFunctionInvocationNode(functionInvocationNode);

            CurrentCodeBlockBuilder.ChildBag.Add(functionInvocationNode);
            NodeRecent = null;

            return functionInvocationNode;
        }

        public VariableDeclarationStatementNode HandleVariableDeclaration(TypeClauseNode typeClauseNode, IdentifierToken identifierToken)
        {
            var variableDeclarationStatementNode = new VariableDeclarationStatementNode(
                typeClauseNode,
                identifierToken,
                false);

            Binder.BindVariableDeclarationStatementNode(variableDeclarationStatementNode);

            CurrentCodeBlockBuilder.ChildBag.Add(variableDeclarationStatementNode);

            if (SyntaxKind.EqualsToken == TokenWalker.Current.SyntaxKind)
            {
                // Variable initialization occurs here.
                HandleVariableAssignment(identifierToken);
            }

            // This conditional branch is for variable declaration, so at this point a StatementDelimiterToken is always expected.
            _ = TokenWalker.Match(SyntaxKind.StatementDelimiterToken);

            NodeRecent = null;
            return variableDeclarationStatementNode;
        }

        public FunctionDefinitionNode HandleFunctionDefinition(
            TypeClauseNode typeClauseNode,
            IdentifierToken identifierToken,
            GenericArgumentsListingNode? genericArgumentsListingNode)
        {
            var openParenthesisToken = (OpenParenthesisToken)TokenWalker.Consume();
            var functionArgumentsListingNode = HandleFunctionArguments(openParenthesisToken);

            var functionDefinitionNode = new FunctionDefinitionNode(
                typeClauseNode,
                identifierToken,
                genericArgumentsListingNode,
                functionArgumentsListingNode,
                null);

            Binder.BindFunctionDefinitionNode(functionDefinitionNode);

            NodeRecent = functionDefinitionNode;

            if (CurrentCodeBlockBuilder.CodeBlockOwner is TypeDefinitionNode typeDefinitionNode &&
                typeDefinitionNode.IsInterface)
            {
                // TODO: Would method constraints break this code? "public T Aaa<T>() where T : OtherClass"
                var statementDelimiterToken = TokenWalker.Match(SyntaxKind.StatementDelimiterToken);

                // TODO: Fabricating an OpenBraceToken in order to not duplicate the logic within 'ParseOpenBraceToken(...)' seems silly. This likely should be changed
                var openBraceToken = new OpenBraceToken(statementDelimiterToken.TextSpan)
                {
                    IsFabricated = true
                };

                General.ParseOpenBraceToken(openBraceToken);

                // TODO: Fabricating a CloseBraceToken in order to not duplicate the logic within 'ParseOpenBraceToken(...)' seems silly. This likely should be changed
                var closeBraceToken = new CloseBraceToken(statementDelimiterToken.TextSpan)
                {
                    IsFabricated = true
                };

                General.ParseCloseBraceToken(closeBraceToken);
            }

            return functionDefinitionNode;
        }

        public FunctionDefinitionNode HandleConstructorDefinition(IdentifierToken identifierToken)
        {
            GenericArgumentsListingNode? genericArgumentsListingNode = null;

            if (SyntaxKind.OpenAngleBracketToken == TokenWalker.Current.SyntaxKind)
            {
                var openAngleBracketToken = (OpenAngleBracketToken)TokenWalker.Consume();
                genericArgumentsListingNode = HandleGenericArguments(openAngleBracketToken);
            }

            var openParenthesisToken = (OpenParenthesisToken)TokenWalker.Consume();
            var functionArgumentsListingNode = HandleFunctionArguments(openParenthesisToken);

            if (CurrentCodeBlockBuilder.CodeBlockOwner is not TypeDefinitionNode typeDefinitionNode)
            {
                throw new ApplicationException("Constructors need to be within a type definition.");
            }

            var typeClauseNode = new TypeClauseNode(
                typeDefinitionNode.TypeIdentifier,
                null,
                null);

            var functionDefinitionNode = new FunctionDefinitionNode(
                typeClauseNode,
                identifierToken,
                genericArgumentsListingNode,
                functionArgumentsListingNode,
                null);

            Binder.BindConstructorDefinitionIdentifierToken(identifierToken);

            NodeRecent = functionDefinitionNode;
            return functionDefinitionNode;
        }

        public void HandleNamespaceReference(
            IdentifierToken identifierToken,
            NamespaceStatementNode boundNamespaceStatementNode)
        {
            Binder.BindNamespaceReference(identifierToken);

            if (SyntaxKind.MemberAccessToken == TokenWalker.Current.SyntaxKind)
            {
                var memberAccessToken = TokenWalker.Consume();

                var memberIdentifierToken = (IdentifierToken)TokenWalker.Match(SyntaxKind.IdentifierToken);

                if (memberIdentifierToken.IsFabricated)
                    throw new NotImplementedException("Implement a namespace being member accessed, but the next token is not an IdentifierToken");

                // Check all the TypeDefinitionNodes that are in the namespace
                var typeDefinitionNodes = boundNamespaceStatementNode.GetTopLevelTypeDefinitionNodes();

                var typeDefinitionNode = typeDefinitionNodes.SingleOrDefault(td =>
                    td.TypeIdentifier.TextSpan.GetText() == memberIdentifierToken.TextSpan.GetText());

                if (typeDefinitionNode is null)
                    throw new NotImplementedException("A namespace member access, where the identifier for the member which is accessed was not a type definition.");

                HandleTypeReference(memberIdentifierToken, typeDefinitionNode);
                return;
            }
            else
            {
                // TODO: (2023-05-28) Report an error diagnostic for 'namespaces are not statements'. Something like this I'm not sure.
                TokenWalker.Consume();
                return;
            }
        }

        public void HandleTypeReference(IdentifierToken memberIdentifierToken, TypeDefinitionNode typeDefinitionNode)
        {
            Binder.BindTypeIdentifier(memberIdentifierToken);

            var memberAccessToken = (MemberAccessToken)TokenWalker.Match(SyntaxKind.MemberAccessToken);

            if (memberAccessToken.IsFabricated)
                throw new NotImplementedException("Implement a static class being member accessed, but the statement ends there --- it is incomplete.");

            var identifierToken = (IdentifierToken)TokenWalker.Match(SyntaxKind.IdentifierToken);

            if (identifierToken.IsFabricated)
                throw new NotImplementedException("Implement a static class being member accessed, but the statement ends there --- it is incomplete.");

            var matchingFunctionDefinitionNodes = typeDefinitionNode
                .GetFunctionDefinitionNodes()
                .Where(fd =>
                    fd.FunctionIdentifier.TextSpan.GetText() == identifierToken.TextSpan.GetText())
                .ToImmutableArray();

            HandleFunctionReferences(identifierToken, matchingFunctionDefinitionNodes);
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
        public void HandleFunctionReferences(
            IdentifierToken functionInvocationIdentifierToken,
            ImmutableArray<FunctionDefinitionNode> functionDefinitionNodes)
        {
            var concatenatedGetTextResults = string.Join(
                '\n',
                functionDefinitionNodes.Select(fd => fd.GetTextRecursively()));

            // (2023-08-03) The input 'System.Console.WriteLine();' had 18 matches.

            if (SyntaxKind.OpenParenthesisToken == TokenWalker.Current.SyntaxKind)
            {
                var openParenthesisToken = (OpenParenthesisToken)TokenWalker.Consume();

                var functionParametersListingNode = HandleFunctionParameters(openParenthesisToken);

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
                    throw new ApplicationException("Handle case where none of the function overloads match the input.");

                // TODO: Don't assume GenericParametersListingNode to be null
                var functionInvocationNode = new FunctionInvocationNode(
                    functionInvocationIdentifierToken,
                    matchingOverload,
                    null,
                    functionParametersListingNode);

                Binder.BindFunctionInvocationNode(functionInvocationNode);

                if (SyntaxKind.StatementDelimiterToken == TokenWalker.Current.SyntaxKind)
                {
                    _ = TokenWalker.Consume();

                    NodeRecent = null;
                    CurrentCodeBlockBuilder.ChildBag.Add(functionInvocationNode);
                }
                else
                {
                    NodeRecent = functionInvocationNode;
                }
            }
        }

        public VariableAssignmentExpressionNode HandleVariableAssignment(IdentifierToken identifierToken)
        {
            // Move past the EqualsToken
            var equalsToken = (EqualsToken)TokenWalker.Consume();

            var rightHandExpression = HandleExpression();

            var variableAssignmentExpressionNode = new VariableAssignmentExpressionNode(
                identifierToken,
                equalsToken,
                rightHandExpression);

            Binder.BindVariableAssignmentExpressionNode(variableAssignmentExpressionNode);

            CurrentCodeBlockBuilder.ChildBag.Add(variableAssignmentExpressionNode);
            return variableAssignmentExpressionNode;
        }

        /// <summary>TODO: Correctly parse object initialization. For now, just skip over it when parsing.</summary>
        public ObjectInitializationNode HandleObjectInitialization(OpenBraceToken openBraceToken)
        {
            ISyntaxToken shouldBeCloseBraceToken = new BadToken(openBraceToken.TextSpan);

            while (!TokenWalker.IsEof)
            {
                shouldBeCloseBraceToken = TokenWalker.Consume();

                if (SyntaxKind.EndOfFileToken == shouldBeCloseBraceToken.SyntaxKind ||
                    SyntaxKind.CloseBraceToken == shouldBeCloseBraceToken.SyntaxKind)
                {
                    break;
                }
            }

            if (SyntaxKind.CloseBraceToken != shouldBeCloseBraceToken.SyntaxKind)
                shouldBeCloseBraceToken = TokenWalker.Match(SyntaxKind.CloseBraceToken);

            return new ObjectInitializationNode(
                openBraceToken,
                (CloseBraceToken)shouldBeCloseBraceToken);
        }

        public IdentifierToken HandleNamespaceIdentifier()
        {
            var combineNamespaceIdentifierIntoOne = new List<ISyntaxToken>();

            while (!TokenWalker.IsEof)
            {
                if (combineNamespaceIdentifierIntoOne.Count % 2 == 0)
                {
                    var matchedToken = TokenWalker.Match(SyntaxKind.IdentifierToken);
                    combineNamespaceIdentifierIntoOne.Add(matchedToken);

                    if (matchedToken.IsFabricated)
                        break;
                }
                else
                {
                    if (SyntaxKind.MemberAccessToken == TokenWalker.Current.SyntaxKind)
                        combineNamespaceIdentifierIntoOne.Add(TokenWalker.Consume());
                    else
                        break;
                }
            }

            var identifierTextSpan = combineNamespaceIdentifierIntoOne.First().TextSpan with
            {
                EndingIndexExclusive = combineNamespaceIdentifierIntoOne.Last().TextSpan.EndingIndexExclusive
            };

            return new IdentifierToken(identifierTextSpan);
        }
        
        public LiteralExpressionNode HandleNumericLiteralExpression(NumericLiteralToken numericLiteralToken)
        {
            var literalExpressionNode = new LiteralExpressionNode(
                numericLiteralToken,
                CSharpLanguageFacts.Types.Int.ToTypeClause());

            literalExpressionNode = Binder.BindLiteralExpressionNode(literalExpressionNode);

            NodeRecent = literalExpressionNode;

            return literalExpressionNode;
        }
        
        public LiteralExpressionNode HandleStringLiteralExpression(StringLiteralToken stringLiteralToken)
        {
            var literalExpressionNode = new LiteralExpressionNode(
                stringLiteralToken,
                CSharpLanguageFacts.Types.String.ToTypeClause());

            literalExpressionNode = Binder.BindLiteralExpressionNode(literalExpressionNode);

            NodeRecent = literalExpressionNode;

            return literalExpressionNode;
        }

        /// <summary>
        /// This method is used for generic type usage such as, 'var words = new List&lt;string&gt;;'
        /// </summary>
        public GenericParametersListingNode? HandleGenericParameters(OpenAngleBracketToken openAngleBracketToken)
        {
            if (SyntaxKind.CloseAngleBracketToken == TokenWalker.Current.SyntaxKind)
            {
                return new GenericParametersListingNode(
                    openAngleBracketToken,
                    ImmutableArray<GenericParameterEntryNode>.Empty,
                    (CloseAngleBracketToken)TokenWalker.Consume());
            }

            var mutableGenericParametersListing = new List<GenericParameterEntryNode>();

            while (true)
            {
                // TypeClause
                var typeClauseNode = Utility.MatchTypeClause();

                if (typeClauseNode.IsFabricated)
                    break;

                var genericParameterEntryNode = new GenericParameterEntryNode(typeClauseNode);

                mutableGenericParametersListing.Add(genericParameterEntryNode);

                if (SyntaxKind.CommaToken == TokenWalker.Current.SyntaxKind)
                {
                    var commaToken = (CommaToken)TokenWalker.Consume();

                    // TODO: Track comma tokens?
                    //
                    // functionArgumentListing.Add(commaToken);
                }
                else
                {
                    break;
                }
            }

            var closeAngleBracketToken = (CloseAngleBracketToken)TokenWalker.Match(SyntaxKind.CloseAngleBracketToken);

            return new GenericParametersListingNode(
                openAngleBracketToken,
                mutableGenericParametersListing.ToImmutableArray(),
                closeAngleBracketToken);
        }

        /// <summary>
        /// This method is used for generic type definition such as, 'class List&lt;T&gt; { ... }'
        /// </summary>
        public GenericArgumentsListingNode? HandleGenericArguments(OpenAngleBracketToken openAngleBracketToken)
        {
            if (SyntaxKind.CloseAngleBracketToken == TokenWalker.Current.SyntaxKind)
            {
                return new GenericArgumentsListingNode(
                    openAngleBracketToken,
                    ImmutableArray<GenericArgumentEntryNode>.Empty,
                    (CloseAngleBracketToken)TokenWalker.Consume());
            }

            var mutableGenericArgumentsListing = new List<GenericArgumentEntryNode>();

            while (true)
            {
                // TypeClause
                var typeClauseNode = Utility.MatchTypeClause();

                if (typeClauseNode.IsFabricated)
                    break;

                var genericArgumentEntryNode = new GenericArgumentEntryNode(typeClauseNode);

                mutableGenericArgumentsListing.Add(genericArgumentEntryNode);

                if (SyntaxKind.CommaToken == TokenWalker.Current.SyntaxKind)
                {
                    var commaToken = (CommaToken)TokenWalker.Consume();

                    // TODO: Track comma tokens?
                    //
                    // functionArgumentListing.Add(commaToken);
                }
                else
                {
                    break;
                }
            }

            var closeAngleBracketToken = (CloseAngleBracketToken)TokenWalker.Match(SyntaxKind.CloseAngleBracketToken);

            return new GenericArgumentsListingNode(
                openAngleBracketToken,
                mutableGenericArgumentsListing.ToImmutableArray(),
                closeAngleBracketToken);
        }

        /// <summary>Use this method for function invocation, whereas <see cref="HandleFunctionArguments"/> should be used for function definition.</summary>
        public FunctionParametersListingNode HandleFunctionParameters(OpenParenthesisToken openParenthesisToken)
        {
            if (SyntaxKind.CloseParenthesisToken == TokenWalker.Current.SyntaxKind)
            {
                return new FunctionParametersListingNode(
                    openParenthesisToken,
                    ImmutableArray<FunctionParameterEntryNode>.Empty,
                    (CloseParenthesisToken)TokenWalker.Consume());
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
                    if (SyntaxKind.OutTokenKeyword == TokenWalker.Current.SyntaxKind)
                    {
                        _ = TokenWalker.Consume();
                        hasOutKeyword = true;

                        if (TokenWalker.Peek(1).SyntaxKind == SyntaxKind.IdentifierToken)
                        {
                            var outVariableTypeClause = Utility.MatchTypeClause();
                            var outVariableIdentifier = (IdentifierToken)TokenWalker.Peek(0);

                            HandleVariableDeclaration(
                                outVariableTypeClause,
                                outVariableIdentifier);
                        }
                    }
                    else if (SyntaxKind.InTokenKeyword == TokenWalker.Current.SyntaxKind)
                    {
                        _ = TokenWalker.Consume();
                        hasInKeyword = true;
                    }
                    else if (SyntaxKind.RefTokenKeyword == TokenWalker.Current.SyntaxKind)
                    {
                        _ = TokenWalker.Consume();
                        hasRefKeyword = true;
                    }
                }

                IExpressionNode expression;

                if (SyntaxKind.IdentifierToken == TokenWalker.Current.SyntaxKind)
                {
                    var variableIdentifierToken = (IdentifierToken)TokenWalker.Consume();

                    if (!Binder.TryGetVariableDeclarationHierarchically(
                            variableIdentifierToken.TextSpan.GetText(),
                            out var variableDeclarationNode)
                        || variableDeclarationNode is null)
                    {
                        variableDeclarationNode = new(
                            CSharpLanguageFacts.Types.Void.ToTypeClause(),
                            variableIdentifierToken,
                            false)
                        {
                            IsFabricated = true
                        };

                        Binder.BindVariableDeclarationStatementNode(variableDeclarationNode);
                    }

                    var variableReferenceNode = new VariableReferenceNode(
                        variableIdentifierToken,
                        variableDeclarationNode);

                    variableReferenceNode = Binder.BindVariableReferenceNode(variableReferenceNode);

                    expression = new VariableExpressionNode(
                        variableReferenceNode.VariableDeclarationStatementNode.TypeClauseNode);
                }
                else
                {
                    expression = HandleExpression(new[]
                    {
                        SyntaxKind.CommaToken,
                        SyntaxKind.CloseParenthesisToken
                    });
                }

                var functionParameterEntryNode = new FunctionParameterEntryNode(
                    expression,
                    hasOutKeyword,
                    hasInKeyword,
                    hasRefKeyword);

                mutableFunctionParametersListing.Add(functionParameterEntryNode);

                if (expression.IsFabricated && SyntaxKind.CommaToken != TokenWalker.Current.SyntaxKind)
                {
                    break;
                }

                if (SyntaxKind.CommaToken == TokenWalker.Current.SyntaxKind)
                {
                    // TODO: Track comma tokens?
                    //
                    // mutableFunctionParametersListing.Add(_cSharpParser._tokenWalker.Consume());
                }
                else
                {
                    break;
                }
            }

            var closeParenthesisToken = (CloseParenthesisToken)TokenWalker.Match(SyntaxKind.CloseParenthesisToken);

            return new FunctionParametersListingNode(
                openParenthesisToken,
                mutableFunctionParametersListing.ToImmutableArray(),
                closeParenthesisToken);
        }

        /// <summary>Use this method for function definition, whereas <see cref="HandleFunctionParameters"/> should be used for function invocation.</summary>
        public FunctionArgumentsListingNode HandleFunctionArguments(OpenParenthesisToken openParenthesisToken)
        {
            if (SyntaxKind.CloseParenthesisToken == TokenWalker.Peek(0).SyntaxKind)
            {
                return new FunctionArgumentsListingNode(
                    openParenthesisToken,
                    ImmutableArray<FunctionArgumentEntryNode>.Empty,
                    (CloseParenthesisToken)TokenWalker.Consume());
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
                    if (SyntaxKind.OutTokenKeyword == TokenWalker.Current.SyntaxKind)
                    {
                        _ = TokenWalker.Consume();
                        hasOutKeyword = true;
                    }
                    else if (SyntaxKind.InTokenKeyword == TokenWalker.Current.SyntaxKind)
                    {
                        _ = TokenWalker.Consume();
                        hasInKeyword = true;
                    }
                    else if (SyntaxKind.RefTokenKeyword == TokenWalker.Current.SyntaxKind)
                    {
                        _ = TokenWalker.Consume();
                        hasRefKeyword = true;
                    }
                }

                // TypeClause
                var typeClauseNode = Utility.MatchTypeClause();

                if (typeClauseNode.IsFabricated)
                    break;

                // Identifier
                var variableIdentifierToken = (IdentifierToken)TokenWalker.Match(SyntaxKind.IdentifierToken);

                if (variableIdentifierToken.IsFabricated)
                    break;

                var variableDeclarationStatementNode = new VariableDeclarationStatementNode(
                    typeClauseNode,
                    variableIdentifierToken,
                    false
                );

                Binder.BindVariableDeclarationStatementNode(variableDeclarationStatementNode);

                var functionArgumentEntryNode = new FunctionArgumentEntryNode(
                    variableDeclarationStatementNode,
                    false,
                    hasOutKeyword,
                    hasInKeyword,
                    hasRefKeyword);

                if (TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
                {
                    var equalsToken = (EqualsToken)TokenWalker.Consume();

                    var compileTimeConstantAbleSyntaxKinds = new[]
                    {
                        SyntaxKind.StringLiteralToken,
                        SyntaxKind.NumericLiteralToken,
                    };

                    var compileTimeConstantToken = TokenWalker.MatchRange(
                        compileTimeConstantAbleSyntaxKinds,
                        SyntaxKind.BadToken);

                    functionArgumentEntryNode = Binder.BindFunctionOptionalArgument(
                        functionArgumentEntryNode,
                        compileTimeConstantToken,
                        hasOutKeyword,
                        hasInKeyword,
                        hasRefKeyword);
                }

                mutableFunctionArgumentListing.Add(functionArgumentEntryNode);

                if (TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
                {
                    var commaToken = (CommaToken)TokenWalker.Consume();

                    // TODO: Track comma tokens?
                    //
                    // functionArgumentListing.Add(commaToken);
                }
                else
                {
                    break;
                }
            }

            var closeParenthesisToken = (CloseParenthesisToken)TokenWalker.Match(SyntaxKind.CloseParenthesisToken);

            var functionArgumentsListingNode = new FunctionArgumentsListingNode(
                openParenthesisToken,
                mutableFunctionArgumentListing.ToImmutableArray(),
                closeParenthesisToken);

            return functionArgumentsListingNode;
        }

        /// <summary>TODO: Correctly implement this method. For now going to skip until the attribute closing square bracket.</summary>
        public AttributeNode? HandleAttribute(OpenSquareBracketToken openSquareBracketToken)
        {
            ISyntaxToken tokenCurrent;

            while (true)
            {
                tokenCurrent = TokenWalker.Consume();

                if (tokenCurrent.SyntaxKind == SyntaxKind.EndOfFileToken ||
                    tokenCurrent.SyntaxKind == SyntaxKind.CloseSquareBracketToken)
                {
                    break;
                }
            }

            if (tokenCurrent.SyntaxKind == SyntaxKind.CloseSquareBracketToken)
            {
                return Binder.BindAttributeNode(
                    openSquareBracketToken,
                    (CloseSquareBracketToken)tokenCurrent);
            }

            return null;
        }

        public IExpressionNode HandleExpression(SyntaxKind[]? extraEndOfExpressionDeliminatingSyntaxKinds = null)
        {
            IExpressionNode? topMostExpressionNode = null;

            while (true)
            {
                var tokenCurrent = TokenWalker.Consume();

                if (tokenCurrent.SyntaxKind == SyntaxKind.EndOfFileToken ||
                    tokenCurrent.SyntaxKind == SyntaxKind.StatementDelimiterToken)
                {
                    TokenWalker.Backtrack();
                    break;
                }
                else if (extraEndOfExpressionDeliminatingSyntaxKinds is not null &&
                         extraEndOfExpressionDeliminatingSyntaxKinds.Contains(tokenCurrent.SyntaxKind))
                {
                    TokenWalker.Backtrack();
                    break;
                }
                else
                {
                    switch (tokenCurrent.SyntaxKind)
                    {
                        case SyntaxKind.NumericLiteralToken:
                            topMostExpressionNode = HandleNumericLiteralToken((NumericLiteralToken)tokenCurrent);
                            break;
                        case SyntaxKind.StringLiteralToken:
                            topMostExpressionNode = HandleStringLiteralToken((StringLiteralToken)tokenCurrent);
                            break;
                    }
                }
            }

            return topMostExpressionNode ?? new LiteralExpressionNode(
                new EndOfFileToken(
                    new TextEditorTextSpan(
                        0,
                        0,
                        (byte)GenericDecorationKind.None,
                        new ResourceUri(string.Empty),
                        string.Empty)),
                CSharpLanguageFacts.Types.Void.ToTypeClause());
        }

        /// <summary>TODO: Implement ParseIfStatementExpression() correctly. Until then, skip until the closing parenthesis of the if statement is found.</summary>
        public IExpressionNode HandleIfStatementExpression()
        {
            var unmatchedParenthesisCount = 0;

            while (true)
            {
                var tokenCurrent = TokenWalker.Consume();

                if (tokenCurrent.SyntaxKind == SyntaxKind.OpenParenthesisToken)
                    unmatchedParenthesisCount++;

                if (tokenCurrent.SyntaxKind == SyntaxKind.CloseParenthesisToken)
                    unmatchedParenthesisCount--;

                if (tokenCurrent.SyntaxKind == SyntaxKind.EndOfFileToken ||
                    unmatchedParenthesisCount == 0)
                {
                    break;
                }
            }

            // TODO: Correctly implement this method. For now just returning a nonsensical token.
            return new LiteralExpressionNode(
                new EndOfFileToken(
                    new TextEditorTextSpan(
                        0,
                        0,
                        (byte)GenericDecorationKind.None,
                        new ResourceUri(string.Empty),
                        string.Empty)),
                CSharpLanguageFacts.Types.Void.ToTypeClause());
        }

        public void HandlePropertyDefinition(TypeClauseNode typeClauseNode, IdentifierToken identifierToken)
        {
            var variableDeclarationStatementNode = new VariableDeclarationStatementNode(
                typeClauseNode,
                identifierToken,
                false);

            Binder.BindPropertyDeclarationNode(variableDeclarationStatementNode);
        }

        public void HandleAsTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleBaseTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleBoolTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleBreakTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleByteTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleCaseTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleCatchTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleCharTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleCheckedTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleConstTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleContinueTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleDecimalTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleDefaultTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleDelegateTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleDoTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleDoubleTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleElseTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleEnumTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleEventTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleExplicitTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleExternTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleFalseTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleFinallyTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleFixedTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleFloatTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleForTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleForeachTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleGotoTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleImplicitTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleInTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleIntTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleIsTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleLockTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleLongTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleNullTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleObjectTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleOperatorTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleOutTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleParamsTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleProtectedTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleReadonlyTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleRefTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleSbyteTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleShortTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleSizeofTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleStackallocTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleStringTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleStructTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleSwitchTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleThisTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleThrowTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleTrueTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleTryTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleTypeofTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleUintTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleUlongTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleUncheckedTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleUnsafeTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleUshortTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleVoidTokenKeyword(KeywordToken keywordToken)
        {
            HandleTypeIdentifierKeyword(keywordToken);
        }

        public void HandleVolatileTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleWhileTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleUnrecognizedTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleDefault(KeywordToken keywordToken)
        {
            if (Utility.IsTypeIdentifierKeywordSyntaxKind(keywordToken.SyntaxKind))
            {
                // One enters this conditional block with the 'keywordToken' having already been consumed.
                TokenWalker.Backtrack();

                var typeClauseNode = Utility.MatchTypeClause();
                NodeRecent = typeClauseNode;
            }
            else
            {
                throw new NotImplementedException($"Implement the {keywordToken.SyntaxKind} keyword.");
            }
        }

        public void HandleTypeIdentifierKeyword(KeywordToken keywordToken)
        {
            if (Utility.IsTypeIdentifierKeywordSyntaxKind(keywordToken.SyntaxKind))
            {
                // One enters this conditional block with the 'keywordToken' having already been consumed.
                TokenWalker.Backtrack();

                var typeClauseNode = Utility.MatchTypeClause();
                NodeRecent = typeClauseNode;
            }
            else
            {
                throw new NotImplementedException($"Implement the {keywordToken.SyntaxKind} keyword.");
            }
        }

        public void HandleNewTokenKeyword()
        {
            var typeClauseToken = Utility.MatchTypeClause();

            if (TokenWalker.Peek(0).SyntaxKind == SyntaxKind.MemberAccessToken)
            {
                // "explicit namespace qualification" OR "nested class"
                throw new NotImplementedException();
            }

            // TODO: Fix _cSharpParser.Binder.TryGetClassReferenceHierarchically, it broke on (2023-07-26)
            //
            // _cSharpParser.Binder.TryGetClassReferenceHierarchically(typeClauseToken, null, out boundClassReferenceNode);

            // TODO: combine the logic for 'new()' without a type identifier and 'new List<int>()' with a type identifier. To start I am going to isolate them in their own if conditional blocks.
            if (typeClauseToken.IsFabricated)
            {
                // If "new()" LACKS a type identifier then the OpenParenthesisToken must be there. This is true even still for when there is object initialization OpenBraceToken. For new() the parenthesis are required.
                // valid inputs:
                //     new()
                //     new(){}
                //     new(...)
                //     new(...){}

                var openParenthesisToken = TokenWalker.Match(SyntaxKind.OpenParenthesisToken);

                var boundFunctionParametersNode = HandleFunctionParameters((OpenParenthesisToken)openParenthesisToken);

                ObjectInitializationNode? boundObjectInitializationNode = null;

                if (TokenWalker.Peek(0).SyntaxKind == SyntaxKind.OpenBraceToken)
                {
                    var openBraceToken = (OpenBraceToken)TokenWalker.Consume();
                    boundObjectInitializationNode = HandleObjectInitialization(openBraceToken);
                }

                // TODO: Fix _cSharpParser.Binder.BindConstructorInvocationNode, it broke on (2023-07-26)
                //
                // var boundConstructorInvocationNode = _cSharpParser.Binder.BindConstructorInvocationNode(
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

                if (TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
                    HandleGenericArguments((OpenAngleBracketToken)TokenWalker.Consume());

                FunctionParametersListingNode? functionParametersListingNode = null;

                if (TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
                {
                    functionParametersListingNode = HandleFunctionParameters(
                        (OpenParenthesisToken)TokenWalker.Consume());
                }

                ObjectInitializationNode? boundObjectInitializationNode = null;

                if (TokenWalker.Peek(0).SyntaxKind == SyntaxKind.OpenBraceToken)
                {
                    var openBraceToken = (OpenBraceToken)TokenWalker.Consume();
                    boundObjectInitializationNode = HandleObjectInitialization(openBraceToken);
                }

                // TODO: Fix _cSharpParser.Binder.BindConstructorInvocationNode, it broke on (2023-07-26)
                //
                // var boundConstructorInvocationNode = _cSharpParser.Binder.BindConstructorInvocationNode(
                //     keywordToken,
                //     boundClassReferenceNode,
                //     functionParametersListingNode,
                //     boundObjectInitializationNode);
                //
                // _cSharpParser._currentCodeBlockBuilder.Children.Add(boundConstructorInvocationNode);
            }
        }

        public void HandlePublicTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleInternalTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandlePrivateTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleStaticTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleOverrideTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleVirtualTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleAbstractTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleSealedTokenKeyword(KeywordToken keywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleIfTokenKeyword(KeywordToken keywordToken)
        {
            var expression = HandleIfStatementExpression();
            var boundIfStatementNode = Binder.BindIfStatementNode(keywordToken, expression);
            NodeRecent = boundIfStatementNode;
        }

        public void HandleUsingTokenKeyword(KeywordToken keywordToken)
        {
            var namespaceIdentifier = HandleNamespaceIdentifier();

            var boundUsingStatementNode = Binder.BindUsingStatementNode(
                keywordToken,
                namespaceIdentifier);

            CurrentCodeBlockBuilder.ChildBag.Add(boundUsingStatementNode);

            NodeRecent = boundUsingStatementNode;
        }

        public void HandleInterfaceTokenKeyword()
        {
            var identifierToken = (IdentifierToken)TokenWalker.Match(SyntaxKind.IdentifierToken);

            GenericArgumentsListingNode? genericArgumentsListingNode = null;

            if (TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
            {
                var openAngleBracketToken = (OpenAngleBracketToken)TokenWalker.Consume();
                genericArgumentsListingNode = HandleGenericArguments(openAngleBracketToken);
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

            Binder.BindTypeDefinitionNode(typeDefinitionNode);
            Binder.BindTypeIdentifier(identifierToken);

            NodeRecent = typeDefinitionNode;
        }

        public void HandleClassTokenKeyword()
        {
            var identifierToken = (IdentifierToken)TokenWalker.Match(SyntaxKind.IdentifierToken);

            GenericArgumentsListingNode? genericArgumentsListingNode = null;

            if (TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
            {
                var openAngleBracketToken = (OpenAngleBracketToken)TokenWalker.Consume();
                genericArgumentsListingNode = HandleGenericArguments(openAngleBracketToken);
            }

            var typeDefinitionNode = new TypeDefinitionNode(
                identifierToken,
                null,
                genericArgumentsListingNode,
                null,
                null);

            Binder.BindTypeDefinitionNode(typeDefinitionNode);
            Binder.BindTypeIdentifier(identifierToken);

            NodeRecent = typeDefinitionNode;
        }

        public void HandleNamespaceTokenKeyword(KeywordToken keywordToken)
        {
            var namespaceIdentifier = HandleNamespaceIdentifier();

            if (_parser._finalizeNamespaceFileScopeCodeBlockNodeAction is not null)
            {
                throw new NotImplementedException(
                    "Need to add logic to report diagnostic when there is" +
                    " already a file scoped namespace.");
            }

            var boundNamespaceStatementNode = Binder.BindNamespaceStatementNode(
                    keywordToken,
                    namespaceIdentifier);

            NodeRecent = boundNamespaceStatementNode;
        }

        public void HandleReturnTokenKeyword(KeywordToken keywordToken)
        {
            var boundReturnStatementNode = Binder.BindReturnStatementNode(
                keywordToken,
                HandleExpression());

            CurrentCodeBlockBuilder.ChildBag.Add(boundReturnStatementNode);

            NodeRecent = boundReturnStatementNode;
        }

        public void HandleVarTokenContextualKeyword(KeywordContextualToken contextualKeywordToken)
        {
            // Check if previous statement is finished, and a new one is starting.
            // TODO: 'Peek(-2)' is horribly confusing. The reason for using -2 is that one consumed the 'var' keyword and moved their position forward by 1. So to read the token behind 'var' one must go back 2 tokens. It feels natural to put '-1' and then this evaluates to the wrong token. Should an expression bound property be made for 'Peek(-2)'?
            var previousToken = TokenWalker.Peek(-2);

            if (previousToken.SyntaxKind == SyntaxKind.StatementDelimiterToken ||
                previousToken.SyntaxKind == SyntaxKind.BadToken)
            {
                // Check if the next token is a second 'var keyword' or an IdentifierToken. Two IdentifierTokens is invalid, and therefore one can contextually take this 'var' as a keyword.
                bool nextTokenIsVarKeyword = SyntaxKind.VarTokenContextualKeyword == TokenWalker.Current.SyntaxKind;

                bool nextTokenIsIdentifierToken = SyntaxKind.IdentifierToken == TokenWalker.Current.SyntaxKind;

                if (nextTokenIsVarKeyword || nextTokenIsIdentifierToken)
                {
                    var varKeyword = new TypeClauseNode(
                        contextualKeywordToken,
                        null,
                        null);

                    NodeRecent = varKeyword;
                }
            }
            else
            {
                // Take 'var' as an identifier
                IdentifierToken varIdentifierToken = new(contextualKeywordToken.TextSpan);
                General.ParseIdentifierToken(varIdentifierToken);
            }
        }

        public void HandlePartialTokenContextualKeyword(KeywordContextualToken contextualKeywordToken)
        {
            // TODO: Implement this method
        }

        public void HandleAddTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleAndTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleAliasTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleAscendingTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleArgsTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleAsyncTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleAwaitTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleByTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleDescendingTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleDynamicTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleEqualsTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleFileTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleFromTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleGetTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleGlobalTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleGroupTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleInitTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleIntoTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleJoinTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleLetTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleManagedTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleNameofTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleNintTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleNotTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleNotnullTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleNuintTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleOnTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleOrTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleOrderbyTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleRecordTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleRemoveTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleRequiredTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleScopedTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleSelectTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleSetTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleUnmanagedTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleValueTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleWhenTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleWhereTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            if (NodeRecent is not null && NodeRecent.SyntaxKind == SyntaxKind.FunctionDefinitionNode)
            {
                /*
                 Examples:

                 public T Clone<T>(T item) where T : class
                 {
	                 return item;
                 }
                 
                 public T Clone<T>(T item) where T : class => item;
                */

                // TODO: Implement generic constraints, until then just read until the generic...
                // ...constraint is finished.

                while (!TokenWalker.IsEof)
                {
                    if (TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
                        TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
                    {
                        break;
                    }

                    _ = TokenWalker.Consume();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void HandleWithTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleYieldTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public void HandleUnrecognizedTokenContextualKeyword(KeywordContextualToken keywordContextualToken)
        {
            // TODO: Implement this method
        }

        public ParenthesizedExpressionNode HandleParenthesizedExpression(ParenthesizedExpressionNode parenthesizedExpressionNode)
        {
            // Presumption: One invoked this method because they found an 'OpenParenthesisToken'.
            //              They then determined given the current context that the language specific meaning
            //              is a parenthesized expression has begun.

            var innerExpression = HandleExpression(new[] { SyntaxKind.CloseParenthesisToken });
            var closeParenthesisToken = (CloseParenthesisToken)TokenWalker.Match(SyntaxKind.CloseParenthesisToken);

            var completeParenthesizedExpression = new ParenthesizedExpressionNode(
                parenthesizedExpressionNode.OpenParenthesisToken,
                innerExpression,
                closeParenthesisToken);

            return completeParenthesizedExpression;
        }

        public IExpressionNode HandleNumericLiteralToken(NumericLiteralToken numericLiteralToken)
        {
            // ParseNumericLiteralToken() is intended to be used when there is no context, just a token.
            //
            // HandleNumericLiteralToken() on the other hand in to be used when one has context, in which they are parsing an expression AND have a token.

            var literalExpressionNode = new LiteralExpressionNode(
                numericLiteralToken,
                CSharpLanguageFacts.Types.Int.ToTypeClause());

            literalExpressionNode = Binder.BindLiteralExpressionNode(literalExpressionNode);

            if (Utility.IsBinaryOperatorSyntaxKind(TokenWalker.Current.SyntaxKind))
            {
                var binaryOperatorToken = TokenWalker.Consume();

                return HandleBinaryOperatorToken(
                    literalExpressionNode,
                    binaryOperatorToken);
            }

            return literalExpressionNode;
        }

        public IExpressionNode HandleStringLiteralToken(StringLiteralToken stringLiteralToken)
        {
            var literalExpressionNode = new LiteralExpressionNode(
                stringLiteralToken,
                CSharpLanguageFacts.Types.String.ToTypeClause());

            literalExpressionNode = Binder.BindLiteralExpressionNode(literalExpressionNode);

            if (Utility.IsBinaryOperatorSyntaxKind(TokenWalker.Current.SyntaxKind))
            {
                var binaryOperatorToken = TokenWalker.Consume();

                return HandleBinaryOperatorToken(
                    literalExpressionNode,
                    binaryOperatorToken);
            }

            return literalExpressionNode;
        }

        public IExpressionNode HandleBinaryOperatorToken(
            IExpressionNode leftExpressionNode,
            ISyntaxToken binaryOperatorToken)
        {
            var rightExpressionNode = HandleExpression(new[] { SyntaxKind.CloseParenthesisToken });

            var binaryOperatorNode = Binder.BindBinaryOperatorNode(
                leftExpressionNode,
                binaryOperatorToken,
                rightExpressionNode);

            return new BinaryExpressionNode(
                leftExpressionNode,
                binaryOperatorNode,
                rightExpressionNode);
        }
    }
}