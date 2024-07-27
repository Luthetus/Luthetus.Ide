using System.Text;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;

namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

/// <summary>Aaa</summary>
public class TerminalInteractive : ITerminalInteractive
{
	private readonly ITerminal _terminal;

	public TerminalInteractive(ITerminal terminal)
	{
		_terminal = terminal;
	}

	private string? _previousWorkingDirectoryAbsolutePathString;
	private string? _workingDirectoryAbsolutePathString;
	
	public string? WorkingDirectoryAbsolutePathString => _workingDirectoryAbsolutePathString;

	public event Action? WorkingDirectoryChanged;
	
	public string? TryHandleCommand(string commandText)
	{
		var parsedCommand = Parse(commandText);
		
		Console.WriteLine($"TargetFileName: {parsedCommand.TargetFileName}");
		Console.WriteLine($"Arguments: {parsedCommand.Arguments}");
		
		if (parsedCommand.TargetFileName == "cd")
		{
			SetWorkingDirectory(parsedCommand.Arguments);
			return null;
		}
	
		return commandText;
		/*
		if (terminalCommand.ChangeWorkingDirectoryTo is not null)
			TerminalInteractive.

		if (terminalCommand.FormattedCommand.TargetFileName == "cd")
		{
			// TODO: Don't keep this logic as it is hacky. I'm trying to set myself up to be able to run "gcc" to compile ".c" files. Then I can work on adding symbol related logic like "go to definition" or etc.
			if (terminalCommand.FormattedCommand.HACK_ArgumentsString is not null)
				SetWorkingDirectoryAbsolutePathString(terminalCommand.FormattedCommand.HACK_ArgumentsString);
			else if (terminalCommand.FormattedCommand.ArgumentsList.Any())
				SetWorkingDirectoryAbsolutePathString(terminalCommand.FormattedCommand.ArgumentsList.ElementAt(0));

			return;
		}

		if (terminalCommand.FormattedCommand.TargetFileName == "clear")
		{
			ClearTerminal();
			WriteWorkingDirectory();
			return;
		}
		*/
	}
	
	public void SetWorkingDirectory(string workingDirectoryAbsolutePathString)
	{
		_previousWorkingDirectoryAbsolutePathString = _workingDirectoryAbsolutePathString;
        _workingDirectoryAbsolutePathString = workingDirectoryAbsolutePathString;

        if (_previousWorkingDirectoryAbsolutePathString != _workingDirectoryAbsolutePathString)
            WorkingDirectoryChanged?.Invoke();
	}
	
	public TerminalCommandParsed Parse(string commandText)
	{
		try
		{
			var stringWalker = new StringWalker(ResourceUri.Empty, commandText);
			
			// Get target file name
			string targetFileName;
			{
				var targetFileNameBuilder = new StringBuilder();
				var startPositionIndex = stringWalker.PositionIndex;
		
				while (!stringWalker.IsEof)
				{
					if (WhitespaceFacts.ALL_LIST.Contains(stringWalker.CurrentCharacter))
						break;
					else
						targetFileNameBuilder.Append(stringWalker.CurrentCharacter);
				
					_ = stringWalker.ReadCharacter();
				}
				
				targetFileName = targetFileNameBuilder.ToString();
			}
			
			// Get arguments
			stringWalker.ReadWhitespace();
			var arguments = stringWalker.RemainingText;
		
		
			return new TerminalCommandParsed(
				targetFileName,
				arguments);
		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
			throw;
		}
	}
	
	public void Dispose()
	{
		return;
	}
}
