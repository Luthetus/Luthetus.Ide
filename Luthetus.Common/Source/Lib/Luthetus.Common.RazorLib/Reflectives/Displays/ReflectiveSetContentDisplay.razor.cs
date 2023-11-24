using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Luthetus.Common.RazorLib.Reflectives.Displays;

public partial class ReflectiveSetContentDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ParameterInfo ParameterInfo { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<string, object?> OnChangeAction { get; set; } = null!;

    public static readonly Key<object> TextEditorModelKey = Key<object>.NewKey();
    public static readonly Key<object> TextEditorViewModelKey = Key<object>.NewKey();

    private string _input = string.Empty;

    public string Input
    {
        get => _input;
        set
        {
            _input = value;
            OnChangeAction.Invoke(ParameterInfo.Name ?? string.Empty, _input);
        }
    }
}