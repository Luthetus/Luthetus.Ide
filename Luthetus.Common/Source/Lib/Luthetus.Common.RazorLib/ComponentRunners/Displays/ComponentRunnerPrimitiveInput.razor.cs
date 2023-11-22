using Luthetus.Common.RazorLib.ComponentRunners.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.ComponentRunners.Displays;

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
            var componentRunnerParameter = DisplayState.GetParameter(
                ParametersKey,
                ComponentRunnerParameter.ConstructString());

            return componentRunnerParameter.Value is null
                ? string.Empty
                : (string)componentRunnerParameter.Value;
        }
        set
        {
            var componentRunnerParameter = DisplayState.GetParameter(
                ParametersKey,
                ComponentRunnerParameter.ConstructString());

            componentRunnerParameter.Value = value;

            DisplayState.SetParameter(ParametersKey, componentRunnerParameter);
        }
    }

    private int IntInput
    {
        get
        {
            var componentRunnerParameter = DisplayState.GetParameter(
                ParametersKey,
                ComponentRunnerParameter.ConstructInt());

            return componentRunnerParameter.Value is null
                ? 0
                : (int)componentRunnerParameter.Value;
        }
        set
        {
            var componentRunnerParameter = DisplayState.GetParameter(
                ParametersKey,
                ComponentRunnerParameter.ConstructInt());

            componentRunnerParameter.Value = value;

            DisplayState.SetParameter(ParametersKey, componentRunnerParameter);
        }
    }
}