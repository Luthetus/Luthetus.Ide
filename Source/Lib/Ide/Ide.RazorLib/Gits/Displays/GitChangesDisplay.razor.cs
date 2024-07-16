using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;

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

    protected override async Task OnAfterRenderAsync(bool firstRender)
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

                TextEditorService.ViewModelApi.Register(
                    InViewModelKey,
                    InResourceUri,
                    new Category(nameof(GitChangesDisplay)));

                TextEditorService.PostDistinct(
                    nameof(TextEditorService.ModelApi.AddPresentationModelFactory),
                    async editContext =>
                    {
                        await TextEditorService.ModelApi.AddPresentationModelFactory(
                                InResourceUri,
                                DiffPresentationFacts.EmptyInPresentationModel)
                            .Invoke(editContext)
                            .ConfigureAwait(false);

                        await TextEditorService.ModelApi.AddPresentationModelFactory(
                                InResourceUri,
                                DiffPresentationFacts.EmptyOutPresentationModel)
                            .Invoke(editContext)
                            .ConfigureAwait(false);

                        var viewModelModifier = editContext.GetViewModelModifier(InViewModelKey);

                        if (viewModelModifier is null)
                            return;

                        var presentationKeys = new[]
                        {
                            DiffPresentationFacts.InPresentationKey,
                        };

                        viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                        {
                            FirstPresentationLayerKeysList = presentationKeys.ToImmutableList()
                        };
                    });
            }
            
            // "Out" Registrations
            {
                TextEditorService.ModelApi.RegisterTemplated(
                    ExtensionNoPeriodFacts.TXT,
                    OutResourceUri,
                    DateTime.UtcNow,
                    "BHDEFCK",
                    "After");

                TextEditorService.ViewModelApi.Register(
                    OutViewModelKey,
                    OutResourceUri,
                    new Category(nameof(GitChangesDisplay)));

                TextEditorService.PostDistinct(
                    nameof(TextEditorService.ModelApi.AddPresentationModelFactory),
                    async editContext =>
                    {
                        await TextEditorService.ModelApi.AddPresentationModelFactory(
                                OutResourceUri,
                                DiffPresentationFacts.EmptyInPresentationModel)
                            .Invoke(editContext)
                            .ConfigureAwait(false);

                        await TextEditorService.ModelApi.AddPresentationModelFactory(
                                OutResourceUri,
                                DiffPresentationFacts.EmptyOutPresentationModel)
                            .Invoke(editContext)
                            .ConfigureAwait(false);

                        var viewModelModifier = editContext.GetViewModelModifier(OutViewModelKey);

                        if (viewModelModifier is null)
                            return;

                        var presentationKeys = new[]
                        {
                            DiffPresentationFacts.OutPresentationKey,
                        };

                        viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                        {
                            FirstPresentationLayerKeysList = presentationKeys.ToImmutableList()
                        };
                    });
            }

            TextEditorService.DiffApi.Register(
                DiffModelKey,
                InViewModelKey,
                OutViewModelKey);
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}