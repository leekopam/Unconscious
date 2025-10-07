using System;
using System.Collections.Generic;
using UnityEngine;

public enum IngredientId
{
    Unknown,
    PeachWine,
    NarinIhwaju,
    Gahyangju,
    Soseulbaram,
    Hojeopjimong,
    Dongjitdal
}

[Serializable]
public class SelectedIngredient
{
    public IngredientId id;
    public string name;
    public int alcoholContent;
    public int sweetness;
    public int bitterness;
    public FlavorType flavor;
    public int flavorIntensity;
}

public static class IngredientNameMapper
{
    private static readonly Dictionary<IngredientId, string> displayNameById = new Dictionary<IngredientId, string>
    {
        { IngredientId.PeachWine, "도화주" },
        { IngredientId.NarinIhwaju, "나린 이화주" },
        { IngredientId.Gahyangju, "가향주" },
        { IngredientId.Soseulbaram, "소슬바람" },
        { IngredientId.Hojeopjimong, "호접지몽" },
        { IngredientId.Dongjitdal, "동짓달" },
    };

    private static readonly Dictionary<string, IngredientId> idByNormalizedName =
        new Dictionary<string, IngredientId>(StringComparer.Ordinal)
        {
            { "도화주", IngredientId.PeachWine },
            { "나린 이화주", IngredientId.NarinIhwaju },
            { "가향주", IngredientId.Gahyangju },
            { "소슬바람", IngredientId.Soseulbaram },
            { "호접지몽", IngredientId.Hojeopjimong },
            { "동짓달", IngredientId.Dongjitdal },
        };

    public static IngredientId ToIngredientId(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
        {
            return IngredientId.Unknown;
        }

        string normalizedName = rawName.Trim();
        return idByNormalizedName.TryGetValue(normalizedName, out IngredientId id)
            ? id
            : IngredientId.Unknown;
    }

    public static string ToDisplayName(IngredientId id, string fallbackName)
    {
        if (displayNameById.TryGetValue(id, out string displayName))
        {
            return displayName;
        }

        return string.IsNullOrWhiteSpace(fallbackName) ? id.ToString() : fallbackName.Trim();
    }
}

public class AlcoholStatus : MonoBehaviour
{
    [SerializeField] private IngredientId ingredientId = IngredientId.Unknown;

    public int AlcoholContent;
    public int Sweetness;
    public int Bitterness;
    public FlavorType Flavor;
    public int FlavorIntensity;

    // 각 음료에 할당된 오브젝트들을 관리하는 배열
    [SerializeField] private GameObject[] layerObjects;

    private MakeCocktail makeCocktail;

    public IngredientId Ingredient => ResolveIngredientId();

    public string DisplayName => IngredientNameMapper.ToDisplayName(
        ResolveIngredientId(),
        gameObject != null ? gameObject.name : string.Empty);

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
        if (makeCocktail == null)
        {
            return;
        }

        bool isAdded = makeCocktail.TryAddIngredient(this);
        if (!isAdded)
        {
            return;
        }

        // 현재 층수에 해당하는 오브젝트 활성화 요청
        int currentLayer = makeCocktail.GetCurrentLayer();
        if (currentLayer < layerObjects.Length)
        {
            makeCocktail.ActivateLayerObject(layerObjects[currentLayer]);
        }
    }

    public SelectedIngredient ToSelectedIngredient()
    {
        return new SelectedIngredient
        {
            id = ResolveIngredientId(),
            name = DisplayName,
            alcoholContent = AlcoholContent,
            sweetness = Sweetness,
            bitterness = Bitterness,
            flavor = Flavor,
            flavorIntensity = FlavorIntensity
        };
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

    private IngredientId ResolveIngredientId()
    {
        if (ingredientId != IngredientId.Unknown)
        {
            return ingredientId;
        }

        return IngredientNameMapper.ToIngredientId(gameObject != null ? gameObject.name : string.Empty);
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
