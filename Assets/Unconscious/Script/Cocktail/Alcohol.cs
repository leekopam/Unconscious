using UnityEngine;

[System.Serializable]
public class Alcohol : MonoBehaviour
{
    public string name;
    public float alcoholContent;  // 도수
    public float sweetness;      // 단맛
    public float bitterness;     // 쓴맛
    public Aroma aroma;         // 향
    public float aromaIntensity; // 향의 세기
}

public enum Aroma //향종류
{
    fruity, //과일향
    nutty, //견과류향
    alcoholic, //알코올향
}