using Luthetus.Common.RazorLib.Reflectives.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Reflectives.Displays;

public partial class ReflectivePrimitiveInput : ComponentBase
{
    [CascadingParameter, EditorRequired]
    public ReflectiveModel DisplayState { get; set; } = null!;

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
            var reflectiveParameter = DisplayState.GetParameter(
                ParametersKey,
                ReflectiveParameter.ConstructString());

            return reflectiveParameter.Value is null
                ? string.Empty
                : (string)reflectiveParameter.Value;
        }
        set
        {
            var reflectiveParameter = DisplayState.GetParameter(
                ParametersKey,
                ReflectiveParameter.ConstructString());

            reflectiveParameter.Value = value;

            DisplayState.SetParameter(ParametersKey, reflectiveParameter);
        }
    }

    private int IntInput
    {
        get
        {
            var reflectiveParameter = DisplayState.GetParameter(
                ParametersKey,
                ReflectiveParameter.ConstructInt());

            return reflectiveParameter.Value is null
                ? 0
                : (int)reflectiveParameter.Value;
        }
        set
        {
            var reflectiveParameter = DisplayState.GetParameter(
                ParametersKey,
                ReflectiveParameter.ConstructInt());

            reflectiveParameter.Value = value;

            DisplayState.SetParameter(ParametersKey, reflectiveParameter);
        }
    }
}