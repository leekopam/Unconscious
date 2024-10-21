using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;


public class CustomerManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> customerPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform targetPoint;
    [SerializeField] private float moveUpDuration = 1f;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button rejectButton;

    private GameObject currentCustomer;


    void Start()
    {
        SpawnRandomCustomer();
    }

    void SpawnRandomCustomer()
    {
        int randomIndex = Random.Range(0, customerPrefabs.Count);
        currentCustomer = Instantiate(customerPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
        MoveCustomerUp();
    }

    void MoveCustomerUp()
    {
        currentCustomer.transform.DOMove(targetPoint.position, moveUpDuration)
       .SetEase(Ease.InOutQuad)  // 부드러운 시작과 끝을 위한 기능
       .OnComplete(ShowDialogue);
    }

    void ShowDialogue()
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = ""; // 초기 텍스트 지우기
        string orderText = "랜덤한 주문 텍스트를 여기에 입력하세요.";

        dialogueText.DOText(orderText, 2f)
            .SetEase(Ease.Linear)
            .OnComplete(ShowOptions);
    }

    void ShowOptions()
    {
        acceptButton.gameObject.SetActive(true);
        rejectButton.gameObject.SetActive(true);

        acceptButton.onClick.AddListener(AcceptOrder);
        rejectButton.onClick.AddListener(RejectOrder);
    }

    void AcceptOrder()
    {
        // 칵테일 제조 화면으로 전환하는 로직
        Debug.Log("주문 수락, 칵테일 제조 화면으로 전환");
    }

    void RejectOrder()
    {
        // 주문 거절 처리 로직
        Debug.Log("주문 거절");
        // 새로운 손님 소환 또는 다른 처리
    }

}
