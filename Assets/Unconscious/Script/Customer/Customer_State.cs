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
        customer.animator.SetInteger("CustomerState", 1);
        string message = customer.dialogueData.lines.FirstLine[0]; //등장 대사 가져오기
        customer.SetDialogueCanvasActive(true, message);
    }

    public void Update(Customer customer)
    {
        
    }

    public void Exit(Customer customer)
    {
        customer.SetDialogueCanvasActive(false, null);
    }
}
public class TasteState : ICustomerState
{
    public void Enter(Customer customer)
    {
        string message_Correct = customer.dialogueData.lines.onCorrectDrink;
        string message_Wrong = customer.dialogueData.lines.onWrongDrink;
        customer.SetDialogueCanvasActive(true, null); //대기 상태에서는 대사 없음
    }

    public void Update(Customer customer)
    {
    }

    public void Exit(Customer customer)
    {
        customer.SetDialogueCanvasActive(false, null);
    }
}
public class ExitState : ICustomerState
{
    public void Enter(Customer customer)
    {
        customer.SetDialogueCanvasActive(true, null);
        customer.animator.SetTrigger("CustomerExit");
    }

    public void Update(Customer customer)
    {
        
    }

    public void Exit(Customer customer)
    {
        
    }
}