using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.FileSystems.Models;

public static class FileTemplateFacts
{
    private static readonly List<IFileTemplate> _empty = new();

    public static readonly IFileTemplate CSharpClass = new FileTemplate(
        "C# Class",
        "luth_ide_c-sharp-class",
        FileTemplateKind.CSharp,
        filename => filename.EndsWith('.' + ExtensionNoPeriodFacts.C_SHARP_CLASS),
        _ => _empty,
        true,
        CSharpClassCreateFileFunc);

    public static readonly IFileTemplate RazorCodebehind = new FileTemplate(
        "Razor codebehind",
        "luth_ide_razor-codebehind-class",
        FileTemplateKind.Razor,
        filename => filename.EndsWith('.' + ExtensionNoPeriodFacts.RAZOR_CODEBEHIND),
        _ => _empty,
        true,
        RazorCodebehindCreateFileFunc);

    public static readonly IFileTemplate RazorMarkup = new FileTemplate(
        "Razor markup",
        "luth_ide_razor-markup-class",
        FileTemplateKind.Razor,
        filename => filename.EndsWith('.' + ExtensionNoPeriodFacts.RAZOR_MARKUP),
        _ => _empty,
        true,
        RazorMarkupCreateFileFunc);

    /// <summary>
    /// Template should be:
    /// -------------------
    /// namespace Luthetus.Ide.ClassLib.FileTemplates;
    ///
    /// public class Asdf
    /// {
    ///     
    /// }
    /// 
    /// </summary>
    private static FileTemplateResult CSharpClassCreateFileFunc(FileTemplateParameter templateParameter)
    {
        var emptyFileAbsolutePathString = templateParameter.ParentDirectory.AbsolutePath.Value +
            templateParameter.Filename;

        // Create AbsolutePath as to leverage it for knowing the file extension and other details
        var emptyFileAbsolutePath = templateParameter.EnvironmentProvider.AbsolutePathFactory(
            emptyFileAbsolutePathString,
            false);

        var templatedFileContent = GetContent(
            emptyFileAbsolutePath.NameNoExtension,
            templateParameter.ParentDirectory.Namespace);

        var templatedFileAbsolutePathString = templateParameter.ParentDirectory.AbsolutePath.Value +
            emptyFileAbsolutePath.NameNoExtension +
            '.' +
            ExtensionNoPeriodFacts.C_SHARP_CLASS;

        var templatedFileAbsolutePath = templateParameter.EnvironmentProvider.AbsolutePathFactory(
            templatedFileAbsolutePathString,
            false);

        var templatedFileNamespacePath = new NamespacePath(
            templateParameter.ParentDirectory.Namespace,
            templatedFileAbsolutePath);

        return new FileTemplateResult(templatedFileNamespacePath, templatedFileContent);

        string GetContent(string fileNameNoExtension, string namespaceString) =>
            $@"namespace {namespaceString};

public class {fileNameNoExtension}
{{
	
}}
".ReplaceLineEndings();
    }

    /// <summary>
    /// Template should be:
    /// -------------------
    /// <h3>Asdf</h3>
    /// 
    /// @code {
    ///     
    /// }
    /// 
    /// </summary>
    private static FileTemplateResult RazorMarkupCreateFileFunc(FileTemplateParameter templateParameter)
    {
        var emptyFileAbsolutePathString = templateParameter.ParentDirectory.AbsolutePath.Value +
            templateParameter.Filename;

        // Create AbsolutePath as to leverage it for knowing the file extension and other details
        var emptyFileAbsolutePath = templateParameter.EnvironmentProvider.AbsolutePathFactory(
            emptyFileAbsolutePathString,
            false);

        var templatedFileContent = GetContent(emptyFileAbsolutePath.NameNoExtension);

        var templatedFileAbsolutePathString = templateParameter.ParentDirectory.AbsolutePath.Value +
            emptyFileAbsolutePath.NameNoExtension +
            '.' +
            ExtensionNoPeriodFacts.RAZOR_MARKUP;

        var templatedFileAbsolutePath = templateParameter.EnvironmentProvider.AbsolutePathFactory(
            templatedFileAbsolutePathString,
            false);

        var templatedFileNamespacePath = new NamespacePath(
            templateParameter.ParentDirectory.Namespace,
            templatedFileAbsolutePath);

        return new FileTemplateResult(templatedFileNamespacePath, templatedFileContent);

        string GetContent(string fileNameNoExtension) =>
            $@"<h3>{fileNameNoExtension}</h3>

@code {{
	
}}".ReplaceLineEndings();
    }

    /// <summary>
    /// Template should be:
    /// -------------------
    /// using Microsoft.AspNetCore.Components;
    /// 
    /// namespace Luthetus.Ide.RazorLib.Menu;
    /// 
    /// public partial class Asdf : ComponentBase
    /// {
    ///     
    /// }
    /// </summary>
    private static FileTemplateResult RazorCodebehindCreateFileFunc(FileTemplateParameter templateParameter)
    {
        string GetContent(string fileNameNoExtension, string namespaceString)
        {
            var className = fileNameNoExtension.Replace('.' + ExtensionNoPeriodFacts.RAZOR_MARKUP, string.Empty);

            var interpolatedResult = $@"using Microsoft.AspNetCore.Components;

namespace {namespaceString};

public partial class {className} : ComponentBase
{{
	
}}".ReplaceLineEndings();

            return interpolatedResult;
        }

        var emptyFileAbsolutePathString = templateParameter.ParentDirectory.AbsolutePath.Value +
            templateParameter.Filename;

        // Create AbsolutePath as to leverage it for knowing the file extension and other details
        var emptyFileAbsolutePath = templateParameter.EnvironmentProvider.AbsolutePathFactory(
            emptyFileAbsolutePathString,
            false);

        var templatedFileContent = GetContent(
            emptyFileAbsolutePath.NameNoExtension,
            templateParameter.ParentDirectory.Namespace);

        var templatedFileAbsolutePathString = templateParameter.ParentDirectory.AbsolutePath.Value +
            emptyFileAbsolutePath.NameNoExtension;

        if (templatedFileAbsolutePathString.EndsWith('.' + ExtensionNoPeriodFacts.RAZOR_MARKUP))
            templatedFileAbsolutePathString += '.' + ExtensionNoPeriodFacts.C_SHARP_CLASS;
        else
            templatedFileAbsolutePathString += '.' + ExtensionNoPeriodFacts.RAZOR_CODEBEHIND;

        var templatedFileAbsolutePath = templateParameter.EnvironmentProvider.AbsolutePathFactory(
            templatedFileAbsolutePathString,
            false);

        var templatedFileNamespacePath = new NamespacePath(
            templateParameter.ParentDirectory.Namespace,
            templatedFileAbsolutePath);

        return new FileTemplateResult(templatedFileNamespacePath, templatedFileContent);
    }
}