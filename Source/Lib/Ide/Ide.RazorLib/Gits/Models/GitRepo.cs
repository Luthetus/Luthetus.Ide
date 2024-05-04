using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.Gits.Models;

public record GitRepo(IAbsolutePath RepoFolderAbsolutePath, IAbsolutePath GitFolderAbsolutePath);
