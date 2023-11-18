using Luthetus.Common.RazorLib.Storages.Models;

namespace Luthetus.Common.Tests.Basis.Storages.Models;

/// <summary>
/// <see cref="DoNothingStorageService"/>
/// </summary>
public class DoNothingStorageServiceTests
{
    private const string DoNothingKey = "abc";
    private const int DoNothingValue = 123;

    /// <summary>
    /// <see cref="DoNothingStorageService()"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var doNothingStorageService = new DoNothingStorageService();
        Assert.NotNull(doNothingStorageService);
    }

    /// <summary>
    /// <see cref="DoNothingStorageService.SetValue(string, object?)"/>
    /// </summary>
    [Fact]
    public void SetValue()
    {
        var doNothingStorageService = new DoNothingStorageService();

        doNothingStorageService.SetValue(DoNothingKey, DoNothingValue);
        Assert.NotNull(doNothingStorageService);
    }

    /// <summary>
    /// <see cref="DoNothingStorageService.GetValue(string)"/>
    /// </summary>
    [Fact]
    public void GetValue()
    {
        var doNothingStorageService = new DoNothingStorageService();

        var value = doNothingStorageService.GetValue(DoNothingKey);
        Assert.NotNull(doNothingStorageService);
    }
}