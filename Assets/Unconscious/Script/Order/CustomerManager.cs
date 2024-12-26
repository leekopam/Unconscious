using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance { get; private set; }

    [SerializeField] private List<GameObject> customerPrefabs; // 고객 프리팹 목록
    [SerializeField] private Transform[] spawnPoints; // 고객 생성 위치
    [SerializeField] private Transform[] targetPoints; // 고객 이동 목표 위치
    [SerializeField] private float moveUpDuration = 1f; // 고객 이동 시간
    [SerializeField] private float orderTimeout = 10f; // 주문 제한 시간
    [SerializeField] private float spawnInterval = 5f; // 고객 생성 간격
    [SerializeField] private Canvas targetCanvas; // 고객을 생성할 캔버스
    [SerializeField] private DialogueData dialogueData; // 대화 데이터

    private List<Customer> currentCustomers = new List<Customer>(); // 현재 존재하는 고객 목록
    private HashSet<int> usedCustomerIndices = new HashSet<int>(); // 이미 사용된 고객 인덱스
    private int maxCustomers = 3; // 최대 고객 수
    private bool isDialogueActive = false; // 대화 활성화 여부
    private float currentOrderTimeout; // 현재 주문 제한 시간
    private float spawnTimer; // 고객 생성 타이머
    private bool isInitialized = false;

    public delegate void TimerUpdated(float timeRemaining);
    public event TimerUpdated OnTimerUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManager();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeManager()
    {
        if (!isInitialized)
        {
            LoadResources();
            isInitialized = true;
        }
    }

    private void LoadResources()
    {
        if (customerPrefabs == null || customerPrefabs.Count == 0)
        {
            // Resources 폴더에서 고객 프리팹 로드
            customerPrefabs = new List<GameObject>();
            GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/Customers");
            customerPrefabs.AddRange(prefabs);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (currentCustomers.Count > 0)
        {
            SaveCustomers();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Order")
        {
            StartCoroutine(SetupAfterSceneLoad());
        }
    }

    private IEnumerator SetupAfterSceneLoad()
    {
        yield return new WaitForSeconds(0.1f);
        SetupCanvas();
        FindPoints();

        string lastScene = PlayerPrefs.GetString("LastScene", "");
        if (lastScene == "Dessert")
        {
            ClearAllCustomers();
            yield return new WaitForSeconds(0.2f);
            LoadCustomers();
        }
    }

    private void SetupCanvas()
    {
        if (targetCanvas == null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
            if (targetCanvas == null)
            {
                Debug.LogWarning("Canvas를 찾을 수 없어 새로 생성합니다.");
                CreateNewCanvas();
            }
        }
    }

    private void CreateNewCanvas()
    {
        GameObject canvasObject = new GameObject("MainCanvas");
        targetCanvas = canvasObject.AddComponent<Canvas>();
        targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
    }

    private void FindPoints()
    {
        // SpawnPoints와 TargetPoints 찾기 로직...
        GameObject[] spawnObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag("TargetPoint");

        // 배열 초기화 및 할당
        spawnPoints = new Transform[spawnObjects.Length];
        targetPoints = new Transform[targetObjects.Length];

        for (int i = 0; i < spawnObjects.Length; i++)
        {
            spawnPoints[i] = spawnObjects[i].transform;
        }

        for (int i = 0; i < targetObjects.Length; i++)
        {
            targetPoints[i] = targetObjects[i].transform;
        }
    }

    private void Start()
    {
        spawnTimer = spawnInterval;

        // targetCanvas가 null인 경우 동적으로 할당
        if (targetCanvas == null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
            if (targetCanvas == null)
            {
                Debug.LogError("Target Canvas를 찾을 수 없습니다.");
                return;
            }
        }

        if (SceneManager.GetActiveScene().name == "Order")
        {
            FindPoints();
            LoadCustomers(); // 저장된 고객 정보 불러오기
            if (currentCustomers.Count == 0)
            {
                SpawnRandomCustomer(); // 저장된 고객이 없으면 새로운 고객 생성
            }
        }

        // DialogueData 할당
        if (dialogueData == null)
        {
            dialogueData = FindObjectOfType<DialogueData>();
            if (dialogueData == null)
            {
                Debug.LogError("DialogueData를 찾을 수 없습니다.");
            }
        }
    }

    private void Update()
    {
        if (!isDialogueActive && currentOrderTimeout > 0)
        {
            currentOrderTimeout -= Time.deltaTime;
            OnTimerUpdated?.Invoke(currentOrderTimeout);
            if (currentOrderTimeout <= 0)
            {
                // 주문 시간 초과 처리
                // TimeoutOrder();
            }
        }

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnRandomCustomer(); // 고객 생성
            spawnTimer = spawnInterval;
        }
    }

    // 랜덤 고객 생성
    private void SpawnRandomCustomer()
    {
        // 프리팹 생성 전 Canvas 유효성 검사
        if (targetCanvas == null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
            if (targetCanvas == null)
            {
                Debug.LogError("Canvas를 찾을 수 없어 고객을 생성할 수 없습니다.");
                return;
            }
        }

        if (currentCustomers.Count >= maxCustomers || IsAllTargetPointsOccupied()) return;

        int randomIndex = GetUniqueRandomIndex();
        if (randomIndex == -1) return;

        int randomSpawnPointIndex = Random.Range(0, spawnPoints.Length);
        int randomTargetPointIndex = GetAvailableTargetPointIndex();
        if (randomTargetPointIndex == -1) return;

        GameObject newCustomerObject = Instantiate(customerPrefabs[randomIndex], targetCanvas.transform);
        RectTransform rectTransform = newCustomerObject.GetComponent<RectTransform>();
        rectTransform.position = spawnPoints[randomSpawnPointIndex].position;

        Customer newCustomer = new Customer(newCustomerObject, randomIndex, spawnPoints[randomSpawnPointIndex], randomTargetPointIndex);
        currentCustomers.Add(newCustomer);
        usedCustomerIndices.Add(randomIndex);
        MoveCustomerUp(newCustomer, randomTargetPointIndex);
    }

    // 모든 목표 위치가 점유되었는지 확인
    private bool IsAllTargetPointsOccupied()
    {
        return currentCustomers.Count >= targetPoints.Length;
    }

    // 사용 가능한 목표 위치 인덱스 반환
    private int GetAvailableTargetPointIndex()
    {
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < targetPoints.Length; i++)
        {
            if (!IsCustomerAtTargetPoint(i))
            {
                availableIndices.Add(i);
            }
        }

        if (availableIndices.Count == 0) return -1;

        return availableIndices[Random.Range(0, availableIndices.Count)];
    }

    // 특정 목표 위치에 고객이 있는지 확인
    private bool IsCustomerAtTargetPoint(int targetPointIndex)
    {
        foreach (var customer in currentCustomers)
        {
            if (customer.TargetPointIndex == targetPointIndex)
            {
                return true;
            }
        }
        return false;
    }

    // 사용되지 않은 랜덤 인덱스 반환
    private int GetUniqueRandomIndex()
    {
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < customerPrefabs.Count; i++)
        {
            if (!usedCustomerIndices.Contains(i))
            {
                availableIndices.Add(i);
            }
        }

        if (availableIndices.Count == 0) return -1;

        int randomIndex = Random.Range(0, availableIndices.Count);
        return availableIndices[randomIndex];
    }

    // 고객을 목표 위치로 이동
    private void MoveCustomerUp(Customer customer, int index)
    {
        Transform targetPoint = targetPoints[index];

        // 고객 오브젝트에서 SpeechBubble 컴포넌트 찾기
        SpeechBubble speechBubble = customer.GameObject.GetComponentInChildren<SpeechBubble>(true);

        // SpeechBubble이 없다면 프리팹에서 생성
        if (speechBubble == null)
        {
            // 말풍선 프리팹 참조 필요
            GameObject speechBubblePrefab = Resources.Load<GameObject>("SpeechBubblePrefab");
            if (speechBubblePrefab == null)
            {
                Debug.LogError("SpeechBubblePrefab을 Resources 폴더에서 찾을 수 없습니다.");
                return;
            }

            GameObject bubbleObj = Instantiate(speechBubblePrefab, customer.GameObject.transform);
            speechBubble = bubbleObj.GetComponent<SpeechBubble>();

            // 말풍선 위치 조정
            RectTransform bubbleRect = speechBubble.GetComponent<RectTransform>();
            bubbleRect.localPosition = new Vector3(0, 100f, 0); // 적절한 위치로 조정
        }

        // 고객과 말풍선 프리팹을 DontDestroyOnLoad로 설정
        DontDestroyOnLoad(customer.GameObject);
        DontDestroyOnLoad(speechBubble.gameObject);

        // 고객 이동 애니메이션
        customer.GameObject.transform.DOMove(targetPoint.position, moveUpDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                EnableCustomerInteraction(customer, speechBubble);
            });
    }

    // 고객 상호작용 활성화
    private void EnableCustomerInteraction(Customer customer, SpeechBubble speechBubble)
    {
        if (dialogueData == null)
        {
            Debug.LogError("DialogueData가 할당되지 않았습니다.");
            return;
        }

        var dialogue = dialogueData.customerDialogues.Find(d => d.customerType == customer.Index);
        if (dialogue == null)
        {
            Debug.LogError($"Customer {customer.Index}의 대화 데이터를 찾을 수 없습니다.");
            return;
        }

        CustomerDialogue customerDialogue = customer.GameObject.AddComponent<CustomerDialogue>();
        customerDialogue.Initialize(
            customer.Index,
            dialogue.dialogueLines,
            dialogue.middleDialogueLines,
            dialogue.clearDialogue,
            dialogue.faileDialogue,
            speechBubble
        );

        SpeechBubbleManager speechBubbleManager = FindObjectOfType<SpeechBubbleManager>();
        if (speechBubbleManager != null)
        {
            speechBubbleManager.ShowSpeechBubble(customer.GameObject);
            customerDialogue.PrintDialogues();
        }
    }

    // 고객 클릭 시 호출
    private void OnCustomerClick(Customer customer)
    {
        Debug.Log($"Customer {customer.Index} clicked");
    }

    // 대화 활성화 상태 설정
    public void SetDialogueActive(bool active)
    {
        isDialogueActive = active;
    }

    // 고객 정보 저장
    private void SaveCustomers()
    {
        List<CustomerData> customerDataList = new List<CustomerData>();
        foreach (var customer in currentCustomers)
        {
            if (customer.GameObject != null)
            {
                // 위치 정보도 함께 저장
                Vector3 position = customer.GameObject.transform.position;
                customerDataList.Add(new CustomerData(
                    customer.Index,
                    customer.TargetPointIndex,
                    position
                ));
            }
        }
        string json = JsonUtility.ToJson(new CustomerDataList { customers = customerDataList });
        PlayerPrefs.SetString("CustomerData", json);
        PlayerPrefs.Save();
    }

    // 고객 정보 불러오기
    private void LoadCustomers()
    {
        Debug.Log("LoadCustomers 시작");
        if (PlayerPrefs.HasKey("CustomerData"))
        {
            string json = PlayerPrefs.GetString("CustomerData");
            Debug.Log($"저장된 고객 데이터: {json}");
            CustomerDataList customerDataList = JsonUtility.FromJson<CustomerDataList>(json);

            foreach (var customerData in customerDataList.customers)
            {
                Debug.Log($"고객 생성 시도: Index={customerData.index}, TargetPoint={customerData.targetPointIndex}");
                SpawnSavedCustomer(customerData);
            }
        }
        else
        {
            Debug.Log("저장된 고객 데이터가 없습니다.");
        }
    }

    // 저장된 고객 생성
    private void SpawnSavedCustomer(CustomerData customerData)
    {
        if (spawnPoints == null || targetPoints == null)
        {
            Debug.LogError("SpawnPoints 또는 TargetPoints가 설정되지 않았습니다.");
            return;
        }

        GameObject newCustomerObject = Instantiate(customerPrefabs[customerData.index], targetCanvas.transform);
        RectTransform rectTransform = newCustomerObject.GetComponent<RectTransform>();
        rectTransform.position = spawnPoints[customerData.targetPointIndex].position;

        Customer newCustomer = new Customer(newCustomerObject, customerData.index, spawnPoints[customerData.targetPointIndex], customerData.targetPointIndex);
        currentCustomers.Add(newCustomer);
        usedCustomerIndices.Add(customerData.index);
        MoveCustomerUp(newCustomer, customerData.targetPointIndex);

        // 대화 상태를 중간 상태로 설정
        CustomerDialogue customerDialogue = newCustomerObject.GetComponent<CustomerDialogue>();
        if (customerDialogue != null)
        {
            customerDialogue.SetDialogueStateToMiddle();
        }
    }

    public void SpawnCustomerAtPosition(int customerType, Vector3 position)
    {
        if (customerPrefabs == null || customerType >= customerPrefabs.Count) return;

        GameObject newCustomerObject = Instantiate(customerPrefabs[customerType], targetCanvas.transform);
        newCustomerObject.transform.position = position;

        // 기존 고객 초기화 로직 호출
        Customer newCustomer = new Customer(newCustomerObject, customerType, null, GetTargetPointIndexForPosition(position));
        currentCustomers.Add(newCustomer);
        usedCustomerIndices.Add(customerType);

        EnableCustomerInteraction(newCustomer, null);
    }

    private int GetTargetPointIndexForPosition(Vector3 position)
    {
        for (int i = 0; i < targetPoints.Length; i++)
        {
            if (Vector3.Distance(targetPoints[i].position, position) < 0.1f)
            {
                return i;
            }
        }
        return -1;
    }

    public void ClearAllCustomers()
    {
        foreach (var customer in currentCustomers)
        {
            if (customer.GameObject != null)
            {
                Destroy(customer.GameObject);
            }
        }
        currentCustomers.Clear();
        usedCustomerIndices.Clear();
    }
}

public class Customer
{
    public GameObject GameObject { get; private set; }
    public int Index { get; private set; }
    public Transform SpawnPoint { get; private set; }
    public int TargetPointIndex { get; private set; }

    public Customer(GameObject gameObject, int index, Transform spawnPoint, int targetPointIndex)
    {
        GameObject = gameObject;
        Index = index;
        SpawnPoint = spawnPoint;
        TargetPointIndex = targetPointIndex;
    }
}

// 고객 데이터 저장을 위한 클래스
[System.Serializable]
public class CustomerData
{
    public int index;
    public int targetPointIndex;
    public Vector3 position;

    public CustomerData(int index, int targetPointIndex, Vector3 position)
    {
        this.index = index;
        this.targetPointIndex = targetPointIndex;
        this.position = position;
    }
}

// 고객 데이터 리스트 저장을 위한 클래스
[System.Serializable]
public class CustomerDataList
{
    public List<CustomerData> customers;
}
