using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luthetus.Common.RazorLib.Installations.Models;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.Usage.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusCommonUsageServices(
        this IServiceCollection services,
        LuthetusHostingInformation luthetusHostingInformation)
    {
        // using Luthetus.Common.RazorLib.Installations.Models;
        // using Fluxor;

        return services
            .AddLuthetusCommonServices(luthetusHostingInformation) // 
            .AddFluxor(options => options.ScanAssemblies(
                typeof(LuthetusCommonOptions).Assembly));
    }
}
