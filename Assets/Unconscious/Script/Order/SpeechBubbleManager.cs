using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SpeechBubbleManager : MonoBehaviour
{
    [SerializeField] private GameObject speechBubblePrefab;
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private float BubblePos = 0f;

    private Dictionary<GameObject, SpeechBubble> activeBubbles = new Dictionary<GameObject, SpeechBubble>();

    public void ShowSpeechBubble(GameObject customer)
    {
        if (activeBubbles.ContainsKey(customer)) return;

        CustomerController controller = customer.GetComponent<CustomerController>();
        if (controller == null) return;

        GameObject bubbleObj = Instantiate(speechBubblePrefab, targetCanvas.transform);
        bubbleObj.SetActive(true);

        SpeechBubble bubble = bubbleObj.GetComponent<SpeechBubble>();
        bubble.OnDialogueComplete += () => {
            //말풍선 대화 끝나는 지점
            Debug.Log($"손님 {controller.CustomerType}의 대화가 종료되었습니다.");
            SceneManager.LoadScene("Cocktail");
            RemoveSpeechBubble(customer);
        };

        List<string> dialogueLines = dialogueData.GetDialogueForCustomer(controller.CustomerType);
        bubble.Initialize(dialogueLines);

        PositionBubbleAboveCustomer(bubbleObj, customer);
        activeBubbles.Add(customer, bubble);
    }

    private void PositionBubbleAboveCustomer(GameObject bubble, GameObject customer)
    {
        Vector3 customerPosition = customer.transform.position;
        Vector3 bubblePosition = new Vector3(
            customerPosition.x,
            customerPosition.y + BubblePos,
            customerPosition.z
        );
        bubble.transform.position = bubblePosition;
    }

    public void RemoveSpeechBubble(GameObject customer)
    {
        if (activeBubbles.TryGetValue(customer, out SpeechBubble bubble))
        {
            Debug.Log("말풍선이 제거되었습니다.");
            Destroy(bubble.gameObject);
            activeBubbles.Remove(customer);
        }
    }

    private void OnDestroy()
    {
        foreach (var bubble in activeBubbles.Values)
        {
            if (bubble != null)
            {
                bubble.OnDialogueComplete = null;
            }
        }
    }
}