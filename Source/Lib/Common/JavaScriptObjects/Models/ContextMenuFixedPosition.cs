using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.JavaScriptObjects.Models;

/// <summary>
/// If the context menu event occurred due to a "RightClick" event then
/// <br/>
/// <see cref="LeftPositionInPixels"/> == <see cref="MouseEventArgs.ClientX"/>
/// <br/>
/// <see cref="TopPositionInPixels"/> == <see cref="MouseEventArgs.ClientY"/>
/// <br/><br/>
/// If the context menu event occurred due to a "keyboard" event then
/// <br/>
/// <see cref="LeftPositionInPixels"/> == element.getBoundingClientRect().left
/// <br/>
/// <see cref="TopPositionInPixels"/> == element.getBoundingClientRect().top + element.getBoundingClientRect().height
/// </summary>
public record ContextMenuFixedPosition(
    bool OccurredDueToMouseEvent,
    double LeftPositionInPixels,
    double TopPositionInPixels);