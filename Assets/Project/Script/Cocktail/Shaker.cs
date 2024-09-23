using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 칵테일을 만드는 과정을 시뮬레이션하는 클래스
public class Shaker
{
    private List<Ingredient> currentIngredients = new List<Ingredient>(); // 현재 쉐이커에 들어있는 재료들

    // 재료를 쉐이커에 추가하는 메서드
    public void AddIngredient(Ingredient ingredient)
    {
        currentIngredients.Add(ingredient);
        Debug.Log($"{ingredient.name}이(가) 쉐이커에 추가되었습니다.");
    }

    // 쉐이커를 비우는 메서드
    public void Clear()
    {
        currentIngredients.Clear();
        Debug.Log("쉐이커가 비워졌습니다.");
    }

    // 칵테일을 만드는 메서드
    public Cocktail Mix(Recipe.MixingMethod method)
    {
        // 재료들을 혼합하여 칵테일 생성
        return new Cocktail(currentIngredients, method);
    }
}