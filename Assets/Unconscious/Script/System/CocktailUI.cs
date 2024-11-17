using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CocktailUIController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button[] ingredientButtons;    // 재료 선택 버튼
    [SerializeField] private Button[] removeButtons;        // 재료 제거 버튼
    [SerializeField] private Button shakeButton;           // 흔들기
    [SerializeField] private Button stirButton;            // 젓기
    [SerializeField] private Button blendButton;           // 섞기
    [SerializeField] private Button makeButton;            // 제조하기
    [SerializeField] private Button clearButton;           // 초기화

    [Header("Result UI")]
    [SerializeField] private TMP_Text[] selectedIngredientTexts;  // 선택된 재료들
    [SerializeField] private TMP_Text methodText;                // 선택된 제조법
    [SerializeField] private TMP_Text resultText;                // 결과 표시

    private CocktailMaker cocktailMaker;

    private void Start()
    {
        cocktailMaker = GetComponent<CocktailMaker>();
        InitializeUI();
    }

    private void InitializeUI()
    {
        // 재료 버튼 설정
        for (int i = 0; i < ingredientButtons.Length; i++)
        {
            int index = i;
            ingredientButtons[i].onClick.AddListener(() => OnIngredientButtonClicked(index));
        }

        // 제거 버튼 설정
        for (int i = 0; i < removeButtons.Length; i++)
        {
            int index = i;
            removeButtons[i].onClick.AddListener(() => OnRemoveButtonClicked(index));
        }

        // 제조 방법 버튼 설정
        shakeButton.onClick.AddListener(() => OnMethodButtonClicked(MixingMethod.Shake));
        stirButton.onClick.AddListener(() => OnMethodButtonClicked(MixingMethod.Stir));
        blendButton.onClick.AddListener(() => OnMethodButtonClicked(MixingMethod.Blend));

        // 기타 버튼 설정
        makeButton.onClick.AddListener(OnMakeButtonClicked);
        clearButton.onClick.AddListener(OnClearButtonClicked);

        UpdateUI();
    }

    // 재료 선택 버튼 클릭
    private void OnIngredientButtonClicked(int index)
    {
        if (cocktailMaker.AddIngredient(AlcoholDatabase.GetAlcohols()[index]))
        {
            UpdateUI();
        }
        else
        {
            Debug.Log("더 이상 재료를 추가할 수 없습니다.");
        }
    }

    // 재료 제거 버튼 클릭
    private void OnRemoveButtonClicked(int index)
    {
        if (cocktailMaker.RemoveIngredient(index))
        {
            UpdateUI();
        }
    }

    // 제조 방법 버튼 클릭
    private void OnMethodButtonClicked(MixingMethod method)
    {
        cocktailMaker.SetMixingMethod(method);
        methodText.text = $"선택된 제조법: {method}";
    }

    // 제조 버튼 클릭
    private void OnMakeButtonClicked()
    {
        CocktailResult result = cocktailMaker.MakeCocktail();
        if (result != null)
        {
            DisplayResult(result);
        }
        else
        {
            resultText.text = "재료와 제조 방법을 모두 선택해주세요.";
        }
    }

    // 초기화 버튼 클릭
    private void OnClearButtonClicked()
    {
        cocktailMaker.Clear();
        ClearUI();
    }

    // UI 업데이트
    private void UpdateUI()
    {
        var selectedIngredients = cocktailMaker.GetSelectedIngredients();

        // 선택된 재료 표시
        for (int i = 0; i < selectedIngredientTexts.Length; i++)
        {
            if (i < selectedIngredients.Count)
            {
                selectedIngredientTexts[i].text = $"재료 {i + 1}: {selectedIngredients[i].name}";
                removeButtons[i].gameObject.SetActive(true);
            }
            else
            {
                selectedIngredientTexts[i].text = $"재료 {i + 1}: 비어있음";
                removeButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // 결과 표시
    private void DisplayResult(CocktailResult result)
    {
        resultText.text = $"칵테일 결과\n" +
                         $"도수: {result.alcoholContent:F1}\n" +
                         $"쓴맛: {result.bitterness:F1}\n" +
                         $"단맛: {result.sweetness:F1}\n" +
                         $"향: {result.aroma}\n" +
                         $"향의 강도: {result.aromaIntensity:F1}";
    }

    // UI 초기화
    private void ClearUI()
    {
        foreach (var text in selectedIngredientTexts)
        {
            text.text = "재료: 비어있음";
        }
        foreach (var button in removeButtons)
        {
            button.gameObject.SetActive(false);
        }
        methodText.text = "제조 방법을 선택하세요";
        resultText.text = "";
    }
}