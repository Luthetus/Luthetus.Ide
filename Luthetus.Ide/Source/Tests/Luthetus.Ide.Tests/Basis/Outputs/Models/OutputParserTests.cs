namespace Luthetus.Ide.Tests.Basis.Outputs.Models;
  
public class OutputParserTests
{
	public List<IOutputLine> Parse(List<string> strList)
	{
		return strList.Select(str => (IOutputLine)new OutputLine(str)).ToList();
	}
}
