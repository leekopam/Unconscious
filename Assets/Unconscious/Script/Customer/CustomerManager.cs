using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> customerPrefabs; // 손님 프리팹으로 관리
    [SerializeField] private Transform[] spawnPoints; // 손님 처음 스폰장소들
    [SerializeField] private Transform[] targetPoints; // 올라오는 지점들
    [SerializeField] private float moveUpDuration = 1f; // 손님 올라오는 속도(낮을수록 빨라짐
    [SerializeField] private float orderTimeout = 10f; // 주문 대기 시간
    [SerializeField] private float spawnInterval = 5f; // 손님 추가되는 시간
    [SerializeField] private GameObject dialoguePanel; // 대화 패널
    [SerializeField] private Text dialogueText; // 손님 대화 텍스트
    [SerializeField] private Button acceptButton; // 주문 수락 버튼
    [SerializeField] private Button rejectButton; // 주문 거절 버튼

    private List<Customer> currentCustomers = new List<Customer>(); // 현재 손님 리스트
    private HashSet<int> usedCustomerIndices = new HashSet<int>(); // 사용 중인 손님 프리팹 인덱스
    private int maxCustomers = 3; // 최대 손님 수
    private bool isDialogueActive = false; // 대화 활성화 여부
    private float currentOrderTimeout; // 현재 주문 대기 시간
    private float spawnTimer; // 손님 추가 타이머

    public delegate void TimerUpdated(float timeRemaining);
    public event TimerUpdated OnTimerUpdated; // 타이머 업데이트 이벤트

    private void Start()
    {
        spawnTimer = spawnInterval; // 스폰 타이머 초기화
        SpawnRandomCustomer(); // 첫 손님 스폰
    }

    private void Update()
    {
        // 대화가 활성화되지 않았고, 주문 대기 시간이 남아 있을 때
        if (!isDialogueActive && currentOrderTimeout > 0)
        {
            currentOrderTimeout -= Time.deltaTime; // 주문 대기 시간 감소
            OnTimerUpdated?.Invoke(currentOrderTimeout); // 타이머 업데이트 이벤트 호출
            if (currentOrderTimeout <= 0)
            {
                TimeoutOrder(); // 주문 시간 초과 처리
            }
        }

        spawnTimer -= Time.deltaTime; // 스폰 타이머 감소
        if (spawnTimer <= 0)
        {
            SpawnRandomCustomer(); // 새로운 손님 스폰
            spawnTimer = spawnInterval; // 스폰 타이머 초기화
        }
    }

    // 랜덤 손님 스폰
    private void SpawnRandomCustomer()
    {
        if (currentCustomers.Count >= maxCustomers) return;

        int randomIndex = GetUniqueRandomIndex(); // 고유한 랜덤 인덱스 가져오기
        if (randomIndex == -1) return; // 모든 손님 프리팹이 사용된 경우

        int randomSpawnPointIndex = Random.Range(0, spawnPoints.Length); // 랜덤 스폰 지점 인덱스
        if (IsCustomerAtTargetPoint(randomSpawnPointIndex)) return; // 손님이 이미 올라와 있는 경우

        GameObject newCustomerObject = Instantiate(customerPrefabs[randomIndex], spawnPoints[randomSpawnPointIndex].position, Quaternion.identity); // 손님 생성
        Customer newCustomer = new Customer(newCustomerObject, randomIndex, spawnPoints[randomSpawnPointIndex]); // 손님 객체 생성
        currentCustomers.Add(newCustomer); // 현재 손님 리스트에 추가
        usedCustomerIndices.Add(randomIndex); // 사용 중인 손님 프리팹 인덱스 추가
        MoveCustomerUp(newCustomer, randomSpawnPointIndex); // 손님 이동
    }

    // 손님이 타겟 지점에 있는지 확인
    private bool IsCustomerAtTargetPoint(int targetPointIndex)
    {
        foreach (var customer in currentCustomers)
        {
            if (Vector3.Distance(customer.GameObject.transform.position, targetPoints[targetPointIndex].position) < 0.1f)
            {
                return true; // 손님이 이미 해당 지점에 있는 경우
            }
        }
        return false;
    }

    // 고유한 랜덤 인덱스 가져오기
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

        if (availableIndices.Count == 0) return -1; // 사용 가능한 손님 프리팹이 없는 경우

        int randomIndex = Random.Range(0, availableIndices.Count); // 랜덤 인덱스 선택
        return availableIndices[randomIndex];
    }

    // 손님을 타겟 지점으로 이동
    private void MoveCustomerUp(Customer customer, int index)
    {
        Transform targetPoint = targetPoints[index];

        customer.GameObject.transform.DOMove(targetPoint.position, moveUpDuration)
            .SetEase(Ease.InOutQuad) // 부드러운 시작과 끝을 위한 기능
            .OnComplete(() =>
            {
                EnableCustomerInteraction(customer); // 손님 상호작용 활성화
                StartOrderTimeout(); // 주문 대기 시간 시작
            });
    }

    // 손님 상호작용 활성화
    private void EnableCustomerInteraction(Customer customer)
    {
        // 손님 프리팹에 Collider 추가
        if (customer.GameObject.GetComponent<Collider>() == null)
        {
            customer.GameObject.AddComponent<BoxCollider>();
        }

        // 손님 프리팹에 CustomerClickHandler 추가
        if (customer.GameObject.GetComponent<CustomerClickHandler>() == null)
        {
            customer.GameObject.AddComponent<CustomerClickHandler>().Initialize(this, customer.Index);
        }
    }

    // 주문 대기 시간 시작
    private void StartOrderTimeout()
    {
        currentOrderTimeout = orderTimeout;
        OnTimerUpdated?.Invoke(currentOrderTimeout); // 타이머 업데이트 이벤트 호출
    }

    // 대화창 표시
    public void ShowDialogue(GameObject customerObject, int customerIndex)
    {
        isDialogueActive = true; // 대화 활성화
        dialoguePanel.SetActive(true); // 대화 패널 활성화
        dialogueText.text = ""; // 초기 텍스트 지우기
        string orderText = "랜덤한 주문 텍스트를 여기에 입력하세요."; // 주문 텍스트

        dialogueText.DOText(orderText, 2f)
            .SetEase(Ease.Linear)
            .OnComplete(() => ShowOptions(customerObject, customerIndex)); // 옵션 표시
    }

    // 옵션 표시
    private void ShowOptions(GameObject customerObject, int customerIndex)
    {
        acceptButton.gameObject.SetActive(true); // 수락 버튼 활성화
        rejectButton.gameObject.SetActive(true); // 거절 버튼 활성화

        acceptButton.onClick.AddListener(() => AcceptOrder(customerObject, customerIndex)); // 수락 버튼 클릭 이벤트 추가
        rejectButton.onClick.AddListener(() => RejectOrder(customerObject, customerIndex)); // 거절 버튼 클릭 이벤트 추가
    }

    // 주문 수락 처리
    public void AcceptOrder(GameObject customerObject, int customerIndex)
    {
        Debug.Log("주문 수락, 칵테일 제조 화면으로 전환"); // 디버그 로그
        RemoveCustomer(customerObject, customerIndex); // 손님 제거
    }

    // 주문 거절 처리
    public void RejectOrder(GameObject customerObject, int customerIndex)
    {
        Debug.Log("주문 거절"); // 디버그 로그
        RemoveCustomer(customerObject, customerIndex); // 손님 제거
    }

    // 주문 시간 초과 처리
    private void TimeoutOrder()
    {
        if (currentCustomers.Count > 0)
        {
            Customer customer = currentCustomers[0];
            Debug.Log("주문 시간 초과"); // 디버그 로그
            RemoveCustomer(customer.GameObject, customer.Index); // 손님 제거
            StartOrderTimeout(); // 다음 손님을 위한 주문 대기 시간 시작
        }
    }

    // 손님 제거
    private void RemoveCustomer(GameObject customerObject, int customerIndex)
    {
        Customer customer = currentCustomers.Find(c => c.GameObject == customerObject);
        if (customer != null)
        {
            customer.GameObject.transform.DOMove(customer.SpawnPoint.position, moveUpDuration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    usedCustomerIndices.Remove(customerIndex); // 사용 중인 손님 프리팹 인덱스 제거
                    currentCustomers.Remove(customer); // 현재 손님 리스트에서 제거
                    Destroy(customer.GameObject); // 손님 오브젝트 파괴
                    dialoguePanel.SetActive(false); // 대화 패널 비활성화
                    acceptButton.gameObject.SetActive(false); // 수락 버튼 비활성화
                    rejectButton.gameObject.SetActive(false); // 거절 버튼 비활성화
                    isDialogueActive = false; // 대화 비활성화
                    currentOrderTimeout = orderTimeout; // 주문 대기 시간 초기화
                    OnTimerUpdated?.Invoke(currentOrderTimeout); // 타이머 업데이트 이벤트 호출
                });
        }
        }
    }

// 손님 클래스
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

// 손님 클릭 핸들러 클래스
public class CustomerClickHandler : MonoBehaviour
{
    private CustomerManager customerManager;
    private int customerIndex;

    // 초기화 메서드
    public void Initialize(CustomerManager manager, int index)
    {
        customerManager = manager;
        customerIndex = index;
    }

    // 마우스 클릭 이벤트
    private void OnMouseDown()
    {
        customerManager.ShowDialogue(gameObject, customerIndex); // 대화창 표시
    }
}
