using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.TextEditor.Tests.Adhoc;

public class TypeStartFileNonLetter
{
	[Fact]
	public void Aaa()
	{
		var model = new TextEditorModel(
			new ResourceUri("/unitTesting.txt"),
			DateTime.UtcNow,
			ExtensionNoPeriodFacts.TXT,
			string.Empty,
			null,
			null,
			partitionSize: 4096);

		var modelModifier = new TextEditorModelModifier(model);

		var cursor = new TextEditorCursor(0, 0, true);

		var cursorModifierBag = new CursorModifierBagTextEditor(
			Key<TextEditorViewModel>.Empty,
			new List<TextEditorCursorModifier> { new(cursor) });

		modelModifier.Insert(
			"(",
			cursorModifierBag);

		var aaa = 2;
	}
}
