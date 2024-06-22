using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.Models.Internals;

public class SolutionVisualizationModel
{
	private readonly Action _onStateChangedAction;

	public SolutionVisualizationModel(IAbsolutePath? solutionAbsolutePath, Action onStateChangedAction)
	{
		SolutionAbsolutePath = solutionAbsolutePath;
		_onStateChangedAction = onStateChangedAction;
		Dimensions = new(_onStateChangedAction);
	}

	public IAbsolutePath? SolutionAbsolutePath { get; set; }
	public SolutionVisualizationDimensions Dimensions { get; set; }
	public List<ISolutionVisualizationDrawing> SolutionVisualizationDrawingList { get; set; } = new();
	public List<List<ISolutionVisualizationDrawing>> SolutionVisualizationDrawingRenderCycleList { get; set; } = new();

	public SolutionVisualizationModel ShallowClone()
	{
		return new SolutionVisualizationModel(SolutionAbsolutePath, _onStateChangedAction)
		{
			Dimensions = Dimensions,
			SolutionVisualizationDrawingList = new(SolutionVisualizationDrawingList),
			SolutionVisualizationDrawingRenderCycleList = new(SolutionVisualizationDrawingRenderCycleList),
		};
	}

	public SolutionVisualizationModel MakeDrawing(
		DotNetSolutionCompilerService dotNetSolutionCompilerService,
		CSharpProjectCompilerService cSharpProjectCompilerService,
		CSharpCompilerService cSharpCompilerService)
	{
		var localSolutionVisualizationModel = ShallowClone();
		localSolutionVisualizationModel.SolutionVisualizationDrawingList.Clear();
		localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList.Clear();

		if (SolutionAbsolutePath is null)
			return localSolutionVisualizationModel;

		var dotNetSolutionResource = dotNetSolutionCompilerService.CompilerServiceResources.FirstOrDefault(
			x => x.ResourceUri.Value == SolutionAbsolutePath.Value);

		if (dotNetSolutionResource is null)
			return localSolutionVisualizationModel;

		var cSharpProjectResourceList = cSharpProjectCompilerService.CompilerServiceResources;
		var cSharpResourceList = cSharpCompilerService.CompilerServiceResources;

		var radius = 12;
		var centerX = 12;
		var centerY = 12;
		var rowIndex = 0;
		var columnIndex = 0;
		var renderCycleIndex = 0;

		DrawSolution(dotNetSolutionResource, localSolutionVisualizationModel, radius, centerX, centerY, rowIndex, columnIndex, renderCycleIndex);
		renderCycleIndex++;
		rowIndex++;

		DrawProjects(cSharpProjectResourceList, localSolutionVisualizationModel, radius, centerX, centerY, rowIndex, columnIndex, renderCycleIndex);
		renderCycleIndex++;
		rowIndex++;

		DrawClasses(cSharpResourceList, localSolutionVisualizationModel, radius, centerX, centerY, rowIndex, columnIndex, renderCycleIndex);
		renderCycleIndex++;
		rowIndex++;

		return localSolutionVisualizationModel;
	}

	private void DrawSolution(
		ILuthCompilerServiceResource dotNetSolutionResource,
		SolutionVisualizationModel localSolutionVisualizationModel,
		int radius,
		int centerX,
		int centerY,
		int rowIndex,
		int columnIndex,
		int renderCycleIndex)
	{
		localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList.Add(new List<ISolutionVisualizationDrawing>());

		var dotNetSolutionDrawing = new SolutionVisualizationDrawing<DotNetSolutionResource>
		{
			Item = (DotNetSolutionResource)dotNetSolutionResource,
			SolutionVisualizationDrawingKind = SolutionVisualizationDrawingKind.Solution,
			CenterX = ((1 + columnIndex) * centerX) + (columnIndex * radius) + (columnIndex * localSolutionVisualizationModel.Dimensions.HorizontalPadding),
			CenterY = ((1 + rowIndex) * centerY) + (rowIndex * radius) + (rowIndex * localSolutionVisualizationModel.Dimensions.VerticalPadding),
			Radius = radius,
			Fill = "var(--luth_icon-solution-font-color)",
			RenderCycle = renderCycleIndex,
		};

		columnIndex++;

		localSolutionVisualizationModel.SolutionVisualizationDrawingList.Add(dotNetSolutionDrawing);
		localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList[renderCycleIndex].Add(dotNetSolutionDrawing);
	}

	private void DrawProjects(
		ImmutableArray<ILuthCompilerServiceResource> cSharpProjectResourceList,
		SolutionVisualizationModel localSolutionVisualizationModel,
		int radius,
		int centerX,
		int centerY,
		int rowIndex,
		int columnIndex,
		int renderCycleIndex)
	{
		localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList.Add(new List<ISolutionVisualizationDrawing>());

		foreach (var cSharpProjectResource in cSharpProjectResourceList)
		{
			var cSharpProjectDrawing = new SolutionVisualizationDrawing<CSharpProjectResource>
			{
				Item = (CSharpProjectResource)cSharpProjectResource,
				SolutionVisualizationDrawingKind = SolutionVisualizationDrawingKind.Project,
				CenterX = ((1 + columnIndex) * centerX) + (columnIndex * radius) + (columnIndex * localSolutionVisualizationModel.Dimensions.HorizontalPadding),
				CenterY = ((1 + rowIndex) * centerY) + (rowIndex * radius) + (rowIndex * localSolutionVisualizationModel.Dimensions.VerticalPadding),
				Radius = radius,
				Fill = "var(--luth_icon-project-font-color)",
				RenderCycle = renderCycleIndex,
			};

			columnIndex++;

			localSolutionVisualizationModel.SolutionVisualizationDrawingList.Add(cSharpProjectDrawing);
			localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList[renderCycleIndex].Add(cSharpProjectDrawing);
		}
	}

	private void DrawClasses(
		ImmutableArray<ILuthCompilerServiceResource> cSharpResourceList,
		SolutionVisualizationModel localSolutionVisualizationModel,
		int radius,
		int centerX,
		int centerY,
		int rowIndex,
		int columnIndex,
		int renderCycleIndex)
	{
		localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList.Add(new List<ISolutionVisualizationDrawing>());

		foreach (var cSharpResource in cSharpResourceList)
		{
			var cSharpDrawing = new SolutionVisualizationDrawing<CSharpResource>
			{
				Item = (CSharpResource)cSharpResource,
				SolutionVisualizationDrawingKind = SolutionVisualizationDrawingKind.Class,
				CenterX = ((1 + columnIndex) * centerX) + (columnIndex * radius) + (columnIndex * localSolutionVisualizationModel.Dimensions.HorizontalPadding),
				CenterY = ((1 + rowIndex) * centerY) + (rowIndex * radius) + (rowIndex * localSolutionVisualizationModel.Dimensions.VerticalPadding),
				Radius = radius,
				Fill = "var(--luth_icon-c-sharp-class-font-color)",
				RenderCycle = renderCycleIndex,
			};

			columnIndex++;

			localSolutionVisualizationModel.SolutionVisualizationDrawingList.Add(cSharpDrawing);
			localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList[renderCycleIndex].Add(cSharpDrawing);
		}
	}
}
