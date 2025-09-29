using System.Collections.Generic;
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

    // 제조된 칵테일 정보를 저장하는 클래스
    [System.Serializable]
    public class CocktailInfo
    {
        public Recipe cocktailRecipe;           // 완성된 칵테일 종류
        public MixState mixMethod;              // 제조 방법 (Shake, Stir, Layer)
        public List<AlcoholIngredient> ingredients; // 사용된 재료들
        public int totalAlcoholContent;         // 총 알코올 도수
        public int totalSweetness;              // 총 단맛
        public int totalBitterness;             // 총 쓴맛
        public int totalFlavorIntensity;        // 총 맛 강도
        public bool isSuccessful;               // 제조 성공 여부
        public System.DateTime createdTime;     // 제조 시간

        // 생성자 추가
        public CocktailInfo()
        {
            ingredients = new List<AlcoholIngredient>();
        }
    }

    // 칵테일에 사용된 재료 정보
    [System.Serializable]
    public class AlcoholIngredient
    {
        public string name;                     // 재료 이름
        public int alcoholContent;              // 알코올 도수
        public int sweetness;                   // 단맛
        public int bitterness;                  // 쓴맛
        public FlavorType flavorType;           // 맛 종류
        public int flavorIntensity;             // 맛 강도
    }

    [Header("Cocktail Manufacturing Data")]
    public List<CocktailInfo> cocktailHistory = new List<CocktailInfo>(); // 제조 이력
    public CocktailInfo currentCocktail;        // 현재 제조 중인 칵테일
    public CocktailInfo lastCompletedCocktail;  // 마지막으로 완성한 칵테일

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

    /// <summary>
    /// MakeCocktail에서 제조 완료된 칵테일 정보를 저장
    /// </summary>
    /// <param name="makeCocktail">MakeCocktail 인스턴스</param>
    /// <param name="result">제조 결과 (Recipe)</param>
    public void SaveCompletedCocktail(MakeCocktail makeCocktail, Recipe? result)
    {
        if (makeCocktail == null) return;

        CocktailInfo cocktailInfo = new CocktailInfo
        {
            cocktailRecipe = result ?? Recipe.실패음료,
            mixMethod = makeCocktail.GetCurrentMixState(),
            ingredients = ConvertAlcoholStatusesToIngredients(makeCocktail.GetAlcoholStatuses()),
            totalAlcoholContent = makeCocktail.GetTotalAlcoholContent(),
            totalSweetness = makeCocktail.GetTotalSweetness(),
            totalBitterness = makeCocktail.GetTotalBitterness(),
            totalFlavorIntensity = makeCocktail.GetTotalFlavorIntensity(),
            isSuccessful = result.HasValue && result.Value != Recipe.실패음료,
            createdTime = System.DateTime.Now
        };

        // 현재 칵테일과 마지막 완성 칵테일 업데이트
        currentCocktail = cocktailInfo;
        lastCompletedCocktail = cocktailInfo;

        // 이력에 추가
        cocktailHistory.Add(cocktailInfo);

        Debug.Log($"칵테일 제조 완료 저장 - {cocktailInfo.cocktailRecipe}: {(cocktailInfo.isSuccessful ? "성공" : "실패")}");
        Debug.Log($"재료 개수: {cocktailInfo.ingredients.Count}, 믹스 방법: {cocktailInfo.mixMethod}");
        Debug.Log($"총 도수: {cocktailInfo.totalAlcoholContent}, 단맛: {cocktailInfo.totalSweetness}, 쓴맛: {cocktailInfo.totalBitterness}");
    }

    /// <summary>
    /// 제조 진행 중인 칵테일 정보 업데이트 (재료 추가시마다 호출)
    /// </summary>
    /// <param name="makeCocktail">MakeCocktail 인스턴스</param>
    public void UpdateCurrentCocktail(MakeCocktail makeCocktail)
    {
        if (makeCocktail == null) return;

        currentCocktail = new CocktailInfo
        {
            cocktailRecipe = Recipe.주문없음, // 아직 완성되지 않음
            mixMethod = makeCocktail.GetCurrentMixState(),
            ingredients = ConvertAlcoholStatusesToIngredients(makeCocktail.GetAlcoholStatuses()),
            totalAlcoholContent = makeCocktail.GetTotalAlcoholContent(),
            totalSweetness = makeCocktail.GetTotalSweetness(),
            totalBitterness = makeCocktail.GetTotalBitterness(),
            totalFlavorIntensity = makeCocktail.GetTotalFlavorIntensity(),
            isSuccessful = false,
            createdTime = System.DateTime.Now
        };

        Debug.Log($"현재 제조 중인 칵테일 업데이트 - 재료: {currentCocktail.ingredients.Count}개");
    }

    /// <summary>
    /// AlcoholStatus 리스트를 AlcoholIngredient 리스트로 변환
    /// </summary>
    private List<AlcoholIngredient> ConvertAlcoholStatusesToIngredients(List<AlcoholStatus> alcoholStatuses)
    {
        List<AlcoholIngredient> ingredients = new List<AlcoholIngredient>();
        
        foreach (var alcohol in alcoholStatuses)
        {
            AlcoholIngredient ingredient = new AlcoholIngredient
            {
                name = alcohol.Name,
                alcoholContent = alcohol.AlcoholContent,
                sweetness = alcohol.Sweetness,
                bitterness = alcohol.Bitterness,
                flavorType = alcohol.Flavor,
                flavorIntensity = alcohol.FlavorIntensity
            };
            ingredients.Add(ingredient);
            
            Debug.Log($"재료 변환: {ingredient.name} (도수:{ingredient.alcoholContent}, 단맛:{ingredient.sweetness}, 쓴맛:{ingredient.bitterness})");
        }
        
        return ingredients;
    }

    #region 기존 헬퍼 메서드들 (더 이상 사용하지 않음)

    private MixState GetMixStateFromMakeCocktail(MakeCocktail makeCocktail)
    {
        return makeCocktail.GetCurrentMixState();
    }

    private List<AlcoholIngredient> GetIngredientsFromMakeCocktail(MakeCocktail makeCocktail)
    {
        return ConvertAlcoholStatusesToIngredients(makeCocktail.GetAlcoholStatuses());
    }

    private int GetTotalAlcoholContent(MakeCocktail makeCocktail)
    {
        return makeCocktail.GetTotalAlcoholContent();
    }

    private int GetTotalSweetness(MakeCocktail makeCocktail)
    {
        return makeCocktail.GetTotalSweetness();
    }

    private int GetTotalBitterness(MakeCocktail makeCocktail)
    {
        return makeCocktail.GetTotalBitterness();
    }

    private int GetTotalFlavorIntensity(MakeCocktail makeCocktail)
    {
        return makeCocktail.GetTotalFlavorIntensity();
    }

    #endregion

    #region 퍼블릭 접근 메서드들

    /// <summary>
    /// 마지막으로 제조한 칵테일 정보 반환
    /// </summary>
    public CocktailInfo GetLastCocktail()
    {
        return lastCompletedCocktail;
    }

    /// <summary>
    /// 현재 제조 중인 칵테일 정보 반환
    /// </summary>
    public CocktailInfo GetCurrentCocktail()
    {
        return currentCocktail;
    }

    /// <summary>
    /// 특정 레시피의 제조 횟수 반환
    /// </summary>
    public int GetRecipeCount(Recipe recipe)
    {
        int count = 0;
        foreach (var cocktail in cocktailHistory)
        {
            if (cocktail.cocktailRecipe == recipe && cocktail.isSuccessful)
                count++;
        }
        return count;
    }

    /// <summary>
    /// 성공한 칵테일 제조 횟수 반환
    /// </summary>
    public int GetSuccessfulCocktailCount()
    {
        int count = 0;
        foreach (var cocktail in cocktailHistory)
        {
            if (cocktail.isSuccessful)
                count++;
        }
        return count;
    }

    /// <summary>
    /// 실패한 칵테일 제조 횟수 반환
    /// </summary>
    public int GetFailedCocktailCount()
    {
        int count = 0;
        foreach (var cocktail in cocktailHistory)
        {
            if (!cocktail.isSuccessful)
                count++;
        }
        return count;
    }

    /// <summary>
    /// 칵테일 제조 이력 초기화
    /// </summary>
    public void ClearHistory()
    {
        cocktailHistory.Clear();
        currentCocktail = null;
        lastCompletedCocktail = null;
        Debug.Log("칵테일 제조 이력이 초기화되었습니다.");
    }

    /// <summary>
    /// 특정 고객 주문에 맞는 칵테일을 제조했는지 확인
    /// </summary>
    /// <param name="orderedRecipe">주문받은 레시피</param>
    /// <returns>주문과 일치하면 true</returns>
    public bool IsLastCocktailCorrect(Recipe orderedRecipe)
    {
        if (lastCompletedCocktail == null) return false;
        return lastCompletedCocktail.isSuccessful && lastCompletedCocktail.cocktailRecipe == orderedRecipe;
    }

    /// <summary>
    /// 마지막 완성된 칵테일의 상세 정보를 출력
    /// </summary>
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
        foreach (var ingredient in lastCompletedCocktail.ingredients)
        {
            Debug.Log($"- {ingredient.name}: 도수{ingredient.alcoholContent}, 단맛{ingredient.sweetness}, 쓴맛{ingredient.bitterness}, {ingredient.flavorType}");
        }
    }

    #endregion
}
