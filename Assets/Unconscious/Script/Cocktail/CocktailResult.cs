//===========칵테일 제조 결과 클래스===========
public class CocktailResult
{
    public float alcoholContent;  // 도수
    public float bitterness;     // 쓴맛
    public float sweetness;      // 단맛
    public Aroma aroma;          // 향
    public float aromaIntensity; // 향의 강도
    public MixingMethod mixingMethod;

    // 맛 판정 메서드 추가
    public string GetTaste()
    {
        if (alcoholContent >= 8) return "도수가 강한";
        if (bitterness >= 7) return "쓴맛이 강한";
        if (sweetness >= 7) return "달콤한";
        if (aromaIntensity >= 7) return "향이 강한";
        return "균형잡힌";
    }
}