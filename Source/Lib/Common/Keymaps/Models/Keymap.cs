using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Text.Json.Serialization;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

public class Keymap
{
    public static readonly Keymap Empty = new Keymap(Key<Keymap>.Empty, string.Empty);

    public Keymap(Key<Keymap> key, string displayName)
    {
        Key = key;
        DisplayName = displayName;
    }

    /// <summary>This constructor is used for JSON deserialization</summary>
    [Obsolete("This constructor is used for JSON deserialization")]
    public Keymap()
    {
    }

	/// <summary>
	/// TODO: This dictionary typed property is an implementation detail and should be...
	///       ...hidden behind an interface.
	///       |
	///       Further details: An integrated terminal is currently being written.
	///       The keymap for the integrated terminal is intended to capture the
	///       keyboard event args.
	///       ...
	///       This would allow the integrated terminal to then check if the cursor
	///       is on the final line of the text editor or not.
	///       ...
	///       If the cursor is on the final line, then write out the text,
	///       otherwise treat the text editor as readonly because the cursor
	///       is at 'history'
	///       ...
	///       But, with all this said, one cannot 'delay' the insertion of the text,
	///       because the integrated terminal when intercepting the keystroke does not have
	///       access to the keyboard event.
	///       ...
	///       If an interface IKeymap existed, then the methods:
	///         -bool TryRegister(KeyboardEventArgs args, Command command)
	///         -bool TryMap(KeyboardEventArgs args, out Command command)
	///		  Could be made, such that 'TryMap' would have a reference to the 'KeyboardEventArgs'
	/// </summary>
	[property: JsonIgnore]
    public Dictionary<KeymapArgument, CommandNoType> Map { get; set; } = new();
    public Key<Keymap> Key { get; set; } = Key<Keymap>.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
