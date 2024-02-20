namespace Luthetus.Ide.RazorLib.Outputs.Models;
  
public class OutputParser : IOutputParser
{
	public List<IOutputLine> Parse(List<string> strList)
	{
		return strList.Select(str => (IOutputLine)new OutputLine(str)).ToList();
	}
}
