using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CocktailData : MonoBehaviour
{
    private static CocktailData instance;

    public static CocktailData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CocktailData>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("CocktailData");
                    instance = obj.AddComponent<CocktailData>();
                    DontDestroyOnLoad(obj);
                }
            }

            return instance;
        }
    }

    [System.Serializable]
    public class CocktailInfo
    {
        public Recipe cocktailRecipe;                 // 완성된 칵테일 종류
        public MixState mixMethod;                    // 제조 방법 (Shake, Stir, Layer)
        public List<AlcoholIngredient> ingredients;   // 사용된 재료들
        public int totalAlcoholContent;               // 총 알코올 도수
        public int totalSweetness;                    // 총 단맛
        public int totalBitterness;                   // 총 쓴맛
        public int totalFlavorIntensity;              // 총 맛 강도
        public bool isSuccessful;                     // 제조 성공 여부
        public System.DateTime createdTime;           // 제조 시간

        public CocktailInfo()
        {
            ingredients = new List<AlcoholIngredient>();
        }
    }

    [System.Serializable]
    public class AlcoholIngredient
    {
        public IngredientId id;       // 안정 식별자
        public string name;           // 재료 이름
        public int alcoholContent;    // 알코올 도수
        public int sweetness;         // 단맛
        public int bitterness;        // 쓴맛
        public FlavorType flavorType; // 향 종류
        public int flavorIntensity;   // 향 강도
    }

    [Header("Cocktail Manufacturing Data")]
    public List<CocktailInfo> cocktailHistory = new List<CocktailInfo>();
    [ReadOnly] public CocktailInfo currentCocktail;
    [ReadOnly] public CocktailInfo lastCompletedCocktail;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SaveCompletedCocktail(MakeCocktail makeCocktail, Recipe result)
    {
        if (makeCocktail == null)
        {
            return;
        }

        CocktailInfo cocktailInfo = new CocktailInfo
        {
            cocktailRecipe = result,
            mixMethod = makeCocktail.GetCurrentMixState(),
            ingredients = ConvertSelectedIngredientsToIngredients(makeCocktail.GetSelectedIngredients()),
            totalAlcoholContent = makeCocktail.GetTotalAlcoholContent(),
            totalSweetness = makeCocktail.GetTotalSweetness(),
            totalBitterness = makeCocktail.GetTotalBitterness(),
            totalFlavorIntensity = makeCocktail.GetTotalFlavorIntensity(),
            isSuccessful = result != Recipe.실패음료,
            createdTime = System.DateTime.Now
        };

        currentCocktail = cocktailInfo;
        lastCompletedCocktail = cocktailInfo;
        cocktailHistory.Add(cocktailInfo);

        Debug.Log($"[Cocktail] 제조 완료 저장 - {cocktailInfo.cocktailRecipe}: {(cocktailInfo.isSuccessful ? "성공" : "실패")}");
        Debug.Log($"[Cocktail] 재료 개수: {cocktailInfo.ingredients.Count}, 믹스 방법: {cocktailInfo.mixMethod}");
        Debug.Log($"[Cocktail] 총 도수: {cocktailInfo.totalAlcoholContent}, 단맛: {cocktailInfo.totalSweetness}, 쓴맛: {cocktailInfo.totalBitterness}");
    }

    // 기존 nullable 호출부 호환
    public void SaveCompletedCocktail(MakeCocktail makeCocktail, Recipe? result)
    {
        SaveCompletedCocktail(makeCocktail, result ?? Recipe.실패음료);
    }

    public void UpdateCurrentCocktail(MakeCocktail makeCocktail)
    {
        if (makeCocktail == null)
        {
            return;
        }

        currentCocktail = new CocktailInfo
        {
            cocktailRecipe = Recipe.주문없음, // 아직 완성되지 않음
            mixMethod = makeCocktail.GetCurrentMixState(),
            ingredients = ConvertSelectedIngredientsToIngredients(makeCocktail.GetSelectedIngredients()),
            totalAlcoholContent = makeCocktail.GetTotalAlcoholContent(),
            totalSweetness = makeCocktail.GetTotalSweetness(),
            totalBitterness = makeCocktail.GetTotalBitterness(),
            totalFlavorIntensity = makeCocktail.GetTotalFlavorIntensity(),
            isSuccessful = false,
            createdTime = System.DateTime.Now
        };

        Debug.Log($"[Cocktail] 현재 제조 중 칵테일 업데이트 - 재료: {currentCocktail.ingredients.Count}개");
    }

    public void ClearCurrentCocktailProgress()
    {
        currentCocktail = null;
    }

    private List<AlcoholIngredient> ConvertSelectedIngredientsToIngredients(List<SelectedIngredient> selectedIngredients)
    {
        List<AlcoholIngredient> ingredients = new List<AlcoholIngredient>();

        foreach (SelectedIngredient selectedIngredient in selectedIngredients)
        {
            AlcoholIngredient ingredient = new AlcoholIngredient
            {
                id = selectedIngredient.id,
                name = selectedIngredient.name,
                alcoholContent = selectedIngredient.alcoholContent,
                sweetness = selectedIngredient.sweetness,
                bitterness = selectedIngredient.bitterness,
                flavorType = selectedIngredient.flavor,
                flavorIntensity = selectedIngredient.flavorIntensity
            };

            ingredients.Add(ingredient);
        }

        return ingredients;
    }

    public CocktailInfo GetLastCocktail()
    {
        return lastCompletedCocktail;
    }

    public CocktailInfo GetCurrentCocktail()
    {
        return currentCocktail;
    }

    public int GetRecipeCount(Recipe recipe)
    {
        int count = 0;
        foreach (CocktailInfo cocktail in cocktailHistory)
        {
            if (cocktail.cocktailRecipe == recipe && cocktail.isSuccessful)
            {
                count++;
            }
        }

        return count;
    }

    public int GetSuccessfulCocktailCount()
    {
        int count = 0;
        foreach (CocktailInfo cocktail in cocktailHistory)
        {
            if (cocktail.isSuccessful)
            {
                count++;
            }
        }

        return count;
    }

    public int GetFailedCocktailCount()
    {
        int count = 0;
        foreach (CocktailInfo cocktail in cocktailHistory)
        {
            if (!cocktail.isSuccessful)
            {
                count++;
            }
        }

        return count;
    }

    public void ClearHistory()
    {
        cocktailHistory.Clear();
        currentCocktail = null;
        lastCompletedCocktail = null;
        Debug.Log("[Cocktail] 제조 이력이 초기화되었습니다.");
    }

    public bool IsLastCocktailCorrect(Recipe orderedRecipe)
    {
        if (lastCompletedCocktail == null)
        {
            return false;
        }

        return lastCompletedCocktail.isSuccessful && lastCompletedCocktail.cocktailRecipe == orderedRecipe;
    }

    public void PrintLastCocktailInfo()
    {
        if (lastCompletedCocktail == null)
        {
            Debug.Log("완성된 칵테일이 없습니다.");
            return;
        }

        Debug.Log("=== 마지막 완성된 칵테일 정보 ===");
        Debug.Log($"칵테일: {lastCompletedCocktail.cocktailRecipe}");
        Debug.Log($"제조 방법: {lastCompletedCocktail.mixMethod}");
        Debug.Log($"성공 여부: {(lastCompletedCocktail.isSuccessful ? "성공" : "실패")}");
        Debug.Log($"총 도수: {lastCompletedCocktail.totalAlcoholContent}");
        Debug.Log($"총 단맛: {lastCompletedCocktail.totalSweetness}");
        Debug.Log($"총 쓴맛: {lastCompletedCocktail.totalBitterness}");
        Debug.Log($"총 맛 강도: {lastCompletedCocktail.totalFlavorIntensity}");

        Debug.Log("사용된 재료:");
        foreach (AlcoholIngredient ingredient in lastCompletedCocktail.ingredients)
        {
            Debug.Log($"- {ingredient.name}: 도수{ingredient.alcoholContent}, 단맛{ingredient.sweetness}, 쓴맛{ingredient.bitterness}, {ingredient.flavorType}");
        }
    }
}
