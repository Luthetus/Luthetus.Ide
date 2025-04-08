using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Ide.RazorLib.FindAllReferences.Models;

namespace Luthetus.Ide.RazorLib.FindAllReferences.Displays;

public partial class FindAllReferencesDisplay : ComponentBase, IDisposable
{
	[Inject]
	private IFindAllReferencesService FindAllReferencesService { get; set; } = null!;

	public static readonly Key<Panel> FindAllReferencesPanelKey = Key<Panel>.NewKey();
    public static readonly Key<IDynamicViewModel> FindAllReferencesDynamicViewModelKey = Key<IDynamicViewModel>.NewKey();
    
    protected override void OnInitialized()
    {
    	FindAllReferencesService.FindAllReferencesStateChanged += OnFindAllReferencesStateChanged;
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