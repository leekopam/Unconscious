// 카테고리 매니저
using System.Collections.Generic;
using UnityEngine;

public class CategoryManager : MonoBehaviour
{
    [Header("카테고리 설정")]
    [SerializeField] private List<GameObject> decorationObjects;  // 데코레이션 오브젝트
    [SerializeField] private List<GameObject> garnishObjects;     // 가니쉬 오브젝트
    [SerializeField] private List<GameObject> glasswareObjects;   // 유리잔 오브젝트

    // 현재 선택된 카테고리의 오브젝트들만 활성화
    public void SelectCategory(string categoryName)
    {
        DisableAllObjects();

        switch (categoryName.ToLower())
        {
            case "decoration":
                SetObjectsActive(decorationObjects, true);
                break;
            case "garnish":
                SetObjectsActive(garnishObjects, true);
                break;
            case "glassware":
                SetObjectsActive(glasswareObjects, true);
                break;
        }
    }

    // 모든 오브젝트 비활성화
    private void DisableAllObjects()
    {
        SetObjectsActive(decorationObjects, false);
        SetObjectsActive(garnishObjects, false);
        SetObjectsActive(glasswareObjects, false);
    }

    // 오브젝트 리스트의 활성화 상태 설정
    private void SetObjectsActive(List<GameObject> objects, bool active)
    {
        foreach (var obj in objects)
        {
            obj.SetActive(active);
        }
    }
}