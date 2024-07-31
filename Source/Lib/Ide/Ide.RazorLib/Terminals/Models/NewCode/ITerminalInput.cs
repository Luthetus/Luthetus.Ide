namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

/// <summary>Input Data</summary>
public interface ITerminalInput : IDisposable
{
	public void SendCommand(string commandText);
}
