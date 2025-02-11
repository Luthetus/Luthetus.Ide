using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;

namespace Luthetus.TextEditor.RazorLib.Options.Displays;

public partial class TextEditorSettings : ComponentBase
{
    [Inject]
    private IAutocompleteIndexer AutocompleteIndexer { get; set; } = null!;

    [Parameter]
    public string InputElementCssClass { get; set; } = string.Empty;
}