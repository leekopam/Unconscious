using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MakeCocktail : MonoBehaviour
{
    private const int MAX_ALCOHOL_COUNT = 2;
    private const int MAX_LAYERS = 4;

    [SerializeField] private GameObject resetButton;

    private int currentLayer = 0;
    private readonly List<GameObject> activeObjects = new List<GameObject>();
    private readonly List<SelectedIngredient> selectedIngredients = new List<SelectedIngredient>();

    private MixState currentMixState = MixState.Layer;

    private int totalAlcoholContent = 0;
    private int totalSweetness = 0;
    private int totalBitterness = 0;
    private int totalFlavorIntensity = 0;

    private bool isTechnic = false;

    private void Awake()
    {
        if (resetButton != null)
        {
            resetButton.SetActive(true);

            Button button = resetButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveListener(ResetAll);
                button.onClick.AddListener(ResetAll);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ResetAll();
        }
    }

    public int GetCurrentLayer()
    {
        return currentLayer;
    }

    public void ActivateLayerObject(GameObject obj)
    {
        if (obj == null || currentLayer >= MAX_LAYERS)
        {
            return;
        }

        DeactivateLayerObjects(currentLayer);
        obj.SetActive(true);
        activeObjects.Add(obj);
        currentLayer++;
    }

    private void DeactivateLayerObjects(int layer)
    {
        activeObjects.RemoveAll(obj =>
        {
            if (obj != null && obj.GetComponent<LayerIdentifier>()?.LayerNumber == layer)
            {
                obj.SetActive(false);
                return true;
            }

            return false;
        });
    }

    public void ResetAll()
    {
        foreach (GameObject obj in activeObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        activeObjects.Clear();
        selectedIngredients.Clear();
        currentLayer = 0;

        currentMixState = MixState.Layer;
        isTechnic = false;
        ResetTotals();

        CocktailData.Instance.ClearCurrentCocktailProgress();
        Debug.Log("[Cocktail] 모든 상태가 초기화되었습니다.");
    }

    public bool TryAddIngredient(AlcoholStatus alcoholStatus)
    {
        if (alcoholStatus == null)
        {
            return false;
        }

        if (selectedIngredients.Count >= MAX_ALCOHOL_COUNT)
        {
            Debug.Log("[Cocktail] 더 이상 술을 추가할 수 없습니다.");
            return false;
        }

        SelectedIngredient ingredient = alcoholStatus.ToSelectedIngredient();
        selectedIngredients.Add(ingredient);

        RecalculateTotals();

        Debug.Log($"[Cocktail] 재료 추가: {ingredient.name} ({selectedIngredients.Count}/{MAX_ALCOHOL_COUNT})");
        Debug.Log($"[Cocktail] 도수:{ingredient.alcoholContent}, 단맛:{ingredient.sweetness}, 쓴맛:{ingredient.bitterness}, 향:{ingredient.flavor}, 향강도:{ingredient.flavorIntensity}");

        // 재료가 추가될 때마다 CocktailData 업데이트
        CocktailData.Instance.UpdateCurrentCocktail(this);
        return true;
    }

    // 기존 씬/호출부와의 호환을 위한 레거시 인터페이스
    public void GetAlcoholStatus(string name, int alcoholContent,
        int sweetness, int bitterness, FlavorType flavorType, int flavorIntensity)
    {
        if (selectedIngredients.Count >= MAX_ALCOHOL_COUNT)
        {
            Debug.Log("[Cocktail] 더 이상 술을 추가할 수 없습니다.");
            return;
        }

        IngredientId id = IngredientNameMapper.ToIngredientId(name);
        SelectedIngredient ingredient = new SelectedIngredient
        {
            id = id,
            name = IngredientNameMapper.ToDisplayName(id, name),
            alcoholContent = alcoholContent,
            sweetness = sweetness,
            bitterness = bitterness,
            flavor = flavorType,
            flavorIntensity = flavorIntensity
        };

        selectedIngredients.Add(ingredient);
        RecalculateTotals();

        Debug.Log($"[Cocktail] 재료 추가(호환): {ingredient.name} ({selectedIngredients.Count}/{MAX_ALCOHOL_COUNT})");

        CocktailData.Instance.UpdateCurrentCocktail(this);
    }

    public void SetMixState(MixState mixState)
    {
        currentMixState = mixState;
        Debug.Log($"[Cocktail] 현재 믹스 상태: {currentMixState}");
    }

    public void SetMixStateShake()
    {
        isTechnic = true;
        SetMixState(MixState.Shake);
    }

    public void SetMixStateStir()
    {
        isTechnic = true;
        SetMixState(MixState.Stir);
    }

    public void SetMixStateLayer()
    {
        // Layer는 따로 누르면 그냥 완성 버튼으로 동작
        if (!isTechnic)
        {
            SetMixState(MixState.Layer);
        }

        CalculateCoktail();
    }

    // 기존 버튼 바인딩 유지 (오타 이름 유지)
    public void CalculateCoktail()
    {
        CalculateCocktail();
    }

    private void CalculateCocktail()
    {
        if (selectedIngredients.Count == 0)
        {
            Debug.Log("[Cocktail] 선택된 술이 없습니다.");
            return;
        }

        if (selectedIngredients.Count != MAX_ALCOHOL_COUNT)
        {
            Debug.Log("[Cocktail] 칵테일을 만들기 위해서는 정확히 두 개의 술이 필요합니다.");
            return;
        }

        RecalculateTotals();

        SelectedIngredient ingredient1 = selectedIngredients[0];
        SelectedIngredient ingredient2 = selectedIngredients[1];

        Recipe result = RecipeBook.CheckCocktailRecipe(
            ingredient1.id,
            ingredient2.id,
            currentMixState);

        // CocktailData에 제조 완료된 칵테일 정보 저장
        CocktailData.Instance.SaveCompletedCocktail(this, result);

        if (result == Recipe.실패음료)
        {
            Debug.Log("[Cocktail] 알려진 레시피가 아닌 조합입니다(실패음료).");
        }
        else
        {
            Debug.Log($"[Cocktail] 완성된 칵테일: {result}");
        }

        isTechnic = false;
        LoadDessertScene();
    }

    public void ClearAlcoholList()
    {
        selectedIngredients.Clear();
        ResetTotals();
        CocktailData.Instance.ClearCurrentCocktailProgress();
        Debug.Log("[Cocktail] 술 목록이 초기화되었습니다.");
    }

    public MixState GetCurrentMixState() => currentMixState;
    public List<SelectedIngredient> GetSelectedIngredients() => new List<SelectedIngredient>(selectedIngredients);
    public int GetTotalAlcoholContent() => totalAlcoholContent;
    public int GetTotalSweetness() => totalSweetness;
    public int GetTotalBitterness() => totalBitterness;
    public int GetTotalFlavorIntensity() => totalFlavorIntensity;

    private void LoadDessertScene()
    {
        if (Game_Manager.Instance != null)
        {
            Game_Manager.Instance.ChangeScene(SceneNames.Dessert);
            return;
        }

        SceneManager.LoadScene(SceneNames.Dessert);
    }

    private void RecalculateTotals()
    {
        ResetTotals();

        foreach (SelectedIngredient ingredient in selectedIngredients)
        {
            totalAlcoholContent += ingredient.alcoholContent;
            totalSweetness += ingredient.sweetness;
            totalBitterness += ingredient.bitterness;
            totalFlavorIntensity += ingredient.flavorIntensity;
        }
    }

    private void ResetTotals()
    {
        totalAlcoholContent = 0;
        totalSweetness = 0;
        totalBitterness = 0;
        totalFlavorIntensity = 0;
    }
}
