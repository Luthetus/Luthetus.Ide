using Fluxor;
using Luthetus.Common.RazorLib.ComponentRunners.States;
using Luthetus.Common.RazorLib.ComponentRunners.Internals.Classes;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Reflection;

namespace Luthetus.Common.RazorLib.ComponentRunners;

public partial class ComponentRunnerDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IStateSelection<ComponentRunnerState, ComponentRunnerDisplayState?> ComponentRunnerStateSelection { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<ComponentRunnerDisplayState> ComponentRunnerDisplayStateKey { get; set; }
    [Parameter, EditorRequired]
    public int Index { get; set; }
    [Parameter, EditorRequired]
    public List<Type> ComponentTypeBag { get; set; } = null!;

    private ErrorBoundary _errorBoundaryComponent = null!;
    private ElementReference _selectElementReference;
    private int _chosenComponentChangeCounter;

    protected override void OnInitialized()
    {
        ComponentRunnerStateSelection
            .Select(x => x.ComponentRunnerDisplayStateBag
                .FirstOrDefault(y => y.Key == ComponentRunnerDisplayStateKey));

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
        var displayState = ComponentRunnerStateSelection.Value;

        if (displayState is null)
            return;

        var chosenTypeGuidString = changeEventArgs.Value as string;

        displayState.CalculateComponentPropertyInfoBag(chosenTypeGuidString, ref _chosenComponentChangeCounter);
    }

    private void WrapRecover()
    {
        var displayState = ComponentRunnerStateSelection.Value;

        if (displayState is null)
            return;

        Dispatcher.Dispatch(new ComponentRunnerState.WithAction(
            displayState.Key, inDisplayState => inDisplayState with { }));

        _errorBoundaryComponent.Recover();
    }

    private void DispatchDisposeComponentRunnerDisplayStateAction(ComponentRunnerDisplayState componentRunnerDisplayState)
    {
        Dispatcher.Dispatch(new ComponentRunnerState.DisposeAction(componentRunnerDisplayState.Key));
    }

    private void DispatchRegisterAction(int insertionIndex)
    {
        var componentRunnerDisplayState = new ComponentRunnerDisplayState(
                Key<ComponentRunnerDisplayState>.NewKey(),
                ComponentTypeBag,
                Guid.Empty,
                Guid.Empty,
                Array.Empty<PropertyInfo>(),
                new(),
                Dispatcher);

        Dispatcher.Dispatch(new ComponentRunnerState.RegisterAction(componentRunnerDisplayState, insertionIndex));
    }

    private bool GetIsOptionSelected(ComponentRunnerDisplayState displayState, Guid typeGuid)
    {
        return typeGuid == displayState.ChosenTypeGuid;
    }
}
