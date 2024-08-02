namespace Luthetus.Ide.RazorLib.Terminals.Models;

/// <summary>Input Data</summary>
public interface ITerminalInput : IDisposable
{
	public void SendCommand(string commandText);
}
