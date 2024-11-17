// CocktailMaker.cs - 칵테일 제조 로직 처리
using System.Collections.Generic;
using UnityEngine;

// CocktailMaker.cs - 칵테일 제조 로직
public class CocktailMaker : MonoBehaviour
{
    private List<Alcohol> selectedIngredients = new List<Alcohol>();
    private MixingMethod currentMethod = MixingMethod.None;
    private const int MAX_INGREDIENTS = 3;

    // 재료 추가
    public bool AddIngredient(Alcohol alcohol)
    {
        if (selectedIngredients.Count >= MAX_INGREDIENTS) return false;
        selectedIngredients.Add(alcohol);
        return true;
    }

    // 재료 제거
    public bool RemoveIngredient(int index)
    {
        if (index >= 0 && index < selectedIngredients.Count)
        {
            selectedIngredients.RemoveAt(index);
            return true;
        }
        return false;
    }

    // 현재 선택된 재료들 반환
    public List<Alcohol> GetSelectedIngredients()
    {
        return new List<Alcohol>(selectedIngredients);
    }

    // 제조 방법 설정
    public void SetMixingMethod(MixingMethod method)
    {
        currentMethod = method;
    }

    // 초기화
    public void Clear()
    {
        selectedIngredients.Clear();
        currentMethod = MixingMethod.None;
    }
    // 칵테일 제조
    public CocktailResult MakeCocktail()
    {
        if (selectedIngredients.Count == 0 || currentMethod == MixingMethod.None)
            return null;

        // 재료 속성 계산
        float totalAlcohol = 0, totalBitter = 0, totalSweet = 0;
        float maxAroma = 0;
        Aroma strongestAroma = Aroma.alcoholic;

        foreach (var ingredient in selectedIngredients)
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

        // 이미지의 공식 적용
        int count = selectedIngredients.Count;
        float finalAlcohol = (totalAlcohol / count) - (count - 1);
        float finalBitter = (totalBitter / count) - (count - 1);
        float finalSweet = (totalSweet / count) - (count - 1);

        // 제조 방법에 따른 보정
        switch (currentMethod)
        {
            case MixingMethod.Shake:
                finalAlcohol *= 0.9f;    // 흔들면 도수 감소
                break;
            case MixingMethod.Stir:
                finalBitter *= 0.8f;     // 젓기는 쓴맛 감소
                break;
            case MixingMethod.Blend:
                maxAroma *= 0.7f;        // 섞기는 향 감소
                break;
        }

        return new CocktailResult
        {
            alcoholContent = Mathf.Max(0, finalAlcohol),
            bitterness = Mathf.Max(0, finalBitter),
            sweetness = Mathf.Max(0, finalSweet),
            aroma = strongestAroma,
            aromaIntensity = maxAroma,
            mixingMethod = currentMethod
        };
    }
}
