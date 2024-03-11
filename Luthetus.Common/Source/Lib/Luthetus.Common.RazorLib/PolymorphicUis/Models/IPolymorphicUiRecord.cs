using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.PolymorphicUis.Models;

public interface IPolymorphicUiRecord
{
    public Key<IPolymorphicUiRecord> Key { get; }
	public string? CssClass { get; }
	public string? CssStyle { get; }
	public string Title { get; }
	public Type RendererType { get; }
	public Dictionary<string, object?>? ParameterMap { get; }
}
