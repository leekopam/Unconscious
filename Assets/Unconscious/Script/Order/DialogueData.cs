using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class CustomerDialogue
    {
        public int customerType; // 고객 유형
        public List<string> dialogueLines; // 초기 대화 라인
        public List<string> middleDialogueLines; // 중간 대화 라인
        public List<string> clearDialogue; // 성공 대화 라인
        public List<string> faileDialogue; // 실패 대화 라인
        public Recipe recipe; // 레시피

        private enum DialogueState
        {
            Initial, // 초기 상태
            Middle, // 중간 상태
            Clear, // 성공 상태
            Fail // 실패 상태
        }

        private DialogueState currentState; // 현재 대화 상태

        // 초기화 메서드
        public void Initialize(int type, List<string> lines = null, List<string> middleLines = null, List<string> clearLines = null, List<string> failLines = null, Recipe recipe = default)
        {
            customerType = type;
            dialogueLines = lines ?? new List<string>();
            middleDialogueLines = middleLines ?? new List<string>();
            clearDialogue = clearLines ?? new List<string>();
            faileDialogue = failLines ?? new List<string>();
            this.recipe = recipe;
            currentState = DialogueState.Initial;
        }

        // 고객 유형 반환
        public int GetCustomerType()
        {
            return customerType;
        }

        // 현재 대화 라인 반환
        public List<string> GetDialogueLines()
        {
            switch (currentState)
            {
                case DialogueState.Middle:
                    return middleDialogueLines;
                case DialogueState.Clear:
                    return clearDialogue;
                case DialogueState.Fail:
                    return faileDialogue;
                default:
                    return dialogueLines;
            }
        }

        // 대화 상태를 중간으로 설정
        public void SetDialogueStateToMiddle()
        {
            currentState = DialogueState.Middle;
        }

        // 대화 상태를 성공으로 설정
        public void SetDialogueStateToClear()
        {
            currentState = DialogueState.Clear;
        }

        // 대화 상태를 실패로 설정
        public void SetDialogueStateToFail()
        {
            currentState = DialogueState.Fail;
        }
    }

    public List<CustomerDialogue> customerDialogues; // 고객 대화 목록

    // 특정 고객 유형에 대한 대화 라인 반환
    public List<string> GetDialogueForCustomer(int customerType)
    {
        CustomerDialogue dialogue = customerDialogues.Find(d => d.customerType == customerType);
        return dialogue?.dialogueLines ?? new List<string>();
    }
}
