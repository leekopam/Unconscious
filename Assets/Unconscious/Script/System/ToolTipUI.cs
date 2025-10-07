using UnityEngine;

public class ToolTipUI : MonoBehaviour
{
    public GameObject panel;

    private void OnMouseOver()
    {
        panel.SetActive(true);
    }

    private void OnMouseExit()
    {
        panel.SetActive(false);
    }
}
