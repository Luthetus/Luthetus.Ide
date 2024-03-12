namespace Luthetus.Common.RazorLib.PolymorphicUis.Models;

public interface IPolymorphicTab : IPolymorphicUiRecord
{
	public string TabGetDynamicCss();
    public Task TabSetAsActiveAsync();
	public Task TabCloseAsync();
}
