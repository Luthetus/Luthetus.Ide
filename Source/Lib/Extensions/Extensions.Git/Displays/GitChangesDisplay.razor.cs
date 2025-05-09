using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.Extensions.Git.Displays;

public partial class GitChangesDisplay : ComponentBase, IGitDisplayRendererType
{
    [Inject]
    private TextEditorService TextEditorService { get; set; } = null!;
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
        	TextEditorService.WorkerArbitrary.PostUnique(nameof(GitChangesDisplay), editContext =>
        	{
        		// "In" Registrations
	            {
	                TextEditorService.ModelApi.RegisterTemplated(
	                	editContext,
	                    ExtensionNoPeriodFacts.TXT,
	                    InResourceUri,
	                    DateTime.UtcNow,
	                    "ABCDEFK",
	                    "Before");
	
	                TextEditorService.ViewModelApi.Register(
	                	editContext,
	                    InViewModelKey,
	                    InResourceUri,
	                    new Category(nameof(GitChangesDisplay)));
	
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
                        return ValueTask.CompletedTask;

                    var presentationKeys = new List<Key<TextEditorPresentationModel>>
                    {
                        DiffPresentationFacts.InPresentationKey,
                    };

                    viewModelModifier.FirstPresentationLayerKeysList = presentationKeys;
	            }
	            
	            // "Out" Registrations
	            {
	                TextEditorService.ModelApi.RegisterTemplated(
	                	editContext,
	                    ExtensionNoPeriodFacts.TXT,
	                    OutResourceUri,
	                    DateTime.UtcNow,
	                    "BHDEFCK",
	                    "After");
	
	                TextEditorService.ViewModelApi.Register(
	                	editContext,
	                    OutViewModelKey,
	                    OutResourceUri,
	                    new Category(nameof(GitChangesDisplay)));
	
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
                        return ValueTask.CompletedTask;

                    var presentationKeys = new List<Key<TextEditorPresentationModel>>()
                    {
                        DiffPresentationFacts.OutPresentationKey,
                    };

                    viewModelModifier.FirstPresentationLayerKeysList = presentationKeys;
	            }
	
	            TextEditorService.DiffApi.Register(
	                DiffModelKey,
	                InViewModelKey,
	                OutViewModelKey);
        	
        		return ValueTask.CompletedTask;
        	});
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}