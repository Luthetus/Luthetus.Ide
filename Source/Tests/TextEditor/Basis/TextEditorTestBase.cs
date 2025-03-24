using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.TextEditor.Tests.Basis;

/// <summary>
/// It is preferred that each test defines its own test data within the '[Fact]' method itself. There however is 100 lines of
/// code at the beginning of every test if one follows this rule. To reduce the noise, this method was created.<br/><br/>
/// 
/// That being said, it is vital that the test defines its test-data, otherwise some readonly and static data might one day have
/// the source code for its definition change. Any dependent tests would then break, and a great deal of confusion would arise,
/// because it would not be clear that the test data was changed.<br/><br/>
/// 
/// For this reason, the '[Fact]' method must invoke this method with every parameter needed to construct the test data.
/// In this method, the exact same parameters that were passed in will be typed out in the source code for this method.<br/><br/>
/// 
/// If the parameters to this method, and the hardcoded local variables of this method match, then the test and this method
/// agree on what the test data is. At that point the test data can be constructed, and the pre-assertions can be done
/// from within this method.<br/><br/>
/// 
/// Lastly, return the test data.
/// </summary>
public class TextEditorTestBase
{
    protected (
        BackgroundTaskService backgroundTaskService,
        ContinuousBackgroundTaskWorker backgroundTaskWorker,
        IDispatcher dispatcher,
        ITextEditorService textEditorService) InitializeBackgroundTasks()
    {
        var backgroundTaskService = new BackgroundTaskService();
        var hostingInformation = new LuthetusHostingInformation(
        	LuthetusHostingKind.UnitTestingSynchronous,
        	LuthetusPurposeKind.TextEditor,
        	backgroundTaskService);

        var services = new ServiceCollection()
            .AddScoped<ILoggerFactory, NullLoggerFactory>()
            .AddScoped<IJSRuntime, DoNothingJsRuntime>()
            .AddLuthetusTextEditor(hostingInformation)
            .AddFluxor(options => options.ScanAssemblies(
                typeof(LuthetusCommonConfig).Assembly,
                typeof(LuthetusTextEditorConfig).Assembly));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        var backgroundTaskWorker = serviceProvider.GetRequiredService<ContinuousBackgroundTaskWorker>();

        return (
            backgroundTaskService,
            backgroundTaskWorker,
            serviceProvider.GetRequiredService<IDispatcher>(),
            serviceProvider.GetRequiredService<ITextEditorService>());
    }

    protected (TextEditorModel inModel, TextEditorModel modelModifier) EmptyEditor_TestData_And_PerformPreAssertions(
        ResourceUri resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string content,
        IDecorationMapper? decorationMapper,
        ICompilerService? compilerService,
        // partitionSize is an optional parameter to the TextEditorModel's constructor,
        // but provide a value so that if the default value changes, it doesn't break this test.
        int partitionSize)
    {
        // Agree with caller on what the test data is.
        {
            var expectedResourceUri = new ResourceUri($"/{nameof(EmptyEditor_TestData_And_PerformPreAssertions)}.txt");
            // Use 'DateTime.MinValue' because agreeing on 'DateTime.UtcNow' is a nightmare
            var expectedResourceLastWriteTime = DateTime.MinValue;
            var expectedFileExtension = ExtensionNoPeriodFacts.TXT;
            var expectedContent =
                (
                    string.Empty
                );
            var expectedDecorationMapper = (IDecorationMapper?)null;
            var expectedCompilerService = (ICompilerService?)null;
            // partitionSize is an optional parameter to the TextEditorModel's constructor,
            // but provide a value so that if the default value changes, it doesn't break this test.
            var expectedPartitionSize = 4096;

            Assert.Equal(expectedResourceUri, resourceUri);
            Assert.Equal(expectedResourceLastWriteTime, resourceLastWriteTime);
            Assert.Equal(expectedFileExtension, fileExtension);
            Assert.Equal(expectedContent, content);
            Assert.Equal(expectedDecorationMapper, decorationMapper);
            Assert.Equal(expectedCompilerService, compilerService);
            Assert.Equal(expectedPartitionSize, partitionSize);
        }

        // Create test data
        TextEditorModel inModel;
        TextEditorModel modelModifier;
        {
            inModel = new TextEditorModel(
                resourceUri,
                resourceLastWriteTime,
                fileExtension,
                content,
                decorationMapper,
                compilerService,
                partitionSize);

            modelModifier = new TextEditorModelModifier(inModel);
        }

        // Pre-assertions
        {
            // Obnoxiously write the constant value for the initialContent's length instead of capturing the TextEditorModel
            // constructor's 'initialContent' parameter, then checking '.Length'.
            //
            // This makes it more clear if the source text changes (accidentally or intentionally).
            // If one day this assertion fails, then someone touched the source text.
            Assert.Equal(0, modelModifier.CharCount);

            // The file extension should NOT change as a result of inserting content.
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            // The text is small, it should write a single partition, nothing more.
            Assert.Single(modelModifier.PartitionList);

            // No tab keys were included in the initial content for the TextEditorModel, therefore the Count is 0.
            Assert.Equal(0, modelModifier.TabKeyPositionList.Count);

            // LineEnd related code-block-grouping:
            {
                // No CarriageReturns were included in the initial content for the TextEditorModel, therefore the count is 0.
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);

                // No LineFeeds were included in the initial content for the TextEditorModel, therefore the count is 0.
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);

                // No CarriageReturnLineFeeds were included in the initial content for the TextEditorModel, therefore the count is 0.
                Assert.Equal(
                    0,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);

                // No line endings where included in the initial content for the TextEditorModel,
                // there is only the always existing 'EndOfFile' line ending, therefore the Count is 1.
                Assert.Equal(1, modelModifier.LineEndList.Count);

                // When invoking the constructor for the TextEditorModel,
                // an string.Empty was used, therefore there are no LineFeeds.
                // Assert that the only line ending in the text is the 'EndOfFile'.
                Assert.Equal(LineEndKind.EndOfFile, modelModifier.LineEndList.Single().LineEndKind);

                // When invoking the constructor for the TextEditorModel,
                // an string.Empty was used, therefore there are no CarriageReturns.
                // Assert that the only line ending in the text is the 'EndOfFile'.
                Assert.Equal(LineEndKind.EndOfFile, modelModifier.LineEndList.Single().LineEndKind);

                // When invoking the constructor for the TextEditorModel,
                // an string.Empty was used, therefore there are no CarriageReturnLineFeeds.
                // Assert that the only line ending in the text is the 'EndOfFile'.
                Assert.Equal(LineEndKind.EndOfFile, modelModifier.LineEndList.Single().LineEndKind);

                // A TextEditorModel always contains at least 1 LineEnd. This LineEnd marks the 'EndOfFile'.
                //
                // The constructor for 'TextEditorModel' takes the 'initialContent'
                // and sets the model's content as it, this results in the positionIndex for 'EndOfFile'
                // to be shifted by the length of the 'initialContent'.
                // But, the constructor was invoked with string.Empty, therefore,
                // the 'EndOfFile' is unchanged.
                var endOfFile = modelModifier.LineEndList.Single();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(0, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(0, endOfFile.EndPositionIndexExclusive);
            }
        }

        return (inModel, modelModifier);
    }

    /// <summary>
    /// Text Editor UI result (with show-newlines and show-whitespace; arrow is tab, and dot is space)<br/>
    /// =====================                                                                         <br/>
    /// 1 \n                                                                                          <br/>
    /// 2 b9\r                                                                                        <br/>
    /// 3 9B\r\n                                                                                      <br/>
    /// 4 --->$;∙EOF                                                                                  <br/>
    /// </summary>
    protected (TextEditorModel inModel, TextEditorModel modelModifier) NotEmptyEditor_TestData_And_PerformPreAssertions(
        ResourceUri resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string content,
        IDecorationMapper? decorationMapper,
        ICompilerService? compilerService,
        // partitionSize is an optional parameter to the TextEditorModel's constructor,
        // but provide a value so that if the default value changes, it doesn't break this test.
        int partitionSize)
    {
        // Agree with caller on what the test data is.
        {
            var expectedResourceUri = new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt");
            // Use 'DateTime.MinValue' because agreeing on 'DateTime.UtcNow' is a nightmare
            var expectedResourceLastWriteTime = DateTime.MinValue;
            var expectedFileExtension = ExtensionNoPeriodFacts.TXT;
            var expectedContent =
                (
                    "\n" +   // LineFeed
                    "b9" +   // LetterOrDigit-Lowercase
                    "\r" +   // CarriageReturn
                    "9B" +   // LetterOrDigit-Uppercase
                    "\r\n" + // CarriageReturnLineFeed
                    "\t" +   // Tab
                    "$" +    // SpecialCharacter
                    ";" +    // Punctuation
                    " "      // Space
                );
            var expectedDecorationMapper = (IDecorationMapper?)null;
            var expectedCompilerService = (ICompilerService?)null;
            // partitionSize is an optional parameter to the TextEditorModel's constructor,
            // but provide a value so that if the default value changes, it doesn't break this test.
            var expectedPartitionSize = 4096;

            Assert.Equal(expectedResourceUri, resourceUri);
            Assert.Equal(expectedResourceLastWriteTime, resourceLastWriteTime);
            Assert.Equal(expectedFileExtension, fileExtension);
            Assert.Equal(expectedContent, content);
            Assert.Equal(expectedDecorationMapper, decorationMapper);
            Assert.Equal(expectedCompilerService, compilerService);
            Assert.Equal(expectedPartitionSize, partitionSize);
        }

        // Create test data
        TextEditorModel inModel;
        TextEditorModel modelModifier;
        {
            inModel = new TextEditorModel(
                resourceUri,
                resourceLastWriteTime,
                fileExtension,
                content,
                decorationMapper,
                compilerService,
                partitionSize);

            modelModifier = new TextEditorModelModifier(inModel);
        }

        // Pre-assertions
        {
            // Obnoxiously write the constant value for the initialContent's length instead of capturing the TextEditorModel
            // constructor's 'initialContent' parameter, then checking '.Length'.
            //
            // This makes it more clear if the source text changes (accidentally or intentionally).
            // If one day this assertion fails, then someone touched the source text.
            Assert.Equal(12, modelModifier.CharCount);

            // The file extension should NOT change as a result of inserting content.
            Assert.Equal(ExtensionNoPeriodFacts.TXT, modelModifier.FileExtension);

            // The text is small, it should write a single partition, nothing more.
            Assert.Single(modelModifier.PartitionList);

            // 1 tab key was included in the string that was passed to the constructor.
            // Therefore, the Count is 1.
            Assert.Equal(1, modelModifier.TabKeyPositionList.Count);
            Assert.Equal(
                8,
                modelModifier.TabKeyPositionList.Single());

            // LineEnd related code-block-grouping:
            {
                // 1 CarriageReturn was included in the string that was passed to the constructor.
                // NOTE: While the Insert(...) method does not allow '\r' or '\r\n', the constructor does.
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
                {
                    var carriageReturn = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturn);
                    // StartPositionIndexInclusive
                    Assert.Equal(
                        3,
                        carriageReturn.StartPositionIndexInclusive);
                    // EndPositionIndexExclusive
                    Assert.Equal(
                        4,
                        carriageReturn.EndPositionIndexExclusive);
                }

                // 1 LineFeed was included in the string that was passed to the constructor.
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
                {
                    var lineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.LineFeed);
                    // StartPositionIndexInclusive
                    Assert.Equal(
                        0,
                        lineFeed.StartPositionIndexInclusive);
                    // EndPositionIndexExclusive
                    Assert.Equal(
                        1,
                        lineFeed.EndPositionIndexExclusive);
                }

                // 1 CarriageReturnLineFeed was included in the string that was passed to the constructor.
                // NOTE: While the Insert(...) method does not allow '\r' or '\r\n', the constructor does.
                Assert.Equal(
                    1,
                    modelModifier.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
                {
                    var carriageReturnLineFeed = modelModifier.LineEndList.Single(x => x.LineEndKind == LineEndKind.CarriageReturnLineFeed);
                    // StartPositionIndexInclusive
                    Assert.Equal(
                        6,
                        carriageReturnLineFeed.StartPositionIndexInclusive);
                    // EndPositionIndexExclusive
                    Assert.Equal(
                        8,
                        carriageReturnLineFeed.EndPositionIndexExclusive);
                }

                // 3 line endings where included in the string that was passed to the constructor,
                // There are 4 in total if one then includes the special-'EndOfFile' LineEnd.
                Assert.Equal(4, modelModifier.LineEndList.Count);

                // A TextEditorModel always contains at least 1 LineEnd. This LineEnd marks the 'EndOfFile'.
                //
                // The constructor for 'TextEditorModel' takes the 'initialContent'
                // and sets the model's content as it, this results in the positionIndex for 'EndOfFile'
                // to be shifted by the length of the 'initialContent'.
                var endOfFile = modelModifier.LineEndList.Last();
                Assert.Equal(LineEndKind.EndOfFile, endOfFile.LineEndKind);
                Assert.Equal(12, endOfFile.StartPositionIndexInclusive);
                Assert.Equal(12, endOfFile.EndPositionIndexExclusive);
            }
        }

        return (inModel, modelModifier);
    }
}
