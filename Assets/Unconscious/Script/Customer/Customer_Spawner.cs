using System.Collections.Generic;
using UnityEngine;

public class Customer_Spawner : MonoBehaviour
{
    public Canvas Canvas_Customer; //손님 생성되는 캔버스
    public List<GameObject> customerPrefabs; //손님 프리팹 리스트
    public List<Vector3> spawnPositions; //손님 생성 위치 리스트

    //손님 착석 여부
    [HideInInspector] public bool seat_Left = false; 
    [HideInInspector] public bool seat_Middle = false;
    [HideInInspector] public bool seat_Right = false;

    private CustomerData customerData;
    // 중복 방지를 위한 생성된 프리팹 추적 리스트 추가
    private List<int> spawnedPrefabIndices = new List<int>();

    private int maxCustomers = 3; //최대 손님 수
    private int spawnIndex = 0; //생성된 손님 수 

    void Start()
    {
        customerData = GetComponent<CustomerData>();
        Load_customerData();
    }

    void Update()
    {
        SpawnCustomer();
    }

    public void SpawnCustomer()
    {
        if (spawnIndex >= customerPrefabs.Count) return;
        if (spawnIndex >= maxCustomers) return;

        List<int> emptySeats = new List<int>();
        if (!seat_Left) emptySeats.Add(0);    // 왼쪽 좌석
        if (!seat_Middle) emptySeats.Add(1);  // 가운데 좌석
        if (!seat_Right) emptySeats.Add(2);   // 오른쪽 좌석
        if (emptySeats.Count == 0) return; // 빈 좌석이 없으면 리턴

        // 사용 가능한 프리팹 인덱스 찾기 (중복 방지)
        List<int> availablePrefabIndices = new List<int>();
        for (int i = 0; i < customerPrefabs.Count; i++)
        {
            if (!spawnedPrefabIndices.Contains(i))
            {
                availablePrefabIndices.Add(i);
            }
        }

        // 사용 가능한 프리팹이 없으면 리턴
        if (availablePrefabIndices.Count == 0) return;

        // 빈 좌석 중 랜덤 선택
        int seatIdx = emptySeats[UnityEngine.Random.Range(0, emptySeats.Count)];
        
        // 사용 가능한 프리팹 중 랜덤 선택
        int randomPrefabIdx = availablePrefabIndices[UnityEngine.Random.Range(0, availablePrefabIndices.Count)];
        
        // 선택된 프리팹 인덱스를 사용된 리스트에 추가
        spawnedPrefabIndices.Add(randomPrefabIdx);

        // 손님 프리팹을 캔버스의 자식으로 생성
        GameObject customer = Instantiate(customerPrefabs[randomPrefabIdx], Canvas_Customer.transform);
        RectTransform rectTransform = customer.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // 캔버스 좌표계에 맞게 위치 설정
            rectTransform.anchoredPosition = spawnPositions[seatIdx];
        }

        Customer customerComponent = customer.GetComponent<Customer>();
        // 프리팹 인덱스 설정 추가
        if (customerComponent != null)
        {
            customerComponent.prefabIndex = randomPrefabIdx;
        }
        
        // 좌석 상태 갱신 및 CustomerManager에 할당
        switch (seatIdx)
        {
            case 0:
                seat_Left = true;
                if (customerComponent != null)
                    CustomerManager.Instance.Set_LeftCustomer(customerComponent);
                break;
            case 1:
                seat_Middle = true;
                if (customerComponent != null)
                    CustomerManager.Instance.Set_MiddleCustomer(customerComponent);
                break;
            case 2:
                seat_Right = true;
                if (customerComponent != null)
                    CustomerManager.Instance.Set_RightCustomer(customerComponent);
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

    private void Load_customerData()
    {
        // CustomerData 인스턴스 가져오기
        CustomerData customerData = CustomerData.Instance;
        if (customerData == null) return;

        // 저장된 손님 데이터가 있는지 확인하고 복원
        for (int seatIndex = 0; seatIndex < 3; seatIndex++)
        {
            // 해당 좌석에 손님이 있었는지 확인
            if (customerData.seatStates[seatIndex] && !string.IsNullOrEmpty(customerData.customerNames[seatIndex]))
            {
                // 저장된 손님 이름에서 프리팹 인덱스 추출
                string customerName = customerData.customerNames[seatIndex];
                int prefabIndex = ExtractPrefabIndexFromName(customerName);
                
                if (prefabIndex >= 0 && prefabIndex < customerPrefabs.Count)
                {
                    // 해당 프리팹을 지정된 좌석에 생성
                    GameObject customer = Instantiate(customerPrefabs[prefabIndex], Canvas_Customer.transform);
                    RectTransform rectTransform = customer.GetComponent<RectTransform>();
                    
                    if (rectTransform != null)
                    {
                        // 좌석 위치에 배치
                        rectTransform.anchoredPosition = spawnPositions[seatIndex];
                    }

                    // 프리팹 인덱스를 사용된 리스트에 추가
                    if (!spawnedPrefabIndices.Contains(prefabIndex))
                    {
                        spawnedPrefabIndices.Add(prefabIndex);
                    }

                    Customer customerComponent = customer.GetComponent<Customer>();
                    if (customerComponent != null)
                    {
                        // 프리팹 인덱스 설정
                        customerComponent.prefabIndex = prefabIndex;
                        
                        // 저장된 대화 인덱스 복원
                        customerComponent.dialogueIndex = customerData.dialogueIndices[seatIndex];
                    }

                    // 좌석 상태 업데이트 및 CustomerManager에 할당
                    switch (seatIndex)
                    {
                        case 0: // 왼쪽 좌석
                            seat_Left = true;
                            if (customerComponent != null)
                                CustomerManager.Instance.Set_LeftCustomer(customerComponent);
                            break;
                        case 1: // 가운데 좌석
                            seat_Middle = true;
                            if (customerComponent != null)
                                CustomerManager.Instance.Set_MiddleCustomer(customerComponent);
                            break;
                        case 2: // 오른쪽 좌석
                            seat_Right = true;
                            if (customerComponent != null)
                                CustomerManager.Instance.Set_RightCustomer(customerComponent);
                            break;
                    }

                    spawnIndex++;
                }
            }
        }

        // 저장된 상태 복원 (CustomerData에서 제공하는 메서드 사용)
        customerData.RestoreCustomerState();
    }

    // 손님 이름에서 프리팹 인덱스를 추출하는 헬퍼 메서드
    private int ExtractPrefabIndexFromName(string customerName)
    {
        // 손님 이름이 "CustomerPrefab_0(Clone)" 형식이라고 가정
        // 실제 명명 규칙에 맞게 수정 필요
        for (int i = 0; i < customerPrefabs.Count; i++)
        {
            string expectedName = customerPrefabs[i].name;
            if (customerName.Contains(expectedName) || customerName.StartsWith(expectedName))
            {
                return i;
            }
        }
        return -1; // 매칭되는 프리팹을 찾지 못한 경우
    }
}
