using System.Collections.Immutable;
using Fluxor;
using Luthetus.Ide.RazorLib.StartupControls.Models;

namespace Luthetus.Ide.RazorLib.StartupControls.States;

[FeatureState]
public partial record StartupControlState(
	IStartupControlModel? ActiveStartupControl,
	ImmutableList<IStartupControlModel> StartupControlList)
{
	public StartupControlState() : this(
		null,
		ImmutableList<IStartupControlModel>.Empty)
	{
	}
}
