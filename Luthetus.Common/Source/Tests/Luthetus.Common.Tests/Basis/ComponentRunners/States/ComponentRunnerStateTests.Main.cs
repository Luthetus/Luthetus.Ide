using Fluxor;
using Luthetus.Common.RazorLib.ComponentRunners.Internals.PersonCase;
using Luthetus.Common.RazorLib.ComponentRunners.Models;
using Luthetus.Common.RazorLib.ComponentRunners.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;
using System.Reflection;

namespace Luthetus.Common.Tests.Basis.ComponentRunners.States;

/// <summary>
/// <see cref="ComponentRunnerState"/>
/// </summary>
public class ComponentRunnerStateMainTests
{
    /// <summary>
    /// <see cref="ComponentRunnerState()"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var componentRunnerState = new ComponentRunnerState();
        Assert.Equal(ImmutableList<ComponentRunnerDisplayState>.Empty, componentRunnerState.ComponentRunnerDisplayStateBag);
    }

    /// <summary>
    /// <see cref="ComponentRunnerState.ComponentRunnerDisplayStateBag"/>
    /// </summary>
    [Fact]
    public void ComponentRunnerDisplayStateBag()
    {
        InitializeComponentRunnerStateMainTests(out var dispatcher, out var componentRunnerDisplayState);

        var componentRunnerState = new ComponentRunnerState();

        var outComponentRunnerDisplayStateBag = componentRunnerState.ComponentRunnerDisplayStateBag
            .Add(componentRunnerDisplayState);

        Assert.NotEqual(
            outComponentRunnerDisplayStateBag,
            componentRunnerState.ComponentRunnerDisplayStateBag);

        componentRunnerState = componentRunnerState with
        {
            ComponentRunnerDisplayStateBag = outComponentRunnerDisplayStateBag
        };

        Assert.Equal(
            outComponentRunnerDisplayStateBag,
            componentRunnerState.ComponentRunnerDisplayStateBag);
    }

    private void InitializeComponentRunnerStateMainTests(
        out IDispatcher dispatcher,
        out ComponentRunnerDisplayState componentRunnerDisplayState)
    {
        var services = new ServiceCollection()
            .AddFluxor(options => options.ScanAssemblies(typeof(ComponentRunnerState).Assembly));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        var key = Key<ComponentRunnerDisplayState>.NewKey();
        var personDisplayType = typeof(PersonDisplay);

        var componentTypeBag = new List<Type>
        {
            personDisplayType,
            typeof(PersonSimpleDisplay),
        };

        var componentPropertyInfoBag = Array.Empty<PropertyInfo>();
        var previousTypeGuid = Guid.Empty;
        var componentRunnerParameterMap = new Dictionary<string, IComponentRunnerParameter>();

        componentRunnerDisplayState = new ComponentRunnerDisplayState(
            key,
            componentTypeBag,
            personDisplayType.GUID,
            previousTypeGuid,
            componentPropertyInfoBag,
            componentRunnerParameterMap,
            dispatcher);

        var chosenComponentChangeCounter = 0;

        componentRunnerDisplayState.CalculateComponentPropertyInfoBag(
            componentRunnerDisplayState.ChosenTypeGuid.ToString(),
            ref chosenComponentChangeCounter);
    }
}