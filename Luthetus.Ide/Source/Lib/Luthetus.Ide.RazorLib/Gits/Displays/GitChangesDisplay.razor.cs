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

    private static readonly ResourceUri InResourceUri = new(nameof(GitChangesDisplay) + "_in");
    private static readonly ResourceUri OutResourceUri = new(nameof(GitChangesDisplay) + "_out");

    private static readonly Key<TextEditorViewModel> InViewModelKey = Key<TextEditorViewModel>.NewKey();
    private static readonly Key<TextEditorViewModel> OutViewModelKey = Key<TextEditorViewModel>.NewKey();

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // "In" Registrations
            {
                TextEditorService.ModelApi.RegisterTemplated(
                    ExtensionNoPeriodFacts.TXT,
                    InResourceUri,
                    DateTime.UtcNow,
                    "ABCDEFK",
                    "Before");

                TextEditorService.ModelApi.RegisterPresentationModel(
                    InResourceUri,
                    DiffPresentationFacts.EmptyInPresentationModel);
                
                TextEditorService.ModelApi.RegisterPresentationModel(
                    InResourceUri,
                    DiffPresentationFacts.EmptyOutPresentationModel);

                TextEditorService.ViewModelApi.Register(
                    InViewModelKey,
                    InResourceUri);

                var presentationKeys = new[]
                {
                    DiffPresentationFacts.InPresentationKey,
                };

                TextEditorService.EnqueueEdit(
                    TextEditorService.ViewModelApi.GetWithValueTask(
                        InViewModelKey,
                        textEditorViewModel => textEditorViewModel with
                        {
                            FirstPresentationLayerKeysBag = presentationKeys.ToImmutableList()
                        }));
            }
            
            // "Out" Registrations
            {
                TextEditorService.ModelApi.RegisterTemplated(
                    ExtensionNoPeriodFacts.TXT,
                    OutResourceUri,
                    DateTime.UtcNow,
                    "BHDEFCK",
                    "After");

                TextEditorService.ModelApi.RegisterPresentationModel(
                    OutResourceUri,
                    DiffPresentationFacts.EmptyInPresentationModel);

                TextEditorService.ModelApi.RegisterPresentationModel(
                    OutResourceUri,
                    DiffPresentationFacts.EmptyOutPresentationModel);

                TextEditorService.ViewModelApi.Register(
                    OutViewModelKey,
                    OutResourceUri);

                var presentationKeys = new[]
                {
                    DiffPresentationFacts.OutPresentationKey,
                };

                TextEditorService.EnqueueEdit(
                    TextEditorService.ViewModelApi.GetWithValueTask(
                        OutViewModelKey,
                        textEditorViewModel => textEditorViewModel with
                        {
                            FirstPresentationLayerKeysBag = presentationKeys.ToImmutableList()
                        }));
            }

            TextEditorService.DiffApi.Register(
                DiffModelKey,
                InViewModelKey,
                OutViewModelKey);
        }

        return base.OnAfterRenderAsync(firstRender);
    }
}