using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.Dynamics.Models;

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

        Assert.Empty(dialogService.DialogStateWrap.Value.DialogList);

        dialogService.RegisterDialogRecord(dialogRecord);

        Assert.NotEmpty(dialogService.DialogStateWrap.Value.DialogList);
        Assert.Contains(dialogService.DialogStateWrap.Value.DialogList, x => x == dialogRecord);
    }

    /// <summary>
    /// <see cref="DialogService.SetDialogRecordIsMaximized(Key{DialogRecord}, bool)"/>
    /// </summary>
    [Fact]
    public void SetDialogRecordIsMaximized()
    {
        InitializeDialogServiceTests(out var dialogService, out var dialogRecord, out _);

        Assert.Empty(dialogService.DialogStateWrap.Value.DialogList);

        dialogService.RegisterDialogRecord(dialogRecord);

        Assert.NotEmpty(dialogService.DialogStateWrap.Value.DialogList);
        Assert.Single(dialogService.DialogStateWrap.Value.DialogList);

        Assert.False(dialogRecord.DialogIsMaximized);
        dialogService.SetDialogRecordIsMaximized(dialogRecord.DynamicViewModelKey, true);

        dialogRecord = dialogService.DialogStateWrap.Value.DialogList.Single();
        Assert.True(dialogRecord.DialogIsMaximized);
    }

    /// <summary>
    /// <see cref="DialogService.DisposeDialogRecord(Key{DialogRecord})"/>
    /// </summary>
    [Fact]
    public void DisposeDialogRecord()
    {
        InitializeDialogServiceTests(out var dialogService, out var dialogRecord, out _);

        Assert.Empty(dialogService.DialogStateWrap.Value.DialogList);

        dialogService.RegisterDialogRecord(dialogRecord);

        Assert.NotEmpty(dialogService.DialogStateWrap.Value.DialogList);
        Assert.Contains(dialogService.DialogStateWrap.Value.DialogList, x => x == dialogRecord);

        dialogService.DisposeDialogRecord(dialogRecord.DynamicViewModelKey);
        
        Assert.Empty(dialogService.DialogStateWrap.Value.DialogList);
    }

    private void InitializeDialogServiceTests(
        out IDialogService dialogService,
        out IDialog sampleDialogRecord,
        out ServiceProvider serviceProvider)
    {
        var services = new ServiceCollection()
            .AddScoped<IDialogService, DialogService>()
            .AddFluxor(options => options.ScanAssemblies(typeof(IDialogService).Assembly));

        serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        dialogService = serviceProvider.GetRequiredService<IDialogService>();

        sampleDialogRecord = new DialogViewModel(Key<IDynamicViewModel>.NewKey(), "Test title",
            typeof(CommonInformativeNotificationDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(CommonInformativeNotificationDisplay.Message),
                    "Test message"
                }
            },
            null,
            true,
            "luth_element-id");
    }
}