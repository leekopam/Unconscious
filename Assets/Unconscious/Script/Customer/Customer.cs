using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class Customer : MonoBehaviour
{
    [HideInInspector] public Animator animator;
    public Customer_DialogueData dialogueData;
    public float typingTime = 1.0f; //타이핑 속도

    [HideInInspector] public int prefabIndex = -1; //손님 프리팹 인덱스

    private ICustomerState currentState;
    private List<GameObject> dialogue_canvas= new List<GameObject>();
    [HideInInspector] public int dialogueIndex = 0; //대화 순서 인덱스

    #region 상태 확인 메서드들
    public bool IsSeated() => currentState is SeatedState;
    public bool IsWating() => currentState is WaitingState; //주문은 받지 않았지만 생성은 된 상태
    public bool IsTasting() => currentState is TasteState;
    public bool IsExiting() => currentState is ExitState;
    public string GetCurrentStateName() => currentState?.GetType().Name ?? "None";
    #endregion
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        animator = GetComponent<Animator>();

        GameObject dialogueParent = GameObject.Find("Canvas/Canvas_Dialogue");
        if(dialogueParent != null)
        {
            foreach(Transform child in dialogueParent.transform)
            {
                dialogue_canvas.Add(child.gameObject);
            }
        }
    }
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        currentState?.Update(this);
    }

    #region 상태전환 기능
    // 외부에서 상태 전환을 호출하는 예시 메서드
    public void state_Sitting() => ChangeState(new SeatedState()); //손님 착석
    public void state_Waiting() => ChangeState(new WaitingState()); //손님 대기(주문하고 다른씬 갔다가 돌아옴)
    public void state_Taste() => ChangeState(new TasteState()); // 주문받은 손님 대기상태 및 음료 검증
    public void state_Exit() => ChangeState(new ExitState());  //손님 퇴장
    public void ChangeState(ICustomerState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }
    #endregion
    #region 대화 기능
    
    // 대화 캔버스 활성화/비활성화 메서드
    public void SetDialogueCanvasActive(bool active, string message )
    {
        // 손님 x 좌표가 -1, 0, 1 일때 각각 2(좌), 1(중), 0(우) 대화창을 활성화
        int customer_xPos = (int)transform.position.x switch
        {   
            1 => 0,
            0 => 1,
            -1 => 2,
            _ => -1 // 범위 밖일 때는 -1로 설정
        };
        if (customer_xPos >= 0 && customer_xPos < dialogue_canvas.Count) 
        { 
            dialogue_canvas[customer_xPos].SetActive(active); //대화창 활성화/비활성화
            if (active)
            {
                TextMeshProUGUI text = dialogue_canvas[customer_xPos].GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (text != null)
                {
                    text.DOKill(); //이전 애니메이션이 있으면 종료
                    text.text = "";
                    text.DOText(message, typingTime).SetEase(Ease.Linear);//대화창에 대사 출력
                }
            }
        }
    }
    // 다음 대사로 넘기기  
    public void NextDialogue()
    {
        if (dialogueIndex > dialogueData.lines.FirstLine.Count) return; //대사 인덱스가 범위 밖이면 리턴

        if (dialogueData != null && dialogueData.lines != null
            && dialogueIndex < dialogueData.lines.FirstLine.Count)
        {
            string message = dialogueData.lines.FirstLine[dialogueIndex];
            SetDialogueCanvasActive(true, message);
            dialogueIndex++;// 다음 대사로 인덱스 증가
        }
        else
        {
            SetDialogueCanvasActive(false, null); //대화 끝나면 대화창 비활성화
            // 손님이 착석 상태일 때만 씬 전환
            if (IsSeated() || IsWating())
            {
                state_Taste(); //손님 대기 상태로 전환
                SetOtherCustomersToWaiting();
                Game_Manager.Instance.ChangeSecene("Cocktail");
                dialogueIndex = 0;
            }
        }
    }
    // 주문한 손님을 제외한 나머지 손님들을 Waiting 상태로 전환
    private void SetOtherCustomersToWaiting()
    {
        CustomerManager manager = CustomerManager.Instance;
        if (manager == null) return;

        // 좌석의 손님들을 확인하고 현재 손님이 아니면 Waiting 상태로 전환
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
    #endregion
}
