using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomerStateData", menuName = "Game/CustomerStateData")]
public class CustomerStateData : ScriptableObject
{
    [Serializable]
    public class CustomerState
    {
        public int customerType;
        public Vector3 position;
        public bool isOrderCompleted;
        public Recipe requestedRecipe;
        public bool hasShownInitialDialogue; // 추가: 초기 대화 표시 여부
    }

    public List<CustomerState> savedCustomerStates = new List<CustomerState>();

    public void SaveCustomerState(GameObject customer, Recipe requestedRecipe)
    {
        ClearStates();

        CustomerState state = new CustomerState
        {
            customerType = customer.GetComponent<CustomerController>().CustomerType,
            position = customer.transform.position,
            requestedRecipe = requestedRecipe,
            isOrderCompleted = false,
            hasShownInitialDialogue = true // 초기 대화 표시 상태 저장
        };

        savedCustomerStates.Add(state);
    }

    public void ClearStates()
    {
        savedCustomerStates.Clear();
    }
}