using System.Collections.Generic;
using UnityEngine;

public class Customer_Spawner : MonoBehaviour
{
    public Canvas Canvas_Customer; // 손님 생성되는 캔버스
    public List<GameObject> customerPrefabs; // 손님 프리팹 리스트
    public List<Vector3> spawnPositions; // 손님 생성 위치 리스트

    // 손님 착석 여부
    [HideInInspector] public bool seat_Left = false;
    [HideInInspector] public bool seat_Middle = false;
    [HideInInspector] public bool seat_Right = false;

    // 중복 방지를 위한 생성된 프리팹 추적 리스트
    private readonly List<int> spawnedPrefabIndices = new List<int>();

    private int maxCustomers = 3; // 최대 손님 수
    private int spawnIndex = 0; // 생성된 손님 수

    private void Start()
    {
        Load_customerData();
    }

    private void Update()
    {
        SpawnCustomer();
    }

    public void SpawnCustomer()
    {
        if (spawnIndex >= customerPrefabs.Count)
        {
            return;
        }

        if (spawnIndex >= maxCustomers)
        {
            return;
        }

        List<int> emptySeats = new List<int>();
        if (!seat_Left) emptySeats.Add(SeatIndex.Left);
        if (!seat_Middle) emptySeats.Add(SeatIndex.Middle);
        if (!seat_Right) emptySeats.Add(SeatIndex.Right);

        if (emptySeats.Count == 0)
        {
            return;
        }

        // 사용 가능한 프리팹 인덱스 찾기 (중복 방지)
        List<int> availablePrefabIndices = new List<int>();
        for (int i = 0; i < customerPrefabs.Count; i++)
        {
            if (!spawnedPrefabIndices.Contains(i))
            {
                availablePrefabIndices.Add(i);
            }
        }

        if (availablePrefabIndices.Count == 0)
        {
            return;
        }

        int seatIdx = emptySeats[Random.Range(0, emptySeats.Count)];
        int randomPrefabIdx = availablePrefabIndices[Random.Range(0, availablePrefabIndices.Count)];
        spawnedPrefabIndices.Add(randomPrefabIdx);

        // 손님 프리팹을 캔버스의 자식으로 생성
        GameObject customer = Instantiate(customerPrefabs[randomPrefabIdx], Canvas_Customer.transform);
        RectTransform rectTransform = customer.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = spawnPositions[seatIdx];
        }

        Customer customerComponent = customer.GetComponent<Customer>();
        if (customerComponent != null)
        {
            customerComponent.prefabIndex = randomPrefabIdx;
            customerComponent.seatIndex = seatIdx;
        }

        // 좌석 상태 갱신 및 CustomerManager에 할당
        switch (seatIdx)
        {
            case SeatIndex.Left:
                seat_Left = true;
                if (customerComponent != null)
                {
                    CustomerManager.Instance.Set_LeftCustomer(customerComponent);
                }
                break;

            case SeatIndex.Middle:
                seat_Middle = true;
                if (customerComponent != null)
                {
                    CustomerManager.Instance.Set_MiddleCustomer(customerComponent);
                }
                break;

            case SeatIndex.Right:
                seat_Right = true;
                if (customerComponent != null)
                {
                    CustomerManager.Instance.Set_RightCustomer(customerComponent);
                }
                break;
        }

        spawnIndex++;
    }

    // 손님이 퇴장할 때 해당 프리팹 인덱스를 다시 사용 가능하게 만듬
    public void ReleasePrefabIndex(int prefabIndex)
    {
        if (spawnedPrefabIndices.Contains(prefabIndex))
        {
            spawnedPrefabIndices.Remove(prefabIndex);
        }
    }

    // 저장된 손님 데이터를 불러와 복원하는 메서드
    private void Load_customerData()
    {
        CustomerData customerData = CustomerData.Instance;
        if (customerData == null)
        {
            return;
        }

        for (int seatIndex = 0; seatIndex < SeatIndex.Count; seatIndex++)
        {
            if (!customerData.seatStates[seatIndex] || string.IsNullOrEmpty(customerData.customerNames[seatIndex]))
            {
                continue;
            }

            string customerName = customerData.customerNames[seatIndex];
            int prefabIndex = ExtractPrefabIndexFromName(customerName);

            if (prefabIndex < 0 || prefabIndex >= customerPrefabs.Count)
            {
                continue;
            }

            GameObject customer = Instantiate(customerPrefabs[prefabIndex], Canvas_Customer.transform);
            RectTransform rectTransform = customer.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = spawnPositions[seatIndex];
            }

            if (!spawnedPrefabIndices.Contains(prefabIndex))
            {
                spawnedPrefabIndices.Add(prefabIndex);
            }

            Customer customerComponent = customer.GetComponent<Customer>();
            if (customerComponent != null)
            {
                customerComponent.prefabIndex = prefabIndex;
                customerComponent.dialogueIndex = customerData.dialogueIndices[seatIndex];
                customerComponent.seatIndex = seatIndex;
            }

            switch (seatIndex)
            {
                case SeatIndex.Left:
                    seat_Left = true;
                    if (customerComponent != null)
                    {
                        CustomerManager.Instance.Set_LeftCustomer(customerComponent);
                    }
                    break;

                case SeatIndex.Middle:
                    seat_Middle = true;
                    if (customerComponent != null)
                    {
                        CustomerManager.Instance.Set_MiddleCustomer(customerComponent);
                    }
                    break;

                case SeatIndex.Right:
                    seat_Right = true;
                    if (customerComponent != null)
                    {
                        CustomerManager.Instance.Set_RightCustomer(customerComponent);
                    }
                    break;
            }

            spawnIndex++;
        }

        // 저장된 주문/상태 복원
        customerData.RestoreOrderDrink();
        customerData.RestoreCustomerState();
    }

    // 손님 이름에서 프리팹 인덱스를 추출하는 헬퍼 메서드
    private int ExtractPrefabIndexFromName(string customerName)
    {
        for (int i = 0; i < customerPrefabs.Count; i++)
        {
            string expectedName = customerPrefabs[i].name;
            if (customerName.Contains(expectedName) || customerName.StartsWith(expectedName))
            {
                return i;
            }
        }

        return -1;
    }
}
