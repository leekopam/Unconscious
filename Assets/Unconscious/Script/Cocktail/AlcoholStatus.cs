using UnityEngine;

public class AlcoholStatus : MonoBehaviour
{
    [HideInInspector] public string Name;
    public int AlcoholContent;
    public int Sweetness;
    public int Bitterness;
    public FlavorType Flavor;
    public int FlavorIntensity;

    private MakeCocktail makeCocktail;

    private void Start()
    {
        // MakeCocktail 스크립트는 다른 게임오브젝트에 있을 수 있으므로
        // FindObjectOfType을 사용하여 찾는다
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
            // 게임오브젝트의 이름을 가져옵니다
            Name = gameObject.name;

            // MakeCocktail 스크립트의 메서드를 호출합니다
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
}

public enum FlavorType
{
    Fruity,
    Nutty,
    Alcohol
}