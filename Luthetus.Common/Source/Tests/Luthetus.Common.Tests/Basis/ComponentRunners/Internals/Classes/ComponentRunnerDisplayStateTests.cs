using Fluxor;
using Luthetus.Common.RazorLib.ComponentRunners.States;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Reflection;

namespace Luthetus.Common.RazorLib.ComponentRunners.Internals.Classes;

public class ComponentRunnerDisplayStateTests
{
    [Fact]
    public void ChosenComponentType()
    {
        /*
        public Type? ChosenComponentType => ComponentTypeBag
            .FirstOrDefault(x => x.GUID == ChosenTypeGuid);
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void GetComponentRunnerType()
    {
        /*
        public IComponentRunnerType GetComponentRunnerType(string name, IComponentRunnerType defaultValueIfKeyNotExists)
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void SetComponentRunnerType()
    {
        /*
        public void SetComponentRunnerType(string name, IComponentRunnerType value)
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void ConstructBlazorParameters()
    {
        /*
        public Dictionary<string, object?>? ConstructBlazorParameters()
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void InvokeConstructorRecursively()
    {
        /*
        public object? InvokeConstructorRecursively(
            string parameterKey, ParameterInfo parameterInfo)
         */

        throw new NotImplementedException();
    }
}
