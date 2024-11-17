using System.Collections.Generic;
using UnityEngine;

public class CocktailMaker : MonoBehaviour
{
    private List<Alcohol> ingredients = new List<Alcohol>();  // 선택된 재료들
    private MixingMethod currentMethod = MixingMethod.None;  // 현재 선택된 제조 방법
    private const int MAX_INGREDIENTS = 3;                   // 최대 재료 수

    // 재료 추가
    public bool AddIngredient(Alcohol alcohol)
    {
        // 최대 재료 수 체크
        if (ingredients.Count >= MAX_INGREDIENTS) return false;

        ingredients.Add(alcohol);
        return true;
    }

    // 재료 제거
    public bool RemoveIngredient(int index)
    {
        if (index >= 0 && index < ingredients.Count)
        {
            ingredients.RemoveAt(index);
            return true;
        }
        return false;
    }

    // 제조 방법 설정
    public void SetMixingMethod(MixingMethod method)
    {
        currentMethod = method;
    }

    // 칵테일 제조
    public CocktailResult MakeCocktail()
    {
        // 재료나 제조 방법이 선택되지 않았으면 null 반환
        if (ingredients.Count == 0 || currentMethod == MixingMethod.None)
            return null;

        // 이미지의 공식대로 계산
        float totalAlcohol = 0, totalBitter = 0, totalSweet = 0;
        float maxAroma = 0;
        Aroma strongestAroma = Aroma.alcoholic;

        // 모든 재료의 속성 합산
        foreach (var ingredient in ingredients)
        {
            totalAlcohol += ingredient.alcoholContent;
            totalBitter += ingredient.bitterness;
            totalSweet += ingredient.sweetness;

            // 가장 강한 향 찾기
            if (ingredient.aromaIntensity > maxAroma)
            {
                maxAroma = ingredient.aromaIntensity;
                strongestAroma = ingredient.aroma;
            }
        }

        // 재료 수에 따른 계산
        int count = ingredients.Count;
        float finalAlcohol = (totalAlcohol / count) - (count - 1);
        float finalBitter = (totalBitter / count) - (count - 1);
        float finalSweet = (totalSweet / count) - (count - 1);

        // 제조 방법에 따른 효과 적용
        switch (currentMethod)
        {
            case MixingMethod.Shake:
                finalAlcohol *= 0.9f;  // 도수 10% 감소
                break;
            case MixingMethod.Stir:
                finalBitter *= 0.8f;   // 쓴맛 20% 감소
                break;
            case MixingMethod.Blend:
                maxAroma *= 0.7f;      // 향 30% 감소
                break;
        }

        // 결과 반환
        return new CocktailResult
        {
            alcoholContent = Mathf.Max(0, finalAlcohol),
            bitterness = Mathf.Max(0, finalBitter),
            sweetness = Mathf.Max(0, finalSweet),
            aroma = strongestAroma,
            aromaIntensity = maxAroma,
            method = currentMethod
        };
    }

    // 모든 선택 초기화
    public void Clear()
    {
        ingredients.Clear();
        currentMethod = MixingMethod.None;
    }

    // 현재 선택된 재료들 반환
    public List<Alcohol> GetSelectedIngredients()
    {
        return new List<Alcohol>(ingredients);
    }
}