using Fluxor;
using Luthetus.Common.RazorLib;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Common.Installation.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusCommonInstallationServices(
        this IServiceCollection services)
    {
        return services
            .AddLuthetusCommonServices()
            .AddFluxor(options =>
                options.ScanAssemblies(
                    typeof(LuthetusCommonOptions).Assembly));
    }
}
