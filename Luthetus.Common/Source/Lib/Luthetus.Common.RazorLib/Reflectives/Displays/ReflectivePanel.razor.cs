using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Reflectives.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using Luthetus.Common.RazorLib.Reflectives.Models;

namespace Luthetus.Common.RazorLib.Reflectives.Displays;

public partial class ReflectivePanel : FluxorComponent
{
    [Inject]
    private IState<ReflectiveState> ReflectiveStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public List<Type> ComponentTypeList { get; set; } = null!;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var model = new ReflectiveModel(
                Key<ReflectiveModel>.NewKey(),
                ComponentTypeList,
                ComponentTypeList.FirstOrDefault(x => x.Name == "SolutionExplorerDisplay")?.GUID ?? Guid.Empty,
                Guid.Empty,
                Array.Empty<PropertyInfo>(),
                new(),
                Dispatcher);

            Dispatcher.Dispatch(new ReflectiveState.RegisterAction(model, 0));
        }

        return base.OnAfterRenderAsync(firstRender);
    }

    private void DispatchRegisterActionOnClick()
    {
        var model = new ReflectiveModel(
            Key<ReflectiveModel>.NewKey(),
            ComponentTypeList,
            Guid.Empty,
            Guid.Empty,
            Array.Empty<PropertyInfo>(),
            new(),
            Dispatcher);

        Dispatcher.Dispatch(new ReflectiveState.RegisterAction(model, 0));
    }
}