using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.Gits.Models;

/// <summary>
/// TODO: Why am I parsing the CLI output? Can I just look at the '.git' folder itself? (2024-05-04)
/// </summary>
public record GitRepo(IAbsolutePath AbsolutePath);
