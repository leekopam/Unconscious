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
    Shake, //흔들기
    Stir, //젓기
    Layer //쌓기
}

public class RecipeBook : MonoBehaviour
{
    // 주문 가능한 랜덤 레시피를 반환하는 메서드
    public static Recipe GetRandomOrderableRecipe()
    {
        // Recipe enum의 모든 값을 가져옴
        Recipe[] allRecipes = (Recipe[])System.Enum.GetValues(typeof(Recipe));

        // 실패음료와 주문없음을 제외한 레시피만 필터링
        List<Recipe> orderableRecipes = allRecipes
            .Where(r => r != Recipe.실패음료 && r != Recipe.주문없음)
            .ToList();

        // 레시피 중 랜덤 선택
        if (orderableRecipes.Count > 0)
        {
            int randomIndex = Random.Range(0, orderableRecipes.Count);
            return orderableRecipes[randomIndex];
        }

        return Recipe.주문없음;
    }

    // 두 재료의 특성을 받아서 해당하는 칵테일 레시피를 반환하는 메서드
    public Recipe? Check_Cocktail_Recipe(
        string name1, int alcoholContent1, int sweetness1, int bitterness1, FlavorType flavorType1, int flavorIntensity1,
        string name2, int alcoholContent2, int sweetness2, int bitterness2, FlavorType flavorType2, int flavorIntensity2
        ,MixState mixstate)
    {
        // 연홍남자 = 도화주 + 나린 이화주
        if ((mixstate==MixState.Shake&&
            name1 == "도화주" && name2 == "나린 이화주") || (name1 == "나린 이화주" && name2 == "도화주"))
        {
            return Recipe.연홍남자;
        }
        // 그린나래 = 가향주 + 소슬바람
        if (mixstate == MixState.Stir && 
            (name1 == "가향주" && name2 == "소슬바람") || (name1 == "소슬바람" && name2 == "가향주"))
        {
            return Recipe.그린나래;
        }
        //경성 = 나린 이화주 + 호접지몽
        if ((mixstate == MixState.Layer && 
            name1 == "나린 이화주" && name2 == " 호접지몽") || (name1 == " 호접지몽" && name2 == "나린 이화주"))
        {
            return Recipe.경성;
        }
        //한양의_일출 = 소슬바람 + 동짓달
        if ((mixstate == MixState.Shake && 
            name1 == "소슬바람" && name2 == "동짓달") || (name1 == "동짓달" && name2 == "소슬바람"))
        {
            return Recipe.한양의일출;
        }
        //명경지수 = 도화주 + 동짓달
        if ((mixstate == MixState.Stir && 
            name1 == "도화주" && name2 == "동짓달") || (name1 == "동짓달" && name2 == "도화주"))
        {
            return Recipe.명경지수;
        }
        //녹색_요정 = 가향주 + 호접지몽
        if ((mixstate == MixState.Layer && 
            name1 == "가향주" && name2 == "호접지몽") || (name1 == "호접지몽" && name2 == "가향주"))
        {
            return Recipe.녹색요정;
        }
        else
        {
            return Recipe.실패음료; // 해당하는 레시피가 없을 경우 null 반환
        }
    }
}
