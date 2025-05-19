using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.Tutorials.RazorLib.Pages;

public partial class Index : ComponentBase
{
	[Inject]
	private TextEditorService TextEditorService { get; set; } = null!;
	[Inject]
	private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
	[Inject]
	private IDecorationMapperRegistry DecorationMapperRegistry { get; set; } = null!;

	public static ResourceUri ResourceUri { get; } = new("/index.txt");
	public static Key<TextEditorViewModel> ViewModelKey { get; } = Key<TextEditorViewModel>.NewKey();
	public static Key<TextEditorViewModel> OtherViewModelKey { get; } = Key<TextEditorViewModel>.NewKey();
	
	private Key<TextEditorViewModel> UseViewModelKey { get; set; } = Key<TextEditorViewModel>.Empty;
	
	protected override void OnInitialized()
	{
		UseViewModelKey = OtherViewModelKey;
	
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
	        @"public class MyClass
{
	public MyClass(string firstName, string lastName)
	{
		FirstName = firstName;
		LastName = lastName;
	}
	
	public string FirstName { get; set; }
	public string LastName { get; set; }
	
	public string DisplayName => $""{{FirstName}} {LastName}"";
	
	public void SomeMethod(int arg1, MyClass arg2)
	{
		if (arg1 == 2)
			return;
		
		return;
	}
}",
	        genericDecorationMapper,
	        cSharpCompilerService,
	        TextEditorService);
	
		TextEditorService.WorkerArbitrary.PostUnique(nameof(Index), editContext =>
		{
			TextEditorService.ModelApi.RegisterCustom(editContext, model);
		
			cSharpCompilerService.RegisterResource(
				model.PersistentState.ResourceUri,
				shouldTriggerResourceWasModified: true);
			
			TextEditorService.ViewModelApi.Register(
				editContext,
				ViewModelKey,
				ResourceUri,
				new Category("main"));
				
			return ValueTask.CompletedTask;
		});
			
		base.OnInitialized();
	}
	
	private void Toggle()
	{
		if (UseViewModelKey == OtherViewModelKey)
		{
			UseViewModelKey = ViewModelKey;
		}
		else if (UseViewModelKey == ViewModelKey)
		{
			UseViewModelKey = OtherViewModelKey;
		}
	}
}