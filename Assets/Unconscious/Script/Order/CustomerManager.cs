using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> customerPrefabs; // 고객 프리팹 목록
    [SerializeField] private Transform[] spawnPoints; // 고객 생성 위치
    [SerializeField] private Transform[] targetPoints; // 고객 이동 목표 위치
    [SerializeField] private float moveUpDuration = 1f; // 고객 이동 시간
    [SerializeField] private float orderTimeout = 10f; // 주문 제한 시간
    [SerializeField] private float spawnInterval = 5f; // 고객 생성 간격
    [SerializeField] private Canvas targetCanvas; // 고객을 생성할 캔버스

    private List<Customer> currentCustomers = new List<Customer>(); // 현재 존재하는 고객 목록
    private HashSet<int> usedCustomerIndices = new HashSet<int>(); // 이미 사용된 고객 인덱스
    private int maxCustomers = 3; // 최대 고객 수
    private bool isDialogueActive = false; // 대화 활성화 여부
    private float currentOrderTimeout; // 현재 주문 제한 시간
    private float spawnTimer; // 고객 생성 타이머

    public delegate void TimerUpdated(float timeRemaining);
    public event TimerUpdated OnTimerUpdated;

    private void Start()
    {
        spawnTimer = spawnInterval;
        LoadCustomers(); // 저장된 고객 정보 불러오기
        if (currentCustomers.Count == 0)
        {
            SpawnRandomCustomer(); // 저장된 고객이 없으면 새로운 고객 생성
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
                //TimeoutOrder();
            }
        }

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnRandomCustomer();
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnRandomCustomer()
    {
        if (currentCustomers.Count >= maxCustomers || IsAllTargetPointsOccupied()) return;

        int randomIndex = GetUniqueRandomIndex();
        if (randomIndex == -1) return;

        int randomSpawnPointIndex = GetAvailableTargetPointIndex();
        if (randomSpawnPointIndex == -1) return;

        GameObject newCustomerObject = Instantiate(customerPrefabs[randomIndex], targetCanvas.transform);
        RectTransform rectTransform = newCustomerObject.GetComponent<RectTransform>();
        rectTransform.position = spawnPoints[randomSpawnPointIndex].position;

        Customer newCustomer = new Customer(newCustomerObject, randomIndex, spawnPoints[randomSpawnPointIndex], randomSpawnPointIndex);
        currentCustomers.Add(newCustomer);
        usedCustomerIndices.Add(randomIndex);
        MoveCustomerUp(newCustomer, randomSpawnPointIndex);
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

    private void MoveCustomerUp(Customer customer, int index)
    {
        Transform targetPoint = targetPoints[index];

        customer.GameObject.transform.DOMove(targetPoint.position, moveUpDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                EnableCustomerInteraction(customer);
                StartOrderTimeout();
            });
    }

    private void EnableCustomerInteraction(Customer customer)
    {
        SpeechBubbleManager speechBubbleManager = FindObjectOfType<SpeechBubbleManager>();
        if (speechBubbleManager != null)
        {
            CustomerDialogue dialogue = customer.GameObject.AddComponent<CustomerDialogue>();
            dialogue.Initialize(customer.Index, new List<string> { "안녕하세요", "주문하고 싶어요" });
            speechBubbleManager.ShowSpeechBubble(customer.GameObject);
        }
    }

    private void OnCustomerClick(Customer customer)
    {
        Debug.Log($"Customer {customer.Index} clicked");
    }

    private void StartOrderTimeout()
    {
        currentOrderTimeout = orderTimeout;
        OnTimerUpdated?.Invoke(currentOrderTimeout);
    }

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
            customerDataList.Add(new CustomerData(customer.Index, customer.TargetPointIndex));
        }
        string json = JsonUtility.ToJson(new CustomerDataList { customers = customerDataList });
        PlayerPrefs.SetString("CustomerData", json);
        PlayerPrefs.Save();
    }

    // 고객 정보 불러오기
    private void LoadCustomers()
    {
        if (PlayerPrefs.HasKey("CustomerData"))
        {
            string json = PlayerPrefs.GetString("CustomerData");
            CustomerDataList customerDataList = JsonUtility.FromJson<CustomerDataList>(json);
            foreach (var customerData in customerDataList.customers)
            {
                SpawnSavedCustomer(customerData);
            }
        }
    }

    // 저장된 고객 생성
    private void SpawnSavedCustomer(CustomerData customerData)
    {
        GameObject newCustomerObject = Instantiate(customerPrefabs[customerData.index], targetCanvas.transform);
        RectTransform rectTransform = newCustomerObject.GetComponent<RectTransform>();
        rectTransform.position = targetPoints[customerData.targetPointIndex].position;

        Customer newCustomer = new Customer(newCustomerObject, customerData.index, spawnPoints[customerData.targetPointIndex], customerData.targetPointIndex);
        currentCustomers.Add(newCustomer);
        usedCustomerIndices.Add(customerData.index);
        EnableCustomerInteraction(newCustomer);
    }

    // 씬 전환 시 고객 정보 저장
    private void OnDisable()
    {
        SaveCustomers();
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

    public CustomerData(int index, int targetPointIndex)
    {
        this.index = index;
        this.targetPointIndex = targetPointIndex;
    }
}

// 고객 데이터 리스트 저장을 위한 클래스
[System.Serializable]
public class CustomerDataList
{
    public List<CustomerData> customers;
}
