using Microsoft.AspNetCore.Components;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Options.Models;

namespace Luthetus.Extensions.DotNet.CompilerServices.Displays;

public partial class CompilerServiceExplorerDisplay : FluxorComponent
{
	[Inject]
	private IAppOptionsService AppOptionsService { get; set; } = null!;
}
