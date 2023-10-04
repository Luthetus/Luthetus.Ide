using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.ComponentRunners.States;
using Luthetus.Common.RazorLib.ComponentRunners.Internals.Classes;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Luthetus.Common.RazorLib.ComponentRunners;

public partial class ComponentRunnerPanel : FluxorComponent
{
    [Inject]
    private IState<ComponentRunnerState> ComponentRunnerStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public List<Type> ComponentTypeBag { get; set; } = null!;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var componentRunnerDisplayState = new ComponentRunnerDisplayState(
                Key<ComponentRunnerDisplayState>.NewKey(),
                ComponentTypeBag,
                ComponentTypeBag.FirstOrDefault(x => x.Name == "SolutionExplorerDisplay")?.GUID ?? Guid.Empty,
                Guid.Empty,
                Array.Empty<PropertyInfo>(),
                new(),
                Dispatcher);

            Dispatcher.Dispatch(new ComponentRunnerState.RegisterAction(componentRunnerDisplayState, 0));
        }

        return base.OnAfterRenderAsync(firstRender);
    }

    private void DispatchRegisterActionOnClick()
    {
        var componentRunnerDisplayState = new ComponentRunnerDisplayState(
            Key<ComponentRunnerDisplayState>.NewKey(),
            ComponentTypeBag,
            Guid.Empty,
            Guid.Empty,
            Array.Empty<PropertyInfo>(),
            new(),
            Dispatcher);

        Dispatcher.Dispatch(new ComponentRunnerState.RegisterAction(componentRunnerDisplayState, 0));
    }
}