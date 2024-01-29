using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.RazorLib.FindAlls.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.FindAlls.Displays;

public partial class FindAllDisplay : FluxorComponent
{
    [Inject]
    private IState<FindAllState> FindAllStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

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
        }
    }
}