using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
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
        var dialogRecord = new DialogRecord(Key<DialogRecord>.NewKey(), "Test title",
            typeof(CommonInformativeNotificationDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(CommonInformativeNotificationDisplay.Message),
                    "Test message"
                }
            },
            null);

        Assert.Equal(ElementPositionKind.Fixed, dialogRecord.ElementDimensions.ElementPositionKind);

        // IsMinimized
        {
            Assert.False(dialogRecord.IsMinimized);
         
            dialogRecord.IsMinimized = true;
            Assert.True(dialogRecord.IsMinimized);

            dialogRecord.IsMinimized = false;
            Assert.False(dialogRecord.IsMinimized);
        }

        // IsMaximized
        {
            Assert.False(dialogRecord.IsMaximized);
         
            dialogRecord.IsMaximized = true;
            Assert.True(dialogRecord.IsMaximized);

            dialogRecord.IsMaximized = false;
            Assert.False(dialogRecord.IsMaximized);
        }

        // IsResizable
        {
            Assert.False(dialogRecord.IsResizable);
         
            dialogRecord.IsResizable = true;
            Assert.True(dialogRecord.IsResizable);

            dialogRecord.IsResizable = false;
            Assert.False(dialogRecord.IsResizable);
        }
    }
    
    /// <summary>
    /// <see cref="DialogRecord.ConstructDefaultDialogDimensions()"/>
    /// </summary>
    [Fact]
    public void ConstructDefaultDialogDimensions()
    {
        var defaultDialogDimensions = DialogRecord.ConstructDefaultDialogDimensions();

        Assert.Equal(ElementPositionKind.Fixed, defaultDialogDimensions.ElementPositionKind);

        Assert.Single(
            defaultDialogDimensions.DimensionAttributeBag,
            x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

        Assert.Single(
            defaultDialogDimensions.DimensionAttributeBag,
            x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

        Assert.Single(
            defaultDialogDimensions.DimensionAttributeBag,
            x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

        Assert.Single(
            defaultDialogDimensions.DimensionAttributeBag,
            x => x.DimensionAttributeKind == DimensionAttributeKind.Top);
    }
}