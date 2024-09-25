using Luthetus.Ide.RazorLib.AppDatas.Models;

namespace Luthetus.Extensions.DotNet.AppDatas.Models;

public class DotNetAppData : IAppData
{
	public string AssemblyNameFullyQualified { get; } = typeof(DotNetAppData).Assembly.GetName().Name;
	public string? SolutionMostRecent { get; set; }
}
