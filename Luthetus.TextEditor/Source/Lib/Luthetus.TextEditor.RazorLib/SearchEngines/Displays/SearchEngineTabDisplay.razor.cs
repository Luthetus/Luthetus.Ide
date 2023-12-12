using Fluxor;
using Luthetus.TextEditor.RazorLib.SearchEngines.States;
using Luthetus.TextEditor.RazorLib.SearchEngines.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.SearchEngines.Displays;

public partial class SearchEngineTabDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public ITextEditorSearchEngine SearchEngine { get; set; } = null!;
    [Parameter, EditorRequired]
    public bool IsActive { get; set; }

    private string IsActiveCssClassString => IsActive ? "luth_active" : "";

    private void SetActiveSearchEngineOnClick()
    {
        Dispatcher.Dispatch(new TextEditorSearchEngineState.SetActiveSearchEngineAction(
            SearchEngine.SearchEngineKey));
    }
}