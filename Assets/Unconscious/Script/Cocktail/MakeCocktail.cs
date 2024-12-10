using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MakeCocktail : MonoBehaviour
{
    private const int MAX_ALCOHOL_COUNT = 2;
    private const int MAX_LAYERS = 4;

    [SerializeField] private GameObject resetButton;

    private int currentLayer = 0;
    private List<GameObject> activeObjects = new List<GameObject>();
    private List<AlcoholStatus> alcoholStatuses = new List<AlcoholStatus>();

    private RecipeBook recipeBook;
    private MixState currentMixState;

    private int totalAlcoholContent = 0;
    private int totalSweetness = 0;
    private int totalBitterness = 0;
    private int totalFlavorIntensity = 0;

    private bool IsTechnic = false;

    private void Awake()
    {
        recipeBook = new RecipeBook();
        if (resetButton != null)
        {
            resetButton.SetActive(true);
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
        if (currentLayer < MAX_LAYERS)
        {
            DeactivateLayerObjects(currentLayer);
            obj.SetActive(true);
            activeObjects.Add(obj);
            currentLayer++;
        }
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
        alcoholStatuses.Clear();
        currentLayer = 0;

        totalAlcoholContent = 0;
        totalSweetness = 0;
        totalBitterness = 0;
        totalFlavorIntensity = 0;

        Debug.Log("모든 상태가 초기화되었습니다.");
    }

    public void GetAlcoholStatus(string name, int alcoholContent,
        int sweetness, int bitterness, FlavorType flavorType, int flavorIntensity)
    {
        if (alcoholStatuses.Count >= MAX_ALCOHOL_COUNT)
        {
            Debug.Log("더 이상 술을 추가할 수 없습니다.");
            return;
        }

        AlcoholStatus alcoholStatus = new AlcoholStatus
        {
            Name = name,
            AlcoholContent = alcoholContent,
            Sweetness = sweetness,
            Bitterness = bitterness,
            Flavor = flavorType,
            FlavorIntensity = flavorIntensity
        };

        alcoholStatuses.Add(alcoholStatus);

        Debug.Log($"현재 저장된 술의 개수: {alcoholStatuses.Count}");
        Debug.Log(
            $"추가된 술: {name}, " +
            $"도수: {alcoholContent}, " +
            $"단맛: {sweetness}, " +
            $"쓴맛: {bitterness}, " +
            $"향의 종류: {flavorType}, " +
            $"향의 세기: {flavorIntensity}");
    }

    public void SetMixState(MixState mixState)
    {
        currentMixState = mixState;
        Debug.Log($"현재 믹스 상태: {currentMixState}");
    }

    public void SetMixStateShake()
    {
        IsTechnic = true;
        SetMixState(MixState.Shake);
    }

    public void SetMixStateStir()
    {
        IsTechnic = true;
        SetMixState(MixState.Stir);
        
    }

    public void SetMixStateLayer()
    {
        //Layer외 기법을 상용하면 그냥 완성버튼
        if (!IsTechnic)
        {
            SetMixState(MixState.Layer);
        }
        
        CalculateCoktail();
        
        
    }

    public void CalculateCoktail()
    {
        if (alcoholStatuses.Count == 0)
        {
            Debug.Log("선택된 술이 없습니다.");
            return;
        }

        if (alcoholStatuses.Count != 2)
        {
            Debug.Log("칵테일을 만들기 위해서는 정확히 두 가지 술이 필요합니다.");
            return;
        }

        AlcoholStatus alcohol1 = alcoholStatuses[0];
        AlcoholStatus alcohol2 = alcoholStatuses[1];

        Recipe? result = recipeBook.Check_Cocktail_Recipe(
            alcohol1.Name, alcohol1.AlcoholContent, alcohol1.Sweetness,
            alcohol1.Bitterness, alcohol1.Flavor, alcohol1.FlavorIntensity,
            alcohol2.Name, alcohol2.AlcoholContent, alcohol2.Sweetness,
            alcohol2.Bitterness, alcohol2.Flavor, alcohol2.FlavorIntensity,
            currentMixState);

        if (result.HasValue)
        {
            Debug.Log($"완성된 칵테일: {result.Value}");
            IsTechnic = false;

            // 레시피 검증을 위해 SpeechBubbleManager에 전달
            SpeechBubbleManager speechBubbleManager = FindObjectOfType<SpeechBubbleManager>();
            speechBubbleManager.ValidateCocktail(result.Value);

            SceneManager.LoadScene("Dessert");
        }
        else
        {
            Debug.Log("알려진 레시피가 없는 조합입니다.");
        }

        foreach (var alcohol in alcoholStatuses)
        {
            totalAlcoholContent += alcohol.AlcoholContent;
            totalSweetness += alcohol.Sweetness;
            totalBitterness += alcohol.Bitterness;
            totalFlavorIntensity += alcohol.FlavorIntensity;
        }
    }

    public void ClearAlcoholList()
    {
        alcoholStatuses.Clear();
        Debug.Log("술 목록이 초기화되었습니다.");
    }
}
