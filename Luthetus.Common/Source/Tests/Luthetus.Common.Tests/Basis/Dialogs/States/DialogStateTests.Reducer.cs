using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Common.Tests.Basis.Dialogs.States;

/// <summary>
/// <see cref="DialogState"/>
/// </summary>
public class DialogStateReducerTests
{
    [Fact]
    public void ReduceRegisterAction()
    {
        /*
        [ReducerMethod]
        public static DialogState ReduceRegisterAction(
            DialogState inState, RegisterAction registerAction)
         */

        InitializeDialogStateReducerTests(
            out var _, out var dialogStateWrap, out var dispatcher, out var dialogRecord);

        Assert.Empty(dialogStateWrap.Value.DialogBag);

        dispatcher.Dispatch(new DialogState.RegisterAction(dialogRecord));

        Assert.NotEmpty(dialogStateWrap.Value.DialogBag);
        Assert.Contains(dialogStateWrap.Value.DialogBag, x => x == dialogRecord);
    }

    [Fact]
    public void ReduceSetIsMaximizedAction()
    {
        /*
        [ReducerMethod]
        public static DialogState ReduceSetIsMaximizedAction(
            DialogState inState, SetIsMaximizedAction setIsMaximizedAction)
         */

        InitializeDialogStateReducerTests(
            out var _, out var dialogStateWrap, out var dispatcher, out var dialogRecord);

        Assert.Empty(dialogStateWrap.Value.DialogBag);

        dispatcher.Dispatch(new DialogState.RegisterAction(dialogRecord));

        Assert.NotEmpty(dialogStateWrap.Value.DialogBag);
        Assert.Single(dialogStateWrap.Value.DialogBag);

        Assert.False(dialogRecord.IsMaximized);
        dispatcher.Dispatch(new DialogState.SetIsMaximizedAction(dialogRecord.Key, true));

        dialogRecord = dialogStateWrap.Value.DialogBag.Single();
        Assert.True(dialogRecord.IsMaximized);
    }

    [Fact]
    public void ReduceDisposeAction()
    {
        /*
        [ReducerMethod]
        public static DialogState ReduceDisposeAction(
            DialogState inState, DisposeAction disposeAction)
         */

        InitializeDialogStateReducerTests(
            out var _, out var dialogStateWrap, out var dispatcher, out var dialogRecord);

        Assert.Empty(dialogStateWrap.Value.DialogBag);

        dispatcher.Dispatch(new DialogState.RegisterAction(dialogRecord));

        Assert.NotEmpty(dialogStateWrap.Value.DialogBag);
        Assert.Contains(dialogStateWrap.Value.DialogBag, x => x == dialogRecord);

        dispatcher.Dispatch(new DialogState.DisposeAction(dialogRecord.Key));

        Assert.Empty(dialogStateWrap.Value.DialogBag);
    }

    private void InitializeDialogStateReducerTests(
        out ServiceProvider serviceProvider,
        out IState<DialogState> dialogStateWrap,
        out IDispatcher dispatcher,
        out DialogRecord sampleDialogRecord)
    {
        var services = new ServiceCollection()
            .AddScoped<IDialogService, DialogService>()
            .AddFluxor(options => options.ScanAssemblies(typeof(IDialogService).Assembly));

        serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        dialogStateWrap = serviceProvider.GetRequiredService<IState<DialogState>>();

        dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        sampleDialogRecord = new DialogRecord(Key<DialogRecord>.NewKey(), "Test title",
            typeof(CommonInformativeNotificationDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(CommonInformativeNotificationDisplay.Message),
                    "Test message"
                }
            },
            null);
    }
}