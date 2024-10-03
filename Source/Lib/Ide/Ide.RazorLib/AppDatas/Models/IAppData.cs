using System.Text;

namespace Luthetus.Ide.RazorLib.AppDatas.Models;

public interface IAppData
{
	/// <summary>
	/// The assembly that contains the type being written.
	/// </summary>
	public string AssemblyName { get; }
	
	/// <summary>
	/// The name of the type being written.
	/// </summary>
	public string TypeName { get; }
	
	/// <summary>
	/// Permits storage of many '.json' files for a given type, as opposed to being limited to just 1.
	///
	/// $"{AssemblyName}_{TypeName}_{UniqueIdentifier}.json"
	/// </summary>
	public string? UniqueIdentifier { get; }
	
	public static string CreateRelativePathNoLeadingDelimiter(IAppData appData)
	{
		return CreateRelativePathNoLeadingDelimiter(
			appData.AssemblyName,
			appData.TypeName,
			appData.UniqueIdentifier);
	}
	
	public static string CreateRelativePathNoLeadingDelimiter(
		string assemblyName,
		string typeName,
		string? uniqueIdentifier)
	{
		var filePathBuilder = new StringBuilder($"{assemblyName}_{typeName}");
		
		if (uniqueIdentifier is not null)
			filePathBuilder.Append($"_{uniqueIdentifier}");
		
		filePathBuilder.Append(".json");
	
		return filePathBuilder.ToString();
	}
}
