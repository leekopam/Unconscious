using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

    // 툴팁 프리팹을 저장할 변수
    [SerializeField] private GameObject tooltipPrefab;

    // 생성된 툴팁들을 저장할 딕셔너리
    private Dictionary<Ingredient, GameObject> tooltips = new Dictionary<Ingredient, GameObject>();

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 툴팁 생성 및 표시 메서드
    public void ShowTooltip(Ingredient ingredient, Vector3 position)
    {
        GameObject tooltip;
        if (!tooltips.TryGetValue(ingredient, out tooltip))
        {
            // 해당 재료의 툴팁이 없으면 새로 생성
            tooltip = Instantiate(tooltipPrefab, transform);
            tooltips[ingredient] = tooltip;
            SetupTooltip(tooltip, ingredient);
        }

        // 툴팁 위치 설정 및 활성화
        tooltip.transform.position = position;
        tooltip.SetActive(true);
    }

    // 툴팁 숨기기 메서드
    public void HideTooltip(Ingredient ingredient)
    {
        if (tooltips.TryGetValue(ingredient, out GameObject tooltip))
        {
            tooltip.SetActive(false);
        }
    }

    // 툴팁 설정 메서드
    private void SetupTooltip(GameObject tooltipObject, Ingredient ingredient)
    {
        Text[] texts = tooltipObject.GetComponentsInChildren<Text>();
        texts[0].text = ingredient.name;
        texts[1].text = $"도수: {ingredient.alcoholContent}";
        texts[2].text = $"향의 종류: {ingredient.flavorType}";
        texts[3].text = $"단맛: {ingredient.sweetness}";
        texts[4].text = $"향의 세기: {ingredient.flavorIntensity}";
        texts[5].text = $"쓴맛: {ingredient.bitterness}";
        texts[6].text = "설명: " + GetDescription(ingredient);
    }

    // 재료 설명 반환 메서드
    private string GetDescription(Ingredient ingredient)
    {
        switch (ingredient.name)
        {
            case "Vodka":
                return "무색무취의 강한 증류주로, 다양한 칵테일의 기본이 됩니다.";
            case "Whiskey":
                return "곡물을 발효시켜 증류한 술로, 풍부한 맛과 향이 특징입니다.";
            case "Orange Juice":
                return "신선한 오렌지를 착즙한 주스로, 상큼한 맛을 더해줍니다.";
            // 다른 재료들에 대한 설명 추가...
            default:
                return "이 재료에 대한 설명이 아직 추가되지 않았습니다.";
        }
    }
}