using Luthetus.Ide.ClassLib.FileConstants;
using System.Text;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.Common.RazorLib.FileSystem.Classes.LuthetusPath;

namespace Luthetus.Ide.ClassLib.FileTemplates;

public static class FileTemplateFacts
{
    public static readonly IFileTemplate CSharpClass = new FileTemplate(
        "C# Class",
        "bstudio-c-sharp-class",
        FileTemplateKind.CSharp,
        filename => filename
            .EndsWith('.' + ExtensionNoPeriodFacts.C_SHARP_CLASS),
        _ => ImmutableArray<IFileTemplate>.Empty,
        true,
        CSharpClassCreateFileFunc);

    public static readonly IFileTemplate RazorMarkup = new FileTemplate(
        "Razor markup",
        "bstudio-razor-markup-class",
        FileTemplateKind.Razor,
        filename => filename
            .EndsWith('.' + ExtensionNoPeriodFacts.RAZOR_MARKUP),
        _ => new[] { RazorCodebehind }.ToImmutableArray(),
        true,
        RazorMarkupCreateFileFunc);

    public static readonly IFileTemplate RazorCodebehind = new FileTemplate(
        "Razor codebehind",
        "bstudio-razor-codebehind-class",
        FileTemplateKind.Razor,
        filename => filename
            .EndsWith('.' + ExtensionNoPeriodFacts.RAZOR_CODEBEHIND),
        _ => ImmutableArray<IFileTemplate>.Empty,
        true,
        RazorCodebehindCreateFileFunc);

    /// <summary>
    /// namespace Luthetus.Ide.ClassLib.FileTemplates;
    ///
    /// public class Asdf
    /// {
    ///     
    /// }
    /// 
    /// </summary>
    private static FileTemplateResult CSharpClassCreateFileFunc(
        FileTemplateParameter fileTemplateParameter)
    {
        string GetContent(string fileNameNoExtension, string namespaceString)
        {
            var templateBuilder = new StringBuilder();

            templateBuilder.Append(
                $"namespace {namespaceString};{Environment.NewLine}");

            templateBuilder.Append(
                Environment.NewLine);

            templateBuilder.Append(
                $"public class {fileNameNoExtension}{Environment.NewLine}");

            templateBuilder.Append(
                $"{{{Environment.NewLine}");

            templateBuilder.Append(
                $"\t{Environment.NewLine}");

            templateBuilder.Append(
                $"}}{Environment.NewLine}");

            return templateBuilder.ToString();
        }

        var emptyFileAbsolutePathString = fileTemplateParameter
                                                  .ParentDirectory.AbsolutePath
                                                  .FormattedInput +
                                              fileTemplateParameter.Filename;

        // Create AbsolutePath as to leverage it for
        // knowing the file extension and other details
        var emptyFileAbsolutePath = new AbsolutePath(
            emptyFileAbsolutePathString,
            false,
            fileTemplateParameter.EnvironmentProvider);

        var templatedFileContent = GetContent(
            emptyFileAbsolutePath.NameNoExtension,
            fileTemplateParameter.ParentDirectory.Namespace);

        var templatedFileAbsolutePathString = fileTemplateParameter
                                                     .ParentDirectory.AbsolutePath
                                                     .FormattedInput +
                                                 emptyFileAbsolutePath.NameNoExtension +
                                                 '.' +
                                                 ExtensionNoPeriodFacts.C_SHARP_CLASS;

        var templatedFileAbsolutePath = new AbsolutePath(
            templatedFileAbsolutePathString,
            false,
            fileTemplateParameter.EnvironmentProvider);

        var templatedFileNamespacePath = new NamespacePath(
            fileTemplateParameter.ParentDirectory.Namespace,
            templatedFileAbsolutePath);

        return new FileTemplateResult(templatedFileNamespacePath, templatedFileContent);
    }

    /// <summary>
    /// <h3>Asdf</h3>
    /// 
    /// @code {
    ///     
    /// }
    /// 
    /// </summary>
    private static FileTemplateResult RazorMarkupCreateFileFunc(
        FileTemplateParameter fileTemplateParameter)
    {
        string GetContent(string fileNameNoExtension)
        {
            var templateBuilder = new StringBuilder();

            templateBuilder.Append(
                $"<h3>{fileNameNoExtension}</h3>{Environment.NewLine}");

            templateBuilder.Append(
                Environment.NewLine);

            templateBuilder.Append(
                $"@code {{{Environment.NewLine}");

            templateBuilder.Append(
                $"\t{Environment.NewLine}");

            templateBuilder.Append(
                $"}}{Environment.NewLine}");

            return templateBuilder.ToString();
        }

        var emptyFileAbsolutePathString = fileTemplateParameter
                                                  .ParentDirectory.AbsolutePath
                                                  .FormattedInput +
                                              fileTemplateParameter.Filename;

        // Create AbsolutePath as to leverage it for
        // knowing the file extension and other details
        var emptyFileAbsolutePath = new AbsolutePath(
            emptyFileAbsolutePathString,
            false,
            fileTemplateParameter.EnvironmentProvider);

        var templatedFileContent = GetContent(
            emptyFileAbsolutePath.NameNoExtension);

        var templatedFileAbsolutePathString = fileTemplateParameter
                                                     .ParentDirectory.AbsolutePath
                                                     .FormattedInput +
                                                 emptyFileAbsolutePath.NameNoExtension +
                                                 '.' +
                                                 ExtensionNoPeriodFacts.RAZOR_MARKUP;

        var templatedFileAbsolutePath = new AbsolutePath(
            templatedFileAbsolutePathString,
            false,
            fileTemplateParameter.EnvironmentProvider);

        var templatedFileNamespacePath = new NamespacePath(
            fileTemplateParameter.ParentDirectory.Namespace,
            templatedFileAbsolutePath);

        return new FileTemplateResult(templatedFileNamespacePath, templatedFileContent);
    }

    /// <summary>
    /// using Microsoft.AspNetCore.Components;
    /// 
    /// namespace Luthetus.Ide.RazorLib.Menu;
    /// 
    /// public partial class Asdf : ComponentBase
    /// {
    ///     
    /// }
    /// </summary>
    private static FileTemplateResult RazorCodebehindCreateFileFunc(
        FileTemplateParameter fileTemplateParameter)
    {
        string GetContent(string fileNameNoExtension, string namespaceString)
        {
            var templateBuilder = new StringBuilder();

            templateBuilder.Append(
                $"using Microsoft.AspNetCore.Components;{Environment.NewLine}");

            templateBuilder.Append(
                Environment.NewLine);

            templateBuilder.Append(
                $"namespace {namespaceString};{Environment.NewLine}");

            templateBuilder.Append(
                Environment.NewLine);

            templateBuilder.Append(
                $"public partial class" +
                $" {fileNameNoExtension.Replace('.' + ExtensionNoPeriodFacts.RAZOR_MARKUP, string.Empty)}" +
                $" : ComponentBase{Environment.NewLine}");

            templateBuilder.Append(
                $"{{{Environment.NewLine}");

            templateBuilder.Append(
                $"\t{Environment.NewLine}");

            templateBuilder.Append(
                $"}}{Environment.NewLine}");

            return templateBuilder.ToString();
        }

        var emptyFileAbsolutePathString = fileTemplateParameter
                                                  .ParentDirectory.AbsolutePath
                                                  .FormattedInput +
                                              fileTemplateParameter.Filename;

        // Create AbsolutePath as to leverage it for
        // knowing the file extension and other details
        var emptyFileAbsolutePath = new AbsolutePath(
            emptyFileAbsolutePathString,
            false,
            fileTemplateParameter.EnvironmentProvider);

        var templatedFileContent = GetContent(
            emptyFileAbsolutePath.NameNoExtension,
            fileTemplateParameter.ParentDirectory.Namespace);

        var templatedFileAbsolutePathString = fileTemplateParameter
                                                     .ParentDirectory.AbsolutePath
                                                     .FormattedInput +
                                                 emptyFileAbsolutePath.NameNoExtension;

        if (templatedFileAbsolutePathString.EndsWith(
                '.' + ExtensionNoPeriodFacts.RAZOR_MARKUP))
        {
            templatedFileAbsolutePathString +=
                '.' + ExtensionNoPeriodFacts.C_SHARP_CLASS;
        }
        else
        {
            templatedFileAbsolutePathString +=
                '.' + ExtensionNoPeriodFacts.RAZOR_CODEBEHIND;
        }

        var templatedFileAbsolutePath = new AbsolutePath(
            templatedFileAbsolutePathString,
            false,
            fileTemplateParameter.EnvironmentProvider);

        var templatedFileNamespacePath = new NamespacePath(
            fileTemplateParameter.ParentDirectory.Namespace,
            templatedFileAbsolutePath);

        return new FileTemplateResult(templatedFileNamespacePath, templatedFileContent);
    }
}