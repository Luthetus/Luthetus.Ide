using Luthetus.Common.RazorLib.Keyboard;
using Luthetus.Common.RazorLib.Menu;
using Luthetus.Common.RazorLib.Store.AccountCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Ide.RazorLib.Account;

public partial class LoginFormDisplay : ComponentBase
{
    [Inject]
    private IState<AccountState> AccountStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter]
    public MenuOptionWidgetParameters? MenuOptionWidgetParameters { get; set; }

    private ElementReference? _containerInputElementReference;

    private string GroupKey
    {
        get => AccountStateWrap.Value.GroupName;
        set
        {
            Dispatcher.Dispatch(new AccountState.AccountStateWithAction(
                inAccountState => inAccountState with
                {
                    GroupName = value
                }));
        }
    }

    private string ContainerKey
    {
        get => AccountStateWrap.Value.ContainerName;
        set
        {
            Dispatcher.Dispatch(new AccountState.AccountStateWithAction(
                inAccountState => inAccountState with
                {
                    ContainerName = value
                }));
        }
    }

    private string Alias
    {
        get => AccountStateWrap.Value.Alias;
        set
        {
            Dispatcher.Dispatch(new AccountState.AccountStateWithAction(
                inAccountState => inAccountState with
                {
                    Alias = value
                }));
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (MenuOptionWidgetParameters is not null &&
                _containerInputElementReference is not null)
            {
                try
                {
                    await _containerInputElementReference.Value.FocusAsync();
                }
                catch (Exception)
                {
                    // 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
                    //             This bug is seemingly happening randomly. I have a suspicion
                    //             that there are race-condition exceptions occurring with "FocusAsync"
                    //             on an ElementReference.
                }
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (MenuOptionWidgetParameters is not null)
        {
            if (keyboardEventArgs.Key == KeyboardKeyFacts.MetaKeys.ESCAPE)
            {
                await MenuOptionWidgetParameters.HideWidgetAsync.Invoke();
            }
            else if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
            {
                await MenuOptionWidgetParameters.CompleteWidgetAsync.Invoke(() => { });
            }
        }
    }
}