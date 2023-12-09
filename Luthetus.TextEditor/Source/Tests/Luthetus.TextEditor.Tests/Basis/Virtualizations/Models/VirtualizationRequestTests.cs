namespace Luthetus.TextEditor.Tests.Basis.Virtualizations.Models;

public record VirtualizationRequestTests(
    VirtualizationScrollPosition ScrollPosition,
    CancellationToken CancellationToken);