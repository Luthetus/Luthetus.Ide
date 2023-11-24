using Fluxor;
using Luthetus.Common.RazorLib.Reflectives.Models;
using Luthetus.Common.RazorLib.Reflectives.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;
using System.Reflection;
using Luthetus.Common.RazorLib.Reflectives.PersonCase;

namespace Luthetus.Common.Tests.Basis.Reflectives.States;

/// <summary>
/// <see cref="ReflectiveState"/>
/// </summary>
public class ReflectiveStateMainTests
{
    /// <summary>
    /// <see cref="ReflectiveState()"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var reflectiveState = new ReflectiveState();
        Assert.Equal(ImmutableList<ReflectiveModel>.Empty, reflectiveState.ReflectiveModelBag);
    }

    /// <summary>
    /// <see cref="ReflectiveState.ReflectiveModelBag"/>
    /// </summary>
    [Fact]
    public void ReflectiveModelBag()
    {
        InitializeReflectiveStateMainTests(out var dispatcher, out var model);

        var reflectiveState = new ReflectiveState();
        var outReflectiveModelBag = reflectiveState.ReflectiveModelBag.Add(model);

        Assert.NotEqual(outReflectiveModelBag, reflectiveState.ReflectiveModelBag);

        reflectiveState = reflectiveState with
        {
            ReflectiveModelBag = outReflectiveModelBag
        };

        Assert.Equal(outReflectiveModelBag, reflectiveState.ReflectiveModelBag);
    }

    private void InitializeReflectiveStateMainTests(
        out IDispatcher dispatcher,
        out ReflectiveModel model)
    {
        var services = new ServiceCollection()
            .AddFluxor(options => options.ScanAssemblies(typeof(ReflectiveState).Assembly));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        var key = Key<ReflectiveModel>.NewKey();
        var personDisplayType = typeof(PersonDisplay);

        var componentTypeBag = new List<Type>
        {
            personDisplayType,
            typeof(PersonSimpleDisplay),
        };

        var componentPropertyInfoBag = Array.Empty<PropertyInfo>();
        var previousTypeGuid = Guid.Empty;
        var reflectiveParameterMap = new Dictionary<string, IReflectiveParameter>();

        model = new ReflectiveModel(
            key,
            componentTypeBag,
            personDisplayType.GUID,
            previousTypeGuid,
            componentPropertyInfoBag,
            reflectiveParameterMap,
            dispatcher);

        var chosenComponentChangeCounter = 0;

        model.CalculateComponentPropertyInfoBag(
            model.ChosenTypeGuid.ToString(),
            ref chosenComponentChangeCounter);
    }
}