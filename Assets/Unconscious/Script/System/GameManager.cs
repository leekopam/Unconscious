using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Ingredient> availableIngredients;
    public List<Recipe> availableRecipes;
    public Shaker shaker;

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonParent;

    public void InitializeGame()
    {
        availableIngredients = new List<Ingredient>();
        availableRecipes = new List<Recipe>();
        shaker = new Shaker();

        // 예시 재료 추가
        availableIngredients.Add(new Ingredient("Vodka", 8, 0, 1, Ingredient.FlavorType.Alcohol, 7));
        availableIngredients.Add(new Ingredient("Whiskey", 7, 2, 3, Ingredient.FlavorType.Grain, 6));
        availableIngredients.Add(new Ingredient("Orange Juice", 0, 7, 1, Ingredient.FlavorType.Fruit, 5));
        // 더 많은 재료 추가...

        // 예시 레시피 추가
        List<RecipeIngredient> screwdriverIngredients = new List<RecipeIngredient>
        {
            new RecipeIngredient(availableIngredients[0], 2), // Vodka
            new RecipeIngredient(availableIngredients[2], 4)  // Orange Juice
        };
        availableRecipes.Add(new Recipe("Screwdriver", screwdriverIngredients, Recipe.MixingMethod.Stir));
        // 더 많은 레시피 추가...

        CreateIngredientButtons();
    }

    private void CreateIngredientButtons()
    {
        foreach (var ingredient in availableIngredients)
        {
            // 버튼 오브젝트 생성
            GameObject buttonObj = Instantiate(buttonPrefab, buttonParent);
            Button button = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();

            // 버튼 텍스트 설정
            buttonText.text = ingredient.name;

            // IngredientButton 컴포넌트 추가 및 설정
            IngredientButton ingredientButton = buttonObj.AddComponent<IngredientButton>();
            ingredientButton.ingredient = ingredient;

            // 버튼 클릭 이벤트 설정
            button.onClick.AddListener(() => SelectIngredient(ingredient));
        }
}

    public void SelectIngredient(Ingredient ingredient)
    {
        shaker.AddIngredient(ingredient);
    }

    public void MixCocktail(Recipe.MixingMethod method)
    {
        Cocktail result = shaker.Mix(method);
        EvaluateResult(result);
    }

    private void EvaluateResult(Cocktail cocktail)
    {
        // 여기서 칵테일의 특성을 평가하고 점수를 계산하거나 피드백을 제공
        Debug.Log($"칵테일 평가 결과: 알코올 도수 {cocktail.alcoholContent}, 단맛 {cocktail.sweetness}, 쓴맛 {cocktail.bitterness}, 주된 향 {cocktail.dominantFlavorType}, 향의 세기 {cocktail.flavorIntensity}");
    }


}