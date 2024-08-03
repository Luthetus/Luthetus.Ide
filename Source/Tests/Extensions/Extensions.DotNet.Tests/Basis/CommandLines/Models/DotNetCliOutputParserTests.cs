using Luthetus.Extensions.DotNet.CommandLines.Models;

namespace Luthetus.Extensions.DotNet.Tests.Basis.CommandLines.Models;

public class DotNetCliOutputParserTests
{
	[Fact]
	public void Aaa()
	{
		var dotNetCliOutputParser = new DotNetCliOutputParser();
		
		dotNetCliOutputParser.ParseOutputEntireDotNetRun(
			terminalCommandParsed: null,
			outputEntire: SAMPLE_TEXT);
			
		throw new NotImplementedException();
	}
	
	private const string SAMPLE_TEXT = @"Building...
C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Data\A.cs(1,36): error CS0234: The type or namespace name 'Characters' does not exist in the namespace 'Luthetus.TextEditor.RazorLib' (are you missing an assembly reference?) [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]
C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Program.cs(4,7): warning CS0105: The using directive for 'Microsoft.AspNetCore.Components.Web' appeared previously in this namespace [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]
C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Program.cs(5,7): warning CS0105: The using directive for 'Microsoft.AspNetCore.Components.Web' appeared previously in this namespace [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]
C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Program.cs(6,7): warning CS0105: The using directive for 'Microsoft.AspNetCore.Components.Web' appeared previously in this namespace [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]
C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Persons\IPersonRepository.cs(5,23): error CS0501: 'IPersonRepository.GetPeople()' must declare a body because it is not marked abstract, extern, or partial [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]
C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Data\A.cs(8,63): error CS0246: The type or namespace name 'RichCharacter' could not be found (are you missing a using directive or an assembly reference?) [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]
C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Data\A.cs(139,80): error CS0246: The type or namespace name 'RichCharacter' could not be found (are you missing a using directive or an assembly reference?) [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]
C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Data\A.cs(237,35): error CS0246: The type or namespace name 'RichCharacter' could not be found (are you missing a using directive or an assembly reference?) [C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg.csproj]

The build failed. Fix the build errors and run again.";
}
