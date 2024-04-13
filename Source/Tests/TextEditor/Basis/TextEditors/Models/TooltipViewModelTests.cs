using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models;

/// <summary>
/// <see cref="TooltipViewModel"/>
/// </summary>
public class TooltipViewModelTests
{
    /// <summary>
    /// <see cref="TooltipViewModel(Type, Dictionary{string, object?}?, RelativeCoordinates, string?, Func{Task})"/>
	/// <br/>----<br/>
    /// <see cref="TooltipViewModel.RendererType"/>
    /// <see cref="TooltipViewModel.ParameterMap"/>
    /// <see cref="TooltipViewModel.RelativeCoordinates"/>
    /// <see cref="TooltipViewModel.CssClassString"/>
    /// <see cref="TooltipViewModel.OnMouseOver"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var rendererType = typeof(SymbolDisplay);
        var parameterMap = new Dictionary<string, object?>();
        var relativeCoordinates = new RelativeCoordinates(50, 100, 25, 50);
        var cssClassString = nameof(TooltipViewModelTests);
        var onMouseOver = () => Task.CompletedTask;

        var tooltipViewModel = new TooltipViewModel(
            rendererType,
            parameterMap,
            relativeCoordinates,
            cssClassString,
            onMouseOver);

        Assert.Equal(rendererType, tooltipViewModel.RendererType);
        Assert.Equal(parameterMap, tooltipViewModel.ParameterMap);
        Assert.Equal(relativeCoordinates, tooltipViewModel.RelativeCoordinates);
        Assert.Equal(cssClassString, tooltipViewModel.CssClassString);
        Assert.Equal(onMouseOver, tooltipViewModel.OnMouseOver);
    }
}