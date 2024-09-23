using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 칵테일 레시피를 나타내는 클래스
public class Recipe
{
    public string name;                     // 레시피 이름
    public List<RecipeIngredient> ingredients; // 레시피에 필요한 재료 목록
    public enum MixingMethod { Shake, Stir, Layer } // 믹싱 방법을 나타내는 열거형
    public MixingMethod mixingMethod;       // 현재 레시피의 믹싱 방법

    // 생성자: 레시피 이름, 재료 목록, 믹싱 방법을 초기화
    public Recipe(string name, List<RecipeIngredient> ingredients, MixingMethod mixingMethod)
    {
        this.name = name;
        this.ingredients = ingredients;
        this.mixingMethod = mixingMethod;
    }
}
