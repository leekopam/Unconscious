using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Program : MonoBehaviour
{
    [SerializeField] private Text timerText; // 타이머 값을 표시할 텍스트 UI

    private CustomerManager customerManager;

    // Start is called before the first frame update
    void Start()
    {
        customerManager = FindObjectOfType<CustomerManager>();
        if (customerManager != null)
        {
            customerManager.OnTimerUpdated += UpdateTimerText; // 타이머 업데이트 이벤트 구독
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void UpdateTimerText(float timeRemaining)
    {
        timerText.text = $"남은 시간: {timeRemaining:F2}초"; // 타이머 값을 텍스트로 표시
    }
}
