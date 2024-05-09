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
///              If the C# defined the dimensions, then after an <see cref="TextEditors.Models.IEditContext"/>
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
/// </summary>
/// <param name="ScrollLeft">The unit of measurement is Pixels (px)</param>
/// <param name="ScrollTop">The unit of measurement is Pixels (px)</param>
/// <param name="ScrollWidth">The unit of measurement is Pixels (px)</param>
/// <param name="ScrollHeight">The unit of measurement is Pixels (px)</param>
/// <param name="MarginScrollHeight">The unit of measurement is Pixels (px)</param>
/// <param name="Width">The unit of measurement is Pixels (px)</param>
/// <param name="Height">The unit of measurement is Pixels (px)</param>
public record TextEditorDimensions(
    double ScrollLeft,
    double ScrollTop,
    double ScrollWidth,
    double ScrollHeight,
    double MarginScrollHeight,
    double Width,
    double Height);