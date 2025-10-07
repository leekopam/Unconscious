using UnityEngine;

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
    }
}

// 손님 대기 상태 (생성될때 애니메이션 사용하지 않고 바로 자리에 있는 상태로 유지)
public class WaitingState : ICustomerState
{
    public void Enter(Customer customer)
    {
        if (customer.animator != null)
        {
            customer.animator.SetBool("WaitingState", true);
        }

        customer.SetDialogueCanvasActive(true, "...");
    }

    public void Update(Customer customer)
    {
    }

    public void Exit(Customer customer)
    {
        customer.animator.SetBool("WaitingState", false);
        customer.SetDialogueCanvasActive(false, null);
    }
}

public class TasteState : ICustomerState
{
    private string message_Correct;
    private string message_Wrong;
    private bool hasVerified = false;      // 음료 확인 여부
    private bool isWaitingForExit = false; // 검증 완료 후 퇴장 대기 상태
    public int Reward_Gold = 10;

    public void Enter(Customer customer)
    {
        if (customer.animator != null)
        {
            customer.animator.SetBool("OrderState", true);
        }

        CustomerManager manager = Object.FindObjectOfType<CustomerManager>();
        if (manager != null)
        {
            manager.DisableOtherButtons(customer); // 다른 손님 버튼 비활성화
        }

        message_Correct = customer.dialogueData.lines.onCorrectDrink;
        message_Wrong = customer.dialogueData.lines.onWrongDrink;

        string message_Waiting = customer.dialogueData.lines.onTasteDrink;
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
        int seatIndex = customer.GetSeatIndex();
        if (!SeatIndex.IsValid(seatIndex))
        {
            Debug.LogError($"[Order] 고객 {customer.gameObject.name}의 좌석 인덱스를 확인할 수 없습니다.");
            customer.SetDialogueCanvasActive(true, message_Wrong);
            return;
        }

        if (!CustomerData.Instance.HasOrdered(seatIndex))
        {
            Debug.LogWarning($"[Order] 고객 {customer.gameObject.name}의 주문 데이터가 없습니다. seatIndex={seatIndex}");
            customer.SetDialogueCanvasActive(true, message_Wrong);
            return;
        }

        // CustomerData에서 주문한 음료 가져오기
        Recipe orderedDrink = CustomerData.Instance.GetOrderedDrink(seatIndex);

        // CocktailData에서 마지막으로 제작한 음료 가져오기
        CocktailData.CocktailInfo lastCocktail = CocktailData.Instance.GetLastCocktail();

        bool isCorrect = lastCocktail != null
                         && lastCocktail.isSuccessful
                         && lastCocktail.cocktailRecipe == orderedDrink;

        if (isCorrect)
        {
            customer.SetDialogueCanvasActive(true, message_Correct);
            Debug.Log($"[Order] 고객 {customer.gameObject.name}: 주문과 일치 ({orderedDrink})");
            RewardData.Instance.AddGold(Reward_Gold);
            return;
        }

        customer.SetDialogueCanvasActive(true, message_Wrong);
        string lastDrinkName = lastCocktail?.cocktailRecipe.ToString() ?? "없음";
        Debug.Log($"[Order] 고객 {customer.gameObject.name}: 주문 불일치. 주문: {orderedDrink}, 제작: {lastDrinkName}");
    }
    #endregion
}

public class ExitState : ICustomerState
{
    private bool hasExecuted = false; // 퇴장 로직이 한 번만 실행되도록 확인하는 변수

    public void Enter(Customer customer)
    {
        if (customer.animator != null)
        {
            customer.animator.SetBool("CustomerExit", true);
        }

        hasExecuted = false;
    }

    public void Update(Customer customer)
    {
        if (!hasExecuted && customer.animator != null)
        {
            AnimatorStateInfo stateInfo = customer.animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Customer_Exit") && stateInfo.normalizedTime >= 0.95f)
            {
                ExecuteLogic(customer);
                hasExecuted = true;
            }
        }
    }

    public void Exit(Customer customer)
    {
        customer.animator.SetBool("CustomerExit", false);
    }

    private void ExecuteLogic(Customer customer)
    {
        CustomerManager.Instance.EndDialogue(); // 나머지 손님 Dialogue 버튼 활성화
        Customer_Spawner spawner = Customer_Spawner.FindObjectOfType<Customer_Spawner>();
        if (spawner != null && customer.prefabIndex >= 0)
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

            customer.seatIndex = SeatIndex.Unknown;

            // 볼일 끝난 손님 오브젝트 제거
            GameObject.Destroy(customer.gameObject);
        }
    }
}
