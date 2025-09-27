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

    private int maxCustomers = 3; //최대 손님 수
    private int spawnIndex = 0; //생성된 손님 수 


    void Update()
    {
        SpawnCustomer();
    }

    public void SpawnCustomer()
    {
        if (spawnIndex >= customerPrefabs.Count) return;
        if (spawnIndex > maxCustomers) return;

        List<int> emptySeats = new List<int>();
        if (!seat_Left) emptySeats.Add(0);    // 왼쪽 좌석
        if (!seat_Middle) emptySeats.Add(1);  // 가운데 좌석
        if (!seat_Right) emptySeats.Add(2);   // 오른쪽 좌석
        if (emptySeats.Count == 0) return; // 빈 좌석이 없으면 리턴

        // 빈 좌석 중 랜덤 선택
        int seatIdx = emptySeats[UnityEngine.Random.Range(0, emptySeats.Count)];

        // 손님 프리팹을 캔버스의 자식으로 생성
        GameObject customer = Instantiate(customerPrefabs[spawnIndex], Canvas_Customer.transform);
        RectTransform rectTransform = customer.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // 캔버스 좌표계에 맞게 위치 설정
            rectTransform.anchoredPosition = spawnPositions[seatIdx];
        }

        
        Customer customerComponent = customer.GetComponent<Customer>();
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
}
