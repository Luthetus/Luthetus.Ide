using Fluxor.Blazor.Web.Components;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.Options.Displays;

public partial class TextEditorSettings : FluxorComponent
{
    [Inject]
    private IAutocompleteIndexer AutocompleteIndexer { get; set; } = null!;

    [Parameter]
    public string InputElementCssClass { get; set; } = string.Empty;
}