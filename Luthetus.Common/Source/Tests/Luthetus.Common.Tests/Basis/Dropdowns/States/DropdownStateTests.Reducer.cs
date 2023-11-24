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
    /// <summary>
    /// <see cref="DropdownState.Reducer.ReduceAddActiveAction(DropdownState, DropdownState.AddActiveAction)"/>
    /// </summary>
    [Fact]
    public void ReduceAddActiveAction()
    {
        InitializeDropdownStateReducerTests(
            out _,
            out var dropdownStateWrap,
            out var dispatcher,
            out var dropdownKey);

        Assert.Empty(dropdownStateWrap.Value.ActiveKeyBag);

        dispatcher.Dispatch(new DropdownState.AddActiveAction(dropdownKey));

        Assert.NotEmpty(dropdownStateWrap.Value.ActiveKeyBag);
        Assert.Single(dropdownStateWrap.Value.ActiveKeyBag);

        Assert.Contains(
            dropdownStateWrap.Value.ActiveKeyBag,
            x => x == dropdownKey);
    }

    /// <summary>
    /// <see cref="DropdownState.Reducer.ReduceRemoveActiveAction(DropdownState, DropdownState.RemoveActiveAction)"/>
    /// </summary>
    [Fact]
    public void ReduceRemoveActiveAction()
    {
        InitializeDropdownStateReducerTests(
            out _,
            out var dropdownStateWrap,
            out var dispatcher,
            out var dropdownKey);

        Assert.Empty(dropdownStateWrap.Value.ActiveKeyBag);

        dispatcher.Dispatch(new DropdownState.AddActiveAction(dropdownKey));

        Assert.Single(dropdownStateWrap.Value.ActiveKeyBag);

        Assert.Contains(
            dropdownStateWrap.Value.ActiveKeyBag,
            x => x == dropdownKey);

        dispatcher.Dispatch(new DropdownState.RemoveActiveAction(dropdownKey));

        Assert.Empty(dropdownStateWrap.Value.ActiveKeyBag);
    }

    /// <summary>
    /// <see cref="DropdownState.Reducer.ReduceClearActivesAction(DropdownState)"/>
    /// </summary>
    [Fact]
    public void ReduceClearActivesAction()
    {
        InitializeDropdownStateReducerTests(
            out _,
            out var dropdownStateWrap,
            out var dispatcher,
            out var dropdownKey);

        Assert.Empty(dropdownStateWrap.Value.ActiveKeyBag);

        var keyCount = 3;

        for (int i = 0; i < keyCount; i++)
        {
            dispatcher.Dispatch(new DropdownState.AddActiveAction(Key<DropdownRecord>.NewKey()));
        }

        Assert.NotEmpty(dropdownStateWrap.Value.ActiveKeyBag);
        Assert.Equal(3, dropdownStateWrap.Value.ActiveKeyBag.Count);

        dispatcher.Dispatch(new DropdownState.ClearActivesAction());

        Assert.Empty(dropdownStateWrap.Value.ActiveKeyBag);
    }

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