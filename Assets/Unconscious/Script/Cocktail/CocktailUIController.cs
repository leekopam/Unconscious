using TMPro;
using UnityEngine;
using UnityEngine.UI;

// CocktailUIController.cs - UI 관련 기능을 담당하는 클래스
public class CocktailUIController : MonoBehaviour
{
    [Header("재료 관련 UI")]
    [SerializeField] private Button[] ingredientButtons;    // 재료 선택 버튼들
    [SerializeField] private Button[] removeButtons;        // 재료 제거 버튼들
    [SerializeField] private TMP_Text[] ingredientTexts;    // 선택된 재료 표시 텍스트

    [Header("제조 방법 관련 UI")]
    [SerializeField] private Button shakeButton;           // 흔들기 버튼
    [SerializeField] private Button stirButton;            // 젓기 버튼
    [SerializeField] private Button blendButton;           // 섞기 버튼
    [SerializeField] private TMP_Text methodText;          // 선택된 제조 방법 텍스트

    [Header("기타 UI")]
    [SerializeField] private Button makeButton;            // 제조 버튼
    [SerializeField] private Button clearButton;           // 초기화 버튼
    [SerializeField] private TMP_Text resultText;          // 결과 표시 텍스트

    private CocktailMaker cocktailMaker;                   // 칵테일 제조 컴포넌트

    private void Start()
    {
        // 컴포넌트 가져오기
        cocktailMaker = GetComponent<CocktailMaker>();

        // 버튼 기능 설정
        SetupButtons();

        // 초기 UI 설정
        UpdateUI();
    }

    // 모든 버튼의 기능 설정
    private void SetupButtons()
    {
        // 재료 버튼 설정
        for (int i = 0; i < ingredientButtons.Length; i++)
        {
            int index = i;
            ingredientButtons[i].onClick.AddListener(() => OnIngredientSelected(index));
        }

        // 제거 버튼 설정
        for (int i = 0; i < removeButtons.Length; i++)
        {
            int index = i;
            removeButtons[i].onClick.AddListener(() => OnRemoveIngredient(index));
        }

        // 제조 방법 버튼 설정
        shakeButton.onClick.AddListener(() => OnMethodSelected(MixingMethod.Shake));
        stirButton.onClick.AddListener(() => OnMethodSelected(MixingMethod.Stir));
        blendButton.onClick.AddListener(() => OnMethodSelected(MixingMethod.Blend));

        // 기타 버튼 설정
        makeButton.onClick.AddListener(OnMakeButtonClicked);
        clearButton.onClick.AddListener(OnClearButtonClicked);
    }

    // 재료 선택시 호출
    private void OnIngredientSelected(int index)
    {
        var alcohols = AlcoholDatabase.GetAlcohols();
        if (index < alcohols.Count)
        {
            cocktailMaker.AddIngredient(alcohols[index]);
            UpdateUI();
        }
    }

    // 재료 제거시 호출
    private void OnRemoveIngredient(int index)
    {
        if (cocktailMaker.RemoveIngredient(index))
        {
            UpdateUI();
        }
    }

    // 제조 방법 선택시 호출
    private void OnMethodSelected(MixingMethod method)
    {
        cocktailMaker.SetMixingMethod(method);
        methodText.text = $"선택된 제조 방법: {method}";
    }

    // 제조 버튼 클릭시 호출
    private void OnMakeButtonClicked()
    {
        CocktailResult result = cocktailMaker.MakeCocktail();
        if (result != null)
        {
            resultText.text = $"칵테일 완성!\n" +
                            $"도수: {result.alcoholContent:F1}\n" +
                            $"쓴맛: {result.bitterness:F1}\n" +
                            $"단맛: {result.sweetness:F1}\n" +
                            $"향: {result.aroma} (강도: {result.aromaIntensity:F1})";
        }
    }

    // 초기화 버튼 클릭시 호출
    private void OnClearButtonClicked()
    {
        cocktailMaker.Clear();
        UpdateUI();
        resultText.text = "재료를 선택하세요";
    }

    // UI 업데이트
    private void UpdateUI()
    {
        var selectedIngredients = cocktailMaker.GetSelectedIngredients();

        // 선택된 재료 표시
        for (int i = 0; i < ingredientTexts.Length; i++)
        {
            if (i < selectedIngredients.Count)
            {
                ingredientTexts[i].text = selectedIngredients[i].name;
                removeButtons[i].gameObject.SetActive(true);
            }
            else
            {
                ingredientTexts[i].text = "비어있음";
                removeButtons[i].gameObject.SetActive(false);
            }
        }
    }
}