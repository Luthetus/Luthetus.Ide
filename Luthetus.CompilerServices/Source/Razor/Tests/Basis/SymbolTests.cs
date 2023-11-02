using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.Razor.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.Razor.Tests.Misc;
using Luthetus.CompilerServices.Lang.Xml.Html.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.CompilerServices.Lang.Razor.Tests.Basics;

public class SymbolTests : RazorCompilerServiceTestsBase
{
    [Fact]
    public void SHOULD_RECOGNIZE_BLAZOR_COMPONENT()
    {
        CreateBlazorComponent();

        var text = "<PersonSimpleDisplay/>";

        var resourceUri = new ResourceUri("TestFile.razor");

        var textEditorModel = new TextEditorModel(
            resourceUri,
            DateTime.UtcNow,
            ".razor",
            text,
            new TextEditorHtmlDecorationMapper(),
            RazorCompilerService,
            null,
            new());

        TextEditorService.Model.RegisterCustom(textEditorModel);
        RazorCompilerService.RegisterResource(textEditorModel.ResourceUri);

        var compilerServiceResource = RazorCompilerService.GetCompilerServiceResourceFor(textEditorModel.ResourceUri);
        Assert.NotNull(compilerServiceResource);

        var razorResource = (RazorResource)compilerServiceResource;
    }

    private void CreateBlazorComponent()
    {
        var text = @"using Microsoft.AspNetCore.Components;

namespace ConsoleApp1.Today;

public partial class PersonSimpleDisplay : ComponentBase
{
	
}";

        var resourceUri = new ResourceUri("TestFile.cs");

        var textEditorModel = new TextEditorModel(
            resourceUri,
            DateTime.UtcNow,
            ".cs",
            text,
            new GenericDecorationMapper(),
            CSharpCompilerService,
            null,
            new());

        TextEditorService.Model.RegisterCustom(textEditorModel);
        CSharpCompilerService.RegisterResource(textEditorModel.ResourceUri);
    }
}
