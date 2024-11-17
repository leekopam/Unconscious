using System.Collections.Generic;

public static class AlcoholDatabase
{
    public static List<Alcohol> GetAlcohols()
    {
        return new List<Alcohol>
        {
            new Alcohol
            {
                name = "1번 술",
                alcoholContent = 8,
                 sweetness = 6,
                bitterness = 2,
                aroma = Aroma.fruity,
                aromaIntensity = 7
            },
            new Alcohol
            {
                name = "2번 술",
                alcoholContent = 6,
                 sweetness = 4,
                bitterness = 7,
                aroma = Aroma.fruity,
                aromaIntensity = 3
            },
            new Alcohol
            {
                name = "3번 술",
               alcoholContent = 4,
                 sweetness = 2,
                bitterness = 8,
                aroma = Aroma.nutty,
                aromaIntensity = 5
            },
            new Alcohol
            {
                name = "4번 술",
               alcoholContent = 2,
                 sweetness = 8,
                bitterness = 2,
                aroma = Aroma.nutty,
                aromaIntensity = 8
            },
            new Alcohol
            {
                name = "5번 술",
               alcoholContent = 7,
                 sweetness = 0,
                bitterness = 9,
                aroma = Aroma.alcoholic,
                aromaIntensity = 9
            },
            new Alcohol
            {
                name = "6번 술",
                alcoholContent = 9,
                 sweetness = 3,
                bitterness = 6,
                aroma = Aroma.alcoholic,
                aromaIntensity = 2
            },
        };
    }
}