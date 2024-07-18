using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.Tests.Basis.Dropdowns.Models;

/// <summary>
/// <see cref="DropdownService"/>
/// </summary>
public class DropdownServiceTests
{
    /// <summary>
    /// <see cref="DropdownService(IDispatcher, IState{DropdownState})"/>
    /// <br/>----<br/>
    /// <see cref="DropdownService.DropdownStateWrap"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        InitializeDropdownServiceTests(out var dropdownService, out _, out _);

        Assert.NotNull(dropdownService);
        Assert.NotNull(dropdownService.DropdownStateWrap);
    }

    /// <summary>
    /// <see cref="DropdownService.AddActiveDropdownKey(Key{DropdownRecord})"/>
    /// </summary>
    [Fact]
    public void AddActiveDropdownKey()
    {
        InitializeDropdownServiceTests(out var dropdownService, out var dropdownKey, out _);

        throw new NotImplementedException("TODO: this test was broken by a substantial rework to the dropdown code, and needs to be re-visited (2024-07-07).");
    }

    /// <summary>
    /// <see cref="DropdownService.RemoveActiveDropdownKey(Key{DropdownRecord})"/>
    /// </summary>
    [Fact]
    public void RemoveActiveDropdownKey()
    {
        InitializeDropdownServiceTests(out var dropdownService, out var dropdownKey, out _);

        throw new NotImplementedException("TODO: this test was broken by a substantial rework to the dropdown code, and needs to be re-visited (2024-07-07).");
    }

    /// <summary>
    /// <see cref="DropdownService.ClearActiveDropdownKeysAction()"/>
    /// </summary>
    [Fact]
    public void ClearActiveDropdownKeysAction()
    {
        InitializeDropdownServiceTests(out var dropdownService, out _, out _);

        throw new NotImplementedException("TODO: this test was broken by a substantial rework to the dropdown code, and needs to be re-visited (2024-07-07).");
    }

    private void InitializeDropdownServiceTests(
        out IDropdownService dropdownService,
        out Key<DropdownRecord> sampleDropdownRecordKey,
        out ServiceProvider serviceProvider)
    {
        var services = new ServiceCollection()
            .AddScoped<IDropdownService, DropdownService>()
            .AddFluxor(options => options.ScanAssemblies(typeof(IDropdownService).Assembly));

        serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        dropdownService = serviceProvider.GetRequiredService<IDropdownService>();

        sampleDropdownRecordKey = Key<DropdownRecord>.NewKey();
    }
}