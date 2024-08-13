using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.Tutorials.RazorLib.Pages;

public partial class Index : ComponentBase
{
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;

	public static ResourceUri ResourceUri { get; } = new("/index.txt");
	public static Key<TextEditorViewModel> ViewModelKey { get; } = Key<TextEditorViewModel>.NewKey();
	
	protected override void OnInitialized()
	{
		var existingModel = TextEditorService.ModelApi.GetOrDefault(ResourceUri);
		if (existingModel is not null)
			return;
	
		var model = new TextEditorModel(
	        ResourceUri,
	        DateTime.UtcNow,
	        ExtensionNoPeriodFacts.TXT,
	        "abc123",
	        decorationMapper: null,
	        compilerService: null);
	
		TextEditorService.ModelApi.RegisterCustom(model);
		
		TextEditorService.ViewModelApi.Register(
			ViewModelKey,
			ResourceUri,
			new Category("main"));
	}
}