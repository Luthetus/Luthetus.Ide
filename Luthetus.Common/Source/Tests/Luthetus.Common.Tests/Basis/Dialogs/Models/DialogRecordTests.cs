using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;

namespace Luthetus.Common.Tests.Basis.Dialogs.Models;

/// <summary>
/// <see cref="DialogRecord"/>
/// </summary>
public class DialogRecordTests
{
    /// <summary>
    /// <see cref="DialogRecord(Key{DialogRecord}, string, Type, Dictionary{string, object?}?, string?)"/>
    /// <br/>----<br/>
    /// <see cref="DialogRecord.ElementDimensions"/>
    /// <see cref="DialogRecord.IsMinimized"/>
    /// <see cref="DialogRecord.IsMaximized"/>
    /// <see cref="DialogRecord.IsResizable"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var dialogRecord = new DialogViewModel(Key<IDynamicViewModel>.NewKey(), "Test title",
            typeof(CommonInformativeNotificationDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(CommonInformativeNotificationDisplay.Message),
                    "Test message"
                }
            },
            null,
            true);

        Assert.Equal(ElementPositionKind.Fixed, dialogRecord.DialogElementDimensions.ElementPositionKind);

        // IsMinimized
        {
            Assert.False(dialogRecord.DialogIsMinimized);
         
            dialogRecord.DialogIsMinimized = true;
            Assert.True(dialogRecord.DialogIsMinimized);

            dialogRecord.DialogIsMinimized = false;
            Assert.False(dialogRecord.DialogIsMinimized);
        }

        // IsMaximized
        {
            Assert.False(dialogRecord.DialogIsMaximized);
         
            dialogRecord.DialogIsMaximized = true;
            Assert.True(dialogRecord.DialogIsMaximized);

            dialogRecord.DialogIsMaximized = false;
            Assert.False(dialogRecord.DialogIsMaximized);
        }

        // IsResizable
        {
            Assert.False(dialogRecord.DialogIsResizable);
         
            dialogRecord.DialogIsResizable = true;
            Assert.True(dialogRecord.DialogIsResizable);

            dialogRecord.DialogIsResizable = false;
            Assert.False(dialogRecord.DialogIsResizable);
        }
    }
    
    /// <summary>
    /// <see cref="DialogRecord.ConstructDefaultDialogDimensions()"/>
    /// </summary>
    [Fact]
    public void ConstructDefaultDialogDimensions()
    {
        var defaultDialogDimensions = DialogHelper.ConstructDefaultElementDimensions();

        Assert.Equal(ElementPositionKind.Fixed, defaultDialogDimensions.ElementPositionKind);

        Assert.Single(
            defaultDialogDimensions.DimensionAttributeList,
            x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

        Assert.Single(
            defaultDialogDimensions.DimensionAttributeList,
            x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

        Assert.Single(
            defaultDialogDimensions.DimensionAttributeList,
            x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

        Assert.Single(
            defaultDialogDimensions.DimensionAttributeList,
            x => x.DimensionAttributeKind == DimensionAttributeKind.Top);
    }
}