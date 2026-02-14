using NUnit.Framework;
using System;
using System.Reflection;

public class RecipeBookTests
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

    private static object EnumValue(string enumTypeName, string valueName)
    {
        Type enumType = FindType(enumTypeName);
        return Enum.Parse(enumType, valueName);
    }

    private static object InvokeCheckCocktailRecipe(string ingredient1, string ingredient2, string mixState)
    {
        Type recipeBookType = FindType("RecipeBook");
        MethodInfo method = recipeBookType.GetMethod("CheckCocktailRecipe", BindingFlags.Public | BindingFlags.Static);
        Assert.IsNotNull(method, "RecipeBook.CheckCocktailRecipe not found");

        return method.Invoke(null, new object[]
        {
            EnumValue("IngredientId", ingredient1),
            EnumValue("IngredientId", ingredient2),
            EnumValue("MixState", mixState)
        });
    }

    [TestCase("PeachWine", "NarinIhwaju", "Shake", "연홍남자")]
    [TestCase("Gahyangju", "Soseulbaram", "Stir", "그린나래")]
    [TestCase("NarinIhwaju", "Hojeopjimong", "Layer", "경성")]
    [TestCase("Soseulbaram", "Dongjitdal", "Shake", "한양의일출")]
    [TestCase("PeachWine", "Dongjitdal", "Stir", "명경지수")]
    [TestCase("Gahyangju", "Hojeopjimong", "Layer", "녹색요정")]
    public void CheckCocktailRecipe_KnownDefinition_ReturnsExpected(
        string first,
        string second,
        string mixState,
        string expected)
    {
        object direct = InvokeCheckCocktailRecipe(first, second, mixState);
        object reversed = InvokeCheckCocktailRecipe(second, first, mixState);

        Assert.AreEqual(expected, direct.ToString());
        Assert.AreEqual(expected, reversed.ToString());
    }

    [Test]
    public void CheckCocktailRecipe_WrongMixState_ReturnsFailure()
    {
        object result = InvokeCheckCocktailRecipe(
            "PeachWine",
            "NarinIhwaju",
            "Stir");

        Assert.AreEqual("실패음료", result.ToString());
    }

    [Test]
    public void CheckCocktailRecipe_UnknownIngredient_ReturnsFailure()
    {
        object result = InvokeCheckCocktailRecipe(
            "Unknown",
            "NarinIhwaju",
            "Layer");

        Assert.AreEqual("실패음료", result.ToString());
    }

    [Test]
    public void CheckCocktailRecipe_LegacyWrapper_TrimsWhitespace()
    {
        Type recipeBookType = FindType("RecipeBook");
        MethodInfo method = recipeBookType.GetMethod("Check_Cocktail_Recipe", BindingFlags.Public | BindingFlags.Static);
        Assert.IsNotNull(method, "RecipeBook.Check_Cocktail_Recipe not found");

        object fruity = EnumValue("FlavorType", "Fruity");
        object layer = EnumValue("MixState", "Layer");

        object result = method.Invoke(null, new object[]
        {
            " 호접지몽", 0, 0, 0, fruity, 0,
            "나린 이화주", 0, 0, 0, fruity, 0,
            layer
        });

        Assert.AreEqual("경성", result.ToString());
    }
}
