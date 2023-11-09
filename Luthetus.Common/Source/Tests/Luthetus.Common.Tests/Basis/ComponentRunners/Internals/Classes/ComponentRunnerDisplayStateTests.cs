using Fluxor;
using Luthetus.Common.RazorLib.ComponentRunners.Internals.Classes;
using Luthetus.Common.RazorLib.ComponentRunners.Internals.PersonCase;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Luthetus.Common.Tests.Basis.ComponentRunners.Internals.Classes;

/// <summary>
/// <see cref="ComponentRunnerDisplayState"/>
/// </summary>
public class ComponentRunnerDisplayStateTests
{
    /// <summary>
    /// <see cref="ComponentRunnerDisplayState(Key{ComponentRunnerDisplayState}, List{Type}, Guid, Guid, System.Reflection.PropertyInfo[], Dictionary{string, IComponentRunnerType}, Fluxor.IDispatcher)"/>
    /// <br/>----<br/>
    /// <see cref="ComponentRunnerDisplayState.ChosenComponentType"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
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
        var componentRunnerTypeParameterMap = new Dictionary<string, IComponentRunnerType>();

        var componentRunnerDisplayState = new ComponentRunnerDisplayState(
            key,
            componentTypeBag,
            personDisplayType.GUID,
            previousTypeGuid,
            componentPropertyInfoBag,
            componentRunnerTypeParameterMap,
            dispatcher);

        Assert.Equal(key, componentRunnerDisplayState.Key);
        Assert.Equal(componentTypeBag, componentRunnerDisplayState.ComponentTypeBag);
        Assert.Equal(personDisplayType.GUID, componentRunnerDisplayState.ChosenTypeGuid);
        Assert.Equal(previousTypeGuid, componentRunnerDisplayState.PreviousTypeGuid);
        Assert.Equal(componentPropertyInfoBag, componentRunnerDisplayState.ComponentPropertyInfoBag);
        Assert.Equal(componentRunnerTypeParameterMap, componentRunnerDisplayState.ComponentRunnerTypeParameterMap);
        Assert.Equal(dispatcher, componentRunnerDisplayState.Dispatcher);
        Assert.Equal(personDisplayType, componentRunnerDisplayState.ChosenComponentType);
    }

    /// <summary>
    /// <see cref="ComponentRunnerDisplayState.GetComponentRunnerType(string, IComponentRunnerType)"/>
    /// </summary>
    [Fact]
    public void GetComponentRunnerType()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ComponentRunnerDisplayState.SetComponentRunnerType(string, IComponentRunnerType)"/>
    /// </summary>
    [Fact]
    public void SetComponentRunnerType()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ComponentRunnerDisplayState.ConstructBlazorParameters()"/>
    /// </summary>
    [Fact]
    public void ConstructBlazorParameters()
    {
        throw new NotImplementedException();
    }

    private void InitializeComponentRunnerDisplayStateTests(
        out IDispatcher dispatcher,
        out ServiceProvider serviceProvider)
    {
        var services = new ServiceCollection()
            .AddScoped<IDialogService, DialogService>()
            .AddFluxor(options => options.ScanAssemblies(typeof(IDialogService).Assembly));

        serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
    }
}
