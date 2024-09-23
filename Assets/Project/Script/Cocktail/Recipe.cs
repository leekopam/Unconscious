using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe
{
    public string name; // 레시피 이름
    public List<RecipeIngredient> ingredients; // 재료 목록
    public enum MixingMethod { Shake, Stir, Layer } // 믹싱 방법
    public MixingMethod mixingMethod; // 현재 레시피의 믹싱 방법

    // 생성자
    public Recipe(string name, List<RecipeIngredient> ingredients, MixingMethod mixingMethod)
    {
        this.name = name;
        this.ingredients = ingredients;
        this.mixingMethod = mixingMethod;
    }
}
