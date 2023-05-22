namespace Luthetus.Ide.ClassLib.Panel;

public record PanelTabKey(Guid Guid)
{
    public static readonly PanelTabKey Empty = new PanelTabKey(Guid.Empty);

    public static PanelTabKey NewPanelTabKey()
    {
        return new PanelTabKey(Guid.NewGuid());
    }
}