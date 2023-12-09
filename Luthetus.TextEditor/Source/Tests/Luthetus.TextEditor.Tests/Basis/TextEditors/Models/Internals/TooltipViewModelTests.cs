using Luthetus.Common.RazorLib.JavaScriptObjects.Models;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models.Internals;

public record TooltipViewModelTests(
    Type RendererType,
    Dictionary<string, object?>? ParameterMap,
    RelativeCoordinates RelativeCoordinates,
    string? CssClassString,
    Func<Task> OnMouseOver);