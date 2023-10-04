using Luthetus.Common.RazorLib.ComponentRunners.Internals.Classes;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Luthetus.Common.RazorLib.ComponentRunners.Internals;

public partial class ComponentRunnerVariable : ComponentBase
{
    [CascadingParameter, EditorRequired]
    public ComponentRunnerDisplayState DisplayState { get; set; } = null!;

    [Parameter, EditorRequired]
    public string ParametersKey { get; set; } = null!;
    [Parameter, EditorRequired]
    public string VariableName { get; set; } = null!;
    [Parameter, EditorRequired]
    public Type VariableType { get; set; } = null!;

    [Parameter]
    public bool IsProperty { get; set; }

    private string VariableNameCssClass => IsProperty
        ? "luth_te_property"
        : "luth_te_variable";

    private void HandleConstructorOnClick(ConstructorInfo constructorInfo)
    {
        var localDisplayState = DisplayState;
        var componentRunnerType = GetComponentRunnerType(localDisplayState);

        componentRunnerType.ChosenConstructorInfo = constructorInfo;

        DisplayState.SetComponentRunnerType(ParametersKey, componentRunnerType);
    }

    private void HandleOnUnsetChosenConstructorInfo()
    {
        var localDisplayState = DisplayState;
        var componentRunnerType = GetComponentRunnerType(localDisplayState);

        componentRunnerType.ChosenConstructorInfo = null;

        DisplayState.SetComponentRunnerType(ParametersKey, componentRunnerType);
    }

    private IComponentRunnerType GetComponentRunnerType(
        ComponentRunnerDisplayState localDisplayState)
    {
        IComponentRunnerType defaultValueIfNotExists = new ComponentRunnerPrimitiveType(
            null,
            () => null,
            null,
            VariableType);

        if (VariableType.IsPrimitive || VariableType == typeof(string))
        {
            if (VariableType == typeof(string))
            {
                defaultValueIfNotExists = new ComponentRunnerPrimitiveType(
                    null,
                    () => null,
                    null,
                    typeof(string));
            }
            else if (VariableType == typeof(int))
            {
                defaultValueIfNotExists = new ComponentRunnerPrimitiveType(
                    null,
                    () => default,
                    default,
                    typeof(int));
            }
        }

        return localDisplayState.GetComponentRunnerType(
            ParametersKey,
            defaultValueIfNotExists);
    }
}