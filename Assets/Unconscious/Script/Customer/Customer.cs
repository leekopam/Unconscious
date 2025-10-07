using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [HideInInspector] public Animator animator;
    public Customer_DialogueData dialogueData;
    public float typingTime = 1.0f; // 타이핑 속도

    [HideInInspector] public int prefabIndex = -1; // 손님 프리팹 인덱스
    [HideInInspector] public int dialogueIndex = 0; // 대화 순서 인덱스
    [HideInInspector] public int seatIndex = SeatIndex.Unknown;

    private ICustomerState currentState;
    private readonly List<GameObject> dialogue_canvas = new List<GameObject>();
    private Recipe currentOrder = Recipe.주문없음;

    #region 상태 확인 메서드들
    public bool IsSeated() => currentState is SeatedState;
    public bool IsWating() => currentState is WaitingState; // 주문은 받지 않았지만 생성은 된 상태
    public bool IsTasting() => currentState is TasteState;
    public bool IsExiting() => currentState is ExitState;
    public string GetCurrentStateName() => currentState?.GetType().Name ?? "None";
    public Recipe CurrentOrder => currentOrder;
    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();

        GameObject dialogueParent = GameObject.Find("Canvas/Canvas_Dialogue");
        if (dialogueParent != null)
        {
            foreach (Transform child in dialogueParent.transform)
            {
                dialogue_canvas.Add(child.gameObject);
            }
        }

        InitializeDrinkOrder();
    }

    private void FixedUpdate()
    {
        currentState?.Update(this);
    }

    #region 상태전환 기능
    public void state_Sitting() => ChangeState(new SeatedState()); // 손님 착석
    public void state_Waiting() => ChangeState(new WaitingState()); // 손님 대기(주문하고 다른씬 갔다가 돌아옴)
    public void state_Taste() => ChangeState(new TasteState()); // 주문받은 손님 대기상태 및 음료 검증
    public void state_Exit() => ChangeState(new ExitState());  // 손님 퇴장

    public void ChangeState(ICustomerState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }
    #endregion

    #region 대화 기능
    public void SetDialogueCanvasActive(bool active, string message)
    {
        int dialogueCanvasIndex = GetDialogueCanvasIndex();
        if (dialogueCanvasIndex < 0 || dialogueCanvasIndex >= dialogue_canvas.Count)
        {
            return;
        }

        GameObject targetCanvas = dialogue_canvas[dialogueCanvasIndex];
        targetCanvas.SetActive(active);
        if (!active)
        {
            return;
        }

        TextMeshProUGUI text = targetCanvas.GetComponentInChildren<TextMeshProUGUI>();
        if (text == null)
        {
            return;
        }

        text.DOKill();
        text.text = "";

        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        int messageLength = message.Length;
        DOTween.To(
            () => 0f,
            value =>
            {
                int visibleLength = Mathf.Clamp(Mathf.FloorToInt(value), 0, messageLength);
                text.text = message.Substring(0, visibleLength);
            },
            messageLength,
            typingTime
        ).SetEase(Ease.Linear);
    }

    // 버튼 클릭 시 호출
    public void OnDialogueClicked()
    {
        if (currentState is TasteState tasteState)
        {
            tasteState.OnDialogueClicked(this);
            return;
        }

        NextDialogue();
    }

    // 다음 대사로 넘기기
    public void NextDialogue()
    {
        if (dialogueData != null && dialogueData.lines != null
            && dialogueData.lines.FirstLine != null
            && dialogueIndex < dialogueData.lines.FirstLine.Count)
        {
            string message = dialogueData.lines.FirstLine[dialogueIndex];
            message = InsertOrderDialogueText(message);
            SetDialogueCanvasActive(true, message);
            dialogueIndex++;
            return;
        }

        SetDialogueCanvasActive(false, null);

        // 손님이 착석 상태일 때만 씬 전환
        if (IsSeated() || IsWating())
        {
            state_Taste();
            SetOtherCustomersToWaiting();

            if (Game_Manager.Instance != null)
            {
                Game_Manager.Instance.ChangeScene(SceneNames.Cocktail);
            }

            dialogueIndex = 0;
        }
    }

    // 주문한 손님을 제외한 나머지 손님들을 Waiting 상태로 전환
    private void SetOtherCustomersToWaiting()
    {
        CustomerManager manager = FindObjectOfType<CustomerManager>();
        if (manager == null)
        {
            return;
        }

        if (manager.leftCustomer != null && manager.leftCustomer != this && manager.leftCustomer.IsSeated())
        {
            manager.leftCustomer.state_Waiting();
        }

        if (manager.middleCustomer != null && manager.middleCustomer != this && manager.middleCustomer.IsSeated())
        {
            manager.middleCustomer.state_Waiting();
        }

        if (manager.rightCustomer != null && manager.rightCustomer != this && manager.rightCustomer.IsSeated())
        {
            manager.rightCustomer.state_Waiting();
        }
    }

    // 대사에 주문한 음료 이름 삽입
    private string InsertOrderDialogueText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        string orderName = currentOrder.ToString();
        return text.Replace("{order}", orderName)
                   .Replace("{onOrder}", orderName);
    }
    #endregion

    #region 주문 기능
    private void InitializeDrinkOrder()
    {
        Recipe configuredOrder = Recipe.주문없음;
        if (dialogueData != null && dialogueData.lines != null)
        {
            configuredOrder = dialogueData.lines.onOrder;
        }

        if (RecipeBook.IsOrderableRecipe(configuredOrder))
        {
            currentOrder = configuredOrder;
            Debug.Log($"[Order] {gameObject.name}의 주문이 설정값으로 지정되었습니다. source=onOrder, recipe={currentOrder}");
            return;
        }

        currentOrder = RecipeBook.GetRandomOrderableRecipe();
        Debug.Log($"[Order] {gameObject.name}의 주문이 랜덤으로 설정되었습니다. source=random, recipe={currentOrder}");
    }

    public void SetRuntimeOrder(Recipe order)
    {
        currentOrder = order;
    }
    #endregion

    public int GetSeatIndex()
    {
        if (SeatIndex.IsValid(seatIndex))
        {
            return seatIndex;
        }

        CustomerManager manager = CustomerManager.Instance;
        if (manager == null)
        {
            return SeatIndex.Unknown;
        }

        if (manager.leftCustomer == this)
        {
            return SeatIndex.Left;
        }

        if (manager.middleCustomer == this)
        {
            return SeatIndex.Middle;
        }

        if (manager.rightCustomer == this)
        {
            return SeatIndex.Right;
        }

        return SeatIndex.Unknown;
    }

    private int GetDialogueCanvasIndex()
    {
        return GetSeatIndex();
    }
}
