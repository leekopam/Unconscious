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
    }

    public List<CustomerDialogue> customerDialogues;

    public List<string> GetDialogueForCustomer(int customerType)
    {
        CustomerDialogue dialogue = customerDialogues.Find(d => d.customerType == customerType);
        return dialogue?.dialogueLines ?? new List<string>();
    }
}