using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Faker.Core.Extensions.Type;
using Faker.Core.Generators.Core.Generators.TypeCreators.utils;

namespace Faker.Core.Generators.Core.Validator;

public static class CircularDependencyDetector
{
    public static bool HasCircularDependency(ConstructorInfo rootConstructor)
    {
        HashSet<Type> processingTypes = new HashSet<Type>();
        
        Type? constructorType = rootConstructor.DeclaringType;
        
        if (constructorType != null)
        {
            processingTypes.Add(constructorType);
        }
        
        return CheckParameters(rootConstructor, processingTypes);
    }

    private static bool CheckParameters(ConstructorInfo constructor, HashSet<Type> processingTypes)
    {
        foreach (ParameterInfo parameter in constructor.GetParameters())
        {
            Type type = parameter.ParameterType;

            if (CheckParameterTypeForCircularDependency(constructor, processingTypes, type))
            {
                return true;
            }
        }
        
        return false;
    }

    private static bool CheckParameterTypeForCircularDependency(ConstructorInfo constructor, HashSet<Type> processingTypes, Type type)
    {
        if (type == constructor.DeclaringType)
        {
            return true;
        }

        return CheckNonDeclaringTypeForCircularDependency(processingTypes, type);
    }

    private static bool CheckNonDeclaringTypeForCircularDependency(HashSet<Type> processingTypes, Type type)
    {
        bool isCycle = false;
        if (type.CanProduceCircularDependency())
        {
            isCycle = CheckPossibleCircularDependencyType(processingTypes, type);
        }

        return isCycle;
    }

    private static bool CheckPossibleCircularDependencyType(HashSet<Type> processingTypes, Type type)
    {
        if (!processingTypes.Add(type))
        {
            return true;
        }

        return CheckConstructorOfTypeForCycles(processingTypes, type);
    }

    private static bool CheckConstructorOfTypeForCycles(HashSet<Type> processingTypes, Type type)
    {
        ConstructorInfo? nextConstructor = ConstructorInfoUtils
            .GetConstructorsOfTypeSortedByParameterCount(type)
            .FirstOrDefault();
        
        bool isCycle = CheckNextConstructorForCycleIfNotNull(processingTypes, nextConstructor);
        if (!isCycle)
        {
            processingTypes.Remove(type);
        }
        
        return isCycle;
    }

    private static bool CheckNextConstructorForCycleIfNotNull(HashSet<Type> processingTypes, ConstructorInfo? nextConstructor)
    {
        bool isCycle = false;
        if (nextConstructor != null)
        {
            isCycle = CheckParameters(nextConstructor, processingTypes);
        }

        return isCycle;
    }
}   