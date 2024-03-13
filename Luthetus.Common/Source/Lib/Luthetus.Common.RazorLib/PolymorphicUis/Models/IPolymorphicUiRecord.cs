using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.PolymorphicUis.Models;

public interface IPolymorphicUiRecord
{
    public Key<IPolymorphicUiRecord> PolymorphicUiKey { get; }
	public string? CssClass { get; }
	public string? CssStyle { get; }
	public string Title { get; }
	public Type RendererType { get; }
}
