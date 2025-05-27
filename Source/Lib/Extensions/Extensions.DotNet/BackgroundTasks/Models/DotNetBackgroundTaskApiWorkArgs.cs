using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Extensions.DotNet.TestExplorers.Models;
using Luthetus.Extensions.DotNet.Nugets.Models;

namespace Luthetus.Extensions.DotNet.BackgroundTasks.Models;

public struct DotNetBackgroundTaskApiWorkArgs
{
	public DotNetBackgroundTaskApiWorkKind WorkKind { get; set; }
	public TreeViewCommandArgs TreeViewCommandArgs { get; set; }
	public TreeViewStringFragment TreeViewStringFragment { get; set; }
	public TreeViewProjectTestModel TreeViewProjectTestModel { get; set; }
	public string FullyQualifiedName { get; set; }
	public INugetPackageManagerQuery NugetPackageManagerQuery { get; set; }
}
