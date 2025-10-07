using NUnit.Framework;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.TestTools;

public class SceneLoopPlayModeTests
{
    private static Type FindType(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType(typeName, false);
            if (type != null)
            {
                return type;
            }
        }

        Assert.Fail($"Type not found: {typeName}");
        return null;
    }

    private static object Invoke(object target, string methodName, params object[] args)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
        Assert.IsNotNull(method, $"Method not found: {target.GetType().Name}.{methodName}");
        return method.Invoke(target, args);
    }

    [UnityTest]
    public IEnumerator ResetAll_ClearsCocktailRuntimeState()
    {
        Type cocktailDataType = FindType("CocktailData");
        Type makeCocktailType = FindType("MakeCocktail");
        Type flavorType = FindType("FlavorType");

        object cocktailData = cocktailDataType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
        Assert.IsNotNull(cocktailData, "CocktailData.Instance is null");
        Invoke(cocktailData, "ClearHistory");

        GameObject makeObject = new GameObject("MakeCocktail_Test");
        Component makeCocktail = makeObject.AddComponent(makeCocktailType);

        yield return null;

        object fruity = Enum.Parse(flavorType, "Fruity");
        Invoke(makeCocktail, "GetAlcoholStatus", "도화주", 10, 2, 1, fruity, 3);
        Invoke(makeCocktail, "SetMixStateShake");

        IList selected = Invoke(makeCocktail, "GetSelectedIngredients") as IList;
        Assert.IsNotNull(selected);
        Assert.AreEqual(1, selected.Count);
        Assert.AreEqual("Shake", Invoke(makeCocktail, "GetCurrentMixState").ToString());

        Invoke(makeCocktail, "ResetAll");

        selected = Invoke(makeCocktail, "GetSelectedIngredients") as IList;
        Assert.IsNotNull(selected);
        Assert.AreEqual(0, selected.Count);
        Assert.AreEqual("Layer", Invoke(makeCocktail, "GetCurrentMixState").ToString());
        Assert.AreEqual(0, (int)Invoke(makeCocktail, "GetTotalAlcoholContent"));
        Assert.IsNull(Invoke(cocktailData, "GetCurrentCocktail"));

        UnityEngine.Object.Destroy(makeObject);
        yield return null;
    }
}

