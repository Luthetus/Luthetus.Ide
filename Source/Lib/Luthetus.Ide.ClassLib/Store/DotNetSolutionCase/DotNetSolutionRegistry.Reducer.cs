using Fluxor;
using Luthetus.Common.RazorLib.FileSystem.Classes.FilePath;
using Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;
using Luthetus.Ide.ClassLib.WebsiteProjectTemplates;

namespace Luthetus.Ide.ClassLib.Store.DotNetSolutionCase;

public partial record DotNetSolutionRegistry
{
    private class Reducer
    {
        [ReducerMethod]
        public static DotNetSolutionRegistry ReduceRegisterAction(
            DotNetSolutionRegistry inDotNetSolutionState,
            RegisterAction registerAction)
        {
            var dotNetSolutionModel = inDotNetSolutionState.DotNetSolutionModel;

            if (dotNetSolutionModel is not null)
                return inDotNetSolutionState;

            var nextList = inDotNetSolutionState.DotNetSolutions.Add(
                registerAction.DotNetSolutionModel);

            return new DotNetSolutionRegistry
            {
                DotNetSolutions = nextList
            };
        }

        [ReducerMethod]
        public static DotNetSolutionRegistry ReduceDisposeAction(
            DotNetSolutionRegistry inDotNetSolutionState,
            DisposeAction disposeAction)
        {
            var dotNetSolutionModel = inDotNetSolutionState.DotNetSolutionModel;

            if (dotNetSolutionModel is null)
                return inDotNetSolutionState;

            var nextList = inDotNetSolutionState.DotNetSolutions.Remove(dotNetSolutionModel);

            return new DotNetSolutionRegistry
            {
                DotNetSolutions = nextList
            };
        }

        [ReducerMethod]
        public DotNetSolutionRegistry ReduceAddExistingProjectToSolutionAction(
            DotNetSolutionRegistry inDotNetSolutionState,
            AddExistingProjectToSolutionAction addExistingProjectToSolutionAction)
        {
            var indexOfDotNetSolutionModel = inDotNetSolutionState.DotNetSolutions.FindIndex(
                x => x.DotNetSolutionModelKey == addExistingProjectToSolutionAction.DotNetSolutionModelKey);

            if (indexOfDotNetSolutionModel == -1)
                return inDotNetSolutionState;

            var dotNetSolutionModel = inDotNetSolutionState.DotNetSolutions[indexOfDotNetSolutionModel];

            var projectTypeGuid = WebsiteProjectTemplateRegistry.GetProjectTypeGuid(
                addExistingProjectToSolutionAction.LocalProjectTemplateShortName);

            var relativePathFromSlnToProject = AbsoluteFilePath.ConstructRelativePathFromTwoAbsoluteFilePaths(
                dotNetSolutionModel.NamespacePath.AbsoluteFilePath,
                addExistingProjectToSolutionAction.CSharpProjectAbsoluteFilePath,
                addExistingProjectToSolutionAction.EnvironmentProvider);

            var projectIdGuid = Guid.NewGuid();

            var cSharpProject = new CSharpProject(
                addExistingProjectToSolutionAction.LocalCSharpProjectName,
                projectTypeGuid,
                relativePathFromSlnToProject,
                projectIdGuid);

            cSharpProject.SetAbsoluteFilePath(addExistingProjectToSolutionAction.CSharpProjectAbsoluteFilePath);

            var dotNetSolutionBuilder = dotNetSolutionModel.AddDotNetProject(
                cSharpProject,
                addExistingProjectToSolutionAction.EnvironmentProvider);

            var nextDotNetSolutions = inDotNetSolutionState.DotNetSolutions.SetItem(
                indexOfDotNetSolutionModel,
                dotNetSolutionBuilder.Build());

            return inDotNetSolutionState with
            {
                DotNetSolutions = nextDotNetSolutions
            };
        }
        
        [ReducerMethod]
        public DotNetSolutionRegistry ReduceWithAction(
            DotNetSolutionRegistry inDotNetSolutionState,
            WithAction withAction)
        {
            return withAction.WithFunc.Invoke(inDotNetSolutionState);
        }
    }
}