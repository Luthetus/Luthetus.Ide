using System.Reflection;

namespace Luthetus.Common.RazorLib.ComponentRunners;

public record ComponentRunnerOptions(params Assembly[] AssembliesToScanBag);