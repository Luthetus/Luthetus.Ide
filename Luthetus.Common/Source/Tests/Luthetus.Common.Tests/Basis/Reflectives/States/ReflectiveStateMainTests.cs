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
        Assert.Equal(ImmutableList<ReflectiveModel>.Empty, reflectiveState.ReflectiveModelList);
    }

    /// <summary>
    /// <see cref="ReflectiveState.ReflectiveModelList"/>
    /// </summary>
    [Fact]
    public void ReflectiveModelList()
    {
        InitializeReflectiveStateMainTests(out var dispatcher, out var model);

        var reflectiveState = new ReflectiveState();
        var outReflectiveModelList = reflectiveState.ReflectiveModelList.Add(model);

        Assert.NotEqual(outReflectiveModelList, reflectiveState.ReflectiveModelList);

        reflectiveState = reflectiveState with
        {
            ReflectiveModelList = outReflectiveModelList
        };

        Assert.Equal(outReflectiveModelList, reflectiveState.ReflectiveModelList);
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

        var componentTypeList = new List<Type>
        {
            personDisplayType,
            typeof(PersonSimpleDisplay),
        };

        var componentPropertyInfoList = Array.Empty<PropertyInfo>();
        var previousTypeGuid = Guid.Empty;
        var reflectiveParameterMap = new Dictionary<string, IReflectiveParameter>();

        model = new ReflectiveModel(
            key,
            componentTypeList,
            personDisplayType.GUID,
            previousTypeGuid,
            componentPropertyInfoList,
            reflectiveParameterMap,
            dispatcher);

        var chosenComponentChangeCounter = 0;

        model.CalculateComponentPropertyInfoList(
            model.ChosenTypeGuid.ToString(),
            ref chosenComponentChangeCounter);
    }
}