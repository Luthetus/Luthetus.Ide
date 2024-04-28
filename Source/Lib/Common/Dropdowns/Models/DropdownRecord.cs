namespace Luthetus.Common.RazorLib.Dropdowns.Models;

/// <summary>
/// <see cref="DropdownRecord"/> has no data, but a type is needed to identify it.
/// The dropdowns are either shown, or not - based on whether 
/// the <see cref="Key{DropdownRecord}"/> is in the list of active dropdowns or not.
/// </summary>
public record DropdownRecord;
