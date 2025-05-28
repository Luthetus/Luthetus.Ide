using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.CompilerServices.DotNetSolution.Models;
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
	public Key<DotNetSolutionModel> DotNetSolutionModelKey { get; set; }
	public string ProjectTemplateShortName { get; set; }
	public string CSharpProjectName { get; set; }
	public AbsolutePath CSharpProjectAbsolutePath { get; set; }
    public AbsolutePath DotNetSolutionAbsolutePath { get; set; }
}
