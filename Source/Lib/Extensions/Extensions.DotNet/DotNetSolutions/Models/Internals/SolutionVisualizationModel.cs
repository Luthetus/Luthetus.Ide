using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.CompilerServices.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.CSharpProject.CompilerServiceCase;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Models.Internals;

public class SolutionVisualizationModel
{
	public const int RENDER_CYCLE_LIST_CONNECTIONS_INDEX = 0;
	public const int RENDER_CYCLE_LIST_SOLUTION_INDEX = 1;
	public const int RENDER_CYCLE_LIST_PROJECTS_INDEX = 2;
	public const int RENDER_CYCLE_LIST_CLASSES_INDEX = 3;

	private readonly Action _onStateChangedAction;

	public SolutionVisualizationModel(AbsolutePath? solutionAbsolutePath, Action onStateChangedAction)
	{
		SolutionAbsolutePath = solutionAbsolutePath;
		_onStateChangedAction = onStateChangedAction;
		Dimensions = new(_onStateChangedAction);
	}

	public AbsolutePath? SolutionAbsolutePath { get; set; }
	public SolutionVisualizationDimensions Dimensions { get; set; }
	public List<ISolutionVisualizationDrawing> SolutionVisualizationDrawingList { get; set; } = new();
	public List<List<ISolutionVisualizationDrawing>> SolutionVisualizationDrawingRenderCycleList { get; set; } = new();
	public Dictionary<string, ISolutionVisualizationDrawing> ParentMap { get; set; } = new();

	public SolutionVisualizationModel ShallowClone()
	{
		return new SolutionVisualizationModel(SolutionAbsolutePath, _onStateChangedAction)
		{
			Dimensions = Dimensions,
			SolutionVisualizationDrawingList = new(SolutionVisualizationDrawingList),
			SolutionVisualizationDrawingRenderCycleList = new(SolutionVisualizationDrawingRenderCycleList),
			ParentMap = new(ParentMap),
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

		// Do not clear the 'ParentMap', it will stop the connections from project to class.
		// localSolutionVisualizationModel.ParentMap.Clear();

		if (SolutionAbsolutePath?.ExactInput is null)
			return localSolutionVisualizationModel;

		var dotNetSolutionResource = dotNetSolutionCompilerService.CompilerServiceResources.FirstOrDefault(
			x => x.ResourceUri.Value == SolutionAbsolutePath.Value.Value);

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

		// Regarding drawing the connections between the solution node, and its child project nodes.
		//
		// We need to draw the connections, before the nodes themselves, as to permit the connections be drawn
		// from the center of each node, rather than calculating the arc on the edge of the circle.
		//
		// i.e.: if the connections are drawn first, the node will perfectly cover the connection, curvature and all,
		//       even though the connection is simply a line from one point to another.
		//
		// That being said, we don't know the position of the nodes until their respective 'Draw...(...)' methods are invoked.
		// But, these draw methods don't actually render anything on the UI. It is just populating a list,
		// in which from index 0 to the end of that list, things will be rendered in that order.
		//
		// So, if at the end we insert the connections at index 0 of the "to be rendered list", it should work out.
		// An issue however, there are properties on the 'ISolutionVisualizationDrawing.cs' relating to the order that things were rendered.
		//
		// If we do not set these properties correctly, the C# code won't be able to properly determine what UI element was
		// clicked, (when two UI elements overlap, the most recently rendered element receives the click event).
		//
		// Therefore, an awkard incrementation of 'renderCycleIndex' is being done here. This leaves renderCycleIndex of index 0
		// free for use at a later time. renderCycleIndex of index 0 is saying, "this was part of the UI element group that was first rendered".
		//
		// Thus, if one clicks a node, the "covered connection" that extends through the perimiter of a node, to its center point.
		// This element won't get the click event.
		{
			// Reserve renderCycleIndex of 0 for the connections drawn between nodes.
			localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList.Add(new List<ISolutionVisualizationDrawing>());
			renderCycleIndex++;
		}

		DrawSolution(dotNetSolutionResource, localSolutionVisualizationModel, radius, centerX, centerY, rowIndex, columnIndex, renderCycleIndex);
		renderCycleIndex++;
		rowIndex++;

		DrawProjects(cSharpProjectResourceList, localSolutionVisualizationModel, radius, centerX, centerY, rowIndex, columnIndex, renderCycleIndex);
		renderCycleIndex++;
		rowIndex++;

		DrawClasses(cSharpResourceList, localSolutionVisualizationModel, radius, centerX, centerY, rowIndex, columnIndex, renderCycleIndex);
		renderCycleIndex++;
		rowIndex++;

		DrawConnections(localSolutionVisualizationModel, renderCycleIndex: 0);

		return localSolutionVisualizationModel;
	}

	private void DrawSolution(
		ICompilerServiceResource dotNetSolutionResource,
		SolutionVisualizationModel localSolutionVisualizationModel,
		int radius,
		int centerX,
		int centerY,
		int rowIndex,
		int columnIndex,
		int renderCycleIndex)
	{
		centerX = localSolutionVisualizationModel.Dimensions.SvgWidth / 2 - radius / 2;

		localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList.Add(new List<ISolutionVisualizationDrawing>());

		var dotNetSolutionDrawing = new SolutionVisualizationDrawingCircle<DotNetSolutionResource>
		{
			Item = (DotNetSolutionResource)dotNetSolutionResource,
			SolutionVisualizationItemKind = SolutionVisualizationItemKind.Solution,
			CenterX = (1 + columnIndex) * centerX + columnIndex * radius + columnIndex * localSolutionVisualizationModel.Dimensions.HorizontalPadding,
			CenterY = (1 + rowIndex) * centerY + rowIndex * radius + rowIndex * localSolutionVisualizationModel.Dimensions.VerticalPadding,
			Radius = radius,
			Fill = "var(--luth_icon-solution-font-color)",
			RenderCycle = renderCycleIndex,
		};

		columnIndex++;

		localSolutionVisualizationModel.SolutionVisualizationDrawingList.Add(dotNetSolutionDrawing);
		localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList[renderCycleIndex].Add(dotNetSolutionDrawing);
	}

	private void DrawProjects(
		IReadOnlyList<ICompilerServiceResource> cSharpProjectResourceList,
		SolutionVisualizationModel localSolutionVisualizationModel,
		int radius,
		int centerX,
		int centerY,
		int rowIndex,
		int columnIndex,
		int renderCycleIndex)
	{
		var solutionCenterX = localSolutionVisualizationModel.Dimensions.SvgWidth / 2 - radius / 2;
		var leftNodeCounter = 0;
		var rightNodeCounter = 0;

		localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList.Add(new List<ISolutionVisualizationDrawing>());

		for (int i = 0; i < cSharpProjectResourceList.Count; i++)
		{
			var cSharpProjectResource = cSharpProjectResourceList[i];

			if (i % 2 == 0)
			{
				leftNodeCounter++;
				var solutionRadius = radius;
				var myRadius = leftNodeCounter * radius;
				var previousCirclesRadius = (leftNodeCounter - 1) * radius;
				var cumulativeHorizontalPadding = (leftNodeCounter - 1) * localSolutionVisualizationModel.Dimensions.HorizontalPadding;
				centerX = solutionCenterX - solutionRadius - myRadius - previousCirclesRadius - cumulativeHorizontalPadding;
			}
			else
			{
				rightNodeCounter++;
				var solutionRadius = radius;
				var myRadius = rightNodeCounter * radius;
				var previousCirclesRadius = (rightNodeCounter - 1) * radius;
				var cumulativeHorizontalPadding = (rightNodeCounter - 1) * localSolutionVisualizationModel.Dimensions.HorizontalPadding;
				centerX = solutionCenterX + solutionRadius + myRadius + previousCirclesRadius + cumulativeHorizontalPadding;
			}

			var cSharpProjectDrawing = new SolutionVisualizationDrawingCircle<CSharpProjectResource>
			{
				Item = (CSharpProjectResource)cSharpProjectResource,
				SolutionVisualizationItemKind = SolutionVisualizationItemKind.Project,
				CenterX = centerX,
				CenterY = (1 + rowIndex) * centerY + rowIndex * radius + rowIndex * localSolutionVisualizationModel.Dimensions.VerticalPadding,
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
		IReadOnlyList<ICompilerServiceResource> cSharpResourceList,
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
			var cSharpDrawing = new SolutionVisualizationDrawingCircle<CSharpResource>
			{
				Item = (CSharpResource)cSharpResource,
				SolutionVisualizationItemKind = SolutionVisualizationItemKind.Class,
				CenterX = (1 + columnIndex) * centerX + columnIndex * radius + columnIndex * localSolutionVisualizationModel.Dimensions.HorizontalPadding,
				CenterY = (1 + rowIndex) * centerY + rowIndex * radius + rowIndex * localSolutionVisualizationModel.Dimensions.VerticalPadding,
				Radius = radius,
				Fill = "var(--luth_icon-c-sharp-class-font-color)",
				RenderCycle = renderCycleIndex,
			};

			columnIndex++;

			localSolutionVisualizationModel.SolutionVisualizationDrawingList.Add(cSharpDrawing);
			localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList[renderCycleIndex].Add(cSharpDrawing);
		}
	}

	private void DrawConnections(
		SolutionVisualizationModel localSolutionVisualizationModel,
		int renderCycleIndex)
	{
		var solutionDrawing = localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList[RENDER_CYCLE_LIST_SOLUTION_INDEX].Single();
		var projectDrawingList = localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList[RENDER_CYCLE_LIST_PROJECTS_INDEX];

		// From solution to project connections
		foreach (var projectDrawing in projectDrawingList)
		{
			var solutionDrawingCircle = (ISolutionVisualizationDrawingCircle)solutionDrawing;
			var projectDrawingCircle = (ISolutionVisualizationDrawingCircle)projectDrawing;

			var connectionDrawing = new SolutionVisualizationDrawingLine<SolutionVisualizationConnection>
			{
				Item = new SolutionVisualizationConnection(solutionDrawing, projectDrawing),
				SolutionVisualizationItemKind = SolutionVisualizationItemKind.Connection,
				StartPoint = (solutionDrawingCircle.CenterX, solutionDrawingCircle.CenterY),
				EndPoint = (projectDrawingCircle.CenterX, projectDrawingCircle.CenterY),
				Stroke = "var(--luth_primary-foreground-color)",
				RenderCycle = renderCycleIndex,
			};

			localSolutionVisualizationModel.SolutionVisualizationDrawingList.Insert(0, connectionDrawing);
			localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList[renderCycleIndex].Add(connectionDrawing);
		}

		var classDrawingList = localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList[RENDER_CYCLE_LIST_CLASSES_INDEX];

		// From project to class connections
		foreach (var classDrawing in classDrawingList)
		{
			var cSharpResource = (CSharpResource)classDrawing.Item;

			Console.WriteLine($"localSolutionVisualizationModel.ParentMap::{localSolutionVisualizationModel.ParentMap.Count}");

			if (!localSolutionVisualizationModel.ParentMap.TryGetValue(cSharpResource.ResourceUri.Value, out var projectDrawing))
				continue;

			var projectDrawingCircle = (ISolutionVisualizationDrawingCircle)projectDrawing;
			var classDrawingCircle = (ISolutionVisualizationDrawingCircle)classDrawing;

			var connectionDrawing = new SolutionVisualizationDrawingLine<SolutionVisualizationConnection>
			{
				Item = new SolutionVisualizationConnection(projectDrawing, classDrawingCircle),
				SolutionVisualizationItemKind = SolutionVisualizationItemKind.Connection,
				StartPoint = (projectDrawingCircle.CenterX, projectDrawingCircle.CenterY),
				EndPoint = (classDrawingCircle.CenterX, classDrawingCircle.CenterY),
				Stroke = "var(--luth_primary-foreground-color)",
				RenderCycle = renderCycleIndex,
			};

			localSolutionVisualizationModel.SolutionVisualizationDrawingList.Insert(0, connectionDrawing);
			localSolutionVisualizationModel.SolutionVisualizationDrawingRenderCycleList[renderCycleIndex].Add(connectionDrawing);
		}
	}
}
