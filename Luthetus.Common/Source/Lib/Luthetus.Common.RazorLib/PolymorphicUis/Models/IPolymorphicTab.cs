using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.PolymorphicUis.Models;

public interface IPolymorphicTab : IPolymorphicUiRecord
{
	public Dictionary<string, object?>? TabParameterMap { get; }
	public bool TabIsActive { get; }

	public string TabGetDynamicCss();
    public Task TabOnClickAsync(MouseEventArgs mouseEventArgs);
	public Task TabCloseAsync();
}
