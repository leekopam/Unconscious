using UnityEngine;
using System.Collections.Generic;

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
        bubbleObj.SetActive(true); // 생성 후 즉시 활성화

        SpeechBubble bubble = bubbleObj.GetComponent<SpeechBubble>();
        List<string> dialogueLines = dialogueData.GetDialogueForCustomer(controller.CustomerType);
        bubble.Initialize(dialogueLines);

        PositionBubbleAboveCustomer(bubbleObj, customer);
        activeBubbles.Add(customer, bubble);
    }

    private void PositionBubbleAboveCustomer(GameObject bubble, GameObject customer)
    {
        // 캐릭터의 현재 위치 가져오기
        Vector3 customerPosition = customer.transform.position;

        // 말풍선 위치 설정 (캐릭터 위 20 유닛)
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
            Destroy(bubble.gameObject);
            activeBubbles.Remove(customer);
        }
    }
}