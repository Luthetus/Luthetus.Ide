namespace Luthetus.Common.RazorLib.Menus.Models;

public class MenuOptionCallbacks
{
    public MenuOptionCallbacks(
        Func<Task> hideWidgetAsync,
        Func<Action, Task> completeWidgetAsync)
    {
        HideWidgetAsync = hideWidgetAsync;
        CompleteWidgetAsync = completeWidgetAsync;
    }

    /// <summary>
    /// No longer display widget but will NOT close the menu
    /// </summary>
    public Func<Task> HideWidgetAsync { get; }
    /// <summary>
    /// No longer display widget, close the menu, and once the menu is closed
    /// proceed to invoke the action that was provided.
    /// <br/><br/>
    /// This is done to ensure that a widget is only completed once
    /// as the completed logic does not run until the widget is no longer rendered.
    /// <br/><br/>
    /// Without this it is believed there is a possibility that one might
    /// hold the 'Enter' key in the example of submitting a
    /// form and submit it many times.
    /// </summary>
    public Func<Action, Task> CompleteWidgetAsync { get; }
}