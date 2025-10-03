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
    public void Enter(Customer customer)
    {
        if (customer.animator != null)
        {
            customer.animator.SetBool("OrderState", true);
        }

        string message_Correct = customer.dialogueData.lines.onCorrectDrink;
        string message_Wrong = customer.dialogueData.lines.onWrongDrink;
        customer.SetDialogueCanvasActive(true, message_Correct); //대기 상태에서는 대사 없음
    }

    public void Update(Customer customer)
    {
    }

    public void Exit(Customer customer)
    {
        customer.animator.SetBool("OrderState", false);
        customer.SetDialogueCanvasActive(false, null);
    }
}
public class ExitState : ICustomerState
{
    public void Enter(Customer customer)
    {
        if (customer.animator != null)
        {
            customer.animator.SetTrigger("CustomerExit");
        }
        customer.SetDialogueCanvasActive(true, null);
    }

    public void Update(Customer customer)
    {
        
    }

    public void Exit(Customer customer)
    {
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