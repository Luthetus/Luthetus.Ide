using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.StartupControls.Models;

namespace Luthetus.Ide.RazorLib.StartupControls.States;

[FeatureState]
public partial record StartupControlState(
	Key<IStartupControlModel> ActiveStartupControlKey,
	ImmutableList<IStartupControlModel> StartupControlList)
{
	public StartupControlState() : this(
		Key<IStartupControlModel>.Empty,
		ImmutableList<IStartupControlModel>.Empty)
	{
	}
	
	public IStartupControlModel ActiveStartupControl => StartupControlList.FirstOrDefault(
		x => x.Key == ActiveStartupControlKey);
}
