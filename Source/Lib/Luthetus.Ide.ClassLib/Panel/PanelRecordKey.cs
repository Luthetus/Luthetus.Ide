namespace Luthetus.Ide.ClassLib.Panel;

public record PanelRecordKey(Guid Guid)
{
    public static readonly PanelRecordKey Empty = new PanelRecordKey(Guid.Empty);

    public static PanelRecordKey NewPanelRecordKey()
    {
        return new PanelRecordKey(Guid.NewGuid());
    }
}