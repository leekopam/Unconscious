using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public string name; // 재료 이름
    public float alcoholContent; // 알코올 도수 (0-10)
    public float sweetness; // 단맛 (0-10)
    public float bitterness; // 쓴맛 (0-10)
    public enum FlavorType { Fruit, Grain, Alcohol } // 향의 종류
    public FlavorType flavorType; // 현재 재료의 향 종류
    public float flavorIntensity; // 향의 세기 (0-10)

    // 생성자
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
