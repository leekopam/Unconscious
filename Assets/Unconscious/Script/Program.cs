using UnityEngine;
using UnityEngine.UI;

public class Program : MonoBehaviour
{
    [SerializeField] private Text timerText; // 타이머 값을 표시할 텍스트 UI

    private void Start()
    {
        CustomerManager customerManager = FindObjectOfType<CustomerManager>();
        if (customerManager != null)
        {
            customerManager.OnTimerUpdated += UpdateTimerText; // 타이머 업데이트 이벤트 구독
        }
    }

    private void UpdateTimerText(float timeRemaining)
    {
        //timerText.text = $"남은 시간: {timeRemaining:F2}초"; // 타이머 값을 텍스트로 표시
    }
}
