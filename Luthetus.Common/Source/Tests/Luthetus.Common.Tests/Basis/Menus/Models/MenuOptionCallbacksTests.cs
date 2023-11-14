using Luthetus.Common.RazorLib.Menus.Models;

namespace Luthetus.Common.Tests.Basis.Menus.Models;

/// <summary>
/// <see cref="MenuOptionCallbacks"/>
/// </summary>
public class MenuOptionCallbacksTests
{
    /// <summary>
    /// <see cref="MenuOptionCallbacks(Func{Task}, Func{Action, Task})"/>
    /// <br/>----<br/>
    /// <see cref="MenuOptionCallbacks.HideWidgetAsync"/>
    /// <see cref="MenuOptionCallbacks.CompleteWidgetAsync"/>
    /// </summary>
    [Fact]
    public async Task ConstructorAsync()
    {
        var shouldDisplayWidget = true;

        Assert.True(shouldDisplayWidget);

        var menuOptionCallbacks = new MenuOptionCallbacks(
            () =>
            {
                shouldDisplayWidget = false;
                return Task.CompletedTask;
            },
            onAfterCompletionAction =>
            {
                shouldDisplayWidget = false;
                onAfterCompletionAction.Invoke();

                return Task.CompletedTask;
            });

        await menuOptionCallbacks.HideWidgetAsync();
        Assert.False(shouldDisplayWidget);
        
        shouldDisplayWidget = true;
        Assert.True(shouldDisplayWidget);

        var hasCompletedWidget = false;
        Assert.False(hasCompletedWidget);

        await menuOptionCallbacks.CompleteWidgetAsync(() => hasCompletedWidget = true);
        Assert.False(shouldDisplayWidget);
        Assert.True(hasCompletedWidget);
    }
}