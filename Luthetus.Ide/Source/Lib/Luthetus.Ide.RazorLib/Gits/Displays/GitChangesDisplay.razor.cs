using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

public partial class GitChangesDisplay : ComponentBase, IGitDisplayRendererType
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
    [Inject]
    private IDecorationMapperRegistry DecorationMapperRegistry { get; set; } = null!;

    private static readonly Key<TextEditorDiffModel> DiffModelKey = Key<TextEditorDiffModel>.NewKey();

    private static readonly ResourceUri BeforeResourceUri = new(nameof(GitChangesDisplay) + "_before");
    private static readonly ResourceUri AfterResourceUri = new(nameof(GitChangesDisplay) + "_after");

    private static readonly Key<TextEditorViewModel> BeforeViewModelKey = Key<TextEditorViewModel>.NewKey();
    private static readonly Key<TextEditorViewModel> AfterViewModelKey = Key<TextEditorViewModel>.NewKey();

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // "Before" Registrations
            {
                TextEditorService.Model.RegisterTemplated(
                    DecorationMapperRegistry,
                    CompilerServiceRegistry,
                    ExtensionNoPeriodFacts.TXT,
                    BeforeResourceUri,
                    DateTime.UtcNow,
                    string.Empty,
                    "Before");

                TextEditorService.ViewModel.Register(
                    BeforeViewModelKey,
                    BeforeResourceUri);
            }
            
            // "After" Registrations
            {
                TextEditorService.Model.RegisterTemplated(
                    DecorationMapperRegistry,
                    CompilerServiceRegistry,
                    ExtensionNoPeriodFacts.TXT,
                    AfterResourceUri,
                    DateTime.UtcNow,
                    string.Empty,
                    "After");

                TextEditorService.ViewModel.Register(
                    AfterViewModelKey,
                    AfterResourceUri);
            }

            TextEditorService.Diff.Register(
                DiffModelKey,
                BeforeViewModelKey,
                AfterViewModelKey);
        }

        return base.OnAfterRenderAsync(firstRender);
    }
}