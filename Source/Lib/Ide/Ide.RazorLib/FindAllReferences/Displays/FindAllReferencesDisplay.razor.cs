using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Ide.RazorLib.FindAllReferences.Models;

namespace Luthetus.Ide.RazorLib.FindAllReferences.Displays;

public partial class FindAllReferencesDisplay : ComponentBase, IDisposable
{
	[Inject]
	private IFindAllReferencesService FindAllReferencesService { get; set; } = null!;
	[Inject]
	private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;

	public static readonly Key<Panel> FindAllReferencesPanelKey = Key<Panel>.NewKey();
    public static readonly Key<IDynamicViewModel> FindAllReferencesDynamicViewModelKey = Key<IDynamicViewModel>.NewKey();
    
    private IExtendedCompilerService _cSharpCompilerService = null!;
    
    /*private string _fullyQualifiedName;
    
    public string FullyQualifiedName
    {
	    get => _fullyQualifiedName;
	    set
	    {
	    	_fullyQualifiedName = value;
	    	FindAllReferencesService.SetFullyQualifiedName(_fullyQualifiedName);
	    }
    }*/
    
    protected override void OnInitialized()
    {
    	FindAllReferencesService.FindAllReferencesStateChanged += OnFindAllReferencesStateChanged;
    	
    	_cSharpCompilerService = (IExtendedCompilerService)CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_CLASS);
    	
    	base.OnInitialized();
    }
    
    private async void OnFindAllReferencesStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
    	FindAllReferencesService.FindAllReferencesStateChanged -= OnFindAllReferencesStateChanged;
    }
}