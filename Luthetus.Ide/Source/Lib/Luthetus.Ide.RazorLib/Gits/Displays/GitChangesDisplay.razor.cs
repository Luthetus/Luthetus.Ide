using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

public partial class GitChangesDisplay : ComponentBase, IGitDisplayRendererType
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private static readonly Key<TextEditorDiffModel> DiffModelKey = Key<TextEditorDiffModel>.NewKey();

    private static readonly ResourceUri BeforeResourceUri = new(nameof(GitChangesDisplay) + "_in");
    private static readonly ResourceUri AfterResourceUri = new(nameof(GitChangesDisplay) + "_out");

    private static readonly Key<TextEditorViewModel> BeforeViewModelKey = Key<TextEditorViewModel>.NewKey();
    private static readonly Key<TextEditorViewModel> AfterViewModelKey = Key<TextEditorViewModel>.NewKey();

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // "In" Registrations
            {
                TextEditorService.ModelApi.RegisterTemplated(
                    ExtensionNoPeriodFacts.TXT,
                    BeforeResourceUri,
                    DateTime.UtcNow,
                    "ABCDEFK",
                    "Before");

                TextEditorService.ModelApi.RegisterPresentationModel(
                    BeforeResourceUri,
                    DiffPresentationFacts.EmptyInPresentationModel);
                
                TextEditorService.ModelApi.RegisterPresentationModel(
                    BeforeResourceUri,
                    DiffPresentationFacts.EmptyOutPresentationModel);

                TextEditorService.ViewModelApi.Register(
                    BeforeViewModelKey,
                    BeforeResourceUri);

                var presentationKeys = new[]
                {
                    DiffPresentationFacts.InPresentationKey,
                };

                TextEditorService.ViewModelApi.WithValueEnqueue(
                    BeforeViewModelKey,
                    textEditorViewModel => textEditorViewModel with
                    {
                        FirstPresentationLayerKeysBag = presentationKeys.ToImmutableList()
                    });
            }
            
            // "Out" Registrations
            {
                TextEditorService.ModelApi.RegisterTemplated(
                    ExtensionNoPeriodFacts.TXT,
                    AfterResourceUri,
                    DateTime.UtcNow,
                    "BHDEFCK",
                    "After");

                TextEditorService.ModelApi.RegisterPresentationModel(
                    AfterResourceUri,
                    DiffPresentationFacts.EmptyInPresentationModel);

                TextEditorService.ModelApi.RegisterPresentationModel(
                    AfterResourceUri,
                    DiffPresentationFacts.EmptyOutPresentationModel);

                TextEditorService.ViewModelApi.Register(
                    AfterViewModelKey,
                    AfterResourceUri);

                var presentationKeys = new[]
                {
                    DiffPresentationFacts.OutPresentationKey,
                };

                TextEditorService.ViewModelApi.WithValueEnqueue(
                    AfterViewModelKey,
                    textEditorViewModel => textEditorViewModel with
                    {
                        FirstPresentationLayerKeysBag = presentationKeys.ToImmutableList()
                    });
            }

            TextEditorService.DiffApi.Register(
                DiffModelKey,
                BeforeViewModelKey,
                AfterViewModelKey);
        }

        return base.OnAfterRenderAsync(firstRender);
    }
}