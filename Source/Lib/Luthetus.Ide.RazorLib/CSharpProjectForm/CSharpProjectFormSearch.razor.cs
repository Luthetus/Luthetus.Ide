using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Ide.RazorLib.CSharpProjectForm;

public partial class CSharpProjectFormSearch : ComponentBase
{
    [Parameter, EditorRequired]
    public EventCallback SearchInputChangedEventCallback { get; set; }

    private string _searchInput = string.Empty;

    public string SearchInput 
    {
        get => _searchInput;
        private set
        {
            _searchInput = value;
            SearchInputChangedEventCallback.InvokeAsync();
        }
    }
}