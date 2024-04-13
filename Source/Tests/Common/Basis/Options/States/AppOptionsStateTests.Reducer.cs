using Fluxor;
using Luthetus.Common.RazorLib.Options.States;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Common.Tests.Basis.Options.States;

/// <summary>
/// <see cref="AppOptionsState.Reducer"/>
/// </summary>
public partial record AppOptionsStateReducerTests
{
    /// <summary>
    /// <see cref="AppOptionsState.Reducer.ReduceWithAction(AppOptionsState, AppOptionsState.WithAction)"/>
    /// </summary>
    [Fact]
    public void ReduceWithAction()
    {
        InitializeAppOptionsStateReducerTests(out var appOptionsStateWrap, out var dispatcher);

        var inFontSize = appOptionsStateWrap.Value.Options.FontSizeInPixels;

        var withAction = new AppOptionsState.WithAction(
            inAppOptionState => inAppOptionState with
            {
                Options = inAppOptionState.Options with
                {
                    FontSizeInPixels = inAppOptionState.Options.FontSizeInPixels + 1
                }
            });

        dispatcher.Dispatch(withAction);

        var outFontSize = appOptionsStateWrap.Value.Options.FontSizeInPixels;

        Assert.Equal(inFontSize + 1, outFontSize);
    }

    private void InitializeAppOptionsStateReducerTests(
        out IState<AppOptionsState> appOptionsStateWrap,
        out IDispatcher dispatcher)
    {
        var services = new ServiceCollection()
            .AddFluxor(options => options.ScanAssemblies(typeof(AppOptionsState).Assembly));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        appOptionsStateWrap = serviceProvider.GetRequiredService<IState<AppOptionsState>>();

        dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
    }
}