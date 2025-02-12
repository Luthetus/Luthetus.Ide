using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;

namespace Luthetus.Ide.RazorLib.CodeSearches.Models;

public interface ICodeSearchService
{
	public event Action? CodeSearchStateChanged;
    
    public CodeSearchState GetCodeSearchState();
    
    public void ReduceWithAction(Func<CodeSearchState, CodeSearchState> withFunc);
    public void ReduceAddResultAction(string result);
    public void ReduceClearResultListAction();
    public void ReduceInitializeResizeHandleDimensionUnitAction(DimensionUnit dimensionUnit);

    /// <summary>
    /// TODO: This method makes use of <see cref="IThrottle"/> and yet is accessing...
    ///       ...searchEffect.CancellationToken.
    ///       The issue here is that the search effect parameter to this method
    ///       could be out of date by the time that the throttle delay is completed.
    ///       This should be fixed. (2024-05-02)
    /// </summary>
    /// <param name="searchEffect"></param>
    /// <param name="dispatcher"></param>
    /// <returns></returns>
    public Task HandleSearchEffect(CancellationToken CancellationToken = default);
}
