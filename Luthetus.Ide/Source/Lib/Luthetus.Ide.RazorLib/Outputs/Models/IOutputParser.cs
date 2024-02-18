using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Luthetus.Ide.RazorLib.Outputs.Models;

public interface IOutputParser
{
	public List<IOutputLine> Parse(List<string> text);
}
