using UnityEngine;

public enum Recipe
{
    연홍남자,
    그린나래,
    경성,
    한양의일출,
    명경지수,
    녹색요정,
   정체불명의음료
}

public enum MixState
{
    Shake, //흔들기
    Stir, //젓기
    Layer //쌓기
}

public class RecipeBook : MonoBehaviour
{
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
            return Recipe.정체불명의음료; // 해당하는 레시피가 없을 경우 null 반환
        }
    }
}
