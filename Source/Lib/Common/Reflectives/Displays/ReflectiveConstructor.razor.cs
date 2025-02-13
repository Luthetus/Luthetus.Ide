using System.Reflection;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Reflectives.Models;
using Luthetus.Common.RazorLib.Options.Models;

namespace Luthetus.Common.RazorLib.Reflectives.Displays;

public partial class ReflectiveConstructor : ComponentBase
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;
    
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