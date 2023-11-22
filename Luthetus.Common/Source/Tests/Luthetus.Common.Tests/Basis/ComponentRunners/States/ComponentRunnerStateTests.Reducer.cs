using Fluxor;
using Luthetus.Common.RazorLib.ComponentRunners.Models;
using Luthetus.Common.RazorLib.ComponentRunners.States;
using Luthetus.Common.RazorLib.Icons.Displays.Codicon;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Luthetus.Common.Tests.Basis.ComponentRunners.States;

/// <summary>
/// <see cref="ComponentRunnerState"/>
/// </summary>
public class ComponentRunnerStateReducerTests
{
    [Fact]
    public void ReduceRegisterAction()
    {
        /*
        [ReducerMethod]
        public static ComponentRunnerState ReduceRegisterAction(
            ComponentRunnerState inState, RegisterAction registerAction)
            */

        InitializeComponentRunnerStateReducerTests(
            out var dispatcher,
            out var componentRunnerStateWrap,
            out var componentRunnerDisplayState,
            out _);

        var insertionIndex = 0;

        var registerAction = new ComponentRunnerState.RegisterAction(componentRunnerDisplayState, insertionIndex);

        Assert.DoesNotContain(
            componentRunnerStateWrap.Value.ComponentRunnerDisplayStateBag,
            x => x == componentRunnerDisplayState);

        dispatcher.Dispatch(registerAction);

        Assert.Contains(
            componentRunnerStateWrap.Value.ComponentRunnerDisplayStateBag,
            x => x == componentRunnerDisplayState);
    }

    [Fact]
    public void ReduceWithAction()
    {
        /*
        [ReducerMethod]
        public static ComponentRunnerState ReduceWithAction(
            ComponentRunnerState inState, WithAction withAction)
            */

        InitializeComponentRunnerStateReducerTests(
            out var dispatcher,
            out var componentRunnerStateWrap,
            out var componentRunnerDisplayState,
            out var componentTypeBag);

        var insertionIndex = 0;

        // Setup: Register a node
        {
            var registerAction = new ComponentRunnerState.RegisterAction(componentRunnerDisplayState, insertionIndex);
            dispatcher.Dispatch(registerAction);
        }

        var type = componentTypeBag.Last();

        componentRunnerDisplayState = componentRunnerStateWrap.Value.ComponentRunnerDisplayStateBag
            .Single(x => x.Key == componentRunnerDisplayState.Key);

        Assert.NotEqual(type.GUID, componentRunnerDisplayState.ChosenTypeGuid);

        var withAction = new ComponentRunnerState.WithAction(
            componentRunnerDisplayState.Key,
            inState =>
            {
                return inState with
                {
                    ChosenTypeGuid = type.GUID
                };
            });

        dispatcher.Dispatch(withAction);

        componentRunnerDisplayState = componentRunnerStateWrap.Value.ComponentRunnerDisplayStateBag
            .Single(x => x.Key == componentRunnerDisplayState.Key);

        Assert.Equal(type.GUID, componentRunnerDisplayState.ChosenTypeGuid);
    }

    [Fact]
    public void ReduceDisposeAction()
    {
        /*
        [ReducerMethod]
        public static ComponentRunnerState ReduceDisposeAction(
            ComponentRunnerState inState, DisposeAction disposeAction)
            */

        InitializeComponentRunnerStateReducerTests(
            out var dispatcher,
            out var componentRunnerStateWrap,
            out var componentRunnerDisplayState,
            out _);

        var insertionIndex = 0;

        // Setup: Register a node
        {
            var registerAction = new ComponentRunnerState.RegisterAction(componentRunnerDisplayState, insertionIndex);
            dispatcher.Dispatch(registerAction);
        }

        var disposeAction = new ComponentRunnerState.DisposeAction(componentRunnerDisplayState.Key);

        Assert.Contains(
            componentRunnerStateWrap.Value.ComponentRunnerDisplayStateBag,
            x => x == componentRunnerDisplayState);

        dispatcher.Dispatch(disposeAction);

        Assert.DoesNotContain(
            componentRunnerStateWrap.Value.ComponentRunnerDisplayStateBag,
            x => x == componentRunnerDisplayState);
    }

    private void InitializeComponentRunnerStateReducerTests(
        out IDispatcher dispatcher,
        out IState<ComponentRunnerState> componentRunnerStateWrap,
        out ComponentRunnerDisplayState componentRunnerDisplayState,
        out List<Type> componentTypeBag)
    {
        var services = new ServiceCollection()
            .AddFluxor(options => options.ScanAssemblies(typeof(ComponentRunnerState).Assembly));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        componentRunnerStateWrap = serviceProvider.GetRequiredService<IState<ComponentRunnerState>>();

        dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        componentTypeBag = new List<Type>
        {
            typeof(IconArrowDown),
            typeof(IconArrowLeft),
            typeof(IconArrowRight),
            typeof(IconArrowUp),
        };

        componentRunnerDisplayState = new ComponentRunnerDisplayState(
                Key<ComponentRunnerDisplayState>.NewKey(),
                componentTypeBag,
                Guid.Empty,
                Guid.Empty,
                Array.Empty<PropertyInfo>(),
                new(),
                dispatcher);
    }
}