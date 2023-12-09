namespace Luthetus.TextEditor.RazorLib.Virtualizations.Models;

public record VirtualizationRequest(
    VirtualizationScrollPosition ScrollPosition,
    CancellationToken CancellationToken);