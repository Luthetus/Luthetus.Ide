using Fluxor;
using Luthetus.Common.RazorLib.ComponentRunners.Internals.Classes;
using Luthetus.Common.RazorLib.ComponentRunners.States;
using Luthetus.Common.RazorLib.Icons.Displays.Codicon;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Luthetus.Common.Tests.Basis.ComponentRunners.States;

/// <summary>
/// <see cref="ComponentRunnerState"/>
/// </summary>
public class ComponentRunnerStateActionTests
{
    /// <summary>
    /// <see cref="ComponentRunnerState.RegisterAction"/>
    /// </summary>
    [Fact]
    public void RegisterAction()
    {
        InitializeComponentRunnerStateActionTests(out var componentRunnerDisplayState);

        var insertionIndex = 0;

        var registerAction = new ComponentRunnerState.RegisterAction(componentRunnerDisplayState, insertionIndex);
        Assert.Equal(componentRunnerDisplayState, registerAction.Entry);
        Assert.Equal(insertionIndex, registerAction.InsertionIndex);
    }

    /// <summary>
    /// <see cref="ComponentRunnerState.DisposeAction"/>
    /// </summary>
    [Fact]
    public void DisposeAction()
    {
        InitializeComponentRunnerStateActionTests(out var componentRunnerDisplayState);

        var disposeAction = new ComponentRunnerState.DisposeAction(componentRunnerDisplayState.Key);
        Assert.Equal(componentRunnerDisplayState.Key, disposeAction.Key);
    }

    /// <summary>
    /// <see cref="ComponentRunnerState.WithAction"/>
    /// </summary>
    [Fact]
    public void WithAction()
    {
        InitializeComponentRunnerStateActionTests(out var componentRunnerDisplayState);

        Func<ComponentRunnerDisplayState, ComponentRunnerDisplayState> withFunc = inState =>
        {
            return inState;
        };

        var withAction = new ComponentRunnerState.WithAction(
            componentRunnerDisplayState.Key,
            withFunc);

        Assert.Equal(componentRunnerDisplayState.Key, withAction.Key);
        Assert.Equal(withFunc, withAction.WithFunc);
    }

    private void InitializeComponentRunnerStateActionTests(
        out ComponentRunnerDisplayState componentRunnerDisplayState)
    {
        var services = new ServiceCollection()
            .AddFluxor(options => options.ScanAssemblies(typeof(ComponentRunnerState).Assembly));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        var componentTypeBag = new List<Type>
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