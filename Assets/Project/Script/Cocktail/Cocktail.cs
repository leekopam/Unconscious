using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cocktail
{
    public float alcoholContent; // 최종 알코올 도수
    public float sweetness; // 최종 단맛
    public float bitterness; // 최종 쓴맛
    public Ingredient.FlavorType dominantFlavorType; // 주된 향의 종류
    public float flavorIntensity; // 최종 향의 세기

    // 생성자
    public Cocktail(List<Ingredient> ingredients, Recipe.MixingMethod method)
    {
        // 재료들의 특성을 계산하여 최종 칵테일 특성 결정
        CalculateProperties(ingredients, method);
    }

    // 칵테일의 특성을 계산하는 private 메서드
    private void CalculateProperties(List<Ingredient> ingredients, Recipe.MixingMethod method)
    {
        // 재료들의 특성을 합산하고 평균 내는 로직
        // 혼합 방법에 따라 가중치를 다르게 적용할 수 있음
        // 이 부분은 게임의 밸런싱에 따라 세부적으로 구현해야 함
    }
}