using UnityEngine;
using UnityEngine.UI;

public class CustomerManager : MonoBehaviour
{
    public Customer leftCustomer;
    public Customer middleCustomer;
    public Customer rightCustomer;

    
    public Button seat_Left;
    public Button seat_Middle;
    public Button seat_Right;

    void Start()
    {
        GameObject dialogueParent = GameObject.Find("Canvas/Canvas_Dialogue");
        //예외처리
        if(seat_Left == null) seat_Left = dialogueParent.transform.Find("seat_Left").GetComponent<Button>();
        if (seat_Middle == null) seat_Middle = dialogueParent.transform.Find("seat_Middle").GetComponent<Button>();
        if (seat_Right == null) seat_Right = dialogueParent.transform.Find("seat_Right").GetComponent<Button>();

        seat_Left.onClick.AddListener(() => leftCustomer.NextDialogue());
        seat_Middle.onClick.AddListener(() => middleCustomer.NextDialogue());
        seat_Right.onClick.AddListener(() => rightCustomer.NextDialogue());
    }
}