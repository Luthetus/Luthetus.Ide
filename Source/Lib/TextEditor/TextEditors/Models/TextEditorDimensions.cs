namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// The unit of measurement is Pixels (px)<br/><br/>
/// ======================================<br/><br/>
/// Goal: Rewrite TextEditorMeasurements. (2024-05-09)<br/><br/>
/// 
/// Description: C# should define the 'dimensions/measurements' of the
///              text editor view model.
///              |
///              This is opposed to the current way, in which the HTML element
///              defines the 'dimensions/measurements'.
///              |
///              Given the current way, one cannot under the same
///              <see cref="TextEditors.Models.IEditContext"/>,
///              sensibly batch modifications to the scroll positions.
///              |
///              If the C# defined the dimensions, then after an <see cref="IEditContext"/>
///              is completed, in just one JavaScript invocation the scrollbar could be moved.
///              |
///              Furthermore, given the current way, everytime the <see cref="Virtualizations.Models.VirtualizationResult{T}"/>
///              is calculated, an invocation to JavaScript is made, in order to measure the HTML element which encompasses the
///              text editor.
///              |
///              With these changes, one could re-calculate the <see cref="Virtualizations.Models.VirtualizationResult{T}"/>
///              without calling into JavaScript, as the C# definition is the true measurements.<br/><br/>
/// 
/// Concern A("Browser Resize"): With these changes, a new issue arises, when the browser/user-agent is resized,
///           the text editor HTML element which is rendered, would likely resize relative
///           to the resizing of the browser.
///           |
///           This results in the C# definition for the text editor's definitions being corrupted.
///           |
///           This can be solved by adding an event handler to the browser being resized.
///           Upon the resize event firing, use a throttle to re-synchronize
///           the C# defined dimensions with the HTML element.<br/><br/>
///           
/// Concern B("Intra-Browser Resize"): With these changes, a new issue arises,
///           if a website has intra-site resizability for various HTML elements,
///           then a browser-resize-event would not catch these.
///           (Ex: <see cref="Common.RazorLib.Resizes.Displays.ResizableDisplay"/>).
///           A way for a developer who uses the text editor, to notify that their
///           intra-browser-resize-event fired, is necessary here.
///           |
///           This results in the C# definition for the text editor's definitions being corrupted.
///           |
///           This can be solved by adding an event handler to the intra-browser-resize-event.
///           Upon the intra-browser-resize-event firing, use a throttle to re-synchronize
///           the C# defined dimensions with the HTML element.<br/><br/>
///           
/// TODO: Delete <see cref="JavaScriptObjects.Models.TextEditorMeasurements"/> and use <see cref="TextEditorDimensions"/>
/// 
/// =====================================================================================================================
/// The comments for <see cref="JavaScriptObjects.Models.TextEditorMeasurements"/> and <see cref="TextEditorDimensions"/>
/// are going to be out of sync at this point.
/// =====================================================================================================================
/// 
/// I am at the initial steps of rewriting, and am finding that the 'Width' and 'Height'
/// are defined by the HTML, but that we can move the 'Scroll...' properties to be C# defined.<br/><br/>
/// 
/// Regarding 'ScrollWidth' and 'ScrollHeight', are these similar to 'Width' and 'Height' where the
/// HTML is defining them, and they must be measured?
/// |
/// Perferably C# can handle these values, as if text is inserted or deleted to the
/// largest-width-line of text, then the 'ScrollWidth' would be effected.
/// To immediately know this change in 'ScrollWidth' with C# could be helpful for optimizations.
/// |
/// One reason for not knowing this information, would be if 'UseMonospaceOptimizations' were turned off.
/// As then, the width of a line cannot be calculated.
/// |
/// 'ScrollHeight' on the otherhand should be easier to calculate, since every line is of the same height,
/// regardless of 'UseMonospaceOptimizations' being on or off.<br/><br/>
/// 
/// I want to at this point delete <see cref="JavaScriptObjects.Models.TextEditorMeasurements"/>.cs
/// and replace all references with <see cref="TextEditorDimensions"/>.
/// |
/// A side note: it seems it would still be a massive optimization, to leave the calculation of virtualization result,
///              to invoke JavaScript to measure the text editor.
///              |
///              This is because, we still gain the advantage of scrolling within an <see cref="IEditContext"/>,
///              and not calling into JavaScript, until the final result.
///              |
///              Preferably the calculation of virtualization result wouldn't invoke JavaScript, as we'd keep the
///              measurements accurate. But, in terms of a step by step plan, focusing on the <see cref="IEditContext"/>
///              side of things is far more important.
/// </summary>
/// <param name="ScrollLeft">The unit of measurement is Pixels (px)</param>
/// <param name="ScrollTop">The unit of measurement is Pixels (px)</param>
/// <param name="ScrollWidth">The unit of measurement is Pixels (px)</param>
/// <param name="ScrollHeight">The unit of measurement is Pixels (px)</param>
/// <param name="MarginScrollHeight">The unit of measurement is Pixels (px)</param>
/// <param name="Width">The unit of measurement is Pixels (px)</param>
/// <param name="Height">The unit of measurement is Pixels (px)</param>
public record TextEditorDimensions(
    int ScrollLeft,
    int ScrollTop,
    int ScrollWidth,
    int ScrollHeight,
    int MarginScrollHeight,
    int Width,
    int Height);