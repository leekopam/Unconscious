public class CocktailResult
{
    public float alcoholContent;  // 최종 도수
    public float bitterness;     // 최종 쓴맛
    public float sweetness;      // 최종 단맛
    public Aroma aroma;          // 최종 향
    public float aromaIntensity; // 최종 향의 강도
    public MixingMethod method;  // 사용된 제조 방법

    // 칵테일의 특징을 문자열로 반환
    public string GetCharacteristic()
    {
        if (alcoholContent >= 8) return "도수가 강한";
        if (bitterness >= 7) return "쓴맛이 강한";
        if (sweetness >= 7) return "달콤한";
        if (aromaIntensity >= 7) return "향이 강한";
        return "균형 잡힌";
    }
}