using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SpeechBubbleManager : MonoBehaviour
{
    [SerializeField] private GameObject speechBubblePrefab;
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private float BubblePos = 0f;
    [SerializeField] private CustomerStateData customerStateData;

    private Dictionary<GameObject, SpeechBubble> activeBubbles =
        new Dictionary<GameObject, SpeechBubble>();

    private Recipe currentRequestedRecipe; // 현재 고객의 레시피 추적
    private GameObject currentCustomer; // 현재 대화 중인 고객


    public void ShowSpeechBubble(GameObject customer)
    {
        CustomerController controller = customer.GetComponent<CustomerController>();
        if (controller == null) return;

        DialogueData.CustomerDialogue customerDialogue =
            dialogueData.GetDialogueForCustomerType(controller.CustomerType);

        // 고객 상태 저장
        SaveCustomerState(customer, customerDialogue);

        // 기존 대화 로직 유지
        currentRequestedRecipe = customerDialogue.requestedRecipe;

        GameObject bubbleObj = Instantiate(speechBubblePrefab, targetCanvas.transform);
        bubbleObj.SetActive(true);

        SpeechBubble bubble = bubbleObj.GetComponent<SpeechBubble>();
        bubble.OnDialogueComplete += () => {
            SceneManager.LoadScene("Cocktail");
            RemoveSpeechBubble(customer);
        };

        List<string> dialogueLines = dialogueData.GetDialogueLines(controller.CustomerType);
        bubble.Initialize(dialogueLines);

        PositionBubbleAboveCustomer(bubbleObj, customer);
        activeBubbles.Add(customer, bubble);
    }

    // 주문 완료된 고객을 위한 대화 메서드 추가
    public void ShowOrderCompletedDialogue(GameObject customer)
    {
        CustomerController controller = customer.GetComponent<CustomerController>();
        if (controller == null) return;

        GameObject bubbleObj = Instantiate(speechBubblePrefab, targetCanvas.transform);
        bubbleObj.SetActive(true);

        SpeechBubble bubble = bubbleObj.GetComponent<SpeechBubble>();
        bubble.OnDialogueComplete += () => {
            RemoveSpeechBubble(customer);
        };

        // 주문 완료 메시지 설정
        List<string> completedDialogueLines = new List<string> {
            "주문이 완료되었습니다!",
            "감사합니다!"
        };
        bubble.Initialize(completedDialogueLines);

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
            Destroy(bubble.gameObject);
            activeBubbles.Remove(customer);
        }
    }
    private void SaveCustomerState(GameObject customer, DialogueData.CustomerDialogue customerDialogue)
    {
        if (customerStateData == null)
        {
            customerStateData = ScriptableObject.CreateInstance<CustomerStateData>();
        }

        customerStateData.ClearStates();
        customerStateData.savedCustomerStates.Add(new CustomerStateData.CustomerState
        {
            customerType = customerDialogue.customerType,
            position = customer.transform.position,
            requestedRecipe = customerDialogue.requestedRecipe,
            isOrderCompleted = false
        });
    }

    public void ValidateCocktail(Recipe madeRecipe)
    {
        // 고객 상태 데이터가 없으면 반환
        if (customerStateData == null ||
            customerStateData.savedCustomerStates.Count == 0)
        {
            Debug.LogWarning("고객 상태 데이터가 없습니다.");
            return;
        }

        var customerState = customerStateData.savedCustomerStates[0];
        bool isCorrectRecipe = (madeRecipe == customerState.requestedRecipe);

        DialogueData.CustomerDialogue customerDialogue =
            dialogueData.customerDialogues
            .Find(d => d.requestedRecipe == customerState.requestedRecipe);

        if (customerDialogue == null)
        {
            Debug.LogWarning("해당 레시피의 대화 데이터를 찾을 수 없습니다.");
            return;
        }

        string reactionText = isCorrectRecipe
            ? customerDialogue.positiveReactions[Random.Range(0, customerDialogue.positiveReactions.Count)]
            : customerDialogue.negativeReactions[Random.Range(0, customerDialogue.negativeReactions.Count)];

        // Canvas가 null인지 확인
        if (targetCanvas == null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
        }

        // Canvas가 여전히 null이면 반환
        if (targetCanvas == null)
        {
            Debug.LogError("Canvas를 찾을 수 없습니다.");
            return;
        }

        CreateReactionBubble(reactionText);
    }

    private void CreateReactionBubble(string reactionText)
    {
        // 저장된 고객 위치에 반응 말풍선 생성
        if (customerStateData.savedCustomerStates.Count == 0) return;

        var customerState = customerStateData.savedCustomerStates[0];

        // 말풍선 프리팹이 null인지 확인
        if (speechBubblePrefab == null)
        {
            Debug.LogError("Speech Bubble 프리팹이 할당되지 않았습니다.");
            return;
        }

        GameObject bubbleObj = Instantiate(speechBubblePrefab, targetCanvas.transform);
        bubbleObj.SetActive(true);

        SpeechBubble bubble = bubbleObj.GetComponent<SpeechBubble>();
        bubble.Initialize(new List<string> { reactionText });

        // 고객 위치에 말풍선 배치
        bubbleObj.transform.position = customerState.position;

        bubble.OnDialogueComplete += () => {
            SceneManager.LoadScene("Order");
            Destroy(bubbleObj);
        };
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