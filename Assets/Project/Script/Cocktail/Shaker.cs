using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker
{
    private List<Ingredient> currentIngredients = new List<Ingredient>(); // 현재 쉐이커에 들어있는 재료들

    // 재료를 쉐이커에 추가하는 메서드
    public void AddIngredient(Ingredient ingredient)
    {
        currentIngredients.Add(ingredient);
    }

    // 쉐이커를 비우는 메서드
    public void Clear()
    {
        currentIngredients.Clear();
    }

    // 칵테일을 만드는 메서드
    public Cocktail Mix(Recipe.MixingMethod method)
    {
        // 재료들을 혼합하여 칵테일 생성 로직
        // 선택된 혼합 방법에 따라 결과가 달라질 수 있음
        return new Cocktail(currentIngredients, method);
    }
}