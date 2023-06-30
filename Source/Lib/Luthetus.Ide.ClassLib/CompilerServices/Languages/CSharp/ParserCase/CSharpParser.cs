using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Expression;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxNodes.Expression;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxNodes.Statement;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.BinderCase;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Lexing;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;

public class CSharpParser
{
    private readonly TokenWalker _tokenWalker;
    private readonly CompilationUnitBuilder _globalCompilationUnitBuilder;
    private readonly LuthetusIdeDiagnosticBag _diagnosticBag = new();
    private readonly ImmutableArray<TextEditorDiagnostic> _lexerDiagnostics;

    private CSharpBinder _binder;

    public CSharpParser(
        ImmutableArray<ISyntaxToken> tokens,
        ImmutableArray<TextEditorDiagnostic> lexerDiagnostics)
    {
        _lexerDiagnostics = lexerDiagnostics;
        _tokenWalker = new TokenWalker(tokens, _diagnosticBag);
        _binder = new CSharpBinder();

        _globalCompilationUnitBuilder = new(null);
        _currentCompilationUnitBuilder = _globalCompilationUnitBuilder;
    }

    /// <summary>If a file scoped namespace is found, then set this field, so that prior to finishing the parser constructs the namespace node.</summary>
    private Action<CompilationUnit>? _finalizeFileScopeAction;

    public ImmutableArray<TextEditorDiagnostic> Diagnostics => _diagnosticBag.ToImmutableArray();
    public CSharpBinder Binder => _binder;

    private ISyntaxNode? _nodeRecent;
    private CompilationUnitBuilder _currentCompilationUnitBuilder;

    /// <summary>When parsing the body of a function this is used in order to keep the function definition node itself in the syntax tree immutable.<br/><br/>That is to say, this action would create the function definition node and then append it.</summary>
    private Stack<Action<CompilationUnit>> _finalizeCompilationUnitActionStack = new();

    /// <summary>This method is used when parsing many files as a single compilation. The first binder instance would be passed to the following parsers. The resourceUri is passed in so if a file is parsed for a second time, the previous symbols can be deleted so they do not duplicate.</summary>
    public CompilationUnit Parse(
        CSharpBinder previousBinder,
        ResourceUri resourceUri)
    {
        _binder = previousBinder;
        _binder.CurrentResourceUri = resourceUri;
        _binder.ClearStateByResourceUri(resourceUri);

        return Parse();
    }

    public CompilationUnit Parse()
    {
        while (true)
        {
            var consumedToken = _tokenWalker.Consume();

            switch (consumedToken.SyntaxKind)
            {
                case SyntaxKind.NumericLiteralToken:
                    ParseNumericLiteralToken((NumericLiteralToken)consumedToken);
                    break;
                case SyntaxKind.StringLiteralToken:
                    ParseStringLiteralToken((StringLiteralToken)consumedToken);
                    break;
                case SyntaxKind.PlusToken:
                    ParsePlusToken((PlusToken)consumedToken);
                    break;
                case SyntaxKind.PreprocessorDirectiveToken:
                    ParsePreprocessorDirectiveToken((PreprocessorDirectiveToken)consumedToken);
                    break;
                case SyntaxKind.CommentSingleLineToken:
                    // Do not parse comments.
                    break;
                case SyntaxKind.KeywordToken:
                    ParseKeywordToken((KeywordToken)consumedToken);
                    break;
                case SyntaxKind.KeywordContextualToken:
                    ParseKeywordContextualToken((KeywordContextualToken)consumedToken);
                    break;
                case SyntaxKind.IdentifierToken:
                    ParseIdentifierToken((IdentifierToken)consumedToken);
                    break;
                case SyntaxKind.OpenBraceToken:
                    ParseOpenBraceToken((OpenBraceToken)consumedToken);
                    break;
                case SyntaxKind.CloseBraceToken:
                    ParseCloseBraceToken((CloseBraceToken)consumedToken);
                    break;
                case SyntaxKind.OpenAngleBracketToken:
                    ParseOpenAngleBracketToken((OpenAngleBracketToken)consumedToken);
                    break;
                case SyntaxKind.CloseAngleBracketToken:
                    ParseCloseAngleBracketToken((CloseAngleBracketToken)consumedToken);
                    break;
                case SyntaxKind.OpenSquareBracketToken:
                    ParseOpenSquareBracketToken((OpenSquareBracketToken)consumedToken);
                    break;
                case SyntaxKind.CloseSquareBracketToken:
                    ParseCloseSquareBracketToken((CloseSquareBracketToken)consumedToken);
                    break;
                case SyntaxKind.DollarSignToken:
                    ParseDollarSignToken((DollarSignToken)consumedToken);
                    break;
                case SyntaxKind.ColonToken:
                    ParseColonToken((ColonToken)consumedToken);
                    break;
                case SyntaxKind.StatementDelimiterToken:
                    ParseStatementDelimiterToken((StatementDelimiterToken)consumedToken);
                    break;
                case SyntaxKind.EndOfFileToken:
                    if (_nodeRecent is IExpressionNode)
                    {
                        _currentCompilationUnitBuilder.IsExpression = true;
                        _currentCompilationUnitBuilder.Children.Add(_nodeRecent);
                    }
                    break;
            }

            if (consumedToken.SyntaxKind == SyntaxKind.EndOfFileToken)
                break;
        }

        if (_finalizeFileScopeAction is not null &&
            _currentCompilationUnitBuilder.Parent is not null)
        {
            // The current token here would be the EOF token.
            _binder.DisposeBoundScope(_tokenWalker.Current.TextSpan);

            _finalizeFileScopeAction.Invoke(
                _currentCompilationUnitBuilder.Build());

            _currentCompilationUnitBuilder = _currentCompilationUnitBuilder.Parent;
        }

        return _currentCompilationUnitBuilder.Build(
            Diagnostics
                .Union(_binder.Diagnostics)
                .Union(_lexerDiagnostics)
                .ToImmutableArray());
    }

    private BoundLiteralExpressionNode ParseNumericLiteralToken(
        NumericLiteralToken numericLiteralToken)
    {
        var literalExpressionNode = new LiteralExpressionNode(numericLiteralToken);

        var boundLiteralExpressionNode = _binder
            .BindLiteralExpressionNode(literalExpressionNode);

        _nodeRecent = boundLiteralExpressionNode;

        return boundLiteralExpressionNode;
    }

    private BoundLiteralExpressionNode ParseStringLiteralToken(
        StringLiteralToken stringLiteralToken)
    {
        var literalExpressionNode = new LiteralExpressionNode(stringLiteralToken);

        var boundLiteralExpressionNode = _binder
                .BindLiteralExpressionNode(literalExpressionNode);

        _nodeRecent = boundLiteralExpressionNode;

        return boundLiteralExpressionNode;
    }

    private void ParsePlusToken(
        PlusToken plusToken)
    {
        var localNodeRecent = _nodeRecent;

        if (localNodeRecent is not BoundLiteralExpressionNode leftBoundLiteralExpressionNode)
            throw new NotImplementedException();

        var validMatches = new SyntaxKind[] 
        {
            SyntaxKind.NumericLiteralToken,
            SyntaxKind.StringLiteralToken,
        };

        var matchedToken = _tokenWalker.MatchRange(
            validMatches,
            SyntaxKind.BadToken);

        BoundLiteralExpressionNode rightBoundLiteralExpressionNode;

        if (matchedToken.SyntaxKind == SyntaxKind.NumericLiteralToken)
        {
            rightBoundLiteralExpressionNode = ParseNumericLiteralToken(
                (NumericLiteralToken)matchedToken);
        }
        else if (matchedToken.SyntaxKind == SyntaxKind.StringLiteralToken)
        {
            rightBoundLiteralExpressionNode = ParseStringLiteralToken(
                (StringLiteralToken)matchedToken);
        }
        else
        {
            // TODO: I'm not sure what to do when the matchedToken is unexpected. For now I'll "simplify" the binary expression to be the left operand alone.
            _nodeRecent = leftBoundLiteralExpressionNode;
            return;
        }

        var boundBinaryOperatorNode = _binder.BindBinaryOperatorNode(
            leftBoundLiteralExpressionNode,
            plusToken,
            rightBoundLiteralExpressionNode);

        var boundBinaryExpressionNode = new BoundBinaryExpressionNode(
            leftBoundLiteralExpressionNode,
            boundBinaryOperatorNode,
            rightBoundLiteralExpressionNode);

        _nodeRecent = boundBinaryExpressionNode;
    }

    private IStatementNode ParsePreprocessorDirectiveToken(
        PreprocessorDirectiveToken preprocessorDirectiveToken)
    {
        var consumedToken = _tokenWalker.Consume();

        if (consumedToken.SyntaxKind == SyntaxKind.LibraryReferenceToken)
        {
            var preprocessorLibraryReferenceStatement = new PreprocessorLibraryReferenceStatement(
                preprocessorDirectiveToken,
                consumedToken);

            _currentCompilationUnitBuilder.Children.Add(preprocessorLibraryReferenceStatement);

            return preprocessorLibraryReferenceStatement;
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private void ParseKeywordToken(
        KeywordToken keywordToken)
    {
        // TODO: Make many keywords SyntaxKinds. Then if SyntaxKind.EndsWith("Keyword"); so that string checking doesn't need to be done.
        var text = keywordToken.TextSpan.GetText();

        if (_binder.TryGetClassReferenceHierarchically(
                keywordToken, 
                null, 
                out var boundClassReferenceNode,
                shouldReportUndefinedTypeOrNamespace: false,
                shouldCreateClassDefinitionIfUndefined: false))
        {
            // 'int', 'string', 'bool', etc...
            if (boundClassReferenceNode is not null)
                _nodeRecent = boundClassReferenceNode;
        }
        else
        {
            // 'return', 'if', 'get', etc...

            if (text == "return")
            {
                var boundReturnStatementNode = _binder.BindReturnStatementNode(
                    keywordToken,
                    ParseExpression());

                _currentCompilationUnitBuilder.Children.Add(boundReturnStatementNode);

                _nodeRecent = boundReturnStatementNode;
            }
            else if (text == "namespace")
            {
                var namespaceIdentifier = ParseNamespaceIdentifier();

                if (_finalizeFileScopeAction is not null)
                {
                    throw new NotImplementedException(
                        "Need to add logic to report diagnostic when there is" +
                        " already a file scoped namespace.");
                }

                var boundNamespaceStatementNode = _binder.BindNamespaceStatementNode(
                        keywordToken,
                        namespaceIdentifier);

                _nodeRecent = boundNamespaceStatementNode;
            }
            else if (text == "class")
            {
                var identifierToken = (IdentifierToken)_tokenWalker.Match(SyntaxKind.IdentifierToken);

                _ = _binder.TryBindClassDefinitionNode(
                    identifierToken,
                    null,
                    out var boundClassDefinitionNode);
                
                _nodeRecent = boundClassDefinitionNode;
            }
            else if (text == "using")
            {
                var namespaceIdentifier = ParseNamespaceIdentifier();

                var boundUsingStatementNode = _binder.BindUsingStatementNode(
                        keywordToken,
                        namespaceIdentifier);

                _currentCompilationUnitBuilder.Children.Add(boundUsingStatementNode);

                _nodeRecent = boundUsingStatementNode;
            }
            else if (text == "public" ||
                     text == "internal" ||
                     text == "private")
            {
                // TODO: Implement keywords for visibility
            }
            else if (text == "static")
            {
                // TODO: Implement keywords for object lifetime
            }
            else if (text == "override" ||
                     text == "virtual" ||
                     text == "abstract" ||
                     text == "sealed")
            {
                // TODO: Implement keywords for inheritance
            }
            else if (text == "partial")
            {
                // TODO: Implement the 'partial' keyword
            }
            else if (text == "await")
            {
                // TODO: Implement the 'await' keyword
            }
            else if (text == "if")
            {
                var expression = ParseIfStatementExpression();
                var boundIfStatementNode = _binder.BindIfStatementNode(keywordToken, expression);
                _nodeRecent = boundIfStatementNode;
            }
            else if (text == "get")
            {
                // TODO: Implement the 'get' keyword
            }
            else if (text == "set")
            {
                // TODO: Implement the 'set' keyword
            }
            else if (text == "new")
            {
                var typeClauseToken = MatchTypeClauseToken();

                if (_tokenWalker.Peek(0).SyntaxKind == SyntaxKind.MemberAccessToken)
                {
                    // "explicit namespace qualification" OR "nested class"
                    throw new NotImplementedException();
                }

                _binder.TryGetClassReferenceHierarchically(typeClauseToken, null, out boundClassReferenceNode);

                // TODO: combine the logic for 'new()' without a type identifier and 'new List<int>()' with a type identifier. To start I am going to isolate them in their own if conditional blocks.
                if (typeClauseToken.IsFabricated)
                {
                    // If "new()" LACKS a type identifier then the OpenParenthesisToken must be there. This is true even still for when there is object initialization OpenBraceToken. For new() the parenthesis are required.
                    // valid inputs:
                    //     new()
                    //     new(){}
                    //     new(...)
                    //     new(...){}

                    var openParenthesisToken = _tokenWalker.Match(SyntaxKind.OpenParenthesisToken);

                    var boundFunctionArgumentsNode = ParseFunctionParameters((OpenParenthesisToken)openParenthesisToken);

                    BoundObjectInitializationNode? boundObjectInitializationNode = null;

                    if (_tokenWalker.Peek(0).SyntaxKind == SyntaxKind.OpenBraceToken)
                    {
                        var openBraceToken = (OpenBraceToken)_tokenWalker.Consume();
                        boundObjectInitializationNode = ParseObjectInitialization(openBraceToken);
                    }

                    var boundConstructorInvocationNode = _binder.BindConstructorInvocationNode(
                        keywordToken,
                        boundClassReferenceNode,
                        boundFunctionArgumentsNode,
                        boundObjectInitializationNode);

                    _currentCompilationUnitBuilder.Children.Add(boundConstructorInvocationNode);
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

                    if (_tokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
                        ParseGenericArguments((OpenAngleBracketToken)_tokenWalker.Consume());

                    BoundFunctionParametersNode? boundFunctionParametersNode = null;

                    if (_tokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
                    {
                        boundFunctionParametersNode = ParseFunctionParameters(
                            (OpenParenthesisToken)_tokenWalker.Consume());
                    }

                    BoundObjectInitializationNode? boundObjectInitializationNode = null;

                    if (_tokenWalker.Peek(0).SyntaxKind == SyntaxKind.OpenBraceToken)
                    {
                        var openBraceToken = (OpenBraceToken)_tokenWalker.Consume();
                        boundObjectInitializationNode = ParseObjectInitialization(openBraceToken);
                    }

                    var boundConstructorInvocationNode = _binder.BindConstructorInvocationNode(
                        keywordToken,
                        boundClassReferenceNode,
                        boundFunctionParametersNode,
                        boundObjectInitializationNode);

                    _currentCompilationUnitBuilder.Children.Add(boundConstructorInvocationNode);
                }
            }
            else if (text == "interface")
            {
                // TODO: Implement the 'interface' keyword
                var identifierToken = (IdentifierToken)_tokenWalker.Match(SyntaxKind.IdentifierToken);

                _ = _binder.TryBindClassDefinitionNode(
                    identifierToken,
                    null,
                    out var boundClassDefinitionNode);

                _nodeRecent = boundClassDefinitionNode;
            }
            else
            {
                throw new NotImplementedException("Implement more keywords");
            }
        }
    }

    /// <summary>TODO: Correctly parse object initialization. For now, just skip over it when parsing.</summary>
    private BoundObjectInitializationNode ParseObjectInitialization(
        OpenBraceToken openBraceToken) 
    {
        ISyntaxToken shouldBeCloseBraceToken = new BadToken(openBraceToken.TextSpan);

        while (!_tokenWalker.IsEof)
        {
            shouldBeCloseBraceToken = _tokenWalker.Consume();

            if (shouldBeCloseBraceToken.SyntaxKind == SyntaxKind.EndOfFileToken ||
                shouldBeCloseBraceToken.SyntaxKind == SyntaxKind.CloseBraceToken)
            {
                break;
            }
        }

        if (shouldBeCloseBraceToken.SyntaxKind != SyntaxKind.CloseBraceToken)
            shouldBeCloseBraceToken = _tokenWalker.Match(SyntaxKind.CloseBraceToken);

        return new BoundObjectInitializationNode(
            openBraceToken,
            (CloseBraceToken)shouldBeCloseBraceToken);
    }

    private IdentifierToken ParseNamespaceIdentifier()
    {
        var combineNamespaceIdentifierIntoOne = new List<ISyntaxToken>();

        while (!_tokenWalker.IsEof)
        {
            if (combineNamespaceIdentifierIntoOne.Count % 2 == 0)
            {
                var matchedToken = _tokenWalker.Match(SyntaxKind.IdentifierToken);
                combineNamespaceIdentifierIntoOne.Add(matchedToken);

                if (matchedToken.IsFabricated)
                    break;
            }
            else
            {
                if (_tokenWalker.Current.SyntaxKind == SyntaxKind.MemberAccessToken)
                    combineNamespaceIdentifierIntoOne.Add(_tokenWalker.Consume());
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

    private void ParseKeywordContextualToken(
        KeywordContextualToken keywordContextualToken)
    {
        // TODO: Make many keywords SyntaxKinds. Then if SyntaxKind.EndsWith("Keyword"); so that string checking doesn't need to be done.
        var text = keywordContextualToken.TextSpan.GetText();
        
        // 'return', 'if', 'get', etc...

        if (text == "var")
        {
            // Check if previous statement is finished, and a new one is starting.
            // TODO: 'Peek(-2)' is horribly confusing. The reason for using -2 is that one consumed the 'var' keyword and moved their position forward by 1. So to read the token behind 'var' one must go back 2 tokens. It feels natural to put '-1' and then this evaluates to the wrong token. Should an expression bound property be made for 'Peek(-2)'?
            var previousToken = _tokenWalker.Peek(-2);

            if (previousToken.SyntaxKind == SyntaxKind.StatementDelimiterToken ||
                previousToken.SyntaxKind == SyntaxKind.BadToken)
            {
                // Check if the next token is a second 'var keyword' or an IdentifierToken. Two IdentifierTokens is invalid, and therefore one can contextually take this 'var' as a keyword.
                bool nextTokenIsVarKeyword =
                    _tokenWalker.Current.SyntaxKind == SyntaxKind.KeywordContextualToken &&
                    _tokenWalker.Current.TextSpan.GetText() == "var";

                bool nextTokenIsIdentifierToken =
                    _tokenWalker.Current.SyntaxKind == SyntaxKind.IdentifierToken;

                if (nextTokenIsVarKeyword || nextTokenIsIdentifierToken)
                {
                    // Take 'var' as a keyword
                    if (_binder.TryGetClassReferenceHierarchically(keywordContextualToken, null, out var boundClassDefinitionNode))
                    {
                        // 'var' type
                        _nodeRecent = boundClassDefinitionNode;
                    }

                    return;
                }
            }

            // Take 'var' as an identifier
            IdentifierToken varIdentifierToken = new(keywordContextualToken.TextSpan);
            ParseIdentifierToken(varIdentifierToken);
        }
    }

    private void ParseDollarSignToken(
        DollarSignToken dollarSignToken)
    {
        if (_tokenWalker.Current.SyntaxKind == SyntaxKind.StringLiteralToken)
        {
            var stringLiteralToken = _tokenWalker.Consume();

            _nodeRecent = ParseStringLiteralToken(
                (StringLiteralToken)stringLiteralToken);

            _binder.BindStringInterpolationExpression(dollarSignToken);
        }
    }

    private void ParseColonToken(
        ColonToken colonToken)
    {
        if (_nodeRecent is not null &&
            _nodeRecent.SyntaxKind == SyntaxKind.BoundClassDefinitionNode)
        {
            var boundClassDefinitionNode = (BoundClassDefinitionNode)_nodeRecent;

            var matchTypeClauseToken = MatchTypeClauseToken();

            var success = _binder.TryGetClassReferenceHierarchically(matchTypeClauseToken, null, out var parentClassReference);

            // TODO: If not successful at getting class reference one should be fabricated instead of returning null.
            if (success && parentClassReference is not null)
            {
                var boundInheritanceStatementNode = _binder.BindInheritanceStatementNode(
                    parentClassReference);

                boundClassDefinitionNode = boundClassDefinitionNode with
                {
                    BoundInheritanceStatementNode = boundInheritanceStatementNode
                };

                _nodeRecent = boundClassDefinitionNode;
            }
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private void ParseIdentifierToken(
        IdentifierToken identifierToken)
    {
        if (_nodeRecent is not null &&
            _nodeRecent.SyntaxKind == SyntaxKind.BoundClassReferenceNode)
        {
            // 'function definition' OR 'variable declaration' OR 'variable initialization'

            BoundGenericArgumentsNode? genericArguments = null;

            if (_tokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
                genericArguments = ParseGenericArguments((OpenAngleBracketToken)_tokenWalker.Consume());
            
            if (_tokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
            {
                // 'function definition'

                var boundFunctionArguments = ParseFunctionArguments((OpenParenthesisToken)_tokenWalker.Consume());

                var boundFunctionDefinitionNode = _binder.BindFunctionDefinitionNode(
                    (BoundClassReferenceNode)_nodeRecent,
                    identifierToken,
                    boundFunctionArguments,
                    genericArguments);

                _nodeRecent = boundFunctionDefinitionNode;
            }
            else if (_tokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken ||
                     _tokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
            {
                // 'variable declaration' OR 'variable initialization' OR 'property which is expression bound'

                if (_tokenWalker.Next.SyntaxKind == SyntaxKind.CloseAngleBracketToken)
                {
                    // Move current token to be the Identifier
                    _tokenWalker.Backtrack();

                    ParsePropertyDefinition();
                }
                else
                {
                    // 'variable declaration'
                    var boundVariableDeclarationStatementNode = _binder.BindVariableDeclarationNode(
                        (BoundClassReferenceNode)_nodeRecent,
                        identifierToken);

                    _currentCompilationUnitBuilder.Children.Add(boundVariableDeclarationStatementNode);

                    if (_tokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
                    {
                        // 'variable initialization'

                        // Move past the EqualsToken
                        _ = _tokenWalker.Consume();

                        var rightHandExpression = ParseExpression();

                        var boundVariableAssignmentNode = _binder.BindVariableAssignmentNode(
                            (IdentifierToken)boundVariableDeclarationStatementNode.IdentifierToken,
                            rightHandExpression);

                        if (boundVariableAssignmentNode is null)
                        {
                            // TODO: Why would boundVariableDeclarationStatementNode ever be null here? The variable had just been defined. I suppose what I mean to say is, should this get the '!' operator? The compiler is correctly complaining and the return type should have nullability in the case of undefined variables. So, use the not null operator?
                            throw new NotImplementedException();
                        }
                        else
                        {
                            _currentCompilationUnitBuilder.Children
                                .Add(boundVariableAssignmentNode);
                        }
                    }

                    // This conditional branch is for variable declaration, so at this point a StatementDelimiterToken is always expected.
                    _ = _tokenWalker.Match(SyntaxKind.StatementDelimiterToken);

                    _nodeRecent = null;
                }
            }
            else if (_tokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
            {
                // 'property'

                // Backtrack to the Property Identifier
                _ = _tokenWalker.Backtrack();

                ParsePropertyDefinition();
            }
        }
        else
        {
            // 'function invocation' OR 'variable assignment' OR 'variable reference' OR 'namespace declaration' OR  'namespace identifier' OR 'static class identifier' OR 'generic class definition'

            if (_tokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken ||
                _tokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
            {
                if (_nodeRecent is not null &&
                    _nodeRecent.SyntaxKind == SyntaxKind.BoundIdentifierReferenceNode)
                {
                    var boundIdentifierReferenceNode = (BoundIdentifierReferenceNode)_nodeRecent;

                    // The contextual identifier can now be understood to be the return Type of a function.
                    _ = _binder.TryGetClassReferenceHierarchically(boundIdentifierReferenceNode.IdentifierToken, null, out var boundClassDefinitionNode);
                    _nodeRecent = boundClassDefinitionNode;

                    // Re-invoke ParseIdentifierToken now that _nodeRecent is known to be a Type identifier
                    {
                        ParseIdentifierToken(identifierToken);
                        return;
                    }
                }

                // 'function invocation' 

                // TODO: (2023-06-04) I believe this if block will run for '<' mathematical operator.

                BoundGenericArgumentsNode? genericArguments = null;

                if (_tokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
                    genericArguments = ParseGenericArguments((OpenAngleBracketToken)_tokenWalker.Consume());

                var openParenthesisToken = (OpenParenthesisToken)_tokenWalker.Match(SyntaxKind.OpenParenthesisToken);

                var functionParameters = ParseFunctionParameters(openParenthesisToken);

                var boundFunctionInvocationNode = _binder.BindFunctionInvocationNode(
                    identifierToken,
                    functionParameters,
                    genericArguments);

                if (boundFunctionInvocationNode is null)
                    throw new ApplicationException($"{nameof(boundFunctionInvocationNode)} was null.");

                _currentCompilationUnitBuilder.Children.Add(boundFunctionInvocationNode);
            }
            else if (_tokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
            {
                // 'variable assignment'

                var rightHandExpression = ParseExpression();

                var boundVariableAssignmentNode = _binder.BindVariableAssignmentNode(
                    identifierToken,
                    rightHandExpression);

                _currentCompilationUnitBuilder.Children
                    .Add(boundVariableAssignmentNode);
            }
            else
            {
                // 'variable reference' OR 'namespace identifier' OR 'static class identifier'

                if (_binder.BoundNamespaceStatementNodes.ContainsKey(identifierToken.TextSpan.GetText()))
                {
                    if (_tokenWalker.Current.SyntaxKind == SyntaxKind.MemberAccessToken)
                    {
                        // TODO: (2023-05-28) Implement explicit namespace qualification checking. If they try to member access 'Console' on the namespace 'System' one should ensure 'Console' is really in the namespace. But, for now just return.
                        _tokenWalker.Consume();
                        return;
                    }
                    else
                    {
                        // TODO: (2023-05-28) Report an error diagnostic for 'namespaces are not statements'. Something like this I'm not sure.
                        _tokenWalker.Consume();
                        return;
                    }
                }
                else
                {
                    // TODO: (2023-05-28) Report an error diagnostic for 'unknown identifier'. Something like this I'm not sure.

                    var boundIdentifierReferenceNode = _binder.BindIdentifierReferenceNode(identifierToken);
                    _nodeRecent = boundIdentifierReferenceNode;

                    return;
                }
            }
        }
    }

    /// <summary>Assumes invocation occurs with the property identifier as _tokenWalker's current token</summary>
    private void ParsePropertyDefinition()
    {
        var propertyTypeClauseToken = _tokenWalker.Peek(-1);
        var propertyIdentifierToken = _tokenWalker.Consume();

        _ = _binder.TryGetClassReferenceHierarchically(propertyTypeClauseToken, null, out var boundClassReferenceNode);

        _binder.BindPropertyDeclarationNode(
            boundClassReferenceNode,
            (IdentifierToken)propertyIdentifierToken);
    }

    /// <summary>TODO: Implement ParseIfStatementExpression() correctly. Until then, skip until the closing parenthesis of the if statement is found.</summary>
    private IBoundExpressionNode ParseIfStatementExpression()
    {
        var unmatchedParenthesisCount = 0;

        while (true)
        {
            var tokenCurrent = _tokenWalker.Consume();

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
        return new BoundLiteralExpressionNode(
            new EndOfFileToken(
                new TextEditorTextSpan(
                    0,
                    0,
                    (byte)GenericDecorationKind.None,
                    new ResourceUri(string.Empty),
                    string.Empty)),
            typeof(void));
    }

    /// <summary>TODO: Implement ParseExpression() correctly. Until then, skip until the statement delimiter token or end of file token is found.</summary>
    private IBoundExpressionNode ParseExpression()
    {
        while (true)
        {
            var tokenCurrent = _tokenWalker.Consume();

            if (tokenCurrent.SyntaxKind == SyntaxKind.EndOfFileToken ||
                tokenCurrent.SyntaxKind == SyntaxKind.StatementDelimiterToken)
            {
                break;
            }
        }

        // #TODO: Correctly implement this method Returning a nonsensical token for now.
        return new BoundLiteralExpressionNode(
            new EndOfFileToken(
                new TextEditorTextSpan(
                    0,
                    0,
                    (byte)GenericDecorationKind.None,
                    new ResourceUri(string.Empty),
                    string.Empty)),
            typeof(void));
    }

    /// <summary>TODO: Implement ParseFunctionParameterExpression() correctly. Until then, skip until the comma token or end of file token is found.</summary>
    private IBoundExpressionNode ParseFunctionParameterExpression()
    {
        while (true)
        {
            if (_tokenWalker.Current.SyntaxKind == SyntaxKind.EndOfFileToken ||
                _tokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken ||
                _tokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
            {
                break;
            }

            var currentToken = _tokenWalker.Consume();

            switch (currentToken.SyntaxKind)
            {
                case SyntaxKind.NumericLiteralToken:
                {
                    var boundLiteralExpressionNode = ParseNumericLiteralToken((NumericLiteralToken)currentToken);
                    break;
                }
                   
                case SyntaxKind.StringLiteralToken:
                {
                    var boundLiteralExpressionNode = ParseStringLiteralToken((StringLiteralToken)currentToken);
                    break;
                }
            }
        }

        // #TODO: Correctly implement this method Returning a nonsensical token for now.
        return new BoundLiteralExpressionNode(
            new EndOfFileToken(
                new TextEditorTextSpan(
                    0,
                    0,
                    (byte)GenericDecorationKind.None,
                    new ResourceUri(string.Empty),
                    string.Empty)),
            typeof(void));
    }

    private void ParseOpenBraceToken(
        OpenBraceToken openBraceToken)
    {
        var closureCurrentCompilationUnitBuilder = _currentCompilationUnitBuilder;
        Type? scopeReturnType = null;

        if (_nodeRecent is not null &&
            _nodeRecent.SyntaxKind == SyntaxKind.BoundNamespaceStatementNode)
        {
            var boundNamespaceStatementNode = (BoundNamespaceStatementNode)_nodeRecent;

            _finalizeCompilationUnitActionStack.Push(compilationUnit =>
            {
                boundNamespaceStatementNode = _binder.RegisterBoundNamespaceEntryNode(
                    boundNamespaceStatementNode,
                    compilationUnit);

                closureCurrentCompilationUnitBuilder.Children
                    .Add(boundNamespaceStatementNode);
            });
        }
        else if (_nodeRecent is not null &&
                 _nodeRecent.SyntaxKind == SyntaxKind.BoundClassDefinitionNode)
        {
            var boundClassDefinitionNode = (BoundClassDefinitionNode)_nodeRecent;

            _finalizeCompilationUnitActionStack.Push(compilationUnit =>
            {
                boundClassDefinitionNode = boundClassDefinitionNode with
                {
                    ClassBodyCompilationUnit = compilationUnit
                };

                closureCurrentCompilationUnitBuilder.Children
                    .Add(boundClassDefinitionNode);
            });
        }
        else if (_nodeRecent is not null &&
                 _nodeRecent.SyntaxKind == SyntaxKind.BoundFunctionDefinitionNode)
        {
            var boundFunctionDefinitionNode = (BoundFunctionDefinitionNode)_nodeRecent;
            
            scopeReturnType = boundFunctionDefinitionNode.ReturnBoundClassReferenceNode.Type;

            _finalizeCompilationUnitActionStack.Push(compilationUnit =>
            {
                boundFunctionDefinitionNode = boundFunctionDefinitionNode with
                {
                    FunctionBodyCompilationUnit = compilationUnit
                };

                closureCurrentCompilationUnitBuilder.Children
                    .Add(boundFunctionDefinitionNode);
            });
        }
        else if (_nodeRecent is not null &&
                 _nodeRecent.SyntaxKind == SyntaxKind.BoundIfStatementNode)
        {
            var boundIfStatementNode = (BoundIfStatementNode)_nodeRecent;

            _finalizeCompilationUnitActionStack.Push(compilationUnit =>
            {
                boundIfStatementNode = boundIfStatementNode with
                {
                    IfStatementBodyCompilationUnit = compilationUnit
                };

                closureCurrentCompilationUnitBuilder.Children
                    .Add(boundIfStatementNode);
            });
        }
        else
        {
            _finalizeCompilationUnitActionStack.Push(compilationUnit =>
            {
                closureCurrentCompilationUnitBuilder.Children
                    .Add(compilationUnit);
            });
        }

        _binder.RegisterBoundScope(
            scopeReturnType,
            openBraceToken.TextSpan);

        if (_nodeRecent is not null &&
            _nodeRecent.SyntaxKind == SyntaxKind.BoundNamespaceStatementNode)
        {
            _binder.AddNamespaceToCurrentScope((BoundNamespaceStatementNode)_nodeRecent);
        }

        _currentCompilationUnitBuilder = new(_currentCompilationUnitBuilder);
    }

    private void ParseCloseBraceToken(
        CloseBraceToken closeBraceToken)
    {
        _binder.DisposeBoundScope(closeBraceToken.TextSpan);

        if (_currentCompilationUnitBuilder.Parent is not null &&
            _finalizeCompilationUnitActionStack.Any())
        {
            _finalizeCompilationUnitActionStack.Pop().Invoke(
                _currentCompilationUnitBuilder.Build());

            _currentCompilationUnitBuilder = _currentCompilationUnitBuilder.Parent;
        }
    }
    
    private void ParseOpenAngleBracketToken(
        OpenAngleBracketToken openAngleBracketToken)
    {
        if (_nodeRecent is not null)
        {
            if (_nodeRecent.SyntaxKind == SyntaxKind.LiteralExpressionNode ||
                _nodeRecent.SyntaxKind == SyntaxKind.BoundLiteralExpressionNode ||
                _nodeRecent.SyntaxKind == SyntaxKind.BoundBinaryExpressionNode ||
                /* Prefer the enum comparison. Will short circuit. This "is" cast is for fallback in case someone in the future adds for expression syntax kinds but does not update this if statement TODO: Check if node ends with "ExpressionNode"? */
                _nodeRecent is IBoundExpressionNode)
            {
                // Mathematical angle bracket
                throw new NotImplementedException();
            }
            else
            {
                // Generic Arguments
                var boundGenericArguments = ParseGenericArguments(openAngleBracketToken);

                if (_nodeRecent.SyntaxKind == SyntaxKind.BoundClassDefinitionNode)
                {
                    var boundClassDefinitionNode = (BoundClassDefinitionNode)_nodeRecent;

                    _nodeRecent = boundClassDefinitionNode with
                    {
                        BoundGenericArgumentsNode = boundGenericArguments
                    };
                }
            }
        }
    }
    
    private void ParseCloseAngleBracketToken(
        CloseAngleBracketToken closeAngleBracketToken)
    {
        // if one: throw new NotImplementedException();
        // then: lambdas will no longer work. So I'm keeping this method empty.
    }

    private void ParseOpenSquareBracketToken(
        OpenSquareBracketToken openSquareBracketToken)
    {
        if (_nodeRecent is not null)
        { 
            if (_nodeRecent.SyntaxKind == SyntaxKind.LiteralExpressionNode ||
                _nodeRecent.SyntaxKind == SyntaxKind.BoundLiteralExpressionNode ||
                _nodeRecent.SyntaxKind == SyntaxKind.BoundBinaryExpressionNode ||
                /* Prefer the enum comparison. Will short circuit. This "is" cast is for fallback in case someone in the future adds for expression syntax kinds but does not update this if statement TODO: Check if node ends with "ExpressionNode"? */
                _nodeRecent is IBoundExpressionNode)
            {
                // Mathematical square bracket
                throw new NotImplementedException();
            }
            else
            {
                // Attribute
                _ = ParseAttribute(openSquareBracketToken);
            }
        }
    }

    private void ParseCloseSquareBracketToken(
        CloseSquareBracketToken closeSquareBracketToken)
    {
        var z = 2;
    }
    
    /// <summary>TODO: Correctly implement this method. For now going to skip until the attribute closing square bracket.</summary>
    private BoundAttributeNode? ParseAttribute(
        OpenSquareBracketToken openSquareBracketToken)
    {
        ISyntaxToken tokenCurrent;

        while (true)
        {
            tokenCurrent = _tokenWalker.Consume();

            if (tokenCurrent.SyntaxKind == SyntaxKind.EndOfFileToken ||
                tokenCurrent.SyntaxKind == SyntaxKind.CloseSquareBracketToken)
            {
                break;
            }
        }

        if (tokenCurrent.SyntaxKind == SyntaxKind.CloseSquareBracketToken)
        {
            return _binder.BindAttributeNode(
                openSquareBracketToken,
                (CloseSquareBracketToken)tokenCurrent);
        }

        return null;
    }

    /// <summary>Use this method for function definition, whereas <see cref="ParseFunctionParameters"/> should be used for function invocation.</summary>
    private BoundFunctionArgumentsNode ParseFunctionArguments(
        OpenParenthesisToken openParenthesisToken)
    {
        if (_tokenWalker.Peek(0).SyntaxKind == SyntaxKind.CloseParenthesisToken)
        {
            return _binder.BindFunctionArguments(
                openParenthesisToken,
                new(),
                (CloseParenthesisToken)_tokenWalker.Consume());
        }

        // Contains 'TypeClause' then 'ArgumentIdentifier' then 'CommaToken' then repeat pattern as long as there are entries.
        var functionArgumentListing = new List<ISyntaxToken>();

        // Alternate between reading TypeClause (null), ArgumentIdentifier (true), and a Comma (false)
        bool? canReadComma = null;

        while (true)
        {
            if (canReadComma is null)
            {
                var typeClauseToken = MatchTypeClauseToken();
                
                functionArgumentListing.Add(typeClauseToken);

                if (typeClauseToken.IsFabricated)
                    break;

                canReadComma = false;
            }
            else if (!canReadComma.Value)
            {
                var variableIdentifierToken = (IdentifierToken)_tokenWalker.Match(SyntaxKind.IdentifierToken);
                functionArgumentListing.Add(variableIdentifierToken);

                if (variableIdentifierToken.IsFabricated)
                    break;

                canReadComma = true;
            }
            else
            {
                if (_tokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
                {
                    var commaToken = (CommaToken)_tokenWalker.Consume();
                    functionArgumentListing.Add(commaToken);
                }
                else
                {
                    break;
                }

                canReadComma = null;
            }
        }

        var closeParenthesisToken = _tokenWalker.Match(SyntaxKind.CloseParenthesisToken);

        return _binder.BindFunctionArguments(
            openParenthesisToken,
            functionArgumentListing,
            (CloseParenthesisToken)closeParenthesisToken);
    }

    

    /// <summary>Use this method for function invocation, whereas <see cref="ParseFunctionArguments"/> should be used for function definition.</summary>
    private BoundFunctionParametersNode ParseFunctionParameters(
        OpenParenthesisToken openParenthesisToken)
    {
        if (_tokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
        {
            return new BoundFunctionParametersNode(
                openParenthesisToken,
                new(),
                (CloseParenthesisToken)_tokenWalker.Consume());
        }

        var functionParametersListing = new List<ISyntax>();

        bool canReadComma = false;

        while (true)
        {
            if (!canReadComma)
            {
                if (_tokenWalker.Current.SyntaxKind == SyntaxKind.KeywordToken)
                {
                    var currentTokenText = _tokenWalker.Current.TextSpan.GetText();

                    if (currentTokenText == "out")
                    {
                        functionParametersListing.Add(_tokenWalker.Consume());

                        if (_tokenWalker.Peek(0).SyntaxKind != SyntaxKind.CommaToken &&
                            _tokenWalker.Peek(1).SyntaxKind != SyntaxKind.CommaToken)
                        {
                            // out with variable declaration
                            //
                            // 'out IPersonModel variableIdentifier' OR 'out string variableIdentifier' OR 'out var variableIdentifier'

                            var outTypeClause = MatchTypeClauseToken();
                            functionParametersListing.Add(outTypeClause);
                        }

                        var outVariableIdentifier = _tokenWalker.Match(SyntaxKind.IdentifierToken);
                        
                        functionParametersListing.Add(outVariableIdentifier);

                        if (outVariableIdentifier.IsFabricated && 
                            _tokenWalker.Current.SyntaxKind != SyntaxKind.CommaToken)
                        {
                            break;
                        }
                    }
                    else if (currentTokenText == "in")
                    {
                        /*
                         * https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/in-parameter-modifier
                         * NOTE: "Overloading based on the presence of in is allowed":
                         * 
                         * void SampleMethod(in int i) { }
                         * void SampleMethod(int i) { }
                        */

                        functionParametersListing.Add(_tokenWalker.Consume());

                        // Function invocation would never specify a type when using the 'in' keyword. (only declaration would include that)

                        var inVariableIdentifier = _tokenWalker.Match(SyntaxKind.IdentifierToken);
                        functionParametersListing.Add(inVariableIdentifier);

                        if (inVariableIdentifier.IsFabricated &&
                            _tokenWalker.Current.SyntaxKind != SyntaxKind.CommaToken)
                        {
                            break;
                        }
                    }
                    else if (currentTokenText == "ref")
                    {
                        /*
                         * https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/ref
                         * NOTE: "methods can be overloaded when one method has a ref, in, or out parameter and the other has a parameter that is passed by value"
                         */

                        functionParametersListing.Add(_tokenWalker.Consume());

                        // Function invocation would never specify a type when using the 'ref' keyword. (only declaration would include that)

                        var refVariableIdentifier = _tokenWalker.Match(SyntaxKind.IdentifierToken);
                        functionParametersListing.Add(refVariableIdentifier);

                        if (refVariableIdentifier.IsFabricated &&
                            _tokenWalker.Current.SyntaxKind != SyntaxKind.CommaToken)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    var expression = ParseFunctionParameterExpression();
                    functionParametersListing.Add(expression);

                    if (expression.IsFabricated &&
                        _tokenWalker.Current.SyntaxKind != SyntaxKind.CommaToken)
                    {
                        break;
                    }
                }

                canReadComma = true;
            }
            else
            {
                if (_tokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
                {
                    functionParametersListing.Add(_tokenWalker.Consume());
                    canReadComma = false;
                }
                else
                {
                    break;
                }
            }
        }

        var closeParenthesisToken = _tokenWalker.Match(SyntaxKind.CloseParenthesisToken);

        return new BoundFunctionParametersNode(
            openParenthesisToken,
            functionParametersListing,
            (CloseParenthesisToken)closeParenthesisToken);
    }

    private BoundGenericArgumentsNode? ParseGenericArguments(
        OpenAngleBracketToken openAngleBracketToken)
    {
        // Contains 'IdentifierToken' then 'CommaToken' then repeat pattern as long as there are entries.
        var genericArgumentListing = new List<ISyntaxToken>();

        // Alternate between reading an identifier (true) and a comma (false)
        bool canReadComma = false;

        while (true)
        {
            if (!canReadComma)
            {
                var identifierToken = _tokenWalker.Match(SyntaxKind.IdentifierToken);
                genericArgumentListing.Add(identifierToken);

                if (identifierToken.IsFabricated)
                    break;

                canReadComma = true;
            }
            else
            {
                canReadComma = false;

                if (_tokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
                {
                    var commaToken = (CommaToken)_tokenWalker.Consume();
                    genericArgumentListing.Add(commaToken);
                }
                else
                {
                    break;
                }
            }
        }

        var closeAngleBracketToken = _tokenWalker.Match(SyntaxKind.CloseAngleBracketToken);

        return _binder.BindGenericArguments(
            openAngleBracketToken,
            genericArgumentListing,
            (CloseAngleBracketToken)closeAngleBracketToken);
    }

    private void ParseStatementDelimiterToken(
        StatementDelimiterToken statementDelimiterToken)
    {
        if (_nodeRecent is not null &&
            _nodeRecent.SyntaxKind == SyntaxKind.BoundNamespaceStatementNode)
        {
            var closureCurrentCompilationUnitBuilder = _currentCompilationUnitBuilder;
            Type? scopeReturnType = null;

            var boundNamespaceStatementNode = (BoundNamespaceStatementNode)_nodeRecent;

            _finalizeFileScopeAction = compilationUnit =>
            {
                boundNamespaceStatementNode = _binder.RegisterBoundNamespaceEntryNode(
                    boundNamespaceStatementNode,
                    compilationUnit);

                closureCurrentCompilationUnitBuilder.Children
                    .Add(boundNamespaceStatementNode);
            };

            _binder.RegisterBoundScope(
                scopeReturnType,
                statementDelimiterToken.TextSpan);

            _binder.AddNamespaceToCurrentScope((BoundNamespaceStatementNode)_nodeRecent);

            _currentCompilationUnitBuilder = new(_currentCompilationUnitBuilder);
        }
    }

    /// <summary>The keywords: "string, var, void, etc..." being Types is constantly resulting in me writing code incorrectly. This method will return either a keyword, or identifier (if they are truly in the tokens list and not fabricated). Otherwise, fabricate an identifier.</summary>
    private ISyntaxToken MatchTypeClauseToken()
    {
        if (_tokenWalker.Current.SyntaxKind == SyntaxKind.KeywordToken)
            return _tokenWalker.Consume();

        return _tokenWalker.Match(SyntaxKind.IdentifierToken);
    }
}
