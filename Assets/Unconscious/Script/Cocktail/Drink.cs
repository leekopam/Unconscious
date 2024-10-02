using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 칵테일의 재료와 특성을 관리하는 클래스
public class Drink
{
    // 재료 목록을 저장하는 리스트
    private List<Dictionary<string, float>> ingredients = new List<Dictionary<string, float>>();

    // 재료를 추가하는 메서드
    public void AddIngredient(Dictionary<string, float> ingredient)
    {
        if(ingredient.Count < 6)
        {
            ingredients.Add(ingredient); // 재료를 리스트에 추가
            Debug.Log($"{ingredient["name"]}이(가) 잔에 추가되었습니다."); // 콘솔에 로그 출력
        }
        else { Debug.Log("잔이 다 찼습니다."); }
        
    }

    // 재료들의 특성을 계산하는 private 메서드
    private (float, float, float) CalculateAttributes(string method)
    {
        // 각 특성의 총합을 계산
        float totalAlcohol = ingredients.Sum(i => i["alcoholContent"]);
        float totalSweetness = ingredients.Sum(i => i["sweetness"]);
        float totalBitterness = ingredients.Sum(i => i["bitterness"]);

        // 계산된 특성 값을 튜플로 반환
        return (totalAlcohol, totalSweetness, totalBitterness);
    }

    // 최종 결과를 계산하는 메서드
    public (float, float, float) CalculateFinalResult(string method)
    {
        var attributes = CalculateAttributes(method); // 특성 값을 계산
        // 각 특성 값을 2로 나눈 결과를 반환 (예시: 믹싱 방법에 따라 다른 로직 적용 가능)
        return (attributes.Item1 / 2, attributes.Item2 / 2, attributes.Item3 / 2);
    }
}