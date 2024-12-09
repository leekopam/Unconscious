using UnityEngine;
using UnityEngine.UI;

public class CustomerController : MonoBehaviour
{
    [SerializeField] private int customerType;
    public int CustomerType => customerType;

    private void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        SpeechBubbleManager manager = FindObjectOfType<SpeechBubbleManager>();
        if (manager != null)
        {
            manager.ShowSpeechBubble(gameObject);
        }
    }
}