using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Ide.RazorLib.Commands;

public interface ICommandFactory
{
	public IDialog? CodeSearchDialog { get; set; }

    public void Initialize();
    public ValueTask OpenCodeSearchDialog();
}
