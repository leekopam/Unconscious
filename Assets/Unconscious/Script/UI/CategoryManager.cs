using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// 카테고리 매니저 - 각 카테고리별 오브젝트 관리
/// </summary>
public class CategoryManager : MonoBehaviour
{
    [Header("카테고리 설정")]
    [SerializeField] private List<GameObject> snackObjects;      // 스낵 오브젝트
    [SerializeField] private List<GameObject> glassObjects;      // 유리잔 오브젝트
    [SerializeField] private List<GameObject> garnishObjects;    // 가니쉬 오브젝트 (유리잔의 자식)

    private List<GameObject> activeGarnishObjects = new List<GameObject>();  // 현재 활성화된 가니쉬 오브젝트

    /// <summary>
    /// 카테고리 선택 시 호출되는 메서드
    /// </summary>
    public void SelectCategory(string categoryName)
    {
        DisableAllObjects();

        switch (categoryName.ToLower())
        {
            case "snack":
                SetObjectsActive(snackObjects, true);
                break;
            case "glass":
                SetObjectsActive(glassObjects, true);
                break;
            case "garnish":
                // 현재 활성화된 유리잔의 가니쉬 자식 오브젝트들 활성화
                foreach (var glassObj in glassObjects)
                {
                    if (glassObj.activeSelf)
                    {
                        Transform garnishParent = glassObj.transform.Find("Garnish");
                        if (garnishParent != null)
                        {
                            foreach (Transform child in garnishParent)
                            {
                                activeGarnishObjects.Add(child.gameObject);
                            }
                        }
                    }
                }
                SetObjectsActive(activeGarnishObjects, true);
                break;
        }
    }

    /// <summary>
    /// 모든 오브젝트 비활성화
    /// </summary>
    private void DisableAllObjects()
    {
        SetObjectsActive(snackObjects, false);
        SetObjectsActive(glassObjects, false);
        SetObjectsActive(activeGarnishObjects, false);
        activeGarnishObjects.Clear();
    }

    /// <summary>
    /// 오브젝트 리스트의 활성화 상태 설정
    /// </summary>
    private void SetObjectsActive(List<GameObject> objects, bool active)
    {
        foreach (var obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(active);
            }
        }
    }
}