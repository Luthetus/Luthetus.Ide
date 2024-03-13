namespace Luthetus.Common.RazorLib.PolymorphicUis.Models;

public interface IPolymorphicTab : IPolymorphicUiRecord
{
	public Dictionary<string, object?>? TabParameterMap { get; }
	public string TabGetDynamicCss();
    public Task TabSetAsActiveAsync();
	public Task TabCloseAsync();
}
