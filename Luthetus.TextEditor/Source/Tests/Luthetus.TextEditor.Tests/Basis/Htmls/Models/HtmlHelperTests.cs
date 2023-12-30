using Xunit;
using Luthetus.TextEditor.RazorLib.Htmls.Models;
using System.Text;

namespace Luthetus.TextEditor.Tests.Basis.Htmls.Models;

/// <summary>
/// <see cref="HtmlHelper"/>
/// </summary>
public class HtmlHelperTests
{
	/// <summary>
	/// <see cref="HtmlHelper.EscapeHtml(char)"/>
	/// </summary>
	[Fact]
	public void EscapeHtml_Char()
	{
		// SpaceString = "&nbsp;";
		{
			var character = ' ';
			Assert.Equal("&nbsp;", character.EscapeHtml());
		}

		// TabString = "&nbsp;&nbsp;&nbsp;&nbsp;";
		{
			var character = '	';
			Assert.Equal("&nbsp;&nbsp;&nbsp;&nbsp;", character.EscapeHtml());
		}

		// NewLineString = "<br/>";
		{
			// '\n'
			{
                var character = '\n';
                Assert.Equal("<br/>", character.EscapeHtml());
            }

            // '\r'
            {
                var character = '\r';
                Assert.Equal("<br/>", character.EscapeHtml());
            }
		}

		// AmpersandString = "&amp;";
		{
			var character = '&';
			Assert.Equal("&amp;", character.EscapeHtml());
		}

		// LeftAngleBracketString = "&lt;";
		{
			var character = '<';
			Assert.Equal("&lt;", character.EscapeHtml());
		}

		// RightAngleBracketString = "&gt;";
		{
			var character = '>';
			Assert.Equal("&gt;", character.EscapeHtml());
		}

		// DoubleQuoteString = "&quot;";
		{
			var character = '"';
			Assert.Equal("&quot;", character.EscapeHtml());
		}

		// SingleQuoteString = "&#39;";
		{
			var character = '\'';
			Assert.Equal("&#39;", character.EscapeHtml());
		}
	}

	/// <summary>
	/// <see cref="HtmlHelper.EscapeHtml(StringBuilder)"/>
	/// </summary>
	[Fact]
	public void EscapeHtml_StringBuilder()
	{
		var stringBuilderNotEscaped = new StringBuilder();

		stringBuilderNotEscaped.Append(' ');
		stringBuilderNotEscaped.Append('	');
		stringBuilderNotEscaped.Append('\n');
		stringBuilderNotEscaped.Append('&');
		stringBuilderNotEscaped.Append('<');
		stringBuilderNotEscaped.Append('>');
		stringBuilderNotEscaped.Append('"');
		stringBuilderNotEscaped.Append('\'');

        // The order of replacements matter, so to test that the order is correct reverse the
        // string and ensure it still is escaped correctly.

        // Not reversed
        {
            var stringBuilderIsEscaped = stringBuilderNotEscaped.EscapeHtml();

            Assert.Equal(
                "&nbsp;" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "<br/>" +
                    "&amp;" +
                    "&lt;" +
                    "&gt;" +
                    "&quot;" +
                    "&#39;",
                stringBuilderIsEscaped);
        }

        // Is reversed
        {
            var stringBuilderReversedNotEscaped = new StringBuilder(
				new string(stringBuilderNotEscaped.ToString().Reverse().ToArray()));

            var stringBuilderIsEscaped = stringBuilderReversedNotEscaped.EscapeHtml();

            Assert.Equal(
                "&#39;" +
                    "&quot;" +
                    "&gt;" +
                    "&lt;" +
                    "&amp;" +
                    "<br/>" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "&nbsp;",
                stringBuilderIsEscaped);
        }
	}

	/// <summary>
	/// <see cref="HtmlHelper.EscapeHtml(string)"/>
	/// </summary>
	[Fact]
	public void EscapeHtml_String()
	{
		var notEscaped = new string(new char[] 
		{
            ' ',
			'	',
			'\n',
			'&',
			'<',
			'>',
			'"',
			'\''
        });

        // The order of replacements matter, so to test that the order is correct reverse the
		// string and ensure it still is escaped correctly.

        // Not reversed
        {
            var isEscaped = notEscaped.EscapeHtml();

            Assert.Equal(
                "&nbsp;" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "<br/>" +
                    "&amp;" +
                    "&lt;" +
                    "&gt;" +
                    "&quot;" +
                    "&#39;",
                isEscaped);
        }

		// Is reversed
		{
			var reversedNotEscaped = new string(notEscaped.Reverse().ToArray());
            var isEscaped = reversedNotEscaped.EscapeHtml();

            Assert.Equal(
				"&#39;" +
					"&quot;" +
					"&gt;" +
					"&lt;" +
					"&amp;" +
					"<br/>" +
					"&nbsp;&nbsp;&nbsp;&nbsp;" +
					"&nbsp;",
                isEscaped);
        }
	}
}