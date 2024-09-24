using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 완성된 칵테일을 나타내는 클래스
public class Cocktail
{
    public float alcoholContent; // 최종 알코올 도수를 저장하는 변수
    public float sweetness; // 최종 단맛을 저장하는 변수
    public float bitterness; // 최종 쓴맛을 저장하는 변수
    public Ingredient.FlavorType dominantFlavorType; // 주된 향의 종류를 저장하는 변수
    public float flavorIntensity; // 최종 향의 세기를 저장하는 변수

    // 생성자: 재료들의 특성을 계산하여 최종 칵테일 특성을 결정
    public Cocktail(List<Ingredient> ingredients, Recipe.MixingMethod method)
    {
        CalculateProperties(ingredients, method); // 재료들의 특성을 계산하여 칵테일의 특성을 설정
        Debug.Log("칵테일이 완성되었습니다."); // 칵테일이 완성되었음을 로그로 출력
    }

    // 칵테일의 특성을 계산하는 private 메서드: 재료들의 특성을 합산하고 평균 내는 로직 구현 필요함.
    private void CalculateProperties(List<Ingredient> ingredients, Recipe.MixingMethod method)
    {
        if (ingredients.Count == 0) return; // 재료가 없으면 계산하지 않고 종료

        // 각 특성(알코올 도수, 단맛, 쓴맛)의 평균을 계산하여 설정
        alcoholContent = ingredients.Sum(i => i.alcoholContent) / ingredients.Count;
        sweetness = ingredients.Sum(i => i.sweetness) / ingredients.Count;
        bitterness = ingredients.Sum(i => i.bitterness) / ingredients.Count;

        // 가장 강한 향의 종류를 결정하기 위해 그룹화하고 합산하여 정렬 후 첫 번째 그룹 선택
        var dominantFlavorGroup = ingredients.GroupBy(i => i.flavorType)
                                             .OrderByDescending(g => g.Sum(i => i.flavorIntensity))
                                             .First();

        dominantFlavorType = dominantFlavorGroup.Key; // 주된 향의 종류 설정
        flavorIntensity = dominantFlavorGroup.Sum(i => i.flavorIntensity) / dominantFlavorGroup.Count(); // 주된 향의 세기 설정

        if (method == Recipe.MixingMethod.Shake)
            ApplyShakeEffect(); // 흔들기 효과 적용
        else if (method == Recipe.MixingMethod.Layer)
            ApplyLayerEffect(); // 쌓기 효과 적용

        Debug.Log($"최종 알코올 도수: {alcoholContent}, 단맛: {sweetness}, 쓴맛: {bitterness}, 주된 향: {dominantFlavorType}, 향의 세기: {flavorIntensity}");
    }

    private void ApplyShakeEffect()
    {
        sweetness *= 1.1f;  // 흔들면 단맛이 10% 증가하도록 설정 (예시 효과)
    }

    private void ApplyLayerEffect()
    {
        bitterness *= 1.1f;  // 쌓으면 쓴맛이 10% 증가하도록 설정 (예시 효과)
    }
}