using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;

namespace Luthetus.TextEditor.Tests.Basis.Virtualizations.Models;

/// <summary>
/// <see cref="VirtualizationResult{T}"/>
/// </summary>
public record VirtualizationResultTests
{
    /// <summary>
    /// <see cref="VirtualizationResult{T}.VirtualizationResult(ImmutableArray{VirtualizationEntry{T}}, VirtualizationBoundary, VirtualizationBoundary, VirtualizationBoundary, VirtualizationBoundary, TextEditorMeasurements, CharAndLineMeasurements)"/>
    /// <br/>----<br/>
    /// <see cref="VirtualizationResult{T}.EntryList"/>
    /// <see cref="VirtualizationResult{T}.LeftVirtualizationBoundary"/>
    /// <see cref="VirtualizationResult{T}.RightVirtualizationBoundary"/>
    /// <see cref="VirtualizationResult{T}.TopVirtualizationBoundary"/>
    /// <see cref="VirtualizationResult{T}.BottomVirtualizationBoundary"/>
    /// <see cref="VirtualizationResult{T}.TextEditorMeasurements"/>
    /// <see cref="VirtualizationResult{T}.CharAndLineMeasurements"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="VirtualizationResult{T}.GetEmptyRichCharacters()"/>
	/// </summary>
	[Fact]
	public void GetEmptyRichCharacters()
	{
		throw new NotImplementedException();
	}
}