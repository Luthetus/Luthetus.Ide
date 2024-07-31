using System.Text;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalOutputFormatterAll : ITerminalOutputFormatter
{
	public static Guid Id { get; } = Guid.NewGuid();

	public static ResourceUri TextEditorModelResourceUri { get; } = new(
        ResourceUriFacts.Terminal_ReservedResourceUri_Prefix + Id.ToString());

    public static Key<TextEditorViewModel> TextEditorViewModelKey { get; } = new Key<TextEditorViewModel>(Id);

	private readonly ITerminal _terminal;
	private readonly ITextEditorService _textEditorService;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;
	private readonly IDispatcher _dispatcher;

	public TerminalOutputFormatterAll(
		ITerminal terminal,
		ITextEditorService textEditorService,
		ICompilerServiceRegistry compilerServiceRegistry,
		IDispatcher dispatcher)
	{
		_terminal = terminal;
		_textEditorService = textEditorService;
		_compilerServiceRegistry = compilerServiceRegistry;
		_dispatcher = dispatcher;
	}

	public string Name { get; } = nameof(TerminalOutputFormatterAll);
	
	public ITerminalOutputFormatted Format()
	{
		return new TerminalOutputFormattedTextEditor(
			string.Empty,
			ImmutableList<TerminalCommandParsed>.Empty,
			ImmutableList<TextEditorTextSpan>.Empty,
			ImmutableList<ITextEditorSymbol>.Empty);
	}
	
	public void Dispose()
	{
	}
}
