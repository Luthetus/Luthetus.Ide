using System.Reflection;

namespace Luthetus.Common.RazorLib.ComponentRunners.Models;

public record ComponentRunnerOptions(params Assembly[] AssembliesToScanBag);