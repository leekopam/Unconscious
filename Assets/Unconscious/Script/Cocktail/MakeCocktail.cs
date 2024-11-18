using System.Collections.Generic;
using UnityEngine;
using static AlcoholStatus;

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

        int totalAlcoholContent = 0;
        int totalSweetness = 0;
        int totalBitterness = 0;
        int totalFlavorIntensity = 0;

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
