using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.Models;
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
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

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

                TextEditorService.PostUnique(
                    nameof(TextEditorService.ModelApi.AddPresentationModel),
                    editContext =>
                    {
                    	var modelModifier = editContext.GetModelModifier(InResourceUri);
                    
                        TextEditorService.ModelApi.AddPresentationModel(
                        	editContext,
                            modelModifier,
                            DiffPresentationFacts.EmptyInPresentationModel);

                        TextEditorService.ModelApi.AddPresentationModel(
                        	editContext,
                            modelModifier,
                            DiffPresentationFacts.EmptyOutPresentationModel);

                        var viewModelModifier = editContext.GetViewModelModifier(InViewModelKey);

                        if (viewModelModifier is null)
                            return Task.CompletedTask;

                        var presentationKeys = new[]
                        {
                            DiffPresentationFacts.InPresentationKey,
                        };

                        viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                        {
                            FirstPresentationLayerKeysList = presentationKeys.ToImmutableList()
                        };
                        
                        return Task.CompletedTask;
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

                TextEditorService.PostUnique(
                    nameof(TextEditorService.ModelApi.AddPresentationModel),
                    editContext =>
                    {
                    	var modelModifier = editContext.GetModelModifier(OutResourceUri);
                    
                        TextEditorService.ModelApi.AddPresentationModel(
                    		editContext,
                            modelModifier,
                            DiffPresentationFacts.EmptyInPresentationModel);

                        TextEditorService.ModelApi.AddPresentationModel(
                    		editContext,
                            modelModifier,
                            DiffPresentationFacts.EmptyOutPresentationModel);

                        var viewModelModifier = editContext.GetViewModelModifier(OutViewModelKey);

                        if (viewModelModifier is null)
                            return Task.CompletedTask;

                        var presentationKeys = new[]
                        {
                            DiffPresentationFacts.OutPresentationKey,
                        };

                        viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                        {
                            FirstPresentationLayerKeysList = presentationKeys.ToImmutableList()
                        };

                        return Task.CompletedTask;
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