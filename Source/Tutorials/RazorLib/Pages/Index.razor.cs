using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.Tutorials.RazorLib.Pages;

public partial class Index : ComponentBase
{
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;
	[Inject]
	private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
	[Inject]
	private IDecorationMapperRegistry DecorationMapperRegistry { get; set; } = null!;

	public static ResourceUri ResourceUri { get; } = new("/index.txt");
	public static Key<TextEditorViewModel> ViewModelKey { get; } = Key<TextEditorViewModel>.NewKey();
	
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
	        cSharpCompilerService);
	
		TextEditorService.ModelApi.RegisterCustom(model);
		
		cSharpCompilerService.RegisterResource(
			model.ResourceUri,
			shouldTriggerResourceWasModified: true);
		
		TextEditorService.ViewModelApi.Register(
			ViewModelKey,
			ResourceUri,
			new Category("main"));
	}
}