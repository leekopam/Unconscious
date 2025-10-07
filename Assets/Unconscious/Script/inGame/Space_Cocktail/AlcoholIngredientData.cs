using UnityEngine;

[CreateAssetMenu(fileName = "AlcoholIngredientData", menuName = "Unconscious/Cocktail/Alcohol Ingredient")]
public class AlcoholIngredientData : ScriptableObject
{
    [Header("Identifier")]
    public IngredientId ingredientId = IngredientId.Unknown;
    public string ingredientName;

    [Header("Stats")]
    [Range(0f, 100f)] public float alcoholContent;
    [Range(0f, 100f)] public float sweetness;
    [Range(0f, 100f)] public float bitterness;

    [Header("Flavor")]
    public FlavorType flavorType;
    [Range(0f, 100f)] public float flavorIntensity;
}
