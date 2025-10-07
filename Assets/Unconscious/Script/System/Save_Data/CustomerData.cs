using UnityEngine;

public static class SeatIndex
{
    public const int Left = 0;
    public const int Middle = 1;
    public const int Right = 2;
    public const int Count = 3;
    public const int Unknown = -1;

    public static bool IsValid(int index)
    {
        return index >= 0 && index < Count;
    }
}

public class CustomerData : MonoBehaviour
{
    private static CustomerData instance;

    public static CustomerData Instance
    {
        // MonoBehaviour 특성상 DontDestroyOnLoad를 사용해 씬 전환 시에도 데이터 유지
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CustomerData>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("CustomerData");
                    instance = obj.AddComponent<CustomerData>();
                    DontDestroyOnLoad(obj);
                }
            }

            return instance;
        }
    }

    // 좌석별 고객 정보만 저장
    [System.Serializable]
    public class SeatCustomerInfo
    {
        public string customerName;  // 어떤 고객인지 식별
        public int seatIndex;        // 0: Left, 1: Middle, 2: Right
        public Recipe orderedDrink;  // 고객이 주문한 음료
        public bool hasOrdered;      // 주문했는지 여부
    }

    [Header("Seat Data")]
    public bool[] seatStates = new bool[SeatIndex.Count];
    public string[] customerNames = new string[SeatIndex.Count];
    public Recipe[] orderedDrinks = new Recipe[SeatIndex.Count];
    public bool[] hasOrderedFlags = new bool[SeatIndex.Count];

    [Header("Customer State Data")]
    public string[] customerStates = new string[SeatIndex.Count];
    public int[] dialogueIndices = new int[SeatIndex.Count];

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 현재 좌석 상태를 저장
    /// </summary>
    public void SaveSeatData()
    {
        Customer_Spawner spawner = FindObjectOfType<Customer_Spawner>();
        if (spawner != null)
        {
            seatStates[SeatIndex.Left] = spawner.seat_Left;
            seatStates[SeatIndex.Middle] = spawner.seat_Middle;
            seatStates[SeatIndex.Right] = spawner.seat_Right;
        }

        CustomerManager customerManager = FindObjectOfType<CustomerManager>();
        if (customerManager != null)
        {
            customerNames[SeatIndex.Left] = customerManager.leftCustomer != null ? customerManager.leftCustomer.gameObject.name : string.Empty;
            customerNames[SeatIndex.Middle] = customerManager.middleCustomer != null ? customerManager.middleCustomer.gameObject.name : string.Empty;
            customerNames[SeatIndex.Right] = customerManager.rightCustomer != null ? customerManager.rightCustomer.gameObject.name : string.Empty;
        }
    }

    /// <summary>
    /// 저장된 좌석 상태를 복원
    /// </summary>
    public void RestoreSeatData()
    {
        Customer_Spawner spawner = FindObjectOfType<Customer_Spawner>();
        if (spawner != null)
        {
            spawner.seat_Left = seatStates[SeatIndex.Left];
            spawner.seat_Middle = seatStates[SeatIndex.Middle];
            spawner.seat_Right = seatStates[SeatIndex.Right];
        }

        RestoreCustomerReferences();
    }

    private void RestoreCustomerReferences()
    {
        CustomerManager customerManager = FindObjectOfType<CustomerManager>();
        if (customerManager == null)
        {
            return;
        }

        Customer[] customers = FindObjectsOfType<Customer>();
        foreach (Customer customer in customers)
        {
            for (int i = 0; i < customerNames.Length; i++)
            {
                if (customer.gameObject.name != customerNames[i])
                {
                    continue;
                }

                switch (i)
                {
                    case SeatIndex.Left:
                        customerManager.Set_LeftCustomer(customer);
                        break;
                    case SeatIndex.Middle:
                        customerManager.Set_MiddleCustomer(customer);
                        break;
                    case SeatIndex.Right:
                        customerManager.Set_RightCustomer(customer);
                        break;
                }

                break;
            }
        }
    }

    /// <summary>
    /// 고객의 주문 음료 정보를 저장
    /// </summary>
    public void SaveOrderDrink()
    {
        CustomerManager customerManager = FindObjectOfType<CustomerManager>();
        if (customerManager == null)
        {
            return;
        }

        SaveCustomerOrder(customerManager.leftCustomer, SeatIndex.Left);
        SaveCustomerOrder(customerManager.middleCustomer, SeatIndex.Middle);
        SaveCustomerOrder(customerManager.rightCustomer, SeatIndex.Right);
    }

    private void SaveCustomerOrder(Customer customer, int seatIndex)
    {
        if (!SeatIndex.IsValid(seatIndex))
        {
            return;
        }

        if (customer != null)
        {
            orderedDrinks[seatIndex] = customer.CurrentOrder;
            hasOrderedFlags[seatIndex] = customer.CurrentOrder != Recipe.주문없음;

            Debug.Log($"[Order] 고객 {customer.gameObject.name}의 주문 저장: {customer.CurrentOrder} (seat={seatIndex})");
            return;
        }

        orderedDrinks[seatIndex] = Recipe.주문없음;
        hasOrderedFlags[seatIndex] = false;
    }

    /// <summary>
    /// 저장된 주문 데이터를 고객 컴포넌트에 복원
    /// </summary>
    public void RestoreOrderDrink()
    {
        CustomerManager customerManager = FindObjectOfType<CustomerManager>();
        if (customerManager == null)
        {
            return;
        }

        RestoreSingleOrder(customerManager.leftCustomer, SeatIndex.Left);
        RestoreSingleOrder(customerManager.middleCustomer, SeatIndex.Middle);
        RestoreSingleOrder(customerManager.rightCustomer, SeatIndex.Right);
    }

    private void RestoreSingleOrder(Customer customer, int seatIndex)
    {
        if (customer == null || !SeatIndex.IsValid(seatIndex))
        {
            return;
        }

        if (!hasOrderedFlags[seatIndex])
        {
            customer.SetRuntimeOrder(Recipe.주문없음);
            return;
        }

        customer.SetRuntimeOrder(orderedDrinks[seatIndex]);
    }

    /// <summary>
    /// 각 고객의 상태 정보를 저장
    /// </summary>
    private void SaveCustomerState()
    {
        CustomerManager customerManager = FindObjectOfType<CustomerManager>();
        if (customerManager == null)
        {
            return;
        }

        SaveIndividualCustomerState(customerManager.leftCustomer, SeatIndex.Left);
        SaveIndividualCustomerState(customerManager.middleCustomer, SeatIndex.Middle);
        SaveIndividualCustomerState(customerManager.rightCustomer, SeatIndex.Right);
    }

    private void SaveIndividualCustomerState(Customer customer, int seatIndex)
    {
        if (!SeatIndex.IsValid(seatIndex))
        {
            return;
        }

        if (customer != null)
        {
            customerStates[seatIndex] = customer.GetCurrentStateName();
            dialogueIndices[seatIndex] = customer.dialogueIndex;

            Debug.Log($"[Order] 고객 {customer.gameObject.name} 상태 저장: {customerStates[seatIndex]}, 대화 인덱스: {dialogueIndices[seatIndex]}");
            return;
        }

        customerStates[seatIndex] = string.Empty;
        dialogueIndices[seatIndex] = 0;
    }

    /// <summary>
    /// 저장된 고객 상태를 복원
    /// </summary>
    public void RestoreCustomerState()
    {
        CustomerManager customerManager = FindObjectOfType<CustomerManager>();
        if (customerManager == null)
        {
            return;
        }

        RestoreIndividualCustomerState(customerManager.leftCustomer, SeatIndex.Left);
        RestoreIndividualCustomerState(customerManager.middleCustomer, SeatIndex.Middle);
        RestoreIndividualCustomerState(customerManager.rightCustomer, SeatIndex.Right);
    }

    private void RestoreIndividualCustomerState(Customer customer, int seatIndex)
    {
        if (customer == null || !SeatIndex.IsValid(seatIndex) || string.IsNullOrEmpty(customerStates[seatIndex]))
        {
            return;
        }

        // 대화 인덱스 복원
        customer.dialogueIndex = dialogueIndices[seatIndex];

        // 상태 복원
        ICustomerState stateToRestore;
        switch (customerStates[seatIndex])
        {
            case "SeatedState":
                stateToRestore = new SeatedState();
                break;
            case "WaitingState":
                stateToRestore = new WaitingState();
                break;
            case "TasteState":
                stateToRestore = new TasteState();
                break;
            case "ExitState":
                stateToRestore = new ExitState();
                break;
            default:
                stateToRestore = new SeatedState();
                break;
        }

        customer.ChangeState(stateToRestore);
        Debug.Log($"[Order] 고객 {customer.gameObject.name} 상태 복원: {customerStates[seatIndex]}, 대화 인덱스: {dialogueIndices[seatIndex]}");
    }

    /// <summary>
    /// 특정 좌석의 고객이 주문한 음료 반환
    /// </summary>
    public Recipe GetOrderedDrink(int seatIndex)
    {
        if (SeatIndex.IsValid(seatIndex))
        {
            return orderedDrinks[seatIndex];
        }

        return Recipe.주문없음;
    }

    /// <summary>
    /// 특정 좌석의 고객이 주문했는지 확인
    /// </summary>
    public bool HasOrdered(int seatIndex)
    {
        if (SeatIndex.IsValid(seatIndex))
        {
            return hasOrderedFlags[seatIndex];
        }

        return false;
    }

    #region 씬 전환 시 호출할 메서드
    /// <summary>
    /// 씬 전환 전 호출 - 좌석 데이터 저장
    /// </summary>
    public void OnSceneChanging()
    {
        SaveSeatData();
        SaveOrderDrink();
        SaveCustomerState();
    }

    /// <summary>
    /// 새 씬 로드 후 호출 - 좌석 데이터 복원
    /// </summary>
    public void OnSceneLoaded()
    {
        RestoreSeatData();
        RestoreOrderDrink();
        RestoreCustomerState();
    }
    #endregion
}

