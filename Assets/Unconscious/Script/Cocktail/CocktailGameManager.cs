using UnityEngine;

public class CocktailGameManager : MonoBehaviour
{
    private CocktailMaker cocktailMaker;
    private CocktailUIController uiController;

    private void Start()
    {
        cocktailMaker = GetComponent<CocktailMaker>();
        uiController = GetComponent<CocktailUIController>();
        InitializeAndTestCocktail();
    }

    // 테스트용 칵테일 제조
    private void InitializeAndTestCocktail()
    {
        // 1번 칵테일 재료
        Alcohol cocktail1 = new Alcohol
        {
            name = "1번 칵테일",
            alcoholContent = 8,    // 높음
            bitterness = 5,       // 약간
            sweetness = 6,        // 조금 많이
            aroma = Aroma.fruity, // 과일향
            aromaIntensity = 4    // 연한
        };

        // 재료 추가 및 제조
        cocktailMaker.AddIngredient(cocktail1);
        cocktailMaker.SetMixingMethod(MixingMethod.Shake);

        // 결과 확인
        CocktailResult result = cocktailMaker.MakeCocktail();
        if (result != null)
        {
            Debug.Log($"칵테일 완성!\n" +
                     $"특징: {result.GetTaste()}\n" +
                     $"도수: {result.alcoholContent:F1}\n" +
                     $"쓴맛: {result.bitterness:F1}\n" +
                     $"단맛: {result.sweetness:F1}\n" +
                     $"향: {result.aroma} (강도: {result.aromaIntensity:F1})");
        }
    }
}