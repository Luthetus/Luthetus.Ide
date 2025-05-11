using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;

namespace Luthetus.TextEditor.RazorLib.Options.Displays;

public partial class InputTextEditorKeymap : ComponentBase, IDisposable
{
    [Inject]
    private TextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    public InputViewModel InputViewModel { get; set; } = InputViewModel.Empty;

	protected override void OnInitialized()
    {
    	TextEditorService.OptionsApi.StaticStateChanged += TextEditorOptionsStateWrapOnStateChanged;
    	base.OnInitialized();
    }

    private void SelectedKeymapChanged(ChangeEventArgs changeEventArgs)
    {
        /*var allKeymapDefinitionsList = TextEditorKeymapFacts.AllKeymapsList;
        var chosenKeymapGuidString = changeEventArgs.Value?.ToString() ?? string.Empty;

        if (Guid.TryParse(chosenKeymapGuidString, out var chosenKeymapKeyGuid))
        {
            var chosenKeymapKey = new Key<Keymap>(chosenKeymapKeyGuid);
            var foundKeymap = allKeymapDefinitionsList.FirstOrDefault(x => x.Key == chosenKeymapKey);

            if (foundKeymap is not null)
                TextEditorService.OptionsApi.SetKeymap(foundKeymap);
        }
        else
        {
            TextEditorService.OptionsApi.SetKeymap(TextEditorKeymapFacts.DefaultKeymap);
        }*/
    }
    
    private async void TextEditorOptionsStateWrapOnStateChanged()
    {
        await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
    	TextEditorService.OptionsApi.StaticStateChanged -= TextEditorOptionsStateWrapOnStateChanged;
    }
}