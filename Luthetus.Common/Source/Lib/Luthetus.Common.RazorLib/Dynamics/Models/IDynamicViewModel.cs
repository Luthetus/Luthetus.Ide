using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dynamics.Models;

public interface IDynamicViewModel
{
	public Key<IDynamicViewModel> DynamicViewModelKey { get; }
	public string Title { get; }
	public Type ComponentType { get; }
	public Dictionary<string, object?>? ComponentParameterMap { get; }
    public string? CssClass { get; set; }
    public string? CssStyle { get; set; }
}
