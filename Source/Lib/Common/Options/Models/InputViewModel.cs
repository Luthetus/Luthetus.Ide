namespace Luthetus.Common.RazorLib.Options.Models;

public class InputViewModel
{
	public static readonly InputViewModel Empty = new(string.Empty, string.Empty);

	public InputViewModel(string cssClass, string cssStyle)
	{
		CssClass = cssClass;
		CssStyle = cssStyle;
	}

	public string CssClass { get; }
	public string CssStyle { get; }
}
