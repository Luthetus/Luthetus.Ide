using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax;

/// <summary>
/// <see cref="BoundScopeKey"/>
/// </summary>
public class BoundScopeKeyTests
{
    /// <summary>
    /// <see cref="BoundScopeKey(Guid)"/>
	/// <br/>----<br/>
    /// <see cref="BoundScopeKey.Empty"/>
    /// <see cref="BoundScopeKey.NewKey()"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		// Constructor
		{
            var guid = Guid.NewGuid();
            var boundScopeKey = new BoundScopeKey(guid);
            Assert.Equal(guid, boundScopeKey.Guid);
        }

        // Empty
        {
            Assert.Equal(BoundScopeKey.Empty, BoundScopeKey.Empty);
            Assert.NotEqual(BoundScopeKey.Empty, BoundScopeKey.NewKey());
        }

        // NewKey()
        {
            Assert.NotEqual(BoundScopeKey.NewKey(), BoundScopeKey.NewKey());
			Assert.NotEqual(BoundScopeKey.Empty, BoundScopeKey.NewKey());
		}
	}
}