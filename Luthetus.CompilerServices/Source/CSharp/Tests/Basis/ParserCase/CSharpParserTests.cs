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
    /// <see cref="CSharpParser.DiagnosticsList"/>
    /// </summary>
    [Fact]
    public void DiagnosticsList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="CSharpParser.Binder"/>
    /// </summary>
    [Fact]
    public void Binder()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="CSharpParser.Lexer"/>
    /// </summary>
    [Fact]
    public void Lexer()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="CSharpParser.Parse()"/>
    /// </summary>
    [Fact]
    public void Parse_A()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="CSharpParser.Parse(CSharpBinder, ResourceUri)"/>
    /// </summary>
    [Fact]
    public void Parse_B()
    {
        throw new NotImplementedException();
    }
}