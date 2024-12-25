using UnityEngine;
using UnityEngine.UI;

public class CustomerController : MonoBehaviour
{
    [SerializeField] private int customerType;
    public int CustomerType => customerType;

    private void OnClick()
    {
        SpeechBubbleManager manager = FindObjectOfType<SpeechBubbleManager>();
        if (manager != null)
        {
            manager.ShowSpeechBubble(gameObject);
        }
    }
}