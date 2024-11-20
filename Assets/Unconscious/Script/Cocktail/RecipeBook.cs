using UnityEngine;

public enum Recipe
{
    연홍남자,
    그린나래,
    경성,
    한양의_일출,
    명경지수,
    풀_빛_아리아,
   정체불명의음료
}

public class RecipeBook : MonoBehaviour
{
    public Recipe? Check_Cocktail_Recipe(
        string name1, int alcoholContent1, int sweetness1, int bitterness1, FlavorType flavorType1, int flavorIntensity1,
        string name2, int alcoholContent2, int sweetness2, int bitterness2, FlavorType flavorType2, int flavorIntensity2)
    {
        // 연홍남자 = 도화주 + 나린 이화주
        if ((name1 == "도화주" && name2 == "나린 이화주") || (name1 == "나린 이화주" && name2 == "도화주"))
        {
            return Recipe.연홍남자;
        }
        // 그린나래 = 가향주 + 소슬바람
        if ((name1 == "가향주" && name2 == "소슬바람") || (name1 == "소슬바람" && name2 == "가향주"))
        {
            return Recipe.그린나래;
        }
        //경성 = 나린 이화주 + 해류 뭄해리
        if ((name1 == "나린 이화주" && name2 == " 해류 뭄해리") || (name1 == " 해류 뭄해리" && name2 == "나린 이화주"))
        {
            return Recipe.경성;
        }
        //한양의_일출 = 소슬바람 + 영반월
        if ((name1 == "소슬바람" && name2 == "영반월") || (name1 == "영반월" && name2 == "소슬바람"))
        {
            return Recipe.한양의_일출;
        }
        //명경지수 = 도화주 + 영반월
        if ((name1 == "도화주" && name2 == "영반월") || (name1 == "영반월" && name2 == "도화주"))
        {
            return Recipe.명경지수;
        }
        //풀_빛_아리아 = 가향주 + 해류 뭄해리
        if ((name1 == "가향주" && name2 == "해류 뭄해리") || (name1 == "해류 뭄해리" && name2 == "가향주"))
        {
            return Recipe.풀_빛_아리아;
        }
        else
        {
            return Recipe.정체불명의음료; // 해당하는 레시피가 없을 경우 null 반환
        }
    }
}
