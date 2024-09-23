using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    public List<Ingredient> availableIngredients; // 사용 가능한 재료 목록
    public List<Recipe> availableRecipes; // 사용 가능한 레시피 목록
    public Shaker shaker; // 현재 사용 중인 쉐이커

    // 재료를 선택하는 메서드
    public void SelectIngredient(Ingredient ingredient)
    {
        shaker.AddIngredient(ingredient);
    }

    // 칵테일을 만드는 메서드
    public void MixCocktail(Recipe.MixingMethod method)
    {
        Cocktail result = shaker.Mix(method);
        EvaluateResult(result);
    }

    // 만들어진 칵테일을 평가하는 private 메서드
    private void EvaluateResult(Cocktail cocktail)
    {
        // 만들어진 칵테일을 레시피와 비교하여 평가
        // 점수 계산이나 게임 내 피드백을 제공하는 로직을 구현
    }

    // 게임 초기화 메서드
    public void InitializeGame()
    {
        // 게임에 필요한 재료와 레시피를 초기화
        availableIngredients = new List<Ingredient>();
        availableRecipes = new List<Recipe>();
        shaker = new Shaker();

        //============================== 술 종류=============================
        availableIngredients.Add(new Ingredient("Vodka", 8, 0, 1, Ingredient.FlavorType.Alcohol, 7));
        availableIngredients.Add(new Ingredient("Orange Juice", 0, 7, 1, Ingredient.FlavorType.Fruit, 6));

        // 예시 레시피 추가
        List<RecipeIngredient> screwdriverIngredients = new List<RecipeIngredient>
        {
            new RecipeIngredient(availableIngredients[0], 2), // Vodka
            new RecipeIngredient(availableIngredients[1], 4)  // Orange Juice
        };
        availableRecipes.Add(new Recipe("Screwdriver", screwdriverIngredients, Recipe.MixingMethod.Stir));
        // 더 많은 레시피 추가...
    }
}
