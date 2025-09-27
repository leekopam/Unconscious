using UnityEngine;
using System.Collections.Generic;

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

    [Header("Customer Canvas Settings")]
    public Canvas customerCanvasTemplate;
    
    // 저장할 고객 데이터 구조
    [System.Serializable]
    public class SavedCustomerInfo
    {
        public string customerName;
        public Vector3 position;
        public int dialogueIndex;
        public bool isActive;
        public int seatIndex; // 0: Left, 1: Middle, 2: Right
    }

    [Header("Saved Data")]
    public List<SavedCustomerInfo> savedCustomers = new List<SavedCustomerInfo>();
    public bool[] seatStates = new bool[3]; // Left, Middle, Right seat occupation

    private Canvas persistentCanvas;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePersistentCanvas();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    #region Canvas 지속성 관리
    
    /// <summary>
    /// 씬 전환에도 유지되는 Canvas 초기화
    /// </summary>
    private void InitializePersistentCanvas()
    {
        if (persistentCanvas == null)
        {
            // Canvas_Customer를 찾아서 지속 Canvas로 설정
            Canvas existingCanvas = FindCanvasCustomer();
            if (existingCanvas != null)
            {
                persistentCanvas = existingCanvas;
                DontDestroyOnLoad(persistentCanvas.gameObject);
            }
            else
            {
                CreatePersistentCanvas();
            }
        }
    }

    /// <summary>
    /// Canvas_Customer 찾기
    /// </summary>
    private Canvas FindCanvasCustomer()
    {
        GameObject canvasObj = GameObject.Find("Canvas/Canvas_Customer");
        if (canvasObj == null)
        {
            // Canvas_Customer가 직접 루트에 있는 경우
            canvasObj = GameObject.Find("Canvas_Customer");
        }
        
        return canvasObj?.GetComponent<Canvas>();
    }

    /// <summary>
    /// 지속적인 Canvas 생성
    /// </summary>
    private void CreatePersistentCanvas()
    {
        GameObject canvasObj = new GameObject("Canvas_Customer_Persistent");
        persistentCanvas = canvasObj.AddComponent<Canvas>();
        
        // Canvas 설정
        persistentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        persistentCanvas.sortingOrder = 10;
        
        // CanvasScaler 추가
        var canvasScaler = canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        
        // GraphicRaycaster 추가
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        DontDestroyOnLoad(canvasObj);
    }

    /// <summary>
    /// 현재 씬의 Canvas_Customer 데이터를 저장
    /// </summary>
    public void SaveCustomerCanvasData()
    {
        Canvas currentCanvas = FindCanvasCustomer();
        if (currentCanvas == null) return;

        // 기존 저장된 데이터 초기화
        savedCustomers.Clear();
        
        // Customer_Spawner에서 좌석 상태 가져오기
        Customer_Spawner spawner = FindObjectOfType<Customer_Spawner>();
        if (spawner != null)
        {
            seatStates[0] = spawner.seat_Left;
            seatStates[1] = spawner.seat_Middle;
            seatStates[2] = spawner.seat_Right;
        }

        // Canvas 내 고객 정보 저장
        Customer[] customers = currentCanvas.GetComponentsInChildren<Customer>();
        foreach (Customer customer in customers)
        {
            SavedCustomerInfo info = new SavedCustomerInfo();
            info.customerName = customer.gameObject.name;
            info.position = customer.transform.position;
            info.dialogueIndex = customer.dialogueIndex;
            info.isActive = customer.gameObject.activeInHierarchy;
            
            // 좌석 인덱스 결정 (위치 기반)
            Vector3 pos = customer.transform.position;
            if (pos.x <= -0.5f) info.seatIndex = 0;      // Left
            else if (pos.x >= 0.5f) info.seatIndex = 2;  // Right
            else info.seatIndex = 1;                     // Middle
            
            savedCustomers.Add(info);
        }

        // 현재 Canvas를 지속 Canvas로 이동
        if (currentCanvas != persistentCanvas && persistentCanvas != null)
        {
            // 현재 Canvas의 모든 자식을 지속 Canvas로 이동
            while (currentCanvas.transform.childCount > 0)
            {
                Transform child = currentCanvas.transform.GetChild(0);
                child.SetParent(persistentCanvas.transform, false);
            }
        }
    }

    /// <summary>
    /// 저장된 Canvas 데이터를 새 씬에서 복원
    /// </summary>
    public void RestoreCustomerCanvasData()
    {
        if (persistentCanvas == null)
        {
            InitializePersistentCanvas();
            return;
        }

        // Customer_Spawner에 좌석 상태 복원
        Customer_Spawner spawner = FindObjectOfType<Customer_Spawner>();
        if (spawner != null)
        {
            spawner.seat_Left = seatStates[0];
            spawner.seat_Middle = seatStates[1];
            spawner.seat_Right = seatStates[2];
            
            // Canvas_Customer 참조 업데이트
            spawner.Canvas_Customer = persistentCanvas;
        }

        // CustomerManager에 복원된 고객들 다시 할당
        RestoreCustomerManagerReferences();
    }

    /// <summary>
    /// CustomerManager의 고객 참조 복원
    /// </summary>
    private void RestoreCustomerManagerReferences()
    {
        CustomerManager customerManager = CustomerManager.Instance;
        if (customerManager == null || persistentCanvas == null) return;

        Customer[] customers = persistentCanvas.GetComponentsInChildren<Customer>();
        
        foreach (Customer customer in customers)
        {
            Vector3 pos = customer.transform.position;
            
            // 위치 기반으로 좌석 결정
            if (pos.x <= -0.5f) // Left seat
            {
                customerManager.Set_LeftCustomer(customer);
            }
            else if (pos.x >= 0.5f) // Right seat
            {
                customerManager.Set_RightCustomer(customer);
            }
            else // Middle seat
            {
                customerManager.Set_MiddleCustomer(customer);
            }
        }
    }
    
    #endregion

    #region 씬 전환 시 호출할 메서드

    /// <summary>
    /// 씬 전환 전 호출 - 데이터 저장
    /// </summary>
    public void OnSceneChanging()
    {
        SaveCustomerCanvasData();
    }

    /// <summary>
    /// 새 씬 로드 후 호출 - 데이터 복원
    /// </summary>
    public void OnSceneLoaded()
    {
        RestoreCustomerCanvasData();
    }

    #endregion

    #region 퍼블릭 접근 메서드

    /// <summary>
    /// 지속 Canvas 반환
    /// </summary>
    public Canvas GetPersistentCanvas()
    {
        if (persistentCanvas == null)
        {
            InitializePersistentCanvas();
        }
        return persistentCanvas;
    }

    /// <summary>
    /// 고객 데이터 수동 저장
    /// </summary>
    public void ManualSave()
    {
        SaveCustomerCanvasData();
        Debug.Log("Customer data manually saved.");
    }

    /// <summary>
    /// 고객 데이터 수동 복원
    /// </summary>
    public void ManualRestore()
    {
        RestoreCustomerCanvasData();
        Debug.Log("Customer data manually restored.");
    }

    #endregion
}
