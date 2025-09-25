using UnityEngine;

public class AlcoholStatus : MonoBehaviour
{
    [HideInInspector] public string Name;
    public int AlcoholContent;
    public int Sweetness;
    public int Bitterness;
    public FlavorType Flavor;
    public int FlavorIntensity;

    // 각 음료에 할당된 오브젝트들을 관리하는 배열
    [SerializeField] private GameObject[] layerObjects;

    private MakeCocktail makeCocktail;

    private void Start()
    {
        makeCocktail = FindObjectOfType<MakeCocktail>();

        if (makeCocktail == null)
        {
            Debug.LogError("MakeCocktail 스크립트를 찾을 수 없습니다!");
        }
    }

    private void OnMouseDown()
    {
        if (makeCocktail != null)
        {
            Name = gameObject.name;

            // 현재 층수에 해당하는 오브젝트 활성화 요청
            int currentLayer = makeCocktail.GetCurrentLayer();
            if (currentLayer < layerObjects.Length)
            {
                makeCocktail.ActivateLayerObject(layerObjects[currentLayer]);
            }

            makeCocktail.GetAlcoholStatus(
                Name,
                AlcoholContent,
                Sweetness,
                Bitterness,
                Flavor,
                FlavorIntensity
            );
        }
    }

    // 이 음료에 할당된 모든 오브젝트 비활성화
    public void DeactivateAllObjects()
    {
        foreach (GameObject obj in layerObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }
}

public enum FlavorType
{
    Fruity,
    Nutty,
    Alcohol
}


public class LayerIdentifier : MonoBehaviour
{
    [SerializeField] public int LayerNumber; // 이 오브젝트가 속한 층수 (0-3)
}