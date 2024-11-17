using System.Collections.Generic;

public class Cocktail
{
    private List<Alcohol> ingredients = new List<Alcohol>();
    private MixingMethod mixingMethod;

    public void AddIngredient(Alcohol ingredient)
    {
        if (ingredients.Count < 3)
        {
            ingredients.Add(ingredient);
        }
    }

    public void SetMixingMethod(MixingMethod method)
    {
        mixingMethod = method;
    }

    public void Clear()
    {
        ingredients.Clear();
        mixingMethod = MixingMethod.None;
    }
}