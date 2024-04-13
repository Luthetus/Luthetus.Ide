using System.Reflection;

namespace Luthetus.Common.RazorLib.Reflectives.Models;

public record ReflectiveOptions(params Assembly[] AssembliesToScanList);