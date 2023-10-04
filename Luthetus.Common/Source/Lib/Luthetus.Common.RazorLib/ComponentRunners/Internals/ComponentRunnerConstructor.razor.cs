using Luthetus.Common.RazorLib.ComponentRunners.Internals.Classes;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Luthetus.Common.RazorLib.ComponentRunners.Internals;

public partial class ComponentRunnerConstructor : ComponentBase
{
    [CascadingParameter, EditorRequired]
    public ComponentRunnerDisplayState DisplayState { get; set; } = null!;

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