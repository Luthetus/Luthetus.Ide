using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.CompilerServices.Lang.CSharp.BinderCase;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis.ParserCase;

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
            var parser = new CSharpParser(lexer);

            // Parse, then assert there were NO diagnostics.
            // This presumes that the sourceText variable is valid C#
            parser.Parse();
            Assert.Empty(parser.DiagnosticsList);
        }
        
        // With diagnostics
        {
            var resourceUri = new ResourceUri("./unitTesting.txt");
            var sourceText = "MyMethod()"; // MyMethod should be undefined
            var lexer = new CSharpLexer(resourceUri, sourceText);
            var parser = new CSharpParser(lexer);

            // Parse, then assert there were diagnostics.
            // This presumes that the sourceText variable is NOT-valid C#
            parser.Parse();
            Assert.NotEmpty(parser.DiagnosticsList);
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
        throw new NotImplementedException();
    }
}