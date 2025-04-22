using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.Tutorials.RazorLib.Pages;

public partial class Group : ComponentBase
{
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;
	[Inject]
	private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
	[Inject]
	private IDecorationMapperRegistry DecorationMapperRegistry { get; set; } = null!;

	public static ResourceUri ResourceUri { get; } = new("/group.txt");
	public static Key<TextEditorViewModel> ViewModelKey { get; } = Key<TextEditorViewModel>.NewKey();
	public static Key<TextEditorGroup> GroupKey { get; } = Key<TextEditorGroup>.NewKey();
	
	protected override void OnInitialized()
	{
		var existingModel = TextEditorService.ModelApi.GetOrDefault(ResourceUri);
		if (existingModel is not null)
			return;

		var genericDecorationMapper = DecorationMapperRegistry.GetDecorationMapper(
			ExtensionNoPeriodFacts.C_SHARP_CLASS);
			
		var cSharpCompilerService = CompilerServiceRegistry.GetCompilerService(
			ExtensionNoPeriodFacts.C_SHARP_CLASS);
			
		var model = new TextEditorModel(
	        ResourceUri,
	        DateTime.UtcNow,
	        ExtensionNoPeriodFacts.TXT,
	        @"namespace Luthetus.TextEditor.RazorLib.Groups.Models;

/// <summary>
/// Store the state of none or many tabs, and which tab is the active one.
/// Each tab represents a <see cref=""TextEditorViewModel""/>.
/// </summary>
public record TextEditorGroup(/*...*/) : ITabGroup
{
    public Key<RenderState> RenderStateKey { get; init; } = Key<RenderState>.NewKey();

    public bool GetIsActive(ITab tab)
    {
        /*...*/
    }

    public Task OnClickAsync(ITab tab, MouseEventArgs mouseEventArgs)
    {
        /*...*/
    }

    public string GetDynamicCss(ITab tab)
    {
        /*...*/
    }

    public Task CloseAsync(ITab tab)
    {
        /*...*/
    }

    public async Task CloseAllAsync()
    {
        /*...*/
    }

	public async Task CloseOthersAsync(ITab safeTab)
    {
        /*...*/
    }
}
",
	        genericDecorationMapper,
	        cSharpCompilerService);
	
		TextEditorService.WorkerArbitrary.PostUnique(nameof(Index), editContext =>
		{
			TextEditorService.ModelApi.RegisterCustom(editContext, model);
		
			cSharpCompilerService.RegisterResource(
				model.ResourceUri,
				shouldTriggerResourceWasModified: true);
			
			TextEditorService.ViewModelApi.Register(
				editContext,
				ViewModelKey,
				ResourceUri,
				new Category("main"));
			
			TextEditorService.GroupApi.Register(GroupKey, category: new("main"));
			TextEditorService.GroupApi.AddViewModel(GroupKey, Group.ViewModelKey);
			TextEditorService.GroupApi.AddViewModel(GroupKey, Index.ViewModelKey);
			
			// TextEditorService.GroupApi.SetActiveViewModel(GroupKey, Group.ViewModelKey);
		
			return ValueTask.CompletedTask;
		});
			
		base.OnInitialized();
	}
}