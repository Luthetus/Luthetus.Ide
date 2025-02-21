using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.StartupControls.Models;

/// <summary>
/// TODO: Investigate making this a record struct
/// </summary>
public record StartupControlState(
	Key<IStartupControlModel> ActiveStartupControlKey,
	IReadOnlyList<IStartupControlModel> StartupControlList)
{
	public StartupControlState() : this(
		Key<IStartupControlModel>.Empty,
		Array.Empty<IStartupControlModel>())
	{
	}
	
	public IStartupControlModel ActiveStartupControl => StartupControlList.FirstOrDefault(
		x => x.Key == ActiveStartupControlKey);
}
