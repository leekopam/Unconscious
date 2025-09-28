using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class Customer : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;
    public Customer_DialogueData dialogueData;
    public float typingTime = 1.0f; //타이핑 속도

    private ICustomerState currentState;
    private List<GameObject> dialogue_canvas= new List<GameObject>();
    public int dialogueIndex = 0; //대화 순서 인덱스
    
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
    public void state_Taste() => ChangeState(new TasteState()); //손님 대기(주문하고 다른씬 갔다가 돌아옴)
    public void state_Exit() => ChangeState(new ExitState());  //손님 퇴장
    public void ChangeState(ICustomerState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }
    #endregion

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
            Game_Manager.Instance.ChangeSecene("Cocktail");
        }
    }
}
