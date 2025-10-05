public interface ICustomerState
{
    void Enter(Customer customer);
    void Update(Customer customer);
    void Exit(Customer customer);
}

public class SeatedState : ICustomerState
{
    public void Enter(Customer customer)
    {
        customer.SetDialogueCanvasActive(true, "...");
    }

    public void Update(Customer customer)
    {
        
    }

    public void Exit(Customer customer)
    {
        customer.animator.SetBool("SpawnState", false);
        customer.SetDialogueCanvasActive(false, null);
    }
}
public class TasteState : ICustomerState
{
    string message_Correct;
    string message_Wrong;
    private bool hasVerified = false; // 음료 먹었는지 확인여부
    private bool isWaitingForExit = false; // 검증 완료 후 퇴장 대기 상태

    public void Enter(Customer customer)
    {
        if (customer.animator != null)
        {
            customer.animator.SetBool("OrderState", true);
        }
        CustomerManager.Instance.DisableOtherButtons(customer); //다른 손님 버튼 비활성화

        message_Correct = customer.dialogueData.lines.onCorrectDrink;
        message_Wrong = customer.dialogueData.lines.onWrongDrink;

        string message_Waiting = customer.dialogueData.lines.onTasteDrink; //대기상태 대사
        customer.SetDialogueCanvasActive(true, message_Waiting);

        hasVerified = false;
        isWaitingForExit = false;
    }

    public void Update(Customer customer)
    {
        
    }

    public void Exit(Customer customer)
    {
        customer.animator.SetBool("OrderState", false);
        customer.SetDialogueCanvasActive(false, null);
    }


    #region 음료 검증 기능
    // 클릭 이벤트를 통해 호출되는 음료 검증 메서드
    public void OnDialogueClicked(Customer customer)
    {
        if (!hasVerified)
        {
            VerifyDrink(customer, message_Correct, message_Wrong);
            hasVerified = true;
            isWaitingForExit = true;
        }
        else if (isWaitingForExit)
        {
            // 검증 후 퇴장 상태로 전환
            customer.state_Exit();
            isWaitingForExit = false;
        }
    }

    private void VerifyDrink(Customer customer, string message_Correct, string message_Wrong)
    {
        // 고객의 좌석 인덱스 계산 (x 좌표 기반)
        int seatIndex = (int)customer.transform.position.x switch
        {
            1 => 0,   // 우석
            0 => 1,   // 중석
            -1 => 2,  // 좌석
            _ => -1   // 잘못된 위치
        };

        if (seatIndex == -1)
        {
            UnityEngine.Debug.LogError($"고객 {customer.gameObject.name}의 좌석 위치를 확인할 수 없습니다.");
            customer.SetDialogueCanvasActive(true, message_Wrong);
            return;
        }

        // CustomerData에서 주문한 음료 가져오기
        Recipe orderedDrink = CustomerData.Instance.GetOrderedDrink(seatIndex);

        // CocktailData에서 마지막으로 제작한 음료 가져오기
        CocktailData.CocktailInfo lastCocktail = CocktailData.Instance.GetLastCocktail();

        // 음료 검증
        bool isCorrect = false;

        if (lastCocktail != null && lastCocktail.isSuccessful)
        {
            isCorrect = (lastCocktail.cocktailRecipe == orderedDrink);
        }

        // 검증 결과에 따른 대사 출력
        if (isCorrect)
        {
            customer.SetDialogueCanvasActive(true, message_Correct);
            UnityEngine.Debug.Log($"고객 {customer.gameObject.name}: 주문한 음료와 일치! ({orderedDrink})");
        }
        else
        {
            customer.SetDialogueCanvasActive(true, message_Wrong);
            string lastDrinkName = lastCocktail?.cocktailRecipe.ToString() ?? "없음";
            UnityEngine.Debug.Log($"고객 {customer.gameObject.name}: 주문한 음료와 불일치! 주문: {orderedDrink}, 제작: {lastDrinkName}");
        }
    }
    #endregion

}
public class ExitState : ICustomerState
{
    public void Enter(Customer customer)
    {
        if (customer.animator != null)
        {
            customer.animator.SetBool("CustomerExit", true);
        }
        //customer.SetDialogueCanvasActive(true, null); //퇴장 대사 필요하면 사용
    }

    public void Update(Customer customer)
    {
        
    }

    public void Exit(Customer customer)
    {
        customer.animator.SetBool("CustomerExit", false);
        CustomerManager.Instance.EndDialogue();//나머지 손님 Dialogue 버튼 활성화
        Customer_Spawner spawner = Customer_Spawner.FindObjectOfType<Customer_Spawner>();
        if (spawner != null && customer.prefabIndex>0)
        {
            spawner.ReleasePrefabIndex(customer.prefabIndex);

            // 해당 손님의 좌석 상태도 초기화
            if (CustomerManager.Instance.leftCustomer == customer)
            {
                spawner.seat_Left = false;
                CustomerManager.Instance.leftCustomer = null;
            }
            else if (CustomerManager.Instance.middleCustomer == customer)
            {
                spawner.seat_Middle = false;
                CustomerManager.Instance.middleCustomer = null;
            }
            else if (CustomerManager.Instance.rightCustomer == customer)
            {
                spawner.seat_Right = false;
                CustomerManager.Instance.rightCustomer = null;
            }
        }
    }
}