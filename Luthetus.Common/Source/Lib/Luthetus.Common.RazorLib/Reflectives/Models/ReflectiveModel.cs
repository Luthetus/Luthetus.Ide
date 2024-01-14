using Fluxor;
using Luthetus.Common.RazorLib.Reflectives.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Luthetus.Common.RazorLib.Reflectives.Models;

public record ReflectiveModel(
    Key<ReflectiveModel> Key,
    List<Type> ComponentTypeList,
    Guid ChosenTypeGuid,
    Guid PreviousTypeGuid,
    PropertyInfo[] ComponentPropertyInfoList,
    Dictionary<string, IReflectiveParameter> ReflectiveParameterMap,
    IDispatcher Dispatcher) // This is gonna be a hack for re-rendering temporarily
{
    public Type? ChosenType => ComponentTypeList
        .FirstOrDefault(x => x.GUID == ChosenTypeGuid);

    private readonly object _parametersLock = new();

    public IReflectiveParameter GetParameter(string variableName, IReflectiveParameter defaultValueIfKeyNotExists)
    {
        lock (_parametersLock)
        {
            if (!ReflectiveParameterMap.TryGetValue(variableName, out var value))
            {
                value = defaultValueIfKeyNotExists;
                ReflectiveParameterMap.Add(variableName, value);
            }

            return value;
        }
    }

    public void SetParameter(string name, IReflectiveParameter value)
    {
        lock (_parametersLock)
        {
            if (!ReflectiveParameterMap.TryAdd(name, value))
            {
                ReflectiveParameterMap[name] = value;
            }
        }

        Dispatcher.Dispatch(new ReflectiveState.WithAction(
            Key,
            inState => inState with { }));

        BubbleUpValue(name, value);
    }

    public Dictionary<string, object?>? ConstructBlazorParameters()
    {
        var blazorParameters = new Dictionary<string, object?>();

        foreach (var parameterInfo in ComponentPropertyInfoList)
        {
            var parameterKey = parameterInfo.Name;

            if (parameterInfo.PropertyType.IsPrimitive ||
                parameterInfo.PropertyType == typeof(string))
            {
                if (ReflectiveParameterMap.TryGetValue(parameterKey, out var value))
                {
                    blazorParameters.Add(parameterKey, value.Value);
                }
            }
            else
            {
                if (ReflectiveParameterMap.TryGetValue(parameterKey, out var value) &&
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

    public void CalculateComponentPropertyInfoList(string? chosenTypeGuidString, ref int chosenComponentChangeCounter)
    {
        var refDisplayState = this;

        if (string.IsNullOrWhiteSpace(chosenTypeGuidString) ||
            chosenTypeGuidString == Guid.Empty.ToString() ||
            !Guid.TryParse(chosenTypeGuidString, out var chosenTypeGuid))
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

            var type = refDisplayState.ChosenType;

            if (type is not null)
            {
                refDisplayState = refDisplayState with
                {
                    ComponentPropertyInfoList = type
                        .GetProperties()
                        .Where(x => Attribute.IsDefined(x, typeof(ParameterAttribute)))
                        .ToArray()
                };
            }
            else
            {
                refDisplayState = refDisplayState with
                {
                    ComponentPropertyInfoList = Array.Empty<PropertyInfo>()
                };
            }
        }

        Dispatcher.Dispatch(new ReflectiveState.WithAction(
            refDisplayState.Key, inDisplayState => inDisplayState with
            {
                ChosenTypeGuid = refDisplayState.ChosenTypeGuid,
                PreviousTypeGuid = refDisplayState.PreviousTypeGuid,
                ComponentPropertyInfoList = refDisplayState.ComponentPropertyInfoList,
                ReflectiveParameterMap = new(),
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
            if (ReflectiveParameterMap.TryGetValue(parameterKey, out var value))
                objectInstance = value.Value;
        }
        else
        {
            if (ReflectiveParameterMap.TryGetValue(parameterKey, out var value) &&
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

    private void BubbleUpValue(string name, IReflectiveParameter value)
    {
        var splitName = name.Split('.');

        for (int i = 0; i < splitName.Length; i++)
        {
            if (i == splitName.Length - 1)
                break;

            var parentKey = string.Join('.', splitName.Take(i + 1));
            var childKey = string.Join('.', splitName.Take(i + 2));

            if (!ReflectiveParameterMap.TryGetValue(parentKey, out var parentValue) ||
                !ReflectiveParameterMap.TryGetValue(childKey, out var childValue))
                continue;

            if (parentValue.ChosenConstructorInfo is null ||
                childValue.ChosenConstructorInfo is null)
                continue;
        }
    }
}
