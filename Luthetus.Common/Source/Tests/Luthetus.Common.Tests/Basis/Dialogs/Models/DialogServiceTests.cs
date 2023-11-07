using Luthetus.Common.RazorLib.Dialogs.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Fluxor;
using Luthetus.Common.RazorLib.Notifications.Displays;

namespace Luthetus.Common.Tests.Basis.Dialogs.Models;

/// <summary>
/// <see cref="DialogService"/>
/// </summary>
public class DialogServiceTests
{
    /// <summary>
    /// <see cref="DialogService(IDispatcher, IState{RazorLib.Dialogs.States.DialogState})"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var services = new ServiceCollection()
            .AddScoped<IDialogService, DialogService>()
            .AddFluxor(options => options.ScanAssemblies(typeof(IDialogService).Assembly));

        var serviceProvider = services.BuildServiceProvider();
        var dialogService = serviceProvider.GetRequiredService<IDialogService>();

        Assert.NotNull(dialogService);
    }

    /// <summary>
    /// <see cref="DialogService.DialogStateWrap"/>
    /// </summary>
    [Fact]
    public void DialogStateWrap()
    {
        var services = new ServiceCollection()
            .AddScoped<IDialogService, DialogService>()
            .AddFluxor(options => options.ScanAssemblies(typeof(IDialogService).Assembly));

        var serviceProvider = services.BuildServiceProvider();
        var dialogService = serviceProvider.GetRequiredService<IDialogService>();

        Assert.NotNull(dialogService.DialogStateWrap);
    }

    /// <summary>
    /// <see cref="DialogService.RegisterDialogRecord(DialogRecord)"/>
    /// </summary>
    [Fact]
    public void RegisterDialogRecord()
    {
        var dialogRecord = new DialogRecord(
            Key<DialogRecord>.NewKey(),
            "Test Dialog",
            typeof(CommonInformativeNotificationDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(CommonInformativeNotificationDisplay.Message),
                    "Test to register a dialog record"
                }
            },
            null);

        var services = new ServiceCollection()
            .AddScoped<IDialogService, DialogService>()
            .AddFluxor(options => options.ScanAssemblies(typeof(IDialogService).Assembly));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        var dialogService = serviceProvider.GetRequiredService<IDialogService>();

        Assert.Empty(dialogService.DialogStateWrap.Value.DialogBag);

        dialogService.RegisterDialogRecord(dialogRecord);

        Assert.NotEmpty(dialogService.DialogStateWrap.Value.DialogBag);
        Assert.Contains(dialogService.DialogStateWrap.Value.DialogBag, x => x == dialogRecord);
    }

    /// <summary>
    /// <see cref="DialogService.SetDialogRecordIsMaximized(Key{DialogRecord}, bool)"/>
    /// </summary>
    [Fact]
    public void SetDialogRecordIsMaximized()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="DialogService.DisposeDialogRecord(Key{DialogRecord})"/>
    /// </summary>
    [Fact]
    public void DisposeDialogRecord()
    {
        var dialogRecord = new DialogRecord(
            Key<DialogRecord>.NewKey(),
            "Test Dialog",
            typeof(CommonInformativeNotificationDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(CommonInformativeNotificationDisplay.Message),
                    "Test to register a dialog record"
                }
            },
            null);

        var services = new ServiceCollection()
            .AddScoped<IDialogService, DialogService>()
            .AddFluxor(options => options.ScanAssemblies(typeof(IDialogService).Assembly));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        var dialogService = serviceProvider.GetRequiredService<IDialogService>();

        Assert.Empty(dialogService.DialogStateWrap.Value.DialogBag);

        dialogService.RegisterDialogRecord(dialogRecord);

        Assert.NotEmpty(dialogService.DialogStateWrap.Value.DialogBag);
        Assert.Contains(dialogService.DialogStateWrap.Value.DialogBag, x => x == dialogRecord);

        dialogService.DisposeDialogRecord(dialogRecord.Key);
        
        Assert.Empty(dialogService.DialogStateWrap.Value.DialogBag);
    }
}