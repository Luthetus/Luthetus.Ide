using Fluxor;
using Luthetus.Common.RazorLib.Reflectives.Models;
using Luthetus.Common.RazorLib.Reflectives.PersonCase;
using Luthetus.Common.RazorLib.Reflectives.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Luthetus.Common.Tests.Basis.Reflectives.Models;

/// <summary>
/// <see cref="ReflectiveModel"/>
/// </summary>
public class ReflectiveModelTests
{
    /// <summary>
    /// <see cref="ReflectiveModel(Key{ReflectiveModel}, List{Type}, Guid, Guid, System.Reflection.PropertyInfo[], Dictionary{string, IReflectiveParameter}, IDispatcher)"/>
    /// <br/>----<br/>
    /// <see cref="ReflectiveModel.ChosenType"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        // InitializeReflectiveModelTests has an out variable for a ReflectiveModel.
        //
        // Do not use that out variable for the Constructor test however,
        // since the inner details must be tested and asserted within this unit test itself.
        InitializeReflectiveModelTests(out var dispatcher, out _);

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

        var model = new ReflectiveModel(
            key,
            componentTypeList,
            personDisplayType.GUID,
            previousTypeGuid,
            componentPropertyInfoList,
            reflectiveParameterMap,
            dispatcher);

        Assert.Equal(key, model.Key);
        Assert.Equal(componentTypeList, model.ComponentTypeList);
        Assert.Equal(personDisplayType.GUID, model.ChosenTypeGuid);
        Assert.Equal(previousTypeGuid, model.PreviousTypeGuid);
        Assert.Equal(componentPropertyInfoList, model.ComponentPropertyInfoList);
        Assert.Equal(reflectiveParameterMap, model.ReflectiveParameterMap);
        Assert.Equal(dispatcher, model.Dispatcher);
        Assert.Equal(personDisplayType, model.ChosenType);
    }

    /// <summary>
    /// <see cref="ReflectiveModel.ConstructBlazorParameters()"/>
    /// <br/>----<br/>
    /// <see cref="ReflectiveModel.GetParameter(string, IReflectiveParameter)"/>
    /// <see cref="ReflectiveModel.SetParameter(string, IReflectiveParameter)"/>
    /// </summary>
    [Fact]
    public void ConstructBlazorParameters()
    {
        InitializeReflectiveModelTests(out var dispatcher, out var model);

        var reflectiveParameter = model.GetParameter(
            nameof(PersonDisplay.PersonModel),
            ReflectiveParameter.ConstructOther(typeof(PersonModel)));

        Assert.Null(reflectiveParameter.Value);

        model.SetParameter(
            nameof(PersonDisplay.PersonModel),
            ReflectiveParameter.ConstructOther(typeof(PersonModel)));

        throw new NotImplementedException();
    }

    private void InitializeReflectiveModelTests(
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
