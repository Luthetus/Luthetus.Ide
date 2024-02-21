using Fluxor;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.States;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models.Internals;

/// <summary>
/// <see cref="DisplayTracker"/>
/// </summary>
public class DisplayTrackerTests
{
    /// <summary>
    /// <see cref="DisplayTracker(ITextEditorService, Func{TextEditorViewModel?}, Func{TextEditorModel?})"/>
    /// <br/>----<br/>
    /// <see cref="DisplayTracker.GetViewModelFunc"/>
    /// <see cref="DisplayTracker.GetModelFunc"/>
    /// <see cref="DisplayTracker.Links"/>
    /// <see cref="DisplayTracker.IsFirstDisplay"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="DisplayTracker.IncrementLinks(IState{TextEditorModelState})"/>
	/// </summary>
	[Fact]
	public void IncrementLinks()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="DisplayTracker.DecrementLinks(IState{TextEditorModelState})"/>
	/// </summary>
	[Fact]
	public void DecrementLinks()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="DisplayTracker.ConsumeIsFirstDisplay()"/>
	/// </summary>
	[Fact]
	public void ConsumeIsFirstDisplay()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="DisplayTracker.Dispose()"/>
	/// </summary>
	[Fact]
	public void Dispose()
	{
		throw new NotImplementedException();
	}
}