using UnityEngine;

public class CustomerClickHandler : MonoBehaviour
{
    private CustomerManager customerManagerInstance;
    private int customerID;
    private SpeechBubbleManager speechBubbleManager;
    private string[] dialogueLines = new string[]
    {
        "안녕하세요!",
        "칵테일 한 잔 주문하고 싶습니다.",
        "오늘의 추천 칵테일은 무엇인가요?"
    };

    public void Initialize(CustomerManager manager, int index, SpeechBubbleManager bubbleManager)
    {
        customerManagerInstance = manager;
        customerID = index;
        speechBubbleManager = bubbleManager;
    }

    private void OnMouseDown()
    {
        speechBubbleManager.ShowDialogue(gameObject, dialogueLines);
    }
}
