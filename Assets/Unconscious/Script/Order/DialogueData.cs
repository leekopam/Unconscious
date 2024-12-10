using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class CustomerDialogue
    {
        public int customerType;
        public List<string> dialogueLines;
        public Recipe requestedRecipe; // 요구하는 레시피
        public List<string> positiveReactions; // 성공 시 대사
        public List<string> negativeReactions; // 실패 시 대사
        public bool isOrderCompleted = false; // 주문 완료 상태 추가
    }

    public List<CustomerDialogue> customerDialogues;

    public CustomerDialogue GetDialogueForCustomerType(int customerType)
    {
        return customerDialogues.Find(d => d.customerType == customerType);
    }

    public List<string> GetDialogueLines(int customerType)
    {
        CustomerDialogue dialogue = GetDialogueForCustomerType(customerType);

        // 대화 라인이 없는 경우 기본 대화 추가
        if (dialogue == null || dialogue.dialogueLines == null || dialogue.dialogueLines.Count == 0)
        {
            Debug.LogWarning($"Customer Type {customerType}에 대한 대화 데이터가 없습니다.");
            return new List<string> { "안녕하세요", "주문하고 싶어요" };
        }

        return dialogue.dialogueLines;
    }

    public void UpdateOrderStatus(int customerType, bool completed)
    {
        var dialogue = GetDialogueForCustomerType(customerType);
        if (dialogue != null)
        {
            dialogue.isOrderCompleted = completed;
        }
    }
}