using Luthetus.Common.RazorLib.Dropdowns.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.Models;

/// <summary>
/// The list provided should not be modified after passing it as a parameter.
/// Make a shallow copy, and pass the shallow copy, if further modification of your list will be necessary.
/// </summary>
public record struct DropdownState(List<DropdownRecord> DropdownList)
{
    public DropdownState() : this(new List<DropdownRecord>())
    {
		
    }
}