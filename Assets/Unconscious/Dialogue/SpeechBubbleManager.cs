using UnityEngine;
using System.Collections.Generic;

public class SpeechBubbleManager : MonoBehaviour
{
    [SerializeField] private GameObject speechBubblePrefab;
    private Dictionary<GameObject, SpeechBubble> activeBubbles = new Dictionary<GameObject, SpeechBubble>();
    private SpeechBubble currentActiveBubble;
    private SpeechBubbleManager speechBubbleManager;

    private void start()
    {
        speechBubbleManager = FindObjectOfType<SpeechBubbleManager>();
    }
    public void CreateSpeechBubble(GameObject customer)
    {
        if (!activeBubbles.ContainsKey(customer))
        {
            GameObject bubbleObj = Instantiate(speechBubblePrefab, customer.transform);
            SpeechBubble bubble = bubbleObj.GetComponent<SpeechBubble>();
            activeBubbles.Add(customer, bubble);
        }
    }

    public void ShowDialogue(GameObject customer, string[] dialogueLines)
    {
        if (currentActiveBubble != null) return; // 다른 대화 진행 중이면 무시

        if (activeBubbles.TryGetValue(customer, out SpeechBubble bubble))
        {
            currentActiveBubble = bubble;
            bubble.StartDialogue(customer, dialogueLines);
        }
    }
}