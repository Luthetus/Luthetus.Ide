using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis.ParserCase.Internals;

public class ParseTypesTests
{
    /*
     # TypeDefinition
         foreach(AccessModifierKind.ToLower()) foreach(StorageModifierKind.ToLower()) MyClass { }
     */

    [Fact]
    public void TypeDefinition_WITH_PublicAccess_AND_ClassStorage()
    {
        var accessModifierKindList = Enum.GetValues<AccessModifierKind>();
        foreach (var accessModifierKind in accessModifierKindList)
        {
            var storageModifierKindList = Enum.GetValues<StorageModifierKind>();
            foreach (var storageModifierKind in storageModifierKindList)
            {
                var accessModifierString = accessModifierKind.ToString().ToLower();
                var storageModifierString = storageModifierKind.ToString().ToLower();

                if (accessModifierString == "protectedinternal")
                    accessModifierString = "protected internal";

                if (accessModifierString == "privateprotected")
                    accessModifierString = "private protected";

                var resourceUri = new ResourceUri("UnitTests");
                var sourceText = $"{accessModifierString} {storageModifierString} MyClass {{ }}";
                var lexer = new CSharpLexer(resourceUri, sourceText);
                lexer.Lex();
                var parser = new CSharpParser(lexer);
                var compilationUnit = parser.Parse();
                var topCodeBlock = compilationUnit.RootCodeBlockNode;

                Assert.Single(topCodeBlock.ChildList);
                var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();

                Assert.Equal(accessModifierKind, typeDefinitionNode.AccessModifierKind);
                Assert.Equal(storageModifierKind, typeDefinitionNode.StorageModifierKind);
                Assert.Equal("MyClass", typeDefinitionNode.TypeIdentifier.TextSpan.GetText());
                Assert.Null(typeDefinitionNode.ValueType);
                Assert.Null(typeDefinitionNode.GenericArgumentsListingNode);
                Assert.Null(typeDefinitionNode.InheritedTypeClauseNode);

                // TypeBodyCodeBlockNode
                {
                    var typeBodyCodeBlockNode = typeDefinitionNode.TypeBodyCodeBlockNode;

                    Assert.NotNull(typeBodyCodeBlockNode);
                    Assert.Empty(typeBodyCodeBlockNode.DiagnosticsList);
                    Assert.Empty(typeBodyCodeBlockNode.ChildList);
                    Assert.False(typeBodyCodeBlockNode.IsFabricated);
                }

                Assert.Empty(compilationUnit.DiagnosticsList);
            }
        }
    }

    [Fact]
    public void TypeDefinition_WITH_PartialKeyword()
    {
        var accessModifierKindList = Enum.GetValues<AccessModifierKind>();
        foreach (var accessModifierKind in accessModifierKindList)
        {
            var storageModifierKindList = Enum.GetValues<StorageModifierKind>();
            foreach (var storageModifierKind in storageModifierKindList)
            {
                var accessModifierString = accessModifierKind.ToString().ToLower();
                var storageModifierString = storageModifierKind.ToString().ToLower();

                if (accessModifierString == "protectedinternal")
                    accessModifierString = "protected internal";

                if (accessModifierString == "privateprotected")
                    accessModifierString = "private protected";

                var resourceUri = new ResourceUri("UnitTests");
                var sourceText = $"{accessModifierString} partial {storageModifierString} MyClass {{ }}";
                var lexer = new CSharpLexer(resourceUri, sourceText);
                lexer.Lex();
                var parser = new CSharpParser(lexer);
                var compilationUnit = parser.Parse();
                var topCodeBlock = compilationUnit.RootCodeBlockNode;

                Assert.Single(topCodeBlock.ChildList);
                var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();

                Assert.Equal(accessModifierKind, typeDefinitionNode.AccessModifierKind);
                Assert.Equal(storageModifierKind, typeDefinitionNode.StorageModifierKind);
                Assert.Equal("MyClass", typeDefinitionNode.TypeIdentifier.TextSpan.GetText());
                Assert.Null(typeDefinitionNode.ValueType);
                Assert.Null(typeDefinitionNode.GenericArgumentsListingNode);
                Assert.Null(typeDefinitionNode.InheritedTypeClauseNode);

                // TypeBodyCodeBlockNode
                {
                    var typeBodyCodeBlockNode = typeDefinitionNode.TypeBodyCodeBlockNode;

                    Assert.NotNull(typeBodyCodeBlockNode);
                    Assert.Empty(typeBodyCodeBlockNode.DiagnosticsList);
                    Assert.Empty(typeBodyCodeBlockNode.ChildList);
                    Assert.False(typeBodyCodeBlockNode.IsFabricated);
                }

                Assert.Empty(compilationUnit.DiagnosticsList);
            }
        }
    }
}
