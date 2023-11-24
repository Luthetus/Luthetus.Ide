using Luthetus.Common.RazorLib.Reflectives.Models;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Luthetus.Common.RazorLib.Reflectives.Displays;

public partial class ReflectiveConstructor : ComponentBase
{
    [CascadingParameter, EditorRequired]
    public ReflectiveModel DisplayState { get; set; } = null!;

    [Parameter, EditorRequired]
    public ConstructorInfo ConstructorInfo { get; set; } = null!;
    [Parameter, EditorRequired]
    public bool IsChosenConstructor { get; set; }
    [Parameter, EditorRequired]
    public Action<ConstructorInfo> OnClick { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action OnUnsetChosenConstructorInfo { get; set; } = null!;
    [Parameter, EditorRequired]
    public string ParametersKey { get; set; } = null!;

    private string IsActiveCssClass => IsChosenConstructor
        ? "luth_active"
        : string.Empty;

    private void InvokeOnClick()
    {
        OnClick.Invoke(ConstructorInfo);
    }

    private void InvokeOnUnsetChosenConstructorInfo()
    {
        OnUnsetChosenConstructorInfo.Invoke();
    }
}