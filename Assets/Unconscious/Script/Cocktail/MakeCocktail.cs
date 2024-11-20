using System.Collections.Generic;
using UnityEngine;

public class MakeCocktail : MonoBehaviour
{
    private const int MAX_ALCOHOL_COUNT = 2; // 최대 저장 가능한 술의 개수

    string AlcoholName;
    private int AlcoholContent; // 도수
    private int Sweetness;      // 단맛
    private int Bitterness;     // 쓴맛
    private FlavorType FlavorType; // 향의 종류
    private int FlavorIntensity;   // 향의 세기

    //현재 선택된 술 정보
    private List<AlcoholStatus> alcoholStatuses = new List<AlcoholStatus>();

    int totalAlcoholContent = 0;
    int totalSweetness = 0;
    int totalBitterness = 0;
    int totalFlavorIntensity = 0;

    private RecipeBook recipeBook;
    private void Awake()
    {
        recipeBook = new RecipeBook();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            CalculateCoktail();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ClearAlcoholList();
        }
        
    }

    //선택한 술 정보를 받아옴
    public void GetAlcoholStatus(string name, int alcoholContent,
    int sweetness, int bitterness, FlavorType flavorType, int flavorIntensity)
    {
        // 최대 개수 체크
        if (alcoholStatuses.Count >= 2)
        {
            Debug.Log("더 이상 술을 추가할 수 없습니다.");
            return;
        }

        // AlcoholStatus 객체 생성 및 속성 설정
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
        Debug.Log($"추가된 술: {name}, 도수: {alcoholContent}, 단맛: {sweetness}, 쓴맛: {bitterness}, 향의 종류: {flavorType}, 향의 세기: {flavorIntensity}");
    }

    

    public void CalculateCoktail()
    {
        if(alcoholStatuses.Count == 0)
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
       alcohol1.Name, alcohol1.AlcoholContent, alcohol1.Sweetness, alcohol1.Bitterness, alcohol1.Flavor, alcohol1.FlavorIntensity,
       alcohol2.Name, alcohol2.AlcoholContent, alcohol2.Sweetness, alcohol2.Bitterness, alcohol2.Flavor, alcohol2.FlavorIntensity);

        if (result.HasValue)
        {
            Debug.Log($"완성된 칵테일: {result.Value}");
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

        /*
        // 평균값 계산
        float avgAlcoholContent = totalAlcoholContent / (float)alcoholStatuses.Count;
        float avgSweetness = totalSweetness / (float)alcoholStatuses.Count;
        float avgBitterness = totalBitterness / (float)alcoholStatuses.Count;
        float avgFlavorIntensity = totalFlavorIntensity / (float)alcoholStatuses.Count;

        Debug.Log($"칵테일 계산 결과:\n" +
                 $"평균 도수: {avgAlcoholContent}\n" +
                 $"평균 단맛: {avgSweetness}\n" +
                 $"평균 쓴맛: {avgBitterness}\n" +
                 $"평균 향 세기: {avgFlavorIntensity}");
        */
    }

    // 리스트 초기화
    public void ClearAlcoholList()
    {
        alcoholStatuses.Clear();
        Debug.Log("술 목록이 초기화되었습니다.");
    }
}
