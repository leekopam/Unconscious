using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Recipe
{
    연홍남자,
    그린나래,
    경성,
    한양의일출,
    명경지수,
    녹색요정,
    실패음료,
    주문없음
}

public enum MixState
{
    Shake, // 흔들기
    Stir,  // 젓기
    Layer  // 쌓기
}

public static class RecipeBook
{
    private sealed class RecipeDefinition
    {
        public IngredientId ingredientA;
        public IngredientId ingredientB;
        public MixState mixState;
        public Recipe recipe;

        public bool IsMatch(IngredientId first, IngredientId second, MixState inputMixState)
        {
            if (mixState != inputMixState)
            {
                return false;
            }

            return (ingredientA == first && ingredientB == second)
                   || (ingredientA == second && ingredientB == first);
        }
    }

    private static readonly List<RecipeDefinition> recipeDefinitions = new List<RecipeDefinition>
    {
        new RecipeDefinition
        {
            ingredientA = IngredientId.PeachWine,
            ingredientB = IngredientId.NarinIhwaju,
            mixState = MixState.Shake,
            recipe = Recipe.연홍남자
        },
        new RecipeDefinition
        {
            ingredientA = IngredientId.Gahyangju,
            ingredientB = IngredientId.Soseulbaram,
            mixState = MixState.Stir,
            recipe = Recipe.그린나래
        },
        new RecipeDefinition
        {
            ingredientA = IngredientId.NarinIhwaju,
            ingredientB = IngredientId.Hojeopjimong,
            mixState = MixState.Layer,
            recipe = Recipe.경성
        },
        new RecipeDefinition
        {
            ingredientA = IngredientId.Soseulbaram,
            ingredientB = IngredientId.Dongjitdal,
            mixState = MixState.Shake,
            recipe = Recipe.한양의일출
        },
        new RecipeDefinition
        {
            ingredientA = IngredientId.PeachWine,
            ingredientB = IngredientId.Dongjitdal,
            mixState = MixState.Stir,
            recipe = Recipe.명경지수
        },
        new RecipeDefinition
        {
            ingredientA = IngredientId.Gahyangju,
            ingredientB = IngredientId.Hojeopjimong,
            mixState = MixState.Layer,
            recipe = Recipe.녹색요정
        }
    };

    // 주문 가능한 랜덤 레시피를 반환하는 메서드
    public static Recipe GetRandomOrderableRecipe()
    {
        Recipe[] allRecipes = (Recipe[])System.Enum.GetValues(typeof(Recipe));
        List<Recipe> orderableRecipes = allRecipes.Where(IsOrderableRecipe).ToList();

        if (orderableRecipes.Count <= 0)
        {
            return Recipe.주문없음;
        }

        int randomIndex = Random.Range(0, orderableRecipes.Count);
        return orderableRecipes[randomIndex];
    }

    public static bool IsOrderableRecipe(Recipe recipe)
    {
        return recipe != Recipe.실패음료 && recipe != Recipe.주문없음;
    }

    public static Recipe CheckCocktailRecipe(IngredientId ingredient1, IngredientId ingredient2, MixState mixState)
    {
        if (ingredient1 == IngredientId.Unknown || ingredient2 == IngredientId.Unknown)
        {
            return Recipe.실패음료;
        }

        foreach (RecipeDefinition definition in recipeDefinitions)
        {
            if (definition.IsMatch(ingredient1, ingredient2, mixState))
            {
                return definition.recipe;
            }
        }

        return Recipe.실패음료;
    }

    // 기존 호출부와의 호환을 위한 래퍼 메서드
    public static Recipe Check_Cocktail_Recipe(
        string name1, int alcoholContent1, int sweetness1, int bitterness1, FlavorType flavorType1, int flavorIntensity1,
        string name2, int alcoholContent2, int sweetness2, int bitterness2, FlavorType flavorType2, int flavorIntensity2,
        MixState mixstate)
    {
        IngredientId ingredientId1 = IngredientNameMapper.ToIngredientId(name1);
        IngredientId ingredientId2 = IngredientNameMapper.ToIngredientId(name2);

        return CheckCocktailRecipe(ingredientId1, ingredientId2, mixstate);
    }
}
