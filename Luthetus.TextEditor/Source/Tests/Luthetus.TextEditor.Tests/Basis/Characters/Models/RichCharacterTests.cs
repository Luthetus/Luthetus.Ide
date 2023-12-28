using Xunit;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;

namespace Luthetus.TextEditor.Tests.Basis.Characters.Models;

/// <summary>
/// <see cref="RichCharacter"/>
/// </summary>
public class RichCharacterTests
{
    /// <summary>
    /// <see cref="RichCharacter.Value"/>
    /// <br/>----<br/>
    /// <see cref="RichCharacter.DecorationByte"/>
    /// </summary>
    [Fact]
	public void Value()
	{
		// Construct two RichCharacter to ensure the constructor is not just assigning the same constant value

		{
			var decorationByte = (byte)GenericDecorationKind.Function;
			var value = 'C';

            var richCharacter = new RichCharacter
            {
                DecorationByte = decorationByte,
                Value = value
            };

			Assert.Equal(decorationByte, richCharacter.DecorationByte);
			Assert.Equal(value, richCharacter.Value);
        }
		
		{
			var decorationByte = (byte)GenericDecorationKind.Keyword;
			var value = 'K';

            var richCharacter = new RichCharacter
            {
                DecorationByte = decorationByte,
                Value = value
            };

			Assert.Equal(decorationByte, richCharacter.DecorationByte);
			Assert.Equal(value, richCharacter.Value);
        }
	}
}