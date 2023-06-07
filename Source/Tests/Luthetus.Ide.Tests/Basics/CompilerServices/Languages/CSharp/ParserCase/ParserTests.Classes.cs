using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.ParserCase;

public partial class ParserTests
{
    [Fact]
    public void SHOULD_PARSE_CLASS_DEFINITION_EMPTY()
    {
        string classIdentifier = "PersonModel";
        string sourceText = @$"public class {classIdentifier} {{ }}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            var globalScope = parser.Binder.BoundScopes.First();

            Assert.True(globalScope.ClassDefinitionMap.ContainsKey(classIdentifier));

            var boundClassDefinitionNode = (BoundClassDefinitionNode)compilationUnit.Children.Single();

            if (boundClassDefinitionNode.ClassBodyCompilationUnit is null)
                throw new ApplicationException("ClassBodyCompilationUnit should not be null here.");

            Assert.Equal(classIdentifier, boundClassDefinitionNode.Identifier.TextSpan.GetText());
            Assert.Empty(boundClassDefinitionNode.ClassBodyCompilationUnit.Children);
        }
    }

    [Fact]
    public void SHOULD_PARSE_CLASS_DEFINITION_WHICH_CONTAINS_A_METHOD()
    {
        string classIdentifier = "PersonModel";
        string methodIdentifier = "Walk";
        string sourceText = @$"public class {classIdentifier} {{ public void {methodIdentifier}() {{ }} }}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            var globalScope = parser.Binder.BoundScopes.First();

            var personModel = globalScope.ClassDefinitionMap.Single();

            Assert.Equal(classIdentifier, personModel.Key);

            var boundClassDefinitionNode =
                (BoundClassDefinitionNode)compilationUnit.Children.Single();

            if (boundClassDefinitionNode.ClassBodyCompilationUnit is null)
                throw new ApplicationException("ClassBodyCompilationUnit should not be null here.");

            var boundFunctionDefinitionNode =
                (BoundFunctionDefinitionNode)boundClassDefinitionNode
                    .ClassBodyCompilationUnit.Children.Single();

            Assert.Equal(
                SyntaxKind.BoundFunctionDefinitionNode,
                boundFunctionDefinitionNode.SyntaxKind);
        }
    }

    [Fact]
    public void SHOULD_PARSE_CLASS_DEFINITION_WHICH_HAS_PARTIAL_MODIFIER()
    {
        string classIdentifier = "PersonModel";
        string sourceText = @$"public partial class {classIdentifier} {{ }}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            var globalScope = parser.Binder.BoundScopes.First();

            var personModel = globalScope.ClassDefinitionMap.Single();

            Assert.Equal(classIdentifier, personModel.Key);

            var boundClassDefinitionNode =
                (BoundClassDefinitionNode)compilationUnit.Children.Single();

            if (boundClassDefinitionNode.ClassBodyCompilationUnit is null)
                throw new ApplicationException("ClassBodyCompilationUnit should not be null here.");

            Assert.Empty(
                boundClassDefinitionNode.ClassBodyCompilationUnit.Children);
        }
    }

    [Fact]
    public void SHOULD_PARSE_CLASS_DEFINITION_WHICH_IS_INHERITING()
    {
        string classIdentifier = "PersonDisplay";
        string sourceText = @$"public class {classIdentifier} : ComponentBase {{ }}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            var globalScope = parser.Binder.BoundScopes.First();

            var personModel = globalScope.ClassDefinitionMap.Single();

            Assert.Equal(classIdentifier, personModel.Key);

            var boundClassDefinitionNode =
                (BoundClassDefinitionNode)compilationUnit.Children.Single();

            if (boundClassDefinitionNode.ClassBodyCompilationUnit is null)
                throw new ApplicationException("ClassBodyCompilationUnit should not be null here.");

            Assert.NotNull(
                boundClassDefinitionNode.BoundInheritanceStatementNode);

            Assert.Empty(
                boundClassDefinitionNode.ClassBodyCompilationUnit.Children);
        }
    }

    [Fact]
    public void SHOULD_PARSE_CLASS_DEFINITION_WHICH_HAS_ONE_GENERIC_ARGUMENT()
    {
        string classIdentifier = "Box", genericArgumentIdentifier = "T";
        string sourceText = @$"public class {classIdentifier}<{genericArgumentIdentifier}> {{ }}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            var globalScope = parser.Binder.BoundScopes.First();

            var personModel = globalScope.ClassDefinitionMap.Single();

            Assert.Equal(classIdentifier, personModel.Key);

            var boundClassDefinitionNode = (BoundClassDefinitionNode)compilationUnit.Children.Single();

            if (boundClassDefinitionNode.ClassBodyCompilationUnit is null)
                throw new ApplicationException("ClassBodyCompilationUnit should not be null here.");

            Assert.NotNull(boundClassDefinitionNode.BoundGenericArgumentsNode);
            Assert.Single(boundClassDefinitionNode.BoundGenericArgumentsNode!.BoundGenericArgumentListing);

            var genericTypeBoundClassDefinitionNode = (BoundClassDefinitionNode)boundClassDefinitionNode.BoundGenericArgumentsNode.BoundGenericArgumentListing.Single();
            Assert.Equal(genericArgumentIdentifier, genericTypeBoundClassDefinitionNode.TypeClauseToken.TextSpan.GetText());

            Assert.Empty(boundClassDefinitionNode.ClassBodyCompilationUnit.Children);
        }
    }
    
    [Fact]
    public void SHOULD_PARSE_CLASS_DEFINITION_WHICH_HAS_TWO_GENERIC_ARGUMENTS()
    {
        string classIdentifier = "Box", genericArgOne = "TItem", genericArgTwo = "TPackager";
        string sourceText = @$"public class {classIdentifier}<{genericArgOne}, {genericArgTwo}> {{ }}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            var globalScope = parser.Binder.BoundScopes.First();

            var personModel = globalScope.ClassDefinitionMap.Single();

            Assert.Equal(classIdentifier, personModel.Key);

            var boundClassDefinitionNode =
                (BoundClassDefinitionNode)compilationUnit.Children.Single();

            if (boundClassDefinitionNode.ClassBodyCompilationUnit is null)
                throw new ApplicationException("ClassBodyCompilationUnit should not be null here.");

            Assert.NotNull(boundClassDefinitionNode.BoundGenericArgumentsNode);
            Assert.Equal(3, boundClassDefinitionNode.BoundGenericArgumentsNode!.BoundGenericArgumentListing.Count);

            var firstBoundClassDefinitionNode = (BoundClassDefinitionNode)boundClassDefinitionNode.BoundGenericArgumentsNode.BoundGenericArgumentListing[0];
            Assert.Equal(genericArgOne, firstBoundClassDefinitionNode.TypeClauseToken.TextSpan.GetText());
            
            var commaToken = (CommaToken)boundClassDefinitionNode.BoundGenericArgumentsNode.BoundGenericArgumentListing[1];
            Assert.Equal(",", commaToken.TextSpan.GetText());
            
            var secondBoundClassDefinitionNode = (BoundClassDefinitionNode)boundClassDefinitionNode.BoundGenericArgumentsNode.BoundGenericArgumentListing[2];
            Assert.Equal(genericArgTwo, secondBoundClassDefinitionNode.TypeClauseToken.TextSpan.GetText());

            Assert.Empty(boundClassDefinitionNode.ClassBodyCompilationUnit.Children);
        }
    }
    
    [Fact]
    public void SHOULD_PARSE_CLASS_DEFINITION_WHICH_HAS_THREE_GENERIC_ARGUMENTS()
    {
        string classIdentifier = "Box", genericArgOne = "TItem", genericArgTwo = "TPackager", genericArgThree = "TDeliverer";
        string sourceText = @$"public class {classIdentifier}<{genericArgOne}, {genericArgTwo}, {genericArgThree}> : ComponentBase {{ }}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            var globalScope = parser.Binder.BoundScopes.First();

            var personModel = globalScope.ClassDefinitionMap.Single();

            Assert.Equal(classIdentifier, personModel.Key);

            var boundClassDefinitionNode =
                (BoundClassDefinitionNode)compilationUnit.Children.Single();

            if (boundClassDefinitionNode.ClassBodyCompilationUnit is null)
                throw new ApplicationException("ClassBodyCompilationUnit should not be null here.");

            Assert.NotNull(boundClassDefinitionNode.BoundGenericArgumentsNode);
            Assert.Equal(5, boundClassDefinitionNode.BoundGenericArgumentsNode!.BoundGenericArgumentListing.Count);

            var firstBoundClassDefinitionNode = (BoundClassDefinitionNode)boundClassDefinitionNode.BoundGenericArgumentsNode.BoundGenericArgumentListing[0];
            Assert.Equal(genericArgOne, firstBoundClassDefinitionNode.TypeClauseToken.TextSpan.GetText());

            var firstCommaToken = (CommaToken)boundClassDefinitionNode.BoundGenericArgumentsNode.BoundGenericArgumentListing[1];
            Assert.Equal(",", firstCommaToken.TextSpan.GetText());

            var secondBoundClassDefinitionNode = (BoundClassDefinitionNode)boundClassDefinitionNode.BoundGenericArgumentsNode.BoundGenericArgumentListing[2];
            Assert.Equal(genericArgTwo, secondBoundClassDefinitionNode.TypeClauseToken.TextSpan.GetText());

            var secondCommaToken = (CommaToken)boundClassDefinitionNode.BoundGenericArgumentsNode.BoundGenericArgumentListing[3];
            Assert.Equal(",", secondCommaToken.TextSpan.GetText());

            var thirdBoundClassDefinitionNode = (BoundClassDefinitionNode)boundClassDefinitionNode.BoundGenericArgumentsNode.BoundGenericArgumentListing[4];
            Assert.Equal(genericArgThree, thirdBoundClassDefinitionNode.TypeClauseToken.TextSpan.GetText());

            Assert.Empty(boundClassDefinitionNode.ClassBodyCompilationUnit.Children);
        }
    }
    
    [Fact]
    public void SHOULD_PARSE_PROPERTY_ATTRIBUTE()
    {
        var attributeIdentifier = "Parameter";
        string sourceText = @$"public partial class PersonDisplay : ComponentBase {{ [{attributeIdentifier}] public IPersonModel PersonModel {{ get; set; }} }}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException("Need to add Assertions");
        }
    }
    
    [Fact]
    public void SHOULD_PARSE_PROPERTY_WITH_TYPE_NOT_FOUND()
    {
        string sourceText = @"public class Aaa { public IPersonModel MyProperty { get; set; } }".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);
        
        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();        
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException("Need to add Assertions");
        }
    }

    /// <summary>TODO: Delete this method. I am testing something.</summary>
    [Fact]
    private void TestingSomethingTodoDeleteThis()
    {
        /*
	        {visibility} {inheritance} {storage} Aaa
	        {
	        }

	        {visibility} => [ "public", "private", "internal", "file?" ]

	        {inheritance} => [ "abstract", "virtual" ]

	        {partial} => [ "partial" ]

	        {storage} => [ "class", "struct", "record" ]
        */

        var classIdentifier = "Aaa";

        var validClassDefinitionStarts = new string[]
        {
            "public abstract partial class",
            "public abstract partial struct",
            "public virtual partial class",
            "public virtual partial struct",
            "public virtual partial record",
            "public virtual class",
            "public virtual struct",
            "public virtual record",
            "public abstract partial record",
            "public abstract class",
            "public abstract struct",
            "public abstract record",
            "private virtual partial class",
            "private virtual partial struct",
            "private virtual partial record",
            "private virtual class",
            "private virtual struct",
            "private virtual record",
            "internal virtual partial class",
            "internal virtual partial struct",
            "internal virtual partial record",
            "internal virtual class",
            "internal virtual struct",
            "internal virtual record",
            "file virtual partial class",
            "file virtual partial struct",
            "file virtual partial record",
            "file virtual class",
            "file virtual struct",
            "file virtual record",
        };

        var validClassDefinitions = new List<string>();

        foreach (var validClassDefinitionStart in validClassDefinitionStarts)
        {
            var validClassDefinition = $"{validClassDefinitionStart} {classIdentifier}{{}}";

            if (!validClassDefinitions.Contains(validClassDefinition))
            {
                validClassDefinitions.Add(validClassDefinition);
            }
        }

        var x = 2;

        throw new NotImplementedException();
    }
}
