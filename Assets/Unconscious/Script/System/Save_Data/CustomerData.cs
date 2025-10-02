using UnityEngine;

public class CustomerData : MonoBehaviour
{
    private static CustomerData instance;
    public static CustomerData Instance
    {
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
        public int seatIndex;       // 0: Left, 1: Middle, 2: Right
        public Recipe orderedDrink; // 고객이 주문한 음료
        public bool hasOrdered;     // 주문했는지 여부
    }

    [Header("Seat Data")]
    public bool[] seatStates = new bool[3];      // Left, Middle, Right 좌석 점유 상태
    public string[] customerNames = new string[3]; // 각 좌석에 앉은 고객 이름
    public Recipe[] orderedDrinks = new Recipe[3]; // 각 좌석의 고객이 주문한 음료
    public bool[] hasOrderedFlags = new bool[3];  // 각 좌석의 고객이 주문했는지 여부

    [Header("Customer State Data")]
    public string[] customerStates = new string[3]; // 각 좌석 고객의 현재 상태 (SeatedState, TasteState, ExitState)
    public int[] dialogueIndices = new int[3];      // 각 좌석 고객의 대화 인덱스

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
        // Customer_Spawner에서 좌석 상태 가져오기
        Customer_Spawner spawner = FindObjectOfType<Customer_Spawner>();
        if (spawner != null)
        {
            seatStates[0] = spawner.seat_Left;
            seatStates[1] = spawner.seat_Middle;
            seatStates[2] = spawner.seat_Right;
        }

        // CustomerManager에서 각 좌석의 고객 이름 가져오기
        CustomerManager customerManager = CustomerManager.Instance;
        if (customerManager != null)
        {
            customerNames[0] = customerManager.leftCustomer != null ? customerManager.leftCustomer.gameObject.name : "";
            customerNames[1] = customerManager.middleCustomer != null ? customerManager.middleCustomer.gameObject.name : "";
            customerNames[2] = customerManager.rightCustomer != null ? customerManager.rightCustomer.gameObject.name : "";
        }
    }

    /// <summary>
    /// 저장된 좌석 상태를 복원
    /// </summary>
    public void RestoreSeatData()
    {
        // Customer_Spawner에 좌석 상태 복원
        Customer_Spawner spawner = FindObjectOfType<Customer_Spawner>();
        if (spawner != null)
        {
            spawner.seat_Left = seatStates[0];
            spawner.seat_Middle = seatStates[1];
            spawner.seat_Right = seatStates[2];
        }

        RestoreCustomerReferences();
    }
    private void RestoreCustomerReferences()
    {
        CustomerManager customerManager = CustomerManager.Instance;
        if (customerManager == null) return;

        // 씬에서 고객들을 찾아서 이름으로 매칭
        Customer[] customers = FindObjectsOfType<Customer>();
        
        foreach (Customer customer in customers)
        {
            // 저장된 고객 이름과 매칭하여 적절한 좌석에 할당
            for (int i = 0; i < customerNames.Length; i++)
            {
                if (customer.gameObject.name == customerNames[i])
                {
                    switch (i)
                    {
                        case 0: // Left seat
                            customerManager.Set_LeftCustomer(customer);
                            break;
                        case 1: // Middle seat
                            customerManager.Set_MiddleCustomer(customer);
                            break;
                        case 2: // Right seat
                            customerManager.Set_RightCustomer(customer);
                            break;
                    }
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 고객의 주문 음료 정보를 저장
    /// </summary>
    public void SaveOrderDrink()
    {
        CustomerManager customerManager = CustomerManager.Instance;
        if (customerManager == null) return;

        // 각 좌석의 고객 주문 정보 저장
        SaveCustomerOrder(customerManager.leftCustomer, 0);
        SaveCustomerOrder(customerManager.middleCustomer, 1);
        SaveCustomerOrder(customerManager.rightCustomer, 2);
    }

    /// <summary>
    /// 특정 고객의 주문 정보를 저장하는 헬퍼 메서드
    /// </summary>
    /// <param name="customer">고객</param>
    /// <param name="seatIndex">좌석 인덱스</param>
    private void SaveCustomerOrder(Customer customer, int seatIndex)
    {
        if (customer != null && customer.dialogueData != null && customer.dialogueData.lines != null)
        {
            orderedDrinks[seatIndex] = customer.dialogueData.lines.onOrder;
            hasOrderedFlags[seatIndex] = true;
            
            Debug.Log($"고객 {customer.gameObject.name}의 주문이 저장되었습니다: {customer.dialogueData.lines.onOrder}");
        }
        else
        {
            //orderedDrinks[seatIndex] = default(Recipe);
            orderedDrinks[seatIndex] = Recipe.주문없음;
            hasOrderedFlags[seatIndex] = false;
        }
    }

    /// <summary>
    /// 각 고객의 상태 정보를 저장
    /// </summary>
    private void SaveCustomerState()
    {
        CustomerManager customerManager = CustomerManager.Instance;
        if (customerManager == null) return;

        // 각 좌석의 고객 상태 정보 저장
        SaveIndividualCustomerState(customerManager.leftCustomer, 0);
        SaveIndividualCustomerState(customerManager.middleCustomer, 1);
        SaveIndividualCustomerState(customerManager.rightCustomer, 2);
    }

    /// <summary>
    /// 특정 고객의 상태 정보를 저장하는 헬퍼 메서드
    /// </summary>
    /// <param name="customer">고객</param>
    /// <param name="seatIndex">좌석 인덱스</param>
    private void SaveIndividualCustomerState(Customer customer, int seatIndex)
    {
        if (customer != null)
        {
            // 현재 상태를 문자열로 저장
            var currentStateField = typeof(Customer).GetField("currentState", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (currentStateField != null)
            {
                var currentState = currentStateField.GetValue(customer);
                customerStates[seatIndex] = currentState?.GetType().Name ?? "SeatedState";
            }
            else
            {
                customerStates[seatIndex] = "SeatedState"; // 기본값
            }

            // 대화 인덱스 저장
            dialogueIndices[seatIndex] = customer.dialogueIndex;
            
            Debug.Log($"고객 {customer.gameObject.name}의 상태가 저장되었습니다: {customerStates[seatIndex]}, 대화 인덱스: {dialogueIndices[seatIndex]}");
        }
        else
        {
            // 고객이 없는 경우 초기화
            customerStates[seatIndex] = "";
            dialogueIndices[seatIndex] = 0;
        }
    }

    /// <summary>
    /// 저장된 고객 상태를 복원
    /// </summary>
    public void RestoreCustomerState()
    {
        CustomerManager customerManager = CustomerManager.Instance;
        if (customerManager == null) return;

        // 각 좌석의 고객 상태 복원
        RestoreIndividualCustomerState(customerManager.leftCustomer, 0);
        RestoreIndividualCustomerState(customerManager.middleCustomer, 1);
        RestoreIndividualCustomerState(customerManager.rightCustomer, 2);
    }

    /// <summary>
    /// 특정 고객의 상태를 복원하는 헬퍼 메서드
    /// </summary>
    /// <param name="customer">고객</param>
    /// <param name="seatIndex">좌석 인덱스</param>
    private void RestoreIndividualCustomerState(Customer customer, int seatIndex)
    {
        if (customer != null && !string.IsNullOrEmpty(customerStates[seatIndex]))
        {
            // 대화 인덱스 복원
            customer.dialogueIndex = dialogueIndices[seatIndex];

            // 상태 복원
            ICustomerState stateToRestore = null;
            switch (customerStates[seatIndex])
            {
                case "SeatedState":
                    stateToRestore = new SeatedState();
                    break;
                case "TasteState":
                    stateToRestore = new TasteState();
                    break;
                case "ExitState":
                    stateToRestore = new ExitState();
                    break;
                default:
                    stateToRestore = new SeatedState(); // 기본값
                    break;
            }

            if (stateToRestore != null)
            {
                customer.ChangeState(stateToRestore);
            }

            Debug.Log($"고객 {customer.gameObject.name}의 상태가 복원되었습니다: {customerStates[seatIndex]}, 대화 인덱스: {dialogueIndices[seatIndex]}");
        }
    }

    /// <summary>
    /// 특정 좌석의 고객이 주문한 음료 반환
    /// </summary>
    /// <param name="seatIndex">0: Left, 1: Middle, 2: Right</param>
    /// <returns>주문한 음료</returns>
    public Recipe GetOrderedDrink(int seatIndex)
    {
        if (seatIndex >= 0 && seatIndex < 3)
            return orderedDrinks[seatIndex];
        return default(Recipe);
    }

    /// <summary>
    /// 특정 좌석의 고객이 주문했는지 확인
    /// </summary>
    /// <param name="seatIndex">0: Left, 1: Middle, 2: Right</param>
    /// <returns>주문했으면 true</returns>
    public bool HasOrdered(int seatIndex)
    {
        if (seatIndex >= 0 && seatIndex < 3)
            return hasOrderedFlags[seatIndex];
        return false;
    }

    #region 씬 전환 시 호출할 메서드

    /// <summary>
    /// 씬 전환 전 호출 - 좌석 데이터 저장
    /// </summary>
    public void OnSceneChanging()
    {
        SaveSeatData();
        SaveOrderDrink(); // 주문 정보도 함께 저장
        SaveCustomerState(); // 고객 상태 정보 저장
    }

    /// <summary>
    /// 새 씬 로드 후 호출 - 좌석 데이터 복원
    /// </summary>
    public void OnSceneLoaded()
    {
        RestoreSeatData();
        RestoreCustomerState(); // 고객 상태 정보 복원
    }

    #endregion
}
