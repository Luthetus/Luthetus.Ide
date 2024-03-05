using Luthetus.Common.Tests.Basis.Reactives.Models;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

internal class TextEditorOnKeyDownEvent : IThrottleEvent
{
    public TextEditorOnKeyDownEvent(
            KeyboardEventArgs item,
            Func<IThrottleEvent, CancellationToken, Task> workItem,
            Func<(IThrottleEvent OldEvent, IThrottleEvent RecentEvent), IThrottleEvent?>? consecutiveEntryFunc)
        : base(nameof(TextEditorOnKeyDownEvent), TimeSpan.Zero, item, workItem, consecutiveEntryFunc)
    {
    }

    (Key<TextEditorViewModel> viewModelKey, List<KeyboardEventArgs> keyboardEventsList)
}
