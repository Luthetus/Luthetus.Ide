using Luthetus.Common.RazorLib.Reflectives.Models;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Luthetus.Common.RazorLib.Reflectives.Displays;

public partial class ReflectiveVariable : ComponentBase
{
    [CascadingParameter, EditorRequired]
    public ReflectiveModel Model { get; set; } = null!;

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
        var localModel = Model;
        var reflectiveParameter = GetParameter(localModel);

        reflectiveParameter.ChosenConstructorInfo = constructorInfo;

        Model.SetParameter(ParametersKey, reflectiveParameter);
    }

    private void HandleOnUnsetChosenConstructorInfo()
    {
        var localModel = Model;
        var reflectiveParameter = GetParameter(localModel);

        reflectiveParameter.ChosenConstructorInfo = null;

        Model.SetParameter(ParametersKey, reflectiveParameter);
    }

    private IReflectiveParameter GetParameter(ReflectiveModel localModel)
    {
        IReflectiveParameter defaultValueIfNotExists = ReflectiveParameter.ConstructOther(VariableType);

        if (VariableType.IsPrimitive || VariableType == typeof(string))
        {
            if (VariableType == typeof(string))
                defaultValueIfNotExists = ReflectiveParameter.ConstructString();
            else if (VariableType == typeof(int))
                defaultValueIfNotExists = ReflectiveParameter.ConstructInt();
        }

        return localModel.GetParameter(
            ParametersKey,
            defaultValueIfNotExists);
    }
}