using Fluxor;
using Luthetus.Common.RazorLib.Drags.Displays;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Common.Tests.Basis.Drags.States;

/// <summary>
/// <see cref="DragState"/>
/// </summary>
public class DragStateReducerTests
{
    [Fact]
    public void ReduceWithAction()
    {
        /*
        [ReducerMethod]
        public static DragState ReduceWithAction(
            DragState inState, WithAction withAction)
         */

        InitializeDragStateReducerTests(out _, out var dragStateWrap, out var dispatcher);

        Assert.False(dragStateWrap.Value.ShouldDisplay);
        Assert.Null(dragStateWrap.Value.MouseEventArgs);

        dispatcher.Dispatch(new DragState.WithAction(x => x with
        {
            ShouldDisplay = true,
            MouseEventArgs = new(),
        }));

        Assert.True(dragStateWrap.Value.ShouldDisplay);
        Assert.NotNull(dragStateWrap.Value.MouseEventArgs);
    }

    private void InitializeDragStateReducerTests(
        out ServiceProvider serviceProvider,
        out IState<DragState> dragStateWrap,
        out IDispatcher dispatcher)
    {
        var services = new ServiceCollection()
            .AddFluxor(options => options.ScanAssemblies(typeof(DragState).Assembly));

        serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        dragStateWrap = serviceProvider.GetRequiredService<IState<DragState>>();

        dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
    }
}