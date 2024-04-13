using Luthetus.Common.RazorLib.WatchWindows.Models;

namespace Luthetus.Common.Tests.Basis.WatchWindows.Models;

/// <summary> 
/// <see cref="WatchWindowObject"/>
/// </summary>
public class WatchWindowObjectTests
{
    /// <summary>
    /// <see cref="WatchWindowObject.WatchWindowObject"/>
    /// <br/>----<br/>
    /// <see cref="WatchWindowObject.Item"/>
    /// <see cref="WatchWindowObject.ItemType"/>
    /// <see cref="WatchWindowObject.DisplayName"/>
    /// <see cref="WatchWindowObject.IsPubliclyReadable"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var johnDoe = new PersonTest("John", "Doe", new());
        var janeDoe = new PersonTest("Jane", "Doe", new());
        var bobSmith = new PersonTest("Bob", "Smith", new());

        johnDoe.Relatives.Add(janeDoe);
        janeDoe.Relatives.Add(johnDoe);

        johnDoe.Relatives.Add(bobSmith);
        bobSmith.Relatives.Add(johnDoe);

        var item = johnDoe;
        var itemType = johnDoe.GetType();
        var displayName = johnDoe.DisplayName;
        var isPubliclyReadable = true;

        var watchWindowObject = new WatchWindowObject(
            item,
            itemType,
            displayName,
            isPubliclyReadable);

        Assert.Equal(johnDoe, watchWindowObject.Item);
        Assert.Equal(itemType, watchWindowObject.ItemType);
        Assert.Equal(displayName, watchWindowObject.DisplayName);
        Assert.Equal(isPubliclyReadable, watchWindowObject.IsPubliclyReadable);
    }
}