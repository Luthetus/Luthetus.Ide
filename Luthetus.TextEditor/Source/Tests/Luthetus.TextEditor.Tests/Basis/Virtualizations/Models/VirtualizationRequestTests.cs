namespace Luthetus.TextEditor.RazorLib.Virtualizations.Models;

public record VirtualizationRequestTests(
    VirtualizationScrollPosition ScrollPosition,
    CancellationToken CancellationToken);