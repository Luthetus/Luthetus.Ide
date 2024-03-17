namespace Luthetus.Common.RazorLib.PolymorphicViewModels.Models;

public interface IDynamicViewModel
{
	public string Title { get; }
	public Type ComponentType { get; }
	public Dictionary<string, object?>? ComponentParameterMap { get; }
}
