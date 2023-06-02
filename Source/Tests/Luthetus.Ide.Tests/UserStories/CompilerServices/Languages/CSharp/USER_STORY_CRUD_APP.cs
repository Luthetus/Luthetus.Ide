using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.Tests.UserStories.CompilerServices.Languages.CSharp;

/// <summary>
/// User Story Description:
/// User wants to type up a CRUD App which
/// has a variety of files such as: "PersonModel.cs" and
/// "PersonRepository.cs". A semantic model of the .NET Solution should be
/// created, which includes a semantic model foreach of the C# Projects,
/// then the projects have semantic models for all their source files.
/// </summary>
public class USER_STORY_CRUD_APP
{
    [Fact]
    public void Enact()
    {
        string sourceText = @"namespace BlazorWasmApp.PersonCase;

public class PersonModel
{
	public PersonModel()
	{
	}

	public string FirstName { get; set; }
	public string LastName { get; set; }

	public string DisplayName => $""{FirstName} + {LastName}"";
}
".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri("PersonModel.cs");

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        throw new NotImplementedException("TODO: Perform assertions");
    }
}