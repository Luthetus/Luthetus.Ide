using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Common.Tests.Basis.Dropdowns.States;

/// <summary>
/// <see cref="DropdownState.Reducer"/>
/// </summary>
public class DropdownStateReducerTests
{
    private void InitializeDropdownStateReducerTests(
        out ServiceProvider serviceProvider,
        out IState<DropdownState> dropdownStateWrap,
        out IDispatcher dispatcher,
        out Key<DropdownRecord> sampleDropdownRecordKey)
    {
        var services = new ServiceCollection()
            .AddFluxor(options => options.ScanAssemblies(typeof(DropdownState).Assembly));

        serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        dropdownStateWrap = serviceProvider.GetRequiredService<IState<DropdownState>>();

        dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        sampleDropdownRecordKey = Key<DropdownRecord>.NewKey();
    }
}