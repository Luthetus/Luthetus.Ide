namespace Luthetus.Ide.Photino;

/// <summary>
/// The 'Luthetus.Ide.Photino' app has a lot of parts to it.
/// I want to isolate the UI more so I can manually interact with it in a more restricted environment.
/// Unit tests should be preferred over this manual testing I'm doing here.
/// But I still believe having this manual isolation that I can turn on as I need is useful to visually understand what is going wrong at times.
/// </summary>
public enum AppKind
{
    Normal,
    TestAppLuthetusCommon,
    TestAppLuthetusIde,
    TestAppLuthetusTextEditor,
}
