using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> customerPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform[] targetPoints;
    [SerializeField] private float moveUpDuration = 1f;
    [SerializeField] private float orderTimeout = 10f;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private CustomerStateData customerStateData;

    private List<Customer> currentCustomers = new List<Customer>();
    private HashSet<int> usedCustomerIndices = new HashSet<int>();
    private int maxCustomers = 3;
    private bool isDialogueActive = false;
    private float currentOrderTimeout;
    private float spawnTimer;

    public delegate void TimerUpdated(float timeRemaining);
    public event TimerUpdated OnTimerUpdated;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 초기화 시 상태 데이터 초기화
        if (customerStateData == null)
        {
            customerStateData = ScriptableObject.CreateInstance<CustomerStateData>();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Order")
        {
            RestoreCustomerState();
        }
    }

    private void Start()
    {
        spawnTimer = spawnInterval;
        SpawnRandomCustomer();
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

    private void RestoreCustomerState()
    {
        if (customerStateData.savedCustomerStates.Count == 0)
        {
            SpawnRandomCustomer();
            return;
        }

        var customerState = customerStateData.savedCustomerStates[0];

        GameObject newCustomerObject = Instantiate(
            customerPrefabs[customerState.customerType],
            targetCanvas.transform
        );

        RectTransform rectTransform = newCustomerObject.GetComponent<RectTransform>();
        rectTransform.position = customerState.position;

        Customer restoredCustomer = new Customer(
            newCustomerObject,
            customerState.customerType,
            null
        );

        currentCustomers.Add(restoredCustomer);
        usedCustomerIndices.Add(customerState.customerType);

        // 주문 완료 상태에 따른 처리
        if (customerState.isOrderCompleted)
        {
            SpeechBubbleManager speechBubbleManager = FindObjectOfType<SpeechBubbleManager>();
            speechBubbleManager?.ShowOrderCompletedDialogue(newCustomerObject);
        }
        else if (!customerState.hasShownInitialDialogue)
        {
            EnableCustomerInteraction(restoredCustomer);
        }
    }

    public void SaveCustomerState()
    {
        // 기존 상태 초기화
        customerStateData.ClearStates();

        // 현재 고객 상태 저장
        foreach (var customer in currentCustomers)
        {
            CustomerStateData.CustomerState state = new CustomerStateData.CustomerState
            {
                customerType = customer.Index,
                position = customer.GameObject.transform.position,
                isOrderCompleted = false // 주문 완료 상태 저장
            };
            customerStateData.savedCustomerStates.Add(state);
        }
    }

    private void SpawnRandomCustomer()
    {
        if (currentCustomers.Count >= maxCustomers) return;

        int randomIndex = GetUniqueRandomIndex();
        if (randomIndex == -1) return;

        int randomSpawnPointIndex = Random.Range(0, spawnPoints.Length);
        if (IsCustomerAtTargetPoint(randomSpawnPointIndex)) return;

        GameObject newCustomerObject = Instantiate(customerPrefabs[randomIndex], targetCanvas.transform);
        RectTransform rectTransform = newCustomerObject.GetComponent<RectTransform>();
        rectTransform.position = spawnPoints[randomSpawnPointIndex].position;

        Customer newCustomer = new Customer(newCustomerObject, randomIndex, spawnPoints[randomSpawnPointIndex]);
        currentCustomers.Add(newCustomer);
        usedCustomerIndices.Add(randomIndex);
        MoveCustomerUp(newCustomer, randomSpawnPointIndex);
    }

    private bool IsCustomerAtTargetPoint(int targetPointIndex)
    {
        foreach (var customer in currentCustomers)
        {
            if (Vector3.Distance(customer.GameObject.transform.position, targetPoints[targetPointIndex].position) < 0.1f)
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
}

public class Customer
{
    public GameObject GameObject { get; private set; }
    public int Index { get; private set; }
    public Transform SpawnPoint { get; private set; }

    public Customer(GameObject gameObject, int index, Transform spawnPoint)
    {
        GameObject = gameObject;
        Index = index;
        SpawnPoint = spawnPoint;
    }
}