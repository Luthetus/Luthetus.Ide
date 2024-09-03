using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Exceptions;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.Tests.Adhoc;

public partial class PartitionTests
{
	/// <summary>
	/// When a deletion of text spans multiple partitions it seems to sometimes break the line endings.
	///
	/// Why do I name things 'Aaa' on occassion?
	///
	/// "What Do You Do When You Can't Do Anything"
	/// https://youtu.be/JRqB2pm33IM?si=UhQxGhT8HcETN4xa
	///
	/// By using the name 'Aaa' consistently in this situations, I can easily find these naming scenarios
	/// and fix them when there is a better mood.
	/// </summary>
	[Fact]
	public async Task Aaa()
	{
		var test = TestInitialize();
		
		var modelList = test.TextEditorService.TextEditorStateWrap.Value.ModelList;
		Assert.Equal(0, modelList.Count);
		
		var model = new TextEditorModel(
			new ResourceUri("/unitTesting.cs"),
	        DateTime.UtcNow,
	        ExtensionNoPeriodFacts.C_SHARP_CLASS,
	        SAMPLE_CASE_THAT_HAS_LINE_ENDINGS_BREAK_BUG,
	        decorationMapper: null,
	        compilerService: null,
			partitionSize: 4_096);
			
		test.TextEditorService.ModelApi.RegisterCustom(model);

		var uniqueTextEditorWork = new UniqueTextEditorWork(
            nameof(PartitionTests),
            editContext =>
			{
				try
				{
					var modelModifier = editContext.GetModelModifier(model.ResourceUri);
	
		            if (modelModifier is null)
		            {
		            	Console.WriteLine("modelModifier is null");
		                return Task.CompletedTask;
		            }
		            
		            var openBraceMatchList = modelModifier.FindMatches("{");
		            var closeBraceMatchList = modelModifier.FindMatches("}");
		            
		            var openBodyBraceTextSpan = openBraceMatchList.First();
		            var closeBodyBraceTextSpan = closeBraceMatchList.Last();
		            
		            var cursor = new TextEditorCursor(isPrimaryCursor: true);
		            var cursorModifier = new TextEditorCursorModifier(cursor);
		            
		            cursorModifier.SelectionAnchorPositionIndex = openBodyBraceTextSpan.EndingIndexExclusive;
		            cursorModifier.SelectionEndingPositionIndex = closeBodyBraceTextSpan.StartingIndexInclusive;
		            
		            var closeBraceLineAndColumnIndices = modelModifier.GetLineAndColumnIndicesFromPositionIndex(
		            	closeBodyBraceTextSpan.StartingIndexInclusive);
		            	
		            cursorModifier.LineIndex = closeBraceLineAndColumnIndices.lineIndex;
		            cursorModifier.ColumnIndex = closeBraceLineAndColumnIndices.columnIndex;
		            
		            var cursorModifierBag = new CursorModifierBagTextEditor(
				        Key<TextEditorViewModel>.Empty,
				        new List<TextEditorCursorModifier> { cursorModifier });

                    // The text that is attempted to delete is actually correct.
                    // In TextEditorModelModifier.InProgress.cs in the method 'Delete(...)'
                    // The following variable is gotten:
                    //     'var textRemoved = this.GetString(positionIndex, charCount);'
                    //
                    // And if one manually selects the text in DotNetSolutionIdeApi.cs at the positions
                    // that this text is deleting, and copy and paste it and compare the two,
                    // its equal.
                    //
                    // So, is it a miscalculation of metadata or is it a miscalculation of the partitions?
                    //
                    // Its suspected to be a miscalculation of the partitions at the moment.
                    // Because when you delete the text in the IDE, the amount of line numbers seems
                    // reasonable (although I should check if its exactly correct).
                    //
                    // The only "off" detail is the gibberish that appears.
                    //
                    // If the partitions are being miscalculated, the path would be to look at
                    // TextEditorModelModifier.Partitions.cs the method 'void __RemoveRange(int globalPositionIndex, int count)'
                    modelModifier.Delete(
				        cursorModifierBag,
				        columnCount: 0, // Delete the selection, odd to give 0?
				        expandWord: false,
				        TextEditorModelModifier.DeleteKind.Delete);
				
					// Console.WriteLine($"\n\n{modelModifier.GetAllText()}\n\n");
					Console.WriteLine($"\n\nAppleSoupBanana\n\n");
					return Task.CompletedTask;
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					return Task.CompletedTask;
				}
			});
			
		await test.TextEditorService.PostAsync(uniqueTextEditorWork).ConfigureAwait(false);
		
		throw new NotImplementedException("In progress of writing this test");
	}
	
	/// <summary>
	/// Second, smaller example
	/// </summary>
	[Fact]
	public async Task Bbb()
	{
		var test = TestInitialize();
		
		var modelList = test.TextEditorService.TextEditorStateWrap.Value.ModelList;
		Assert.Equal(0, modelList.Count);
		
		var model = new TextEditorModel(
			new ResourceUri("/unitTesting.cs"),
	        DateTime.UtcNow,
	        ExtensionNoPeriodFacts.C_SHARP_CLASS,
            @"namespace BlazorApp4NetCoreDbg.Persons;

public class Abc
{
	
}
".ReplaceLineEndings("\r\n"),
	        decorationMapper: null,
	        compilerService: null,
			partitionSize: 4_096);
			
		test.TextEditorService.ModelApi.RegisterCustom(model);

		Exception? exception = null;

		var uniqueTextEditorWork = new UniqueTextEditorWork(
            nameof(PartitionTests),
            editContext =>
			{
				try
				{
					var modelModifier = editContext.GetModelModifier(model.ResourceUri);
	
		            if (modelModifier is null)
		            {
		            	Console.WriteLine("modelModifier is null");
		                return Task.CompletedTask;
		            }
		            
		            var cursor = new TextEditorCursor(isPrimaryCursor: true);
		            var cursorModifier = new TextEditorCursorModifier(cursor);
		            
		            cursorModifier.LineIndex = 2;
		            cursorModifier.ColumnIndex = 8;

		            var cursorPositionIndex = modelModifier.GetPositionIndex(cursorModifier);

		            cursorModifier.SelectionAnchorPositionIndex = cursorPositionIndex;
		            cursorModifier.SelectionEndingPositionIndex = cursorPositionIndex + 2;

                    // Select the "la" of "class" on the line "public class Abc"

                    var cursorModifierBag = new CursorModifierBagTextEditor(
				        Key<TextEditorViewModel>.Empty,
				        new List<TextEditorCursorModifier> { cursorModifier });

                    modelModifier.Delete(
				        cursorModifierBag,
				        columnCount: 0, // Delete the selection, odd to give 0?
				        expandWord: false,
				        TextEditorModelModifier.DeleteKind.Delete);
				
					Console.WriteLine($"\n\n{modelModifier.GetAllText()}\n\n");

					var expectedText = @"namespace BlazorApp4NetCoreDbg.Persons;

public css Abc
{
	
}
".ReplaceLineEndings("\r\n");

					var actualText = modelModifier.GetAllText();

                    Assert.Equal(expectedText, actualText);

					// Console.WriteLine($"\n\nAppleSoupBanana\n\n");
					return Task.CompletedTask;
				}
				catch (Exception e)
				{
					exception = e;

                    Console.WriteLine(e);
					return Task.CompletedTask;
				}
			});
			
		await test.TextEditorService.PostAsync(uniqueTextEditorWork).ConfigureAwait(false);

		if (exception is not null)
			throw exception;
		
		throw new NotImplementedException("In progress of writing this test");
	}
	
	public class TestContext
	{
		public TestContext(LuthetusHostingInformation hostingInformation, IServiceProvider serviceProvider)
		{
			HostingInformation = hostingInformation;
			ServiceProvider = serviceProvider;
			
			TextEditorService = ServiceProvider.GetRequiredService<ITextEditorService>();
			ContinuousBackgroundTaskWorker = ServiceProvider.GetRequiredService<ContinuousBackgroundTaskWorker>();
			
			HostingInformation.StartBackgroundTaskWorkers(serviceProvider);
		}
	
		public LuthetusHostingInformation HostingInformation { get; }
		public IServiceProvider ServiceProvider { get; }
		
		public ITextEditorService TextEditorService { get; }
		public ContinuousBackgroundTaskWorker ContinuousBackgroundTaskWorker { get; }
	}
	
	public TestContext TestInitialize()
	{
		var services = new ServiceCollection();
		
		var hostingInformation = new LuthetusHostingInformation(
			LuthetusHostingKind.UnitTestingAsync,
	        LuthetusPurposeKind.TextEditor,
	        new BackgroundTaskService());
		
		services
			.AddLuthetusTextEditor(hostingInformation)
			.AddScoped<ILoggerFactory, NullLoggerFactory>()
			.AddScoped<IJSRuntime, DoNothingJsRuntime>();
            
		// If one doesn't use a LuthetusPurposeKind of Photino,
		// then this code block is redundant, because the ServieCollectionExtensions.cs
		// for Luthetus.Common will register InMemory for Environment and FileSystem providers.
		//
		// But, because of the importance that under no circumstance one's actual FileSystem is used during
		// unit testing, this redundancy is here.
		{
			services.AddScoped<IEnvironmentProvider, InMemoryEnvironmentProvider>();
			services.AddScoped<IFileSystemProvider, InMemoryFileSystemProvider>();
		}
		
		// At the moment one has to manually set the following services to their InMemory implementation.
		{
			services.AddScoped<IClipboardService, InMemoryClipboardService>();
			services.AddScoped<IStorageService, InMemoryStorageService>();
		}
		
		services.AddFluxor(options => options.ScanAssemblies(
			typeof(LuthetusCommonConfig).Assembly,
			typeof(LuthetusTextEditorConfig).Assembly));
		
		var testContext = new TestContext(hostingInformation, services.BuildServiceProvider());
		
		// Check various services that should be InMemory implementations.
		{	
			{
				var fileSystemProvider = testContext.ServiceProvider.GetRequiredService<IFileSystemProvider>();
				if (fileSystemProvider is not InMemoryFileSystemProvider)
					throw new LuthetusUnitTestException(
						$"A UnitTest's {nameof(IFileSystemProvider)} must be " +
						$"{nameof(InMemoryFileSystemProvider)}, " +
						$"but was {fileSystemProvider.GetType().Name}");
			}
			
			{
				var environmentProvider = testContext.ServiceProvider.GetRequiredService<IEnvironmentProvider>();
				if (environmentProvider is not InMemoryEnvironmentProvider)
					throw new LuthetusUnitTestException(
						$"A UnitTest's {nameof(IEnvironmentProvider)} must be " +
						$"{nameof(InMemoryEnvironmentProvider)}, " +
						$"but was {environmentProvider.GetType().Name}");
			}
	
			{
				var clipboardService = testContext.ServiceProvider.GetRequiredService<IClipboardService>();
				if (clipboardService is not InMemoryClipboardService)
					throw new LuthetusUnitTestException(
						$"A UnitTest's {nameof(IClipboardService)} must be " +
						$"{nameof(InMemoryClipboardService)}, " +
						$"but was {clipboardService.GetType().Name}");
			}
	
			{
				var storageService = testContext.ServiceProvider.GetRequiredService<IStorageService>();
				if (storageService is not InMemoryStorageService)
					throw new LuthetusUnitTestException(
						$"A UnitTest's {nameof(IStorageService)} must be " +
						$"{nameof(InMemoryStorageService)}, " +
						$"but was {storageService.GetType().Name}");
			}
		}
		
		var store = testContext.ServiceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();
	
		return testContext;
	}
}
