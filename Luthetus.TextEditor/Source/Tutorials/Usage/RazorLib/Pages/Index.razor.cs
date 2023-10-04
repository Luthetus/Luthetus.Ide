using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.CompilerServiceCase.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Model;
using Luthetus.TextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.Usage.RazorLib.Pages;

public partial class Index : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private CSharpCompilerService CSharpCompilerService { get; set; } = null!;

    private static readonly TextEditorModelKey IndexTextEditorModelKey = TextEditorModelKey.NewTextEditorModelKey();
    private static readonly TextEditorViewModelKey IndexTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();

    protected override void OnInitialized()
    {
        var textEditorModel = new TextEditorModel(
            new ResourceUri("uniqueIdentifierGoesHere.cs"),
            DateTime.UtcNow,
            ".cs",
            "public class MyClass\n{\n\n}\n",
            CSharpCompilerService,
            new GenericDecorationMapper(),
            null,
            new(),
            IndexTextEditorModelKey);

        TextEditorService.Model.RegisterCustom(textEditorModel);

        CSharpCompilerService.RegisterModel(textEditorModel);

        TextEditorService.ViewModel.Register(
            IndexTextEditorViewModelKey,
            IndexTextEditorModelKey);

        base.OnInitialized();
    }
}