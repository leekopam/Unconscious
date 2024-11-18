using System.Collections.Generic;

public static class AlcoholDatabase
{
    // 모든 술 정보 반환
    public static List<Alcohol> GetAlcohols()
    {
        return new List<Alcohol>
        {
            new Alcohol
            {
                name = "1번 술",
                alcoholContent = 8,    // 높음
                bitterness = 5,        // 보통
                sweetness = 6,         // 조금 달콤
                aroma = Aroma.fruity,  // 과일향
                aromaIntensity = 4     // 연한
            },
            new Alcohol
            {
                name = "2번 술",
                alcoholContent = 6,    // 보통
                bitterness = 5,        // 보통
                sweetness = 10,        // 매우 달콤
                aroma = Aroma.fruity,  // 과일향
                aromaIntensity = 9     // 진한
            },
            new Alcohol
            {
                name = "3번 술",
                alcoholContent = 10,   // 매우 강함
                bitterness = 5,        // 보통
                sweetness = 2,         // 약간 달콤
                aroma = Aroma.nutty,   // 견과류향
                aromaIntensity = 5     // 보통
            }
        };
    }
}