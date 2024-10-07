using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            tooltip.SetActive(true);
            Debug.Log("마우스 들어옴");
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            tooltip.SetActive(false);
            Debug.Log("마우스 나감");
        }
         
    }
}
