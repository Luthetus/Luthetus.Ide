using Fluxor;
using Luthetus.Common.RazorLib.ComponentRunners.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Luthetus.Common.RazorLib.ComponentRunners.Internals.Classes;

public record ComponentRunnerDisplayState(
    Key<ComponentRunnerDisplayState> Key,
    List<Type> ComponentTypeBag,
    Guid ChosenTypeGuid,
    Guid PreviousTypeGuid,
    PropertyInfo[] ComponentPropertyInfoBag,
    Dictionary<string, IComponentRunnerType> ComponentRunnerTypeParameterMap,
    IDispatcher Dispatcher) // This is gonna be a hack for re-rendering temporarily
{
    public Type? ChosenComponentType => ComponentTypeBag
        .FirstOrDefault(x => x.GUID == ChosenTypeGuid);

    private readonly object _parametersLock = new();

    public IComponentRunnerType GetComponentRunnerType(string name, IComponentRunnerType defaultValueIfKeyNotExists)
    {
        lock (_parametersLock)
        {
            if (!ComponentRunnerTypeParameterMap.TryGetValue(name, out var value))
            {
                value = defaultValueIfKeyNotExists;
                ComponentRunnerTypeParameterMap.Add(name, value);
            }

            return value;
        }
    }

    public void SetComponentRunnerType(string name, IComponentRunnerType value)
    {
        lock (_parametersLock)
        {
            if (!ComponentRunnerTypeParameterMap.TryAdd(name, value))
            {
                ComponentRunnerTypeParameterMap[name] = value;
            }
        }

        Dispatcher.Dispatch(new ComponentRunnerState.WithAction(
            Key,
            inState => inState with { }));

        BubbleUpValue(name, value);
    }

    public Dictionary<string, object?>? ConstructBlazorParameters()
    {
        var blazorParameters = new Dictionary<string, object?>();

        foreach (var parameterInfo in ComponentPropertyInfoBag)
        {
            var parameterKey = parameterInfo.Name;

            if (parameterInfo.PropertyType.IsPrimitive ||
                parameterInfo.PropertyType == typeof(string))
            {
                if (ComponentRunnerTypeParameterMap.TryGetValue(parameterKey, out var value))
                {
                    blazorParameters.Add(parameterKey, value.Value);
                }
            }
            else
            {
                if (ComponentRunnerTypeParameterMap.TryGetValue(parameterKey, out var value) &&
                    value.ChosenConstructorInfo is not null)
                {
                    var topLevelConstructorArgs = new List<object?>();

                    foreach (var constructorParameter in value.ChosenConstructorInfo.GetParameters())
                    {
                        topLevelConstructorArgs.Add(InvokeConstructorRecursively(
                            parameterKey,
                            constructorParameter));
                    }

                    var objectInstance = value.ChosenConstructorInfo.Invoke(
                        topLevelConstructorArgs.ToArray());

                    value.Value = objectInstance;

                    blazorParameters.Add(parameterKey, value.Value);
                }
            }
        }

        return blazorParameters;
    }

    public void CalculateComponentPropertyInfoBag(string? stringValue, ref int chosenComponentChangeCounter)
    {
        var refDisplayState = this;

        if (string.IsNullOrWhiteSpace(stringValue) ||
            stringValue == Guid.Empty.ToString() ||
            !Guid.TryParse(stringValue, out var chosenTypeGuid))
        {
            refDisplayState = refDisplayState with
            {
                ChosenTypeGuid = Guid.Empty
            };
        }
        else
        {
            refDisplayState = refDisplayState with
            {
                ChosenTypeGuid = chosenTypeGuid
            };
        }

        if (refDisplayState.PreviousTypeGuid != refDisplayState.ChosenTypeGuid)
        {
            refDisplayState = refDisplayState with
            {
                PreviousTypeGuid = refDisplayState.ChosenTypeGuid
            };

            chosenComponentChangeCounter++;

            var type = refDisplayState.ChosenComponentType;

            if (type is not null)
            {
                refDisplayState = refDisplayState with
                {
                    ComponentPropertyInfoBag = type
                        .GetProperties()
                        .Where(x => Attribute.IsDefined(x, typeof(ParameterAttribute)))
                        .ToArray()
                };
            }
            else
            {
                refDisplayState = refDisplayState with
                {
                    ComponentPropertyInfoBag = Array.Empty<PropertyInfo>()
                };
            }
        }

        Dispatcher.Dispatch(new ComponentRunnerState.WithAction(
            refDisplayState.Key, inDisplayState => inDisplayState with
            {
                ChosenTypeGuid = refDisplayState.ChosenTypeGuid,
                PreviousTypeGuid = refDisplayState.PreviousTypeGuid,
                ComponentPropertyInfoBag = refDisplayState.ComponentPropertyInfoBag,
                ComponentRunnerTypeParameterMap = new(),
            }));
    }

    /// <summary>
    /// TODO: Don't duplicate this logic, it is similar to <see cref="ConstructBlazorParameters"/>
    /// </summary>
    private object? InvokeConstructorRecursively(
        string parameterKey,
        ParameterInfo parameterInfo)
    {
        parameterKey += $".{parameterInfo.Name}";

        var objectInstance = (object?)null;

        if (parameterInfo.ParameterType.IsPrimitive ||
            parameterInfo.ParameterType == typeof(string))
        {
            if (ComponentRunnerTypeParameterMap.TryGetValue(parameterKey, out var value))
                objectInstance = value.Value;
        }
        else
        {
            if (ComponentRunnerTypeParameterMap.TryGetValue(parameterKey, out var value) &&
                value.ChosenConstructorInfo is not null)
            {
                var topLevelConstructorArgs = new List<object?>();

                foreach (var constructorParameter in value.ChosenConstructorInfo.GetParameters())
                {
                    topLevelConstructorArgs.Add(InvokeConstructorRecursively(
                        parameterKey,
                        constructorParameter));
                }

                objectInstance = value.ChosenConstructorInfo.Invoke(
                    topLevelConstructorArgs.ToArray());

                value.Value = objectInstance;
            }
        }

        return objectInstance;
    }    

    private void BubbleUpValue(string name, IComponentRunnerType value)
    {
        var splitName = name.Split('.');

        for (int i = 0; i < splitName.Length; i++)
        {
            if (i == splitName.Length - 1)
                break;

            var parentKey = string.Join('.', splitName.Take(i + 1));
            var childKey = string.Join('.', splitName.Take(i + 2));

            if (!ComponentRunnerTypeParameterMap.TryGetValue(parentKey, out var parentValue) ||
                !ComponentRunnerTypeParameterMap.TryGetValue(childKey, out var childValue))
                continue;

            if (parentValue.ChosenConstructorInfo is null ||
                childValue.ChosenConstructorInfo is null)
                continue;
        }
    }
}
