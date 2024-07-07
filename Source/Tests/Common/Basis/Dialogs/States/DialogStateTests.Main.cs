using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;

namespace Luthetus.Common.Tests.Basis.Dialogs.States;

/// <summary>
/// <see cref="DialogState"/>
/// </summary>
public class DialogStateMainTests
{
    /// <summary>
    /// <see cref="DialogState()"/>
    /// <br/>----<br/>
    /// <see cref="DialogState.DialogList"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var dialogState = new DialogState();
        Assert.Equal(ImmutableList<IDialog>.Empty, dialogState.DialogList);

        var sampleDialogRecord = new DialogViewModel(Key<IDynamicViewModel>.NewKey(), "Test title",
            typeof(CommonInformativeNotificationDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(CommonInformativeNotificationDisplay.Message),
                    "Test message"
                }
            },
            null,
            true,
            "luth_element-id");

        var outDialogList = dialogState.DialogList.Add(sampleDialogRecord);

        dialogState = dialogState with
        {
            DialogList = outDialogList
        };

        Assert.Contains(dialogState.DialogList, x => x == sampleDialogRecord);
    }
}