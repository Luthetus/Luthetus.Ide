using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;

namespace Luthetus.Common.Tests.Basis.Installations.Models;

/// <summary>
/// <see cref="LuthetusHostingInformation"/>
/// </summary>
public record LuthetusHostingInformationTests
{
    /// <summary>
    /// <see cref="LuthetusHostingInformation(LuthetusHostingKind, IBackgroundTaskService)"/>
    /// <br/>----<br/>
    /// <see cref="LuthetusHostingInformation.LuthetusHostingKind"/>
    /// <see cref="LuthetusHostingInformation.BackgroundTaskService"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        // Two tests are done to ensure the enum 'LuthetusHostingKind' does not just
        // get set to the default enum value within the constructor.
        {
            var hostingKind = LuthetusHostingKind.UnitTesting;
            var backgroundTaskServiceSynchronous = new BackgroundTaskServiceSynchronous();

            var hostingInformation = new LuthetusHostingInformation(
                hostingKind,
                backgroundTaskServiceSynchronous);

            Assert.Equal(hostingKind, hostingInformation.LuthetusHostingKind);
            Assert.Equal(backgroundTaskServiceSynchronous, hostingInformation.BackgroundTaskService);
        }

        // Two tests are done to ensure the enum 'LuthetusHostingKind' does not just
        // get set to the default enum value within the constructor.
        {
            var hostingKind = LuthetusHostingKind.Wasm;
            var backgroundTaskService = new BackgroundTaskService();

            var hostingInformation = new LuthetusHostingInformation(
                hostingKind,
                backgroundTaskService);

            Assert.Equal(hostingKind, hostingInformation.LuthetusHostingKind);
            Assert.Equal(backgroundTaskService, hostingInformation.BackgroundTaskService);
        }
    }
}