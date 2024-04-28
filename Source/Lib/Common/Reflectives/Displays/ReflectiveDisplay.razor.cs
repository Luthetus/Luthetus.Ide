using Fluxor;
using Luthetus.Common.RazorLib.Reflectives.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Reflection;
using Luthetus.Common.RazorLib.Reflectives.Models;

namespace Luthetus.Common.RazorLib.Reflectives.Displays;

public partial class ReflectiveDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IStateSelection<ReflectiveState, ReflectiveModel?> ReflectiveStateSelection { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<ReflectiveModel> ReflectiveModelKey { get; set; }
    [Parameter, EditorRequired]
    public int Index { get; set; }
    [Parameter, EditorRequired]
    public List<Type> ComponentTypeList { get; set; } = null!;

    private ErrorBoundary _errorBoundaryComponent = null!;
    private ElementReference _selectElementReference;
    private int _chosenComponentChangeCounter;

    protected override void OnInitialized()
    {
        ReflectiveStateSelection
            .Select(x => x.ReflectiveModelList
                .FirstOrDefault(y => y.Key == ReflectiveModelKey));

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_chosenComponentChangeCounter > 0)
        {
            _chosenComponentChangeCounter = 0;

            // Invoke 'Recover' directly, do not use 'WrapRecover' here.
            _errorBoundaryComponent.Recover();

            await _selectElementReference.FocusAsync();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private void OnSelectChanged(ChangeEventArgs changeEventArgs)
    {
        var model = ReflectiveStateSelection.Value;

        if (model is null)
            return;

        var chosenTypeGuidString = changeEventArgs.Value as string;

        model.CalculateComponentPropertyInfoList(chosenTypeGuidString, ref _chosenComponentChangeCounter);
    }

    private void WrapRecover()
    {
        var displayState = ReflectiveStateSelection.Value;

        if (displayState is null)
            return;

        Dispatcher.Dispatch(new ReflectiveState.WithAction(
            displayState.Key, inDisplayState => inDisplayState with { }));

        _errorBoundaryComponent.Recover();
    }

    private void DispatchDisposeAction(ReflectiveModel reflectiveModel)
    {
        Dispatcher.Dispatch(new ReflectiveState.DisposeAction(reflectiveModel.Key));
    }

    private void DispatchRegisterAction(int insertionIndex)
    {
        var model = new ReflectiveModel(
                Key<ReflectiveModel>.NewKey(),
                ComponentTypeList,
                Guid.Empty,
                Guid.Empty,
                Array.Empty<PropertyInfo>(),
                new(),
                Dispatcher);

        Dispatcher.Dispatch(new ReflectiveState.RegisterAction(model, insertionIndex));
    }

    private bool GetIsOptionSelected(ReflectiveModel model, Guid typeGuid)
    {
        return typeGuid == model.ChosenTypeGuid;
    }
}
