using BlazorCommon.RazorLib.Misc;
using BlazorStudio.ClassLib.DotNet.CSharp;
using BlazorStudio.ClassLib.FileSystem.Classes.FilePath;
using BlazorStudio.ClassLib.FileSystem.Classes.Local;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxActors;

namespace Luthetus.Ide.Tests.Basics.DotNet;

public class CSharpProjectParserTests
{
    [Fact]
    public void AdhocTest()
    {
        var localEnvironmentProvider = new LocalEnvironmentProvider();

        var projectAbsoluteFilePathString = @"C:\Users\hunte\Repos\Demos\BlazorCrudApp\BlazorCrudApp\BlazorCrudApp.csproj";

        var projectAbsoluteFilePath = new AbsoluteFilePath(
            projectAbsoluteFilePathString,
            false,
            localEnvironmentProvider);

        var htmlLexer = new TextEditorHtmlLexer();

        var textSpans = htmlLexer.Lex(PROJECT_TEST_DATA, RenderStateKey.NewRenderStateKey());

        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(PROJECT_TEST_DATA);

        var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;

        var cSharpProjectSyntaxWalker = new CSharpProjectSyntaxWalker();

        cSharpProjectSyntaxWalker.Visit(syntaxNodeRoot);

        var packageReferences = cSharpProjectSyntaxWalker.TagSyntaxes
            .Where(ts => (ts.OpenTagNameSyntax?.Value ?? string.Empty) == "PackageReference")
            .ToList();

        var attributeNameValueTuples = packageReferences
            .SelectMany(x => x.AttributeSyntaxes)
            .Select(x => (
                $"Name: {x.AttributeNameSyntax.TextEditorTextSpan.GetText(PROJECT_TEST_DATA)}",
                $"Value: {x.AttributeValueSyntax.TextEditorTextSpan.GetText(PROJECT_TEST_DATA)}"))
            .ToArray();

        var z = 2;
    }

    [Fact]
    public void GetReferencedNugetPackages()
    {
        var localEnvironmentProvider = new LocalEnvironmentProvider();

        var projectAbsoluteFilePathString = @"C:\Users\hunte\Repos\Demos\BlazorCrudApp\BlazorCrudApp\BlazorCrudApp.csproj";

        var projectAbsoluteFilePath = new AbsoluteFilePath(
            projectAbsoluteFilePathString,
            false,
            localEnvironmentProvider);

        var htmlLexer = new TextEditorHtmlLexer();

        var textSpans = htmlLexer.Lex(PROJECT_TEST_DATA, RenderStateKey.NewRenderStateKey());

        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(PROJECT_TEST_DATA);

        var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;

        var cSharpProjectSyntaxWalker = new CSharpProjectSyntaxWalker();

        cSharpProjectSyntaxWalker.Visit(syntaxNodeRoot);

        var packageReferences = cSharpProjectSyntaxWalker.TagSyntaxes
            .Where(ts => (ts.OpenTagNameSyntax?.Value ?? string.Empty) == "PackageReference")
            .ToList();

        var attributeNameValueTuples = packageReferences
            .SelectMany(x => x.AttributeSyntaxes)
            .Select(x => (
                $"Name: {x.AttributeNameSyntax.TextEditorTextSpan.GetText(PROJECT_TEST_DATA)}",
                $"Value: {x.AttributeValueSyntax.TextEditorTextSpan.GetText(PROJECT_TEST_DATA)}"))
            .ToArray();

        var z = 2;
    }

    private const string PROJECT_TEST_DATA = @"<Project Sdk=""Microsoft.NET.Sdk.Web"">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

	<ItemGroup>
		<PackageReference Include=""Fluxor.Blazor.Web"" Version=""5.7.0"" />
	</ItemGroup>
</Project>
";
}