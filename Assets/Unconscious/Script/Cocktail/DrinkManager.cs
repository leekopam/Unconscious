using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Unity UI와 연동하여 사용자 입력을 처리하고 결과를 표시하는 클래스
public class DrinkManager : MonoBehaviour
{
    public Button vodkaButton;          // Vodka 추가 버튼 
    public Button whiskeyButton;        // Whiskey 추가 버튼 
    public Button mixButton;            // 혼합 버튼 
    public Button stackButton;          // 쌓기 버튼 
    public Button shakeButton;          // 흔들기 버튼 
    public Text resultText;             // 결과 텍스트 표시 

    private Drink currentDrink;         // 현재 칵테일 객체

    void Start()
    {
        currentDrink = new Drink();     // Drink 객체 초기화

        vodkaButton.onClick.AddListener(() => AddIngredient("Vodka"));   // Vodka 버튼 클릭 시 AddIngredient 호출
        whiskeyButton.onClick.AddListener(() => AddIngredient("Whiskey"));   // Whiskey 버튼 클릭 시 AddIngredient 호출

        mixButton.onClick.AddListener(() => CalculateResult("mix"));     // 믹스 버튼 클릭 시 CalculateResult 호출
        stackButton.onClick.AddListener(() => CalculateResult("stack")); // 쌓기 버튼 클릭 시 CalculateResult 호출
        shakeButton.onClick.AddListener(() => CalculateResult("shake")); // 흔들기 버튼 클릭 시 CalculateResult 호출
    }

    void AddIngredient(string ingredientName)
    {
        Dictionary<string, float> ingredient = new Dictionary<string, float>();  // 새로운 재료 딕셔너리 생성

        if (ingredientName == "Vodka")
        {
            ingredient = new Dictionary<string, float>
            {
                {"name", 0},  {"alcoholContent", 8}, {"sweetness", 0}, {"bitterness", 1} };  // Vodka 재료 정보 설정
        }
        else if (ingredientName == "Whiskey")
        {
            ingredient = new Dictionary<string, float> { { "name", 1 }, { "alcoholContent", 7 }, { "sweetness", 2 }, { "bitterness", 3 } };  // Whiskey 재료 정보 설정
        }
        currentDrink.AddIngredient(ingredient);  // Drink 객체에 재료 추가
    }

    void CalculateResult(string method)
    {
        var result = currentDrink.CalculateFinalResult(method);  // 최종 결과 계산
        resultText.text = $"결과: 알코올 {result.Item1:F1}, 단맛 {result.Item2:F1}, 쓴맛 {result.Item3:F1}";  // 결과 텍스트 업데이트
    }
}