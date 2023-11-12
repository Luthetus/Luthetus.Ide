using Luthetus.Common.RazorLib.Dialogs.Models;
using Microsoft.Extensions.DependencyInjection;
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
    /// <br/>----<br/>
    /// <see cref="DialogService.DialogStateWrap"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        InitializeDialogServiceTests(out var dialogService, out var dialogRecord, out _);

        Assert.NotNull(dialogService);
        Assert.NotNull(dialogService.DialogStateWrap);
    }
    
    /// <summary>
    /// <see cref="DialogService.RegisterDialogRecord(DialogRecord)"/>
    /// </summary>
    [Fact]
    public void RegisterDialogRecord()
    {
        InitializeDialogServiceTests(out var dialogService, out var dialogRecord, out _);

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
        InitializeDialogServiceTests(out var dialogService, out var dialogRecord, out _);

        Assert.Empty(dialogService.DialogStateWrap.Value.DialogBag);

        dialogService.RegisterDialogRecord(dialogRecord);

        Assert.NotEmpty(dialogService.DialogStateWrap.Value.DialogBag);
        Assert.Single(dialogService.DialogStateWrap.Value.DialogBag);

        Assert.False(dialogRecord.IsMaximized);
        dialogService.SetDialogRecordIsMaximized(dialogRecord.Key, true);

        dialogRecord = dialogService.DialogStateWrap.Value.DialogBag.Single();
        Assert.True(dialogRecord.IsMaximized);
    }

    /// <summary>
    /// <see cref="DialogService.DisposeDialogRecord(Key{DialogRecord})"/>
    /// </summary>
    [Fact]
    public void DisposeDialogRecord()
    {
        InitializeDialogServiceTests(out var dialogService, out var dialogRecord, out _);

        Assert.Empty(dialogService.DialogStateWrap.Value.DialogBag);

        dialogService.RegisterDialogRecord(dialogRecord);

        Assert.NotEmpty(dialogService.DialogStateWrap.Value.DialogBag);
        Assert.Contains(dialogService.DialogStateWrap.Value.DialogBag, x => x == dialogRecord);

        dialogService.DisposeDialogRecord(dialogRecord.Key);
        
        Assert.Empty(dialogService.DialogStateWrap.Value.DialogBag);
    }

    private void InitializeDialogServiceTests(
        out IDialogService dialogService,
        out DialogRecord sampleDialogRecord,
        out ServiceProvider serviceProvider)
    {
        var services = new ServiceCollection()
            .AddScoped<IDialogService, DialogService>()
            .AddFluxor(options => options.ScanAssemblies(typeof(IDialogService).Assembly));

        serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        dialogService = serviceProvider.GetRequiredService<IDialogService>();

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