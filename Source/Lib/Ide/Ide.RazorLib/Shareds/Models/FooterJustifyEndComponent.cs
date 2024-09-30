using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.Shareds.Models;

/// <summary>
/// A RenderFragment would be more optimized than rendering a DynamicComponent.
/// 
/// However, there are various issues relating to UI events and focus when using
/// a RenderFragment (focus refers to HTML focus being lost on re-render if the focus
/// is on an HTML element within the RenderFragment).
///
/// If a RenderFragment renders a component, and the inner component is what has focus,
/// will re-rendering the RenderFragment cause the HTML focus to be lost?
/// This is presumed to be true, but how is that [Parameter] RenderFragment ChildContent
/// isn't an issue if this is the case?
///
/// There shouldn't be that many icons rendered via this class.
/// So, we can just use the DynamicComponent way without concern, but
/// the question should be answered as to if a RenderFragment would cause issues here.
/// </summary>
public record FooterJustifyEndComponent(
	Key<FooterJustifyEndComponent> Key,
	Type ComponentType,
	Dictionary<string, object?>? ComponentParameterMap);
