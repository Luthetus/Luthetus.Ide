using Fluxor;
using Luthetus.Common.RazorLib.Reflectives.Models;
using Luthetus.Common.RazorLib.Reflectives.States;
using Luthetus.Common.RazorLib.Icons.Displays.Codicon;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Luthetus.Common.Tests.Basis.Reflectives.States;

/// <summary>
/// <see cref="ReflectiveState"/>
/// </summary>
public class ReflectiveStateActionsTests
{
    /// <summary>
    /// <see cref="ReflectiveState.RegisterAction"/>
    /// </summary>
    [Fact]
    public void RegisterAction()
    {
        InitializeReflectiveStateActionsTests(out var model);

        var insertionIndex = 0;

        var registerAction = new ReflectiveState.RegisterAction(model, insertionIndex);
        Assert.Equal(model, registerAction.Entry);
        Assert.Equal(insertionIndex, registerAction.InsertionIndex);
    }

    /// <summary>
    /// <see cref="ReflectiveState.DisposeAction"/>
    /// </summary>
    [Fact]
    public void DisposeAction()
    {
        InitializeReflectiveStateActionsTests(out var model);

        var disposeAction = new ReflectiveState.DisposeAction(model.Key);
        Assert.Equal(model.Key, disposeAction.Key);
    }

    /// <summary>
    /// <see cref="ReflectiveState.WithAction"/>
    /// </summary>
    [Fact]
    public void WithAction()
    {
        InitializeReflectiveStateActionsTests(out var model);

        Func<ReflectiveModel, ReflectiveModel> withFunc = inState =>
        {
            return inState;
        };

        var withAction = new ReflectiveState.WithAction(
            model.Key,
            withFunc);

        Assert.Equal(model.Key, withAction.Key);
        Assert.Equal(withFunc, withAction.WithFunc);
    }

    private void InitializeReflectiveStateActionsTests(out ReflectiveModel model)
    {
        var services = new ServiceCollection()
            .AddFluxor(options => options.ScanAssemblies(typeof(ReflectiveState).Assembly));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        var componentTypeList = new List<Type>
        {
            typeof(IconArrowDownFragment),
            typeof(IconArrowLeftFragment),
            typeof(IconArrowRightFragment),
            typeof(IconArrowUpFragment),
        };

        model = new ReflectiveModel(
                Key<ReflectiveModel>.NewKey(),
                componentTypeList,
                Guid.Empty,
                Guid.Empty,
                Array.Empty<PropertyInfo>(),
                new(),
                dispatcher);
    }
}