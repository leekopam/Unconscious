using UnityEngine;
using UnityEngine.EventSystems;

public class IngredientButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Ingredient ingredient;

    // 마우스가 버튼 위에 올라갔을 때 호출되는 메서드
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 버튼의 월드 위치를 기준으로 툴팁 표시
        Vector3 tooltipPosition = transform.position + new Vector3(0, 50, 0); // 버튼 위에 표시
        TooltipManager.Instance.ShowTooltip(ingredient, tooltipPosition);
    }

    // 마우스가 버튼에서 벗어났을 때 호출되는 메서드
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip(ingredient);
    }
}