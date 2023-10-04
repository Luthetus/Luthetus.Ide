using Luthetus.Common.RazorLib.ComponentRunners.Internals.Classes;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.ComponentRunners.Internals;

public partial class ComponentRunnerPrimitiveInput : ComponentBase
{
    [CascadingParameter, EditorRequired]
    public ComponentRunnerDisplayState DisplayState { get; set; } = null!;

    [Parameter, EditorRequired]
    public string ParametersKey { get; set; } = null!;
    [Parameter, EditorRequired]
    public string VariableName { get; set; } = null!;
    [Parameter, EditorRequired]
    public Type VariableType { get; set; } = null!;

    private string StringInput
    {
        get
        {
            var componentRunnerType = DisplayState
                .GetComponentRunnerType(ParametersKey, new ComponentRunnerPrimitiveType(
                    null,
                    () => null,
                    null,
                    typeof(string)));

            return componentRunnerType.Value is null
                ? string.Empty
                : (string)componentRunnerType.Value;
        }
        set
        {
            var componentRunnerType = DisplayState
                .GetComponentRunnerType(ParametersKey, new ComponentRunnerPrimitiveType(
                    null,
                    () => null,
                    null,
                    typeof(string)));

            componentRunnerType.Value = value;

            DisplayState.SetComponentRunnerType(ParametersKey, componentRunnerType);
        }
    }

    private int IntInput
    {
        get
        {
            var componentRunnerType = DisplayState
                .GetComponentRunnerType(ParametersKey, new ComponentRunnerPrimitiveType(
                    null,
                    () => default,
                    default,
                    typeof(int)));

            return componentRunnerType.Value is null
                ? 0
                : (int)componentRunnerType.Value;
        }
        set
        {
            var componentRunnerType = DisplayState
                .GetComponentRunnerType(ParametersKey, new ComponentRunnerPrimitiveType(
                    null,
                    () => default,
                    default,
                    typeof(int)));

            componentRunnerType.Value = value;

            DisplayState.SetComponentRunnerType(ParametersKey, componentRunnerType);
        }
    }
}