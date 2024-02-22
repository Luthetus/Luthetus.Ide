namespace Luthetus.Ide.RazorLib.Outputs.Models;

public interface IOutputParser
{
	public List<IOutputLine> Parse(List<string> text);
}
