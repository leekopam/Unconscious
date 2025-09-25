using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "CustomerDialogueData", menuName = "Dialogue/CustomerDialogueData")]
public class Customer_DialogueData : ScriptableObject
{
    public string customerName;
    public DialogueLine lines;
}

[System.Serializable]
public class DialogueLine
{
    public List<string> FirstLine; //첫(등장) 대사
    public Recipe onOrder; //주문하는 음료
    public string onCorrectDrink; //음료가 맞았을 때 출력대사
    public string onWrongDrink; //음료가 틀렸을 때 출력대사
}
