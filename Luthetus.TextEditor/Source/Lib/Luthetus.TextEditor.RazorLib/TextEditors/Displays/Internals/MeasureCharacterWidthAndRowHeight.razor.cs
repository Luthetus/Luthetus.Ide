using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class MeasureCharacterWidthAndRowHeight : ComponentBase
{
    [Parameter, EditorRequired]
    public string HtmlElementId { get; set; } = null!;

    private string _testStringForMeasurement = "abcdefghijklmnopqrstuvwxyz0123456789";
    private int _testStringRepeatCount = 6;

    public int CountOfTestCharacters => _testStringRepeatCount * _testStringForMeasurement.Length;
}