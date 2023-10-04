namespace Luthetus.CompilerServices.Lang.Xml.Tests.TestDataFolder;

public static partial class TestData
{
    public static class Html
    {
        public const string EXAMPLE_TEXT = @"<!DOCTYPE html>
<html lang=""en"">
	<head>
		<meta charset=""UTF-8"">
		<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
		<meta http-equiv=""X-UA-Compatible"" content=""ie=edge"">
		<title>HTML 5 Boilerplate</title>
		<link rel=""stylesheet"" href=""style.css"">
	</head>
	<body>
		<!-- TEST: Cover tag usage -->
		<div>
			<!-- TEST: Self closing tag -->
			<br/>
			<!-- TEST: NOT self closing tag NO child content -->
			<div></div>

			<!-- TEST: Cover NOT self closing tag WITH child content -->
			<div>
				<!-- TEST: Child content = { TextNode }  -->
				<div>TextNode Child content</div>
				<!-- TEST: Child content = { TextNode, HtmlElement }  -->
				<div>
					TextNode Child content
					<span>Html element</span>
				</div>
				<!-- TEST: Child content = { HtmlElement, TextNode, HtmlElement }  -->
				<div>
					<span>First Html element</span>
					TextNode Child content
					<span>Second Html element</span>
				</div>
			<div>
		</div>

		<!-- TEST: Cover attribute usage -->
		<div>
			<!-- TEST: Self closing tag name does NOT have a space after  -->
			<div/>
			<!-- TEST: Element with child content has tag name does NOT have a space after -->
			<div>
				<span>Child</span>
			</div>

			<!-- TEST: Self closing tag name has a space after it but no attributes  -->
			<div />
			<!-- TEST: Element with child content has tag name with a space after it but no attributes -->
			<div >
				<span>Child</span>
			</div>

			<!-- TEST: Self closing tag has a valueless attribute (Example: 'disabled') -->
			<div disabled/>
			<!-- TEST: Element with child content has a valueless attribute (Example: 'disabled') -->
			<div disabled>
				<span>Child</span>
			</div>

			<!-- TEST: Self closing tag has an attribute name with the equals but nothing follows -->
			<div class=/>
			<!-- TEST: Element with child content has an attribute name with the equals but nothing follows -->
			<div class=>
				<span>Child</span>
			</div>

			<!-- TEST: Self closing tag has an attribute name with the equals however the attribute value is not wrapped in double quotes -->
			<div class=luth_te_test/>
			<!-- TEST: Element with child content has an attribute name with the equals however the attribute value is not wrapped in double quotes -->
			<div class=luth_te_test>
				<span>Child</span>
			</div>

			<!-- TEST: Self closing tag has an attribute name with the equals and one double quote but nothing follows -->
			<div class=""/>
			<!-- TEST: Element with child content has an attribute name with the equals and one double quote but nothing follows -->
			<div class="">
				<span>Child</span>
			</div>

			<!-- TEST: Self closing tag name has an attribute name with an empty string for its value -->
			<div class=""""/>
			<!-- TEST: Element with child content has an attribute name with an empty string for its value -->
			<div class="""">
				<span>Child</span>
			</div>

			<!-- TEST: Self closing tag name has an attribute name with an attribute value which contains spaces -->
			<div class=""luth_te_test luth_te_placeholder""/>
			<!-- TEST: Element with child content has an attribute name with an empty string for its value -->
			<div class=""luth_te_test luth_te_placeholder"">
				<span>Child</span>
			</div>
		<div>

		<script src=""index.js""></script>
	</body>
</html>";
    }
}