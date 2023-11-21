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
        var componentRunnerParameter = GetComponentRunnerParameter(localDisplayState);

        componentRunnerParameter.ChosenConstructorInfo = constructorInfo;

        DisplayState.SetParameter(ParametersKey, componentRunnerParameter);
    }

    private void HandleOnUnsetChosenConstructorInfo()
    {
        var localDisplayState = DisplayState;
        var componentRunnerParameter = GetComponentRunnerParameter(localDisplayState);

        componentRunnerParameter.ChosenConstructorInfo = null;

        DisplayState.SetParameter(ParametersKey, componentRunnerParameter);
    }

    private IComponentRunnerParameter GetComponentRunnerParameter(
        ComponentRunnerDisplayState localDisplayState)
    {
        IComponentRunnerParameter defaultValueIfNotExists = ComponentRunnerParameter.ConstructOther(VariableType);

        if (VariableType.IsPrimitive || VariableType == typeof(string))
        {
            if (VariableType == typeof(string))
                defaultValueIfNotExists = ComponentRunnerParameter.ConstructString();
            else if (VariableType == typeof(int))
                defaultValueIfNotExists = ComponentRunnerParameter.ConstructInt();
        }

        return localDisplayState.GetParameter(
            ParametersKey,
            defaultValueIfNotExists);
    }
}