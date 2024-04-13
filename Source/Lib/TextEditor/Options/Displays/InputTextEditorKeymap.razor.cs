using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.RazorLib.Options.Displays;

public partial class InputTextEditorKeymap : FluxorComponent
{
    [Inject]
    private IState<TextEditorOptionsState> TextEditorOptionsStateWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    public string TopLevelDivElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string InputElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string LabelElementCssClassString { get; set; } = string.Empty;

    private void SelectedKeymapChanged(ChangeEventArgs changeEventArgs)
    {
        var allKeymapDefinitionsList = TextEditorKeymapFacts.AllKeymapsList;
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
        }
    }
}