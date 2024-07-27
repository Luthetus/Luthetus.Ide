namespace Luthetus.Ide.RazorLib.Terminals.Models;

/// <summary>input -> CliWrap -> output</summary>
public interface ITerminal
{
	public string DisplayName { get; }
	public ITerminalInteractive TerminalInteractive { get; }
	public ITerminalInput TerminalInput { get; }
	public ITerminalOutput TerminalOutput { get; }
}

/// <summary>State (i.e.: working directory)</summary>
public interface ITerminalInteractive
{
	private string? _workingDirectoryAbsolutePathString;
	public string? WorkingDirectoryAbsolutePathString => _workingDirectoryAbsolutePathString;
	
	public void SetWorkingDirectory();
}

/// <summary>Transferral of Data</summary>
public interface ITerminalPipe
{
	public void OnAfterWorkingDirectoryChanged(string workingDirectoryAbsolutePathString);
	public void OnHandleCommandStarting();
}

/// <summary>Input Data</summary>
public interface ITerminalInput : ITerminalPipe
{
}

/// <summary>Output Data</summary>
public interface ITerminalOutput : ITerminalPipe
{
}

/// <summary>
/// This implementation of <see cref="ITerminal"/> is a "blank slate".
/// </summary>
public class Terminal : ITerminal
{
	public Terminal(
		string displayName,
		ITerminalInteractive terminalInteractive,
		ITerminalInput terminalInput,
		ITerminalOutput terminalOutput)
	{
		DisplayName = displayName;
		TerminalInteractive = terminalInteractive;
		TerminalInput = terminalInput;
		TerminalOutput = terminalOutput;
	}

	public string DisplayName { get; }
	public ITerminalInteractive TerminalInteractive { get; }
	public ITerminalInput TerminalInput { get; }
	public ITerminalOutput TerminalOutput { get; }
}

/// <summary>
/// This implementation of <see cref="ITerminal"/> is a specific implementation
/// mean for the "Execution" terminal in the IDE.
/// </summary>
public class TerminalExecution : ITerminal
{
	public TerminalExecution(
		string displayName,
		ITerminalInteractive terminalInteractive,
		ITerminalInput terminalInput,
		ITerminalOutput terminalOutput)
	{
		DisplayName = "Execution";
		TerminalInteractive = terminalInteractive;
		TerminalInput  = new TerminalInputTextEditor(/*ITerminal*/ this);
		TerminalOutput = new TerminalOutputTextEditor(/*ITerminal*/ this);
	}

	public string DisplayName { get; }
	public ITerminalInteractive TerminalInteractive { get; }
	public ITerminalInput TerminalInput { get; }
	public ITerminalOutput TerminalOutput { get; }
}

/// <summary>
/// This implementation of <see cref="ITerminal"/> is a specific implementation
/// mean for the "General" terminal in the IDE.
/// </summary>
public class TerminalGeneral : ITerminal
{
}

public class TerminalInputTextEditor : ITerminalInput
{
	private readonly ITerminal _terminal;

	public TerminalInputTextEditor(ITerminal terminal)
	{
		_terminal = terminal;
	}

	public void OnAfterWorkingDirectoryChanged(string workingDirectoryAbsolutePathString)
	{
	}
	
	public void OnHandleCommandStarting()
	{
	}
}

public class TerminalOutputTextEditor : ITerminalOutput
{
	private readonly ITerminal _terminal;

	public TerminalInputTextEditor(ITerminal terminal)
	{
		_terminal = terminal;
	}

	public void OnAfterWorkingDirectoryChanged(string workingDirectoryAbsolutePathString)
	{
	}
	
	public void OnHandleCommandStarting()
	{
	}
}

public class TerminalInputStringBuilder : ITerminalInput
{
	private readonly ITerminal _terminal;

	public TerminalInputTextEditor(ITerminal terminal)
	{
		_terminal = terminal;
	}

	public void OnAfterWorkingDirectoryChanged(string workingDirectoryAbsolutePathString)
	{
	}
	
	public void OnHandleCommandStarting()
	{
	}
}

public class TerminalOutputStringBuilder : ITerminalOutput
{
	private readonly ITerminal _terminal;

	public TerminalInputTextEditor(ITerminal terminal)
	{
		_terminal = terminal;
	}

	public void OnAfterWorkingDirectoryChanged(string workingDirectoryAbsolutePathString)
	{
	}
	
	public void OnHandleCommandStarting()
	{
	}
}

// Terminal                # Take string, invoke CliWrap, ???
//                               Aaa
//     TerminalInteractive # Maintain state (i.e.: the working directory)
//                               Is one to emulate shells or somehow run the
//                                   corresponding shell and somehow render the UI in the IDE?
//     TerminalInput       # Is this necessary? (I'm thinking of the ICompilerService)
//                               I think the best implementation will result
//                                   from me creating a <textarea/> and <input type="text"/>
//                                   implementation of a terminal in addition to the
//                                   readonly text editor model, and single line modifiable text editor model.
//     TerminalOutput      # Store the output
//                               Aaa

// I'm thinking and, it seems that TerminalInput and TerminalOutput perfectly map to the
// new idea I have of two text editor models.
//
// The editable single line model would be the TerminalInput and the TerminalOutput
// would be the readonly model.
//
// And, I could just the same create a different implementation where
// an <input type="text"/> is the TerminalInput, and a <textarea/> is the TerminalOutput.
//
// The 'TerminalInteractive' could be an emulator for 'bash' or 'cmd', etc...
// But, I also wonder, do other integrated terminals emulate these shells,
// or are they somehow rendering the direct output of the shell running in the background.
