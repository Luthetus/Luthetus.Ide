namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

/// <summary>Input Data</summary>
public interface ITerminalInput : ITerminalPipe
{
	public void SendCommand(string commandText);
}
