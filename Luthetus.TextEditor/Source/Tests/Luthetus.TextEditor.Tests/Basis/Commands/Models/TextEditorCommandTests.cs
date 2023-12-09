using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.Commands.Models;

public class TextEditorCommandTests
{
	[Fact]
	public void Constructor()
	{
		//public TextEditorCommand(
		//		string displayName,
		//		string internalIdentifier,
		//		bool shouldBubble,
		//		bool shouldScrollCursorIntoView,
		//		TextEditKind textEditKind,
		//		string? otherTextEditKindIdentifier,
		//		Func<ICommandArgs, Task> doAsyncFunc)
	}

	[Fact]
	public void ShouldScrollCursorIntoView()
	{
		//public bool ShouldScrollCursorIntoView { get; }
	}

	[Fact]
	public void TextEditKind()
	{
		//public TextEditKind TextEditKind { get; }
	}

	[Fact]
	public void OtherTextEditKindIdentifier()
	{
		//public string? OtherTextEditKindIdentifier { get; }
	}

	[Fact]
	public void ThrowOtherTextEditKindIdentifierWasExpectedException()
	{
		//public static ApplicationException ThrowOtherTextEditKindIdentifierWasExpectedException(TextEditKind textEditKind)
	}
}