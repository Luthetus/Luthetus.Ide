using System.Text;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.Displays;

public partial class DropdownV2Display : ComponentBase
{
	[Parameter, EditorRequired]
	public DropdownRecord Dropdown { get; set; } = null!;

	public string GetStyleCssString(DropdownRecord localDropdown)
	{
		var styleBuilder = new StringBuilder();

		if (Dropdown.Width is not null)
			styleBuilder.Append($"width: {Dropdown.Width.Value.ToCssValue()}px; ");

		if (Dropdown.Height is not null)
			styleBuilder.Append($"height: {Dropdown.Height.Value.ToCssValue()}px; ");

		styleBuilder.Append($"left: {Dropdown.Left.ToCssValue()}px; ");
		styleBuilder.Append($"top: {Dropdown.Top.ToCssValue()}px; ");

		return styleBuilder.ToString();
	}
}