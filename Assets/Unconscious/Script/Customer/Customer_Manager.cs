using UnityEngine;
using UnityEngine.UI;

public class CustomerManager : MonoBehaviour
{
    private static CustomerManager instance;
    public static CustomerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CustomerManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("CustomerManager");
                    instance = obj.AddComponent<CustomerManager>();
                }
            }
            return instance;
        }
    }

    public Customer leftCustomer;
    public Customer middleCustomer;
    public Customer rightCustomer;

    public Button seat_Left;
    public Button seat_Middle;
    public Button seat_Right;

    // 현재 대화 중인 손님을 추적하는 변수
    private Customer currentTalkingCustomer = null;

    private void Awake()
    {
        
    }

    void Start()
    {
        GameObject dialogueParent = GameObject.Find("Canvas/Canvas_Dialogue");
        //예외처리
        if(seat_Left == null) seat_Left = dialogueParent.transform.Find("seat_Left").GetComponent<Button>();
        if (seat_Middle == null) seat_Middle = dialogueParent.transform.Find("seat_Middle").GetComponent<Button>();
        if (seat_Right == null) seat_Right = dialogueParent.transform.Find("seat_Right").GetComponent<Button>();

        seat_Left.onClick.AddListener(() => StartDialogue(leftCustomer));
        seat_Middle.onClick.AddListener(() => StartDialogue(middleCustomer));
        seat_Right.onClick.AddListener(() => StartDialogue(rightCustomer));
    }

    #region Dialogue 버튼 제어 및 설정
    
    /// <summary>
    /// 대화를 시작하는 메서드. 다른 손님들의 버튼을 비활성화합니다.
    /// </summary>
    /// <param name="customer">대화를 시작할 손님</param>
    public void StartDialogue(Customer customer)
    {
        if (customer == null) return;

        // 이미 다른 손님이 대화 중이면 무시
        if (currentTalkingCustomer != null && currentTalkingCustomer != customer) 
            return;

        // 현재 대화 중인 손님 설정
        currentTalkingCustomer = customer;

        // 다른 버튼들 비활성화
        DisableOtherButtons(customer);

        // 대화 진행
        customer.NextDialogue();
    }

    /// <summary>
    /// 대화를 종료하고 모든 버튼을 다시 활성화합니다.
    /// </summary>
    public void EndDialogue()
    {
        currentTalkingCustomer = null;
        EnableAllButtons();
    }

    /// <summary>
    /// 특정 손님을 제외한 나머지 버튼들을 비활성화합니다.
    /// </summary>
    /// <param name="activeCustomer">활성화 상태를 유지할 손님</param>
    private void DisableOtherButtons(Customer activeCustomer)
    {
        if (activeCustomer != leftCustomer && leftCustomer != null)
            seat_Left.interactable = false;
        
        if (activeCustomer != middleCustomer && middleCustomer != null)
            seat_Middle.interactable = false;
        
        if (activeCustomer != rightCustomer && rightCustomer != null)
            seat_Right.interactable = false;
    }

    /// <summary>
    /// 모든 버튼을 활성화합니다.
    /// </summary>
    private void EnableAllButtons()
    {
        if (leftCustomer != null) seat_Left.interactable = true;
        if (middleCustomer != null) seat_Middle.interactable = true;
        if (rightCustomer != null) seat_Right.interactable = true;
    }

    /// <summary>
    /// 현재 대화 중인 손님을 반환합니다.
    /// </summary>
    public Customer GetCurrentTalkingCustomer()
    {
        return currentTalkingCustomer;
    }

    /// <summary>
    /// 특정 손님이 현재 대화 중인지 확인합니다.
    /// </summary>
    /// <param name="customer">확인할 손님</param>
    /// <returns>대화 중이면 true, 아니면 false</returns>
    public bool IsCustomerTalking(Customer customer)
    {
        return currentTalkingCustomer == customer;
    }

    public void Set_LeftCustomer(Customer customer) { leftCustomer = customer.GetComponent<Customer>(); }
    public void Set_MiddleCustomer(Customer customer) { middleCustomer = customer.GetComponent<Customer>(); }
        
    public void Set_RightCustomer(Customer customer) { rightCustomer = customer.GetComponent<Customer>(); }
    #endregion
}