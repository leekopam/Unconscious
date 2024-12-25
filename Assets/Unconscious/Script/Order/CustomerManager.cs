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
    [SerializeField] private DialogueData dialogueData; // 대화 데이터

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

        // 고객을 목표 위치로 이동
        customer.GameObject.transform.DOMove(targetPoint.position, moveUpDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // 고객이 목표 지점에 도달했을 때 호출되는 부분
                EnableCustomerInteraction(customer); // 고객 상호작용 활성화

                // SpeechBubbleManager를 찾아서 말풍선을 표시
                SpeechBubbleManager speechBubbleManager = FindObjectOfType<SpeechBubbleManager>();
                if (speechBubbleManager != null)
                {
                    // DialogueData를 찾아서 고객의 대화 데이터를 가져옴
                    if (dialogueData != null)
                    {
                        var dialogue = dialogueData.customerDialogues.Find(d => d.customerType == customer.Index);
                        if (dialogue != null)
                        {
                            // CustomerDialogue 컴포넌트를 추가하고 초기화
                            SpeechBubble speechBubble = customer.GameObject.GetComponentInChildren<SpeechBubble>();
                            CustomerDialogue customerDialogue = customer.GameObject.AddComponent<CustomerDialogue>();
                            customerDialogue.Initialize(
                                customer.Index,
                                dialogue.dialogueLines,
                                dialogue.middleDialogueLines,
                                dialogue.clearDialogue,
                                dialogue.faileDialogue,
                                speechBubble
                            );

                            // 말풍선을 표시하고 대화 내용을 출력
                            speechBubbleManager.ShowSpeechBubble(customer.GameObject);
                            customerDialogue.PrintDialogues(); // 대화 내용 출력
                        }
                    }
                }
            });
    }

    // 고객 상호작용 활성화
    private void EnableCustomerInteraction(Customer customer)
    {
        // SpeechBubbleManager를 찾아서 말풍선을 표시
        SpeechBubbleManager speechBubbleManager = FindObjectOfType<SpeechBubbleManager>();
        if (speechBubbleManager != null)
        {
            // DialogueData를 찾아서 고객의 대화 데이터를 가져옴
            if (dialogueData != null)
            {
                var dialogue = dialogueData.customerDialogues.Find(d => d.customerType == customer.Index);
                if (dialogue != null)
                {
                    // CustomerDialogue 컴포넌트를 추가하고 초기화
                    SpeechBubble speechBubble = customer.GameObject.GetComponentInChildren<SpeechBubble>();
                    CustomerDialogue customerDialogue = customer.GameObject.AddComponent<CustomerDialogue>();
                    customerDialogue.Initialize(
                        customer.Index,
                        dialogue.dialogueLines,
                        dialogue.middleDialogueLines,
                        dialogue.clearDialogue,
                        dialogue.faileDialogue,
                        speechBubble
                    );

                    // 말풍선을 표시
                    speechBubbleManager.ShowSpeechBubble(customer.GameObject);
                }
            }
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
                SpawnSavedCustomer(customerData); // 저장된 고객 생성
            }
        }
    }

    // 저장된 고객 생성
    private void SpawnSavedCustomer(CustomerData customerData)
    {
        GameObject newCustomerObject = Instantiate(customerPrefabs[customerData.index], targetCanvas.transform);
        RectTransform rectTransform = newCustomerObject.GetComponent<RectTransform>();
        rectTransform.position = spawnPoints[customerData.targetPointIndex].position;

        Customer newCustomer = new Customer(newCustomerObject, customerData.index, spawnPoints[customerData.targetPointIndex], customerData.targetPointIndex);
        currentCustomers.Add(newCustomer);
        usedCustomerIndices.Add(customerData.index);
        MoveCustomerUp(newCustomer, customerData.targetPointIndex);
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

