using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis.ParserCase.Internals;

public class ParseVariablesTests
{
    /*
     # VariableDeclaration
         int x;
         Person x;
         var x;
         var var;
         public class var { } var var;
     # VariableAssignment
         x = true;
         x = 2;
         x = "Hello World!";
         var = "Hello World!";
     # COMBINED_VariableDeclaration_AND_VariableAssignment
         int x = 2;
         var x = 2;
         Person person = new Person();
         Person person = new Person { };
         Person person = new Person() { };
         Person person = new Person { FirstName = "John", LastName = "Doe" };
         Person person = new Person() { FirstName = "John", LastName = "Doe" };
     */

    [Fact]
    public void VariableDeclaration_WITH_ExplicitType_TypeIsKeyword()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "int x;";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        Assert.Single(topCodeBlock.ChildList);
        var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList.Single();

        var typeClauseNode = variableDeclarationNode.TypeClauseNode;
        Assert.Equal("int", typeClauseNode.TypeIdentifierToken.TextSpan.GetText());
        Assert.Equal(typeof(int), typeClauseNode.ValueType);

        var identifierToken = variableDeclarationNode.IdentifierToken;
        Assert.Equal("x", identifierToken.TextSpan.GetText());

        Assert.Empty(compilationUnit.DiagnosticsList);
    }
    
    [Fact]
    public void VariableDeclaration_WITH_ExplicitType_TypeIsIdentifier()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "Person x;";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Single(topCodeBlock.ChildList);
        var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList.Single();

        var typeClauseNode = variableDeclarationNode.TypeClauseNode;
        Assert.Equal("Person", typeClauseNode.TypeIdentifierToken.TextSpan.GetText());
        Assert.Null(typeClauseNode.ValueType);

        var identifierToken = variableDeclarationNode.IdentifierToken;
        Assert.Equal("x", identifierToken.TextSpan.GetText());

        Guid idOfExpectedDiagnostic;
        {
            // TODO: Reporting the diagnostic to get the Id like this is silly.
            var fakeDiagnosticBag = new LuthDiagnosticBag();
            fakeDiagnosticBag.ReportUndefinedTypeOrNamespace(
                TextEditorTextSpan.FabricateTextSpan(string.Empty),
                string.Empty);
            idOfExpectedDiagnostic = fakeDiagnosticBag.Single().Id;
        }
        
        Assert.Single(compilationUnit.DiagnosticsList);
        Assert.Equal(idOfExpectedDiagnostic, compilationUnit.DiagnosticsList.Single().Id);
    }

    [Fact]
    public void VariableDeclaration_WITH_ImplicitType()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "var x;";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Single(topCodeBlock.ChildList);
        var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList.Single();

        var typeClauseNode = variableDeclarationNode.TypeClauseNode;
        Assert.Equal("var", typeClauseNode.TypeIdentifierToken.TextSpan.GetText());
        Assert.Equal(typeof(void), typeClauseNode.ValueType);

        var identifierToken = variableDeclarationNode.IdentifierToken;
        Assert.Equal("x", identifierToken.TextSpan.GetText());

        Guid idOfExpectedDiagnostic;
        {
            // TODO: Reporting the diagnostic to get the Id like this is silly.
            var fakeDiagnosticBag = new LuthDiagnosticBag();
            fakeDiagnosticBag.ReportImplicitlyTypedVariablesMustBeInitialized(
                TextEditorTextSpan.FabricateTextSpan(string.Empty));
            idOfExpectedDiagnostic = fakeDiagnosticBag.Single().Id;
        }

        Assert.Single(compilationUnit.DiagnosticsList);
        Assert.Equal(idOfExpectedDiagnostic, compilationUnit.DiagnosticsList.Single().Id);
    }

    [Fact]
    public void VariableDeclaration_WITH_ImplicitType_WITH_VarIdentifier()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "var var;";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Single(topCodeBlock.ChildList);
        var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList.Single();

        var typeClauseNode = variableDeclarationNode.TypeClauseNode;
        Assert.Equal("var", typeClauseNode.TypeIdentifierToken.TextSpan.GetText());
        Assert.Equal(typeof(void), typeClauseNode.ValueType);

        var identifierToken = variableDeclarationNode.IdentifierToken;
        Assert.Equal("var", identifierToken.TextSpan.GetText());

        Guid idOfExpectedDiagnostic;
        {
            // TODO: Reporting the diagnostic to get the Id like this is silly.
            var fakeDiagnosticBag = new LuthDiagnosticBag();
            fakeDiagnosticBag.ReportImplicitlyTypedVariablesMustBeInitialized(
                TextEditorTextSpan.FabricateTextSpan(string.Empty));
            idOfExpectedDiagnostic = fakeDiagnosticBag.Single().Id;
        }

        Assert.Single(compilationUnit.DiagnosticsList);
        Assert.Equal(idOfExpectedDiagnostic, compilationUnit.DiagnosticsList.Single().Id);
    }

    [Fact]
    public void VariableDeclaration_WITH_VarAsDefinedType_WITH_VarIdentifier()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "public class var { } var var;";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Equal(2, topCodeBlock.ChildList.Length);

        // typeDefinitionNode
        {
            var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList[0];

            var identifierToken = (IdentifierToken)typeDefinitionNode.ChildList[0];
            Assert.Equal("var", identifierToken.TextSpan.GetText());

            var codeBlockNode = (CodeBlockNode)typeDefinitionNode.ChildList[1];
            Assert.Empty(codeBlockNode.ChildList);
        }

        // variableDeclarationNode
        {
            var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList[1];

            var typeClauseNode = variableDeclarationNode.TypeClauseNode;
            Assert.Equal("var", typeClauseNode.TypeIdentifierToken.TextSpan.GetText());

            // Assert that the type isn't the var contextual keyword
            // (since it was defined as a class)
            Assert.IsType<IdentifierToken>(typeClauseNode.TypeIdentifierToken);

            Assert.Null(typeClauseNode.ValueType);

            var identifierToken = variableDeclarationNode.IdentifierToken;
            Assert.Equal("var", identifierToken.TextSpan.GetText());
        }

        Assert.Empty(compilationUnit.DiagnosticsList);
    }

    [Fact]
    public void VariableAssignment_WITH_BoolLiteral()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "x = true;";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Single(topCodeBlock.ChildList);
        var variableAssignmentExpressionNode = (VariableAssignmentExpressionNode)topCodeBlock.ChildList.Single();

        var identifierToken = variableAssignmentExpressionNode.VariableIdentifierToken;
        Assert.Equal("x", identifierToken.TextSpan.GetText());

        var equalsToken = variableAssignmentExpressionNode.EqualsToken;
        Assert.Equal("=", equalsToken.TextSpan.GetText());

        var literalExpressionNode = (LiteralExpressionNode)variableAssignmentExpressionNode.ExpressionNode;
        Assert.Equal("true", literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
        Assert.Equal(typeof(bool), literalExpressionNode.ResultTypeClauseNode.ValueType);

        Guid idOfExpectedDiagnostic;
        {
            // TODO: Reporting the diagnostic to get the Id like this is silly.
            var fakeDiagnosticBag = new LuthDiagnosticBag();
            fakeDiagnosticBag.ReportUndefinedVariable(
                TextEditorTextSpan.FabricateTextSpan(string.Empty),
                string.Empty);
            idOfExpectedDiagnostic = fakeDiagnosticBag.Single().Id;
        }

        Assert.Single(compilationUnit.DiagnosticsList);
        Assert.Equal(idOfExpectedDiagnostic, compilationUnit.DiagnosticsList.Single().Id);
    }

    [Fact]
    public void VariableAssignment_WITH_IntLiteral()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "x = 2;";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Single(topCodeBlock.ChildList);
        var variableAssignmentExpressionNode = (VariableAssignmentExpressionNode)topCodeBlock.ChildList.Single();

        var identifierToken = variableAssignmentExpressionNode.VariableIdentifierToken;
        Assert.Equal("x", identifierToken.TextSpan.GetText());

        var equalsToken = variableAssignmentExpressionNode.EqualsToken;
        Assert.Equal("=", equalsToken.TextSpan.GetText());

        var literalExpressionNode = (LiteralExpressionNode)variableAssignmentExpressionNode.ExpressionNode;
        Assert.Equal("2", literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
        Assert.Equal(typeof(int), literalExpressionNode.ResultTypeClauseNode.ValueType);

        Guid idOfExpectedDiagnostic;
        {
            // TODO: Reporting the diagnostic to get the Id like this is silly.
            var fakeDiagnosticBag = new LuthDiagnosticBag();
            fakeDiagnosticBag.ReportUndefinedVariable(
                TextEditorTextSpan.FabricateTextSpan(string.Empty),
                string.Empty);
            idOfExpectedDiagnostic = fakeDiagnosticBag.Single().Id;
        }

        Assert.Single(compilationUnit.DiagnosticsList);
        Assert.Equal(idOfExpectedDiagnostic, compilationUnit.DiagnosticsList.Single().Id);
    }

    [Fact]
    public void VariableAssignment_WITH_StringLiteral()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "x = \"Hello World!\";";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Single(topCodeBlock.ChildList);
        var variableAssignmentExpressionNode = (VariableAssignmentExpressionNode)topCodeBlock.ChildList.Single();

        var identifierToken = variableAssignmentExpressionNode.VariableIdentifierToken;
        Assert.Equal("x", identifierToken.TextSpan.GetText());

        var equalsToken = variableAssignmentExpressionNode.EqualsToken;
        Assert.Equal("=", equalsToken.TextSpan.GetText());

        var literalExpressionNode = (LiteralExpressionNode)variableAssignmentExpressionNode.ExpressionNode;
        Assert.Equal("\"Hello World!\"", literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
        Assert.Equal(typeof(string), literalExpressionNode.ResultTypeClauseNode.ValueType);

        Guid idOfExpectedDiagnostic;
        {
            // TODO: Reporting the diagnostic to get the Id like this is silly.
            var fakeDiagnosticBag = new LuthDiagnosticBag();
            fakeDiagnosticBag.ReportUndefinedVariable(
                TextEditorTextSpan.FabricateTextSpan(string.Empty),
                string.Empty);
            idOfExpectedDiagnostic = fakeDiagnosticBag.Single().Id;
        }

        Assert.Single(compilationUnit.DiagnosticsList);
        Assert.Equal(idOfExpectedDiagnostic, compilationUnit.DiagnosticsList.Single().Id);
    }

    [Fact]
    public void VariableAssignment_WITH_StringLiteral_WITH_VarIdentifier()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "var = \"Hello World!\";";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Single(topCodeBlock.ChildList);
        var variableAssignmentExpressionNode = (VariableAssignmentExpressionNode)topCodeBlock.ChildList.Single();

        var identifierToken = variableAssignmentExpressionNode.VariableIdentifierToken;
        Assert.Equal("var", identifierToken.TextSpan.GetText());

        var equalsToken = variableAssignmentExpressionNode.EqualsToken;
        Assert.Equal("=", equalsToken.TextSpan.GetText());

        var literalExpressionNode = (LiteralExpressionNode)variableAssignmentExpressionNode.ExpressionNode;
        Assert.Equal("\"Hello World!\"", literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
        Assert.Equal(typeof(string), literalExpressionNode.ResultTypeClauseNode.ValueType);

        Guid idOfExpectedDiagnostic;
        {
            // TODO: Reporting the diagnostic to get the Id like this is silly.
            var fakeDiagnosticBag = new LuthDiagnosticBag();
            fakeDiagnosticBag.TheNameDoesNotExistInTheCurrentContext(
                TextEditorTextSpan.FabricateTextSpan(string.Empty),
                string.Empty);
            idOfExpectedDiagnostic = fakeDiagnosticBag.Single().Id;
        }

        Assert.Single(compilationUnit.DiagnosticsList);
        Assert.Equal(idOfExpectedDiagnostic, compilationUnit.DiagnosticsList.Single().Id);
    }

    [Fact]
    public void COMBINED_VariableDeclaration_AND_VariableAssignment_WITH_ExplicitType()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "int x = 2;";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Equal(2, topCodeBlock.ChildList.Length);

        // variableDeclarationNode
        {
            var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList[0];

            var typeClauseNode = variableDeclarationNode.TypeClauseNode;
            Assert.Equal("int", typeClauseNode.TypeIdentifierToken.TextSpan.GetText());
            Assert.Equal(typeof(int), typeClauseNode.ValueType);

            var identifierToken = variableDeclarationNode.IdentifierToken;
            Assert.Equal("x", identifierToken.TextSpan.GetText());
        }

        // variableAssignmentNode
        {
            var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.ChildList[1];

            var identifierToken = variableAssignmentNode.VariableIdentifierToken;
            Assert.Equal("x", identifierToken.TextSpan.GetText());

            var equalsToken = variableAssignmentNode.EqualsToken;
            Assert.Equal("=", equalsToken.TextSpan.GetText());

            var literalExpressionNode = (LiteralExpressionNode)variableAssignmentNode.ExpressionNode;
            Assert.Equal("2", literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
            Assert.Equal(typeof(int), literalExpressionNode.ResultTypeClauseNode.ValueType);
        }

        Assert.Empty(compilationUnit.DiagnosticsList);
    }
    
    [Fact]
    public void COMBINED_VariableDeclaration_AND_VariableAssignment_WITH_ImplicitType()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "var x = 2;";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Equal(2, topCodeBlock.ChildList.Length);

        // variableDeclarationNode
        {
            // TODO: The var TypeClauseNode should be an integer for this case.

            var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList[0];

            var typeClauseNode = variableDeclarationNode.TypeClauseNode;
            Assert.Equal("var", typeClauseNode.TypeIdentifierToken.TextSpan.GetText());
            Assert.Equal(typeof(void), typeClauseNode.ValueType);

            var identifierToken = variableDeclarationNode.IdentifierToken;
            Assert.Equal("x", identifierToken.TextSpan.GetText());
        }

        // variableAssignmentNode
        {
            var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.ChildList[1];

            var identifierToken = variableAssignmentNode.VariableIdentifierToken;
            Assert.Equal("x", identifierToken.TextSpan.GetText());

            var equalsToken = variableAssignmentNode.EqualsToken;
            Assert.Equal("=", equalsToken.TextSpan.GetText());

            var literalExpressionNode = (LiteralExpressionNode)variableAssignmentNode.ExpressionNode;
            Assert.Equal("2", literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText());
            Assert.Equal(typeof(int), literalExpressionNode.ResultTypeClauseNode.ValueType);
        }

        Assert.Empty(compilationUnit.DiagnosticsList);
    }
    
    [Fact]
    public void COMBINED_VariableDeclaration_AND_VariableAssignment_WITH_ExplicitType_WITH_ConstructorInvocation()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "Person person = new Person();";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Equal(2, topCodeBlock.ChildList.Length);

        // variableDeclarationNode
        {
            var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList[0];

            var typeClauseNode = variableDeclarationNode.TypeClauseNode;
            Assert.Equal("Person", typeClauseNode.TypeIdentifierToken.TextSpan.GetText());
            Assert.Null(typeClauseNode.ValueType);

            var identifierToken = variableDeclarationNode.IdentifierToken;
            Assert.Equal("person", identifierToken.TextSpan.GetText());
        }

        // variableAssignmentNode
        {
            var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.ChildList[1];

            var identifierToken = variableAssignmentNode.VariableIdentifierToken;
            Assert.Equal("person", identifierToken.TextSpan.GetText());

            var equalsToken = variableAssignmentNode.EqualsToken;
            Assert.Equal("=", equalsToken.TextSpan.GetText());

            // constructorInvocationExpressionNode
            {
                var constructorInvocationNode = (ConstructorInvocationExpressionNode)variableAssignmentNode.ExpressionNode;

                Assert.Equal("new", constructorInvocationNode.NewKeywordToken.TextSpan.GetText());
                Assert.Equal("Person", constructorInvocationNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());

                Assert.NotNull(constructorInvocationNode.FunctionParametersListingNode);
                Assert.Equal("(", constructorInvocationNode.FunctionParametersListingNode.OpenParenthesisToken.TextSpan.GetText());
                Assert.Empty(constructorInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodeList);
                Assert.Equal(")", constructorInvocationNode.FunctionParametersListingNode.CloseParenthesisToken.TextSpan.GetText());
            }
        }

        Guid idOfExpectedDiagnostic;
        {
            // TODO: Reporting the diagnostic to get the Id like this is silly.
            var fakeDiagnosticBag = new LuthDiagnosticBag();
            fakeDiagnosticBag.ReportUndefinedTypeOrNamespace(
                TextEditorTextSpan.FabricateTextSpan(string.Empty),
                string.Empty);
            idOfExpectedDiagnostic = fakeDiagnosticBag.Single().Id;
        }

        Assert.Single(compilationUnit.DiagnosticsList);
        Assert.Equal(idOfExpectedDiagnostic, compilationUnit.DiagnosticsList.Single().Id);
    }
    
    [Fact]
    public void COMBINED_VariableDeclaration_AND_VariableAssignment_WITH_ExplicitType_With_ObjectInitialization()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "Person person = new Person { };";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Equal(2, topCodeBlock.ChildList.Length);

        // variableDeclarationNode
        {
            var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList[0];

            var typeClauseNode = variableDeclarationNode.TypeClauseNode;
            Assert.Equal("Person", typeClauseNode.TypeIdentifierToken.TextSpan.GetText());
            Assert.Null(typeClauseNode.ValueType);

            var identifierToken = variableDeclarationNode.IdentifierToken;
            Assert.Equal("person", identifierToken.TextSpan.GetText());
        }

        // variableAssignmentNode
        {
            var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.ChildList[1];

            var identifierToken = variableAssignmentNode.VariableIdentifierToken;
            Assert.Equal("person", identifierToken.TextSpan.GetText());

            var equalsToken = variableAssignmentNode.EqualsToken;
            Assert.Equal("=", equalsToken.TextSpan.GetText());

            // constructorInvocationExpressionNode
            {
                var constructorInvocationNode = (ConstructorInvocationExpressionNode)variableAssignmentNode.ExpressionNode;

                Assert.Equal("new", constructorInvocationNode.NewKeywordToken.TextSpan.GetText());
                Assert.Equal("Person", constructorInvocationNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());

                Assert.Null(constructorInvocationNode.FunctionParametersListingNode);

                Assert.NotNull(constructorInvocationNode.ObjectInitializationParametersListingNode);
                Assert.Equal("{", constructorInvocationNode.ObjectInitializationParametersListingNode.OpenBraceToken.TextSpan.GetText());
                Assert.Empty(constructorInvocationNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList);
                Assert.Equal("}", constructorInvocationNode.ObjectInitializationParametersListingNode.CloseBraceToken.TextSpan.GetText());
            }
        }

        Guid idOfExpectedDiagnostic;
        {
            // TODO: Reporting the diagnostic to get the Id like this is silly.
            var fakeDiagnosticBag = new LuthDiagnosticBag();
            fakeDiagnosticBag.ReportUndefinedTypeOrNamespace(
                TextEditorTextSpan.FabricateTextSpan(string.Empty),
                string.Empty);
            idOfExpectedDiagnostic = fakeDiagnosticBag.Single().Id;
        }

        Assert.Single(compilationUnit.DiagnosticsList);
        Assert.Equal(idOfExpectedDiagnostic, compilationUnit.DiagnosticsList.Single().Id);
    }
    
    [Fact]
    public void COMBINED_VariableDeclaration_AND_VariableAssignment_WITH_ExplicitType_WITH_ConstructorInvocation_AND_ObjectInitialization()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "Person person = new Person() { };";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Equal(2, topCodeBlock.ChildList.Length);

        // variableDeclarationNode
        {
            var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList[0];

            var typeClauseNode = variableDeclarationNode.TypeClauseNode;
            Assert.Equal("Person", typeClauseNode.TypeIdentifierToken.TextSpan.GetText());
            Assert.Null(typeClauseNode.ValueType);

            var identifierToken = variableDeclarationNode.IdentifierToken;
            Assert.Equal("person", identifierToken.TextSpan.GetText());
        }

        // variableAssignmentNode
        {
            var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.ChildList[1];

            var identifierToken = variableAssignmentNode.VariableIdentifierToken;
            Assert.Equal("person", identifierToken.TextSpan.GetText());

            var equalsToken = variableAssignmentNode.EqualsToken;
            Assert.Equal("=", equalsToken.TextSpan.GetText());

            // constructorInvocationExpressionNode
            {
                var constructorInvocationNode = (ConstructorInvocationExpressionNode)variableAssignmentNode.ExpressionNode;

                Assert.Equal("new", constructorInvocationNode.NewKeywordToken.TextSpan.GetText());
                Assert.Equal("Person", constructorInvocationNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());

                Assert.NotNull(constructorInvocationNode.FunctionParametersListingNode);
                Assert.Equal("(", constructorInvocationNode.FunctionParametersListingNode.OpenParenthesisToken.TextSpan.GetText());
                Assert.Empty(constructorInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodeList);
                Assert.Equal(")", constructorInvocationNode.FunctionParametersListingNode.CloseParenthesisToken.TextSpan.GetText());

                Assert.NotNull(constructorInvocationNode.ObjectInitializationParametersListingNode);
                Assert.Equal("{", constructorInvocationNode.ObjectInitializationParametersListingNode.OpenBraceToken.TextSpan.GetText());
                Assert.Empty(constructorInvocationNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList);
                Assert.Equal("}", constructorInvocationNode.ObjectInitializationParametersListingNode.CloseBraceToken.TextSpan.GetText());
            }
        }

        Guid idOfExpectedDiagnostic;
        {
            // TODO: Reporting the diagnostic to get the Id like this is silly.
            var fakeDiagnosticBag = new LuthDiagnosticBag();
            fakeDiagnosticBag.ReportUndefinedTypeOrNamespace(
                TextEditorTextSpan.FabricateTextSpan(string.Empty),
                string.Empty);
            idOfExpectedDiagnostic = fakeDiagnosticBag.Single().Id;
        }

        Assert.Single(compilationUnit.DiagnosticsList);
        Assert.Equal(idOfExpectedDiagnostic, compilationUnit.DiagnosticsList.Single().Id);
    }
    
    [Fact]
    public void COMBINED_VariableDeclaration_AND_VariableAssignment_WITH_ExplicitType_WITH_ObjectInitializationEntries()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "Person person = new Person { FirstName = \"John\", LastName = \"Doe\" };";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Equal(2, topCodeBlock.ChildList.Length);

        // variableDeclarationNode
        {
            var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList[0];

            var typeClauseNode = variableDeclarationNode.TypeClauseNode;
            Assert.Equal("Person", typeClauseNode.TypeIdentifierToken.TextSpan.GetText());
            Assert.Null(typeClauseNode.ValueType);

            var identifierToken = variableDeclarationNode.IdentifierToken;
            Assert.Equal("person", identifierToken.TextSpan.GetText());
        }

        // variableAssignmentNode
        {
            var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.ChildList[1];

            var identifierToken = variableAssignmentNode.VariableIdentifierToken;
            Assert.Equal("person", identifierToken.TextSpan.GetText());

            var equalsToken = variableAssignmentNode.EqualsToken;
            Assert.Equal("=", equalsToken.TextSpan.GetText());

            // constructorInvocationExpressionNode
            {
                var constructorInvocationNode = (ConstructorInvocationExpressionNode)variableAssignmentNode.ExpressionNode;

                Assert.Equal("new", constructorInvocationNode.NewKeywordToken.TextSpan.GetText());
                Assert.Equal("Person", constructorInvocationNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());

                Assert.Null(constructorInvocationNode.FunctionParametersListingNode);

                Assert.NotNull(constructorInvocationNode.ObjectInitializationParametersListingNode);
                Assert.Equal("{", constructorInvocationNode.ObjectInitializationParametersListingNode.OpenBraceToken.TextSpan.GetText());

                // constructorInvocationNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList
                {
                    Assert.Equal(2, constructorInvocationNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList.Length);

                    // First ObjectInitializationParameterEntryNode
                    {
                        var objectInitializationParameterEntryNode = constructorInvocationNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList[0];

                        Assert.Equal("FirstName", objectInitializationParameterEntryNode.PropertyIdentifierToken.TextSpan.GetText());
                        var stringLiteralExpression = (LiteralExpressionNode)objectInitializationParameterEntryNode.ExpressionNode;
                        Assert.Equal("\"John\"", stringLiteralExpression.LiteralSyntaxToken.TextSpan.GetText());
                    }

                    // Second ObjectInitializationParameterEntryNode
                    {
                        var objectInitializationParameterEntryNode = constructorInvocationNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList[1];

                        Assert.Equal("LastName", objectInitializationParameterEntryNode.PropertyIdentifierToken.TextSpan.GetText());
                        var stringLiteralExpression = (LiteralExpressionNode)objectInitializationParameterEntryNode.ExpressionNode;
                        Assert.Equal("\"Doe\"", stringLiteralExpression.LiteralSyntaxToken.TextSpan.GetText());
                    }
                }

                Assert.Equal("}", constructorInvocationNode.ObjectInitializationParametersListingNode.CloseBraceToken.TextSpan.GetText());
            }
        }

        Guid idOfExpectedDiagnostic;
        {
            // TODO: Reporting the diagnostic to get the Id like this is silly.
            var fakeDiagnosticBag = new LuthDiagnosticBag();
            fakeDiagnosticBag.ReportUndefinedTypeOrNamespace(
                TextEditorTextSpan.FabricateTextSpan(string.Empty),
                string.Empty);
            idOfExpectedDiagnostic = fakeDiagnosticBag.Single().Id;
        }

        Assert.Single(compilationUnit.DiagnosticsList);
        Assert.Equal(idOfExpectedDiagnostic, compilationUnit.DiagnosticsList.Single().Id);
    }
    
    [Fact]
    public void COMBINED_VariableDeclaration_AND_VariableAssignment_WITH_ExplicitType_WITH_ConstructorInvocation_AND_ObjectInitializationEntries()
    {
        var resourceUri = new ResourceUri("UnitTests");
        var sourceText = "Person person = new Person() { FirstName = \"John\", LastName = \"Doe\" };";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        Assert.Equal(2, topCodeBlock.ChildList.Length);

        // variableDeclarationNode
        {
            var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList[0];

            var typeClauseNode = variableDeclarationNode.TypeClauseNode;
            Assert.Equal("Person", typeClauseNode.TypeIdentifierToken.TextSpan.GetText());
            Assert.Null(typeClauseNode.ValueType);

            var identifierToken = variableDeclarationNode.IdentifierToken;
            Assert.Equal("person", identifierToken.TextSpan.GetText());
        }

        // variableAssignmentNode
        {
            var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.ChildList[1];

            var identifierToken = variableAssignmentNode.VariableIdentifierToken;
            Assert.Equal("person", identifierToken.TextSpan.GetText());

            var equalsToken = variableAssignmentNode.EqualsToken;
            Assert.Equal("=", equalsToken.TextSpan.GetText());

            // constructorInvocationExpressionNode
            {
                var constructorInvocationNode = (ConstructorInvocationExpressionNode)variableAssignmentNode.ExpressionNode;

                Assert.Equal("new", constructorInvocationNode.NewKeywordToken.TextSpan.GetText());
                Assert.Equal("Person", constructorInvocationNode.ResultTypeClauseNode.TypeIdentifierToken.TextSpan.GetText());

                Assert.NotNull(constructorInvocationNode.FunctionParametersListingNode);
                Assert.Equal("(", constructorInvocationNode.FunctionParametersListingNode.OpenParenthesisToken.TextSpan.GetText());
                Assert.Empty(constructorInvocationNode.FunctionParametersListingNode.FunctionParameterEntryNodeList);
                Assert.Equal(")", constructorInvocationNode.FunctionParametersListingNode.CloseParenthesisToken.TextSpan.GetText());

                Assert.NotNull(constructorInvocationNode.ObjectInitializationParametersListingNode);
                Assert.Equal("{", constructorInvocationNode.ObjectInitializationParametersListingNode.OpenBraceToken.TextSpan.GetText());

                // constructorInvocationNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList
                {
                    Assert.Equal(2, constructorInvocationNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList.Length);

                    // First ObjectInitializationParameterEntryNode
                    {
                        var objectInitializationParameterEntryNode = constructorInvocationNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList[0];

                        Assert.Equal("FirstName", objectInitializationParameterEntryNode.PropertyIdentifierToken.TextSpan.GetText());
                        var stringLiteralExpression = (LiteralExpressionNode)objectInitializationParameterEntryNode.ExpressionNode;
                        Assert.Equal("\"John\"", stringLiteralExpression.LiteralSyntaxToken.TextSpan.GetText());
                    }

                    // Second ObjectInitializationParameterEntryNode
                    {
                        var objectInitializationParameterEntryNode = constructorInvocationNode.ObjectInitializationParametersListingNode.ObjectInitializationParameterEntryNodeList[1];

                        Assert.Equal("LastName", objectInitializationParameterEntryNode.PropertyIdentifierToken.TextSpan.GetText());
                        var stringLiteralExpression = (LiteralExpressionNode)objectInitializationParameterEntryNode.ExpressionNode;
                        Assert.Equal("\"Doe\"", stringLiteralExpression.LiteralSyntaxToken.TextSpan.GetText());
                    }
                }

                Assert.Equal("}", constructorInvocationNode.ObjectInitializationParametersListingNode.CloseBraceToken.TextSpan.GetText());
            }
        }

        Guid idOfExpectedDiagnostic;
        {
            // TODO: Reporting the diagnostic to get the Id like this is silly.
            var fakeDiagnosticBag = new LuthDiagnosticBag();
            fakeDiagnosticBag.ReportUndefinedTypeOrNamespace(
                TextEditorTextSpan.FabricateTextSpan(string.Empty),
                string.Empty);
            idOfExpectedDiagnostic = fakeDiagnosticBag.Single().Id;
        }

        Assert.Single(compilationUnit.DiagnosticsList);
        Assert.Equal(idOfExpectedDiagnostic, compilationUnit.DiagnosticsList.Single().Id);
    }
}
