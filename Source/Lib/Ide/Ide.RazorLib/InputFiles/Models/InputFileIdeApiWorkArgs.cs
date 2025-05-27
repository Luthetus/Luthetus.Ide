using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.Models;

public struct InputFileIdeApiWorkArgs
{
	public InputFileIdeApiWorkKind WorkKind { get; set; }
    public string Message { get; set; }
    public Func<AbsolutePath, Task> OnAfterSubmitFunc { get; set; }
    public Func<AbsolutePath, Task<bool>> SelectionIsValidFunc { get; set; }
    public List<InputFilePattern> InputFilePatterns { get; set; }
}
