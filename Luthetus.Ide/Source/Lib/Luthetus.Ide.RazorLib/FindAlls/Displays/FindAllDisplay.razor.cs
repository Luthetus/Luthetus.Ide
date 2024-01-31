using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.RazorLib.FindAlls.Models;
using Luthetus.Ide.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.FindAlls.Displays;

public partial class FindAllDisplay : FluxorComponent
{
    [Inject]
    private IState<FindAllState> FindAllStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private LuthetusTextEditorOptions LuthetusTextEditorOptions { get; set; } = null!;
    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = null!;

    private string InputValue 
    {
        get => FindAllStateWrap.Value.Query;
        set
        {
            if (value is null)
                value = string.Empty;

            Dispatcher.Dispatch(new FindAllState.WithAction(inState => inState with
            {
                Query = value
            }));

            Dispatcher.Dispatch(new FindAllState.SearchEffect());
        }
    }

    private string GetIsActiveCssClass(FindAllFilterKind findAllFilterKind)
    {
        return FindAllStateWrap.Value.FindAllFilterKind == findAllFilterKind
            ? "luth_active"
            : string.Empty;
    }

    private async Task OpenInEditorOnClick(string filePath)
    {
        if (LuthetusTextEditorOptions.OpenInEditorAsyncFunc is null)
            return;

        await LuthetusTextEditorOptions.OpenInEditorAsyncFunc.Invoke(
            filePath,
            ServiceProvider);
    }
}