using Fluxor;
using Luthetus.Common.RazorLib.ComponentRunners.Internals.PersonCase;
using Luthetus.Common.RazorLib.ComponentRunners.Models;
using Luthetus.Common.RazorLib.ComponentRunners.PersonCase;
using Luthetus.Common.RazorLib.ComponentRunners.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Luthetus.Common.Tests.Basis.ComponentRunners.Internals.Classes;

/// <summary>
/// <see cref="ComponentRunnerDisplayState"/>
/// </summary>
public class ComponentRunnerDisplayStateTests
{
    /// <summary>
    /// <see cref="ComponentRunnerDisplayState(Key{ComponentRunnerDisplayState}, List{Type}, Guid, Guid, System.Reflection.PropertyInfo[], Dictionary{string, IComponentRunnerParameter}, IDispatcher)"/>
    /// <br/>----<br/>
    /// <see cref="ComponentRunnerDisplayState.ChosenComponentType"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        // InitializeComponentRunnerDisplayStateTests has an out variable
        // for ComponentRunnerDisplayState.
        //
        // Do not use that out variable for the Constructor test however,
        // since the inner details must be tested and asserted within this unit test itself.
        InitializeComponentRunnerDisplayStateTests(out var dispatcher, out _);

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

        var componentRunnerDisplayState = new ComponentRunnerDisplayState(
            key,
            componentTypeBag,
            personDisplayType.GUID,
            previousTypeGuid,
            componentPropertyInfoBag,
            componentRunnerParameterMap,
            dispatcher);

        Assert.Equal(key, componentRunnerDisplayState.Key);
        Assert.Equal(componentTypeBag, componentRunnerDisplayState.ComponentTypeBag);
        Assert.Equal(personDisplayType.GUID, componentRunnerDisplayState.ChosenTypeGuid);
        Assert.Equal(previousTypeGuid, componentRunnerDisplayState.PreviousTypeGuid);
        Assert.Equal(componentPropertyInfoBag, componentRunnerDisplayState.ComponentPropertyInfoBag);
        Assert.Equal(componentRunnerParameterMap, componentRunnerDisplayState.ComponentRunnerParameterMap);
        Assert.Equal(dispatcher, componentRunnerDisplayState.Dispatcher);
        Assert.Equal(personDisplayType, componentRunnerDisplayState.ChosenComponentType);
    }

    /// <summary>
    /// <see cref="ComponentRunnerDisplayState.ConstructBlazorParameters()"/>
    /// <br/>----<br/>
    /// <see cref="ComponentRunnerDisplayState.GetParameter(string, IComponentRunnerParameter)"/>
    /// <see cref="ComponentRunnerDisplayState.SetParameter(string, IComponentRunnerParameter)"/>
    /// </summary>
    [Fact]
    public void ConstructBlazorParameters()
    {
        InitializeComponentRunnerDisplayStateTests(out var dispatcher, out var componentRunnerDisplayState);

        var componentRunnerParameter = componentRunnerDisplayState.GetParameter(
            nameof(PersonDisplay.PersonModel),
            ComponentRunnerParameter.ConstructOther(typeof(PersonModel)));

        Assert.Null(componentRunnerParameter.Value);

        componentRunnerDisplayState.SetParameter(
            nameof(PersonDisplay.PersonModel),
            ComponentRunnerParameter.ConstructOther(typeof(PersonModel)));

        throw new NotImplementedException();
    }

    private void InitializeComponentRunnerDisplayStateTests(
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
