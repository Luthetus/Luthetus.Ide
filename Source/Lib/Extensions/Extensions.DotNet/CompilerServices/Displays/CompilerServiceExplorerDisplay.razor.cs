using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.Models;

namespace Luthetus.Extensions.DotNet.CompilerServices.Displays;

public partial class CompilerServiceExplorerDisplay : ComponentBase
{
	[Inject]
	private IAppOptionsService AppOptionsService { get; set; } = null!;
}
