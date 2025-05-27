using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.CompilerServices.DotNetSolution.Models;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Models;

public struct DotNetSolutionIdeWorkArgs
{
	public DotNetSolutionIdeWorkKind WorkKind { get; set; }
	public Key<DotNetSolutionModel> DotNetSolutionModelKey { get; set; }
	public string ProjectTemplateShortName { get; set; }
	public string CSharpProjectName { get; set; }
	public AbsolutePath CSharpProjectAbsolutePath { get; set; }
    public AbsolutePath DotNetSolutionAbsolutePath { get; set; }
}
