using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeIngredient
{
    public Ingredient ingredient; // 재료
    public int amount; // 재료의 양

    // 생성자
    public RecipeIngredient(Ingredient ingredient, int amount)
    {
        this.ingredient = ingredient;
        this.amount = amount;
    }
}

