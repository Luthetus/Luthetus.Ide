using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

public class OptionalArgumentsTests
{
	[Fact]
	public void Aaa()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText = TEST_DATA;

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        foreach (var child in topCodeBlock.ChildList)
        {
        	Console.WriteLine(child.SyntaxKind);
        }
	}
	
	private const string TEST_DATA = 
@"using Luthetus.Common.RazorLib.Exceptions;

namespace Luthetus.Common.RazorLib.Reactives.Models;

public class ProgressBarModel : IDisposable
{
	private readonly object _progressLock = new();

	public ProgressBarModel()
		: this(0, null)
	{
	}

	public ProgressBarModel(double decimalPercentProgress)
		: this(decimalPercentProgress, null)
	{
	}

	public ProgressBarModel(double decimalPercentProgress, string? message)
	{
		DecimalPercentProgress = decimalPercentProgress;
		Message = message;
	}

	public double DecimalPercentProgress { get; private set; }
	public string? Message { get; private set; }
	public string? SecondaryMessage { get; private set; }
	public bool IsDisposed { get; private set; }

	/// <summary>
	/// When <see cref=""SetProgress(double)""/> is invoked, then this event is raised
	/// with the bool value of 'false'. In this scenario the <see cref=""DecimalPercentProgress""/>
	/// is ""implied"" to have changed, and perhaps one should re-render the progress bar UI.
	/// 
	/// When <see cref=""Dispose()""/> is invoked, then this event is raised with
	/// the bool value of 'true'. In this scenario, the subscriber should unsubscribe
	/// from this event, as <see cref=""SetProgress(double)""/> is no longer allowed to be invoked.
	/// Note: if one has a reference to this object, they can still invoke <see cref=""GetProgress()""/>,
	/// even if the <see cref=""IsDisposed""/> property is set to a value of 'true'.
	/// </summary>
	public event Action<bool>? ProgressChanged;

	/// <summary>
	/// If a parameter is null, then their corresponding property will not be changed.
	/// </summary>
	public void SetProgress(double? decimalPercentProgress, string? message = null, string? secondaryMessage = null)
	{
		if (IsDisposed)
			throw new LuthetusCommonException($""The {nameof(ProgressBarModel)} has {nameof(IsDisposed)} set to true, therefore cannot be set anymore."");

		lock (_progressLock)
		{
			if (decimalPercentProgress is not null)
				DecimalPercentProgress = decimalPercentProgress.Value;

			if (message is not null)
				Message = message;

			if (secondaryMessage is not null)
				SecondaryMessage = secondaryMessage;
		}

		ProgressChanged?.Invoke(false);
	}

	public double GetProgress()
	{
		// TODO: Is returning from inside of a lock equivalent to this capturing of the value logic in regards to thread safety/concurrency?
		double decimalPercentProgress;

		lock (_progressLock)
		{
			decimalPercentProgress = DecimalPercentProgress;
		}

		return decimalPercentProgress;
	}

	public void Dispose()
	{
		lock (_progressLock)
		{
			IsDisposed = true;
			ProgressChanged?.Invoke(true);
		}
		
	}
}
";
}
