using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalOutputFormatterAll : ITerminalOutputFormatter
{
	public static Guid Id { get; } = Guid.NewGuid();

	public static ResourceUri TextEditorModelResourceUri { get; } = new(
        ResourceUriFacts.Terminal_ReservedResourceUri_Prefix + Id.ToString());

    public static Key<TextEditorViewModel> TextEditorViewModelKey { get; } = new Key<TextEditorViewModel>(Id);

	private readonly ITerminal _terminal;
	private readonly TextEditorService _textEditorService;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;

	public TerminalOutputFormatterAll(
		ITerminal terminal,
		TextEditorService textEditorService,
		ICompilerServiceRegistry compilerServiceRegistry)
	{
		_terminal = terminal;
		_textEditorService = textEditorService;
		_compilerServiceRegistry = compilerServiceRegistry;
	}

	public string Name { get; } = nameof(TerminalOutputFormatterAll);
	
	public ITerminalOutputFormatted Format()
	{
		return new TerminalOutputFormattedTextEditor(
			string.Empty,
			new List<TerminalCommandParsed>(),
			new List<TextEditorTextSpan>(),
			new List<Symbol>());
	}
	
	public void Dispose()
	{
	}
}
