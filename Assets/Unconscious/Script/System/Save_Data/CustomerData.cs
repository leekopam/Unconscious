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
    }

    [Header("Seat Data")]
    public bool[] seatStates = new bool[3];      // Left, Middle, Right 좌석 점유 상태
    public string[] customerNames = new string[3]; // 각 좌석에 앉은 고객 이름

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

        // 필요시 CustomerManager 참조도 복원 가능
        RestoreCustomerReferences();
    }

    /// <summary>
    /// CustomerManager의 고객 참조 복원
    /// </summary>
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

    #region 씬 전환 시 호출할 메서드

    /// <summary>
    /// 씬 전환 전 호출 - 좌석 데이터 저장
    /// </summary>
    public void OnSceneChanging()
    {
        SaveSeatData();
    }

    /// <summary>
    /// 새 씬 로드 후 호출 - 좌석 데이터 복원
    /// </summary>
    public void OnSceneLoaded()
    {
        RestoreSeatData();
    }

    #endregion

    #region 퍼블릭 접근 메서드

    /// <summary>
    /// 특정 좌석에 고객이 앉아있는지 확인
    /// </summary>
    /// <param name="seatIndex">0: Left, 1: Middle, 2: Right</param>
    /// <returns></returns>
    public bool IsSeatOccupied(int seatIndex)
    {
        if (seatIndex >= 0 && seatIndex < 3)
            return seatStates[seatIndex];
        return false;
    }

    /// <summary>
    /// 특정 좌석의 고객 이름 반환
    /// </summary>
    /// <param name="seatIndex">0: Left, 1: Middle, 2: Right</param>
    /// <returns></returns>
    public string GetCustomerAtSeat(int seatIndex)
    {
        if (seatIndex >= 0 && seatIndex < 3)
            return customerNames[seatIndex];
        return "";
    }

    /// <summary>
    /// 좌석 데이터 수동 저장
    /// </summary>
    public void ManualSave()
    {
        SaveSeatData();
        Debug.Log("Seat data manually saved.");
    }

    /// <summary>
    /// 좌석 데이터 수동 복원
    /// </summary>
    public void ManualRestore()
    {
        RestoreSeatData();
        Debug.Log("Seat data manually restored.");
    }

    #endregion
}
