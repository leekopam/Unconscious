// 칵테일의 재료를 나타내는 클래스
public class Ingredient
{
    public string name;                 // 재료의 이름
    public float alcoholContent;        // 알코올 도수 (0-10 범위)
    public float sweetness;             // 단맛 정도 (0-10 범위)
    public float bitterness;            // 쓴맛 정도 (0-10 범위)
    public enum FlavorType { Fruit, Grain, Alcohol } // 향의 종류를 나타내는 열거형
    public FlavorType flavorType;       // 현재 재료의 향 종류
    public float flavorIntensity;       // 향의 세기 (0-10 범위)

    // 생성자: 재료의 모든 속성을 초기화
    public Ingredient(string name, float alcoholContent, float sweetness, float bitterness, FlavorType flavorType, float flavorIntensity)
    {
        this.name = name;
        this.alcoholContent = alcoholContent;
        this.sweetness = sweetness;
        this.bitterness = bitterness;
        this.flavorType = flavorType;
        this.flavorIntensity = flavorIntensity;
    }
}