[System.Serializable]
public class Alcohol
{
    public string name;          // 술 이름
    public float alcoholContent; // 도수 (0-10)
    public float bitterness;     // 쓴맛 (0-10)
    public float sweetness;      // 단맛 (0-10)
    public Aroma aroma;          // 향 종류
    public float aromaIntensity; // 향의 강도 (0-10)
}

// Aroma.cs - 향 종류를 정의하는 열거형
public enum Aroma
{
    fruity,     // 과일 향
    nutty,      // 견과류 향
    alcoholic   // 알코올 향
}