using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.CSharp.BinderCase;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase;

/// <summary>
/// <see cref="CSharpParser"/>
/// </summary>
public class CSharpParserTests
{
    /// <summary>
    /// <see cref="CSharpParser(CSharpLexer)"/>
    /// <br/>----<br/>
    /// <see cref="CSharpParser.Lexer"/>
    /// <see cref="CSharpParser.Binder"/>
    /// /// <see cref="CSharpParser.DiagnosticsList"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = "public class MyClass { }";
        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);

        Assert.Equal(ImmutableArray<TextEditorDiagnostic>.Empty, parser.DiagnosticsList);
        Assert.Equal(lexer, parser.Lexer);
        Assert.NotNull(parser.Binder);
    }

    /// <summary>
    /// <see cref="CSharpParser.Parse()"/>
    /// <br/>----<br/>
    /// <see cref="CSharpParser.DiagnosticsList"/>
    /// </summary>
    [Fact]
    public void Parse_A()
    {
        // No diagnostics
        {
            var resourceUri = new ResourceUri("./unitTesting.txt");
            var sourceText = "public class MyClass { }";
            var lexer = new CSharpLexer(resourceUri, sourceText);
            lexer.Lex();
            var parser = new CSharpParser(lexer);

            // Parse, then assert there were NO diagnostics.
            // This presumes that the sourceText variable is valid C#
            var compilationUnit = parser.Parse();
            Assert.Empty(compilationUnit.DiagnosticsList);
        }
        
        // With diagnostics
        {
            var resourceUri = new ResourceUri("./unitTesting.txt");
            var sourceText = "MyMethod()"; // MyMethod should be undefined
            var lexer = new CSharpLexer(resourceUri, sourceText);
            lexer.Lex();
            var parser = new CSharpParser(lexer);

            // Parse, then assert there were diagnostics.
            // This presumes that the sourceText variable is NOT-valid C#
            var compilationUnit = parser.Parse();
            Assert.NotEmpty(compilationUnit.DiagnosticsList);
        }
    }

    /// <summary>
    /// <see cref="CSharpParser.Parse(CSharpBinder, ResourceUri)"/>
    /// <br/>----<br/>
    /// <see cref="CSharpParser.Binder"/>
    /// <see cref="CSharpParser.DiagnosticsList"/>
    /// </summary>
    [Fact]
    public void Parse_B()
    {
        // No diagnostics
        {
            var binder = new CSharpBinder();

            // Define 'MyMethod()' at the global scope.
            {
                var resourceUri = new ResourceUri("./unitTesting.txt");
                var sourceText = "public void MyMethod() { }";
                var lexer = new CSharpLexer(resourceUri, sourceText);
                lexer.Lex();
                var parser = new CSharpParser(lexer);

                var compilationUnit = parser.Parse(binder, resourceUri);
                Assert.Empty(compilationUnit.DiagnosticsList);
            }

            // Invoke 'MyMethod()' from from the global scope definition.
            {
                var resourceUri = new ResourceUri("./unitTesting.txt");
                var sourceText = "MyMethod()";
                var lexer = new CSharpLexer(resourceUri, sourceText);
                lexer.Lex();
                var parser = new CSharpParser(lexer);

                var compilationUnit = parser.Parse(binder, resourceUri);
                Assert.Empty(compilationUnit.DiagnosticsList);
            }
        }

        // With diagnostics
        {
            var binder = new CSharpBinder();

            // Define 'SomeOtherMethod()' at the global scope.
            {
                var resourceUri = new ResourceUri("./unitTesting.txt");
                var sourceText = "public void SomeOtherMethod() { }";
                var lexer = new CSharpLexer(resourceUri, sourceText);
                lexer.Lex();
                var parser = new CSharpParser(lexer);

                var compilationUnit = parser.Parse(binder, resourceUri);
                Assert.Empty(compilationUnit.DiagnosticsList);
            }

            // Invoke 'MyMethod()', which is undefined.
            {
                var resourceUri = new ResourceUri("./unitTesting.txt");
                var sourceText = "MyMethod()";
                var lexer = new CSharpLexer(resourceUri, sourceText);
                lexer.Lex();
                var parser = new CSharpParser(lexer);

                var compilationUnit = parser.Parse(binder, resourceUri);
                Assert.NotEmpty(compilationUnit.DiagnosticsList);
            }
        }
    }
}