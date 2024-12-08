using System.Collections.Generic;
using UnityEngine;

public class CustomerDialogue : MonoBehaviour
{
    private int customerType;
    private List<string> dialogueLines;

    public void Initialize(int type, List<string> lines = null)
    {
        customerType = type;
        dialogueLines = lines ?? new List<string>();
    }

    public int GetCustomerType()
    {
        return customerType;
    }

    public List<string> GetDialogueLines()
    {
        return dialogueLines;
    }
}