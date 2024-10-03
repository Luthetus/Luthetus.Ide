using Luthetus.Ide.RazorLib.AppDatas.Models;

namespace Luthetus.Extensions.DotNet.AppDatas.Models;

public class DotNetAppData : IAppData
{
	public static readonly string AssemblyName = typeof(DotNetAppData).Assembly.GetName().Name;
	public static readonly string TypeName = nameof(DotNetAppData);

	string IAppData.AssemblyName => AssemblyName;
	string IAppData.TypeName => TypeName;
	public string? UniqueIdentifier { get; }
	
	public string? SolutionMostRecent { get; set; }
}
