using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.TextEditor.Usage.RazorLib.Pages;

public partial class Index : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private static readonly ResourceUri TextEditorResourceUri = new ResourceUri("/index");
    private static readonly Key<TextEditorViewModel> TextEditorViewModelKey = Key<TextEditorViewModel>.NewKey();

    private CSharpCompilerService _cSharpCompilerService = null!;

    protected override void OnInitialized()
    {
        _cSharpCompilerService = new CSharpCompilerService(
            TextEditorService, BackgroundTaskService, Dispatcher);

        var textEditorModel = new TextEditorModel(
            TextEditorResourceUri,
            DateTime.UtcNow,
            ".cs",
            "public class MyClass\n{\n\n}\n",
            new GenericDecorationMapper(),
            _cSharpCompilerService,
            null,
            new());

        TextEditorService.Model.RegisterCustom(textEditorModel);

        _cSharpCompilerService.RegisterResource(textEditorModel.ResourceUri);

        TextEditorService.ViewModel.Register(
            TextEditorViewModelKey,
            TextEditorResourceUri);

        base.OnInitialized();
    }
}