using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SpeechBubble : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private RectTransform backgroundRect;      // 말풍선 배경의 RectTransform
    [SerializeField] private TextMeshProUGUI messageText;       // 대화 텍스트
    [SerializeField] private Button actionButton;               // 액션 버튼
    [SerializeField] private TextMeshProUGUI actionButtonText;  // 버튼 텍스트
    [SerializeField] private CanvasGroup canvasGroup;           // 페이드 효과용

    [Header("Settings")]
    [SerializeField] private float bubbleOffset = 2f;           // 캐릭터 위 오프셋
    [SerializeField] private float typingSpeed = 0.05f;         // 텍스트 타이핑 속도
    [SerializeField] private float minWidth = 100f;             // 최소 말풍선 너비
    [SerializeField] private float maxWidth = 300f;             // 최대 말풍선 너비
    [SerializeField] private float paddingHorizontal = 40f;     // 좌우 여백

    private GameObject targetCustomer;                          // 대상 손님
    private string[] dialogueLines;                            // 대화 내용 배열
    private int currentLineIndex = -1;                         // 현재 대화 인덱스
    private bool isTyping = false;                            // 타이핑 중인지 여부
    private Tweener fadeOutTweener;                           // 페이드 아웃 Tweener

    private void Awake()
    {
        // 컴포넌트 자동 할당
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (actionButtonText == null && actionButton != null)
            actionButtonText = actionButton.GetComponentInChildren<TextMeshProUGUI>();

        Initialize();
    }

    private void Initialize()
    {
        // 초기 설정
        if (actionButton != null)
        {
            actionButton.gameObject.SetActive(false);
            actionButton.onClick.AddListener(OnActionButtonClick);
        }

        if (messageText != null)
        {
            messageText.text = "...";
            SetBubbleSize(minWidth);
        }

        // 초기 알파값 설정
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }

    // 말풍선 크기 조절
    private void SetBubbleSize(float width)
    {
        if (backgroundRect != null)
        {
            width = Mathf.Clamp(width, minWidth, maxWidth);
            Vector2 currentSize = backgroundRect.sizeDelta;
            backgroundRect.sizeDelta = new Vector2(width, currentSize.y);
        }
    }

    // 대화 시작
    public void StartDialogue(GameObject customer, string[] lines)
    {
        if (isTyping) return;

        targetCustomer = customer;
        dialogueLines = lines;
        currentLineIndex = -1;

        // 기존 페이드 아웃 취소
        fadeOutTweener?.Kill();

        // 말풍선 표시
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        ShowNextLine();
    }

    // 다음 대화 표시
    public void ShowNextLine()
    {
        if (isTyping) return;

        currentLineIndex++;
        if (currentLineIndex >= dialogueLines.Length)
        {
            ShowActionButton();
            return;
        }

        StartTypingEffect(dialogueLines[currentLineIndex]);
    }

    // 타이핑 효과
    private void StartTypingEffect(string line)
    {
        isTyping = true;
        messageText.text = "";

        // DOTween을 사용한 타이핑 효과
        DOTween.To(() => 0f, x => {
            int letterCount = Mathf.FloorToInt(x);
            messageText.text = line.Substring(0, letterCount);
            AdjustBubbleSize();
        }, line.Length, line.Length * typingSpeed)
        .OnComplete(() => {
            messageText.text = line;
            isTyping = false;
            AdjustBubbleSize();
        });
    }

    // 말풍선 크기 자동 조절
    private void AdjustBubbleSize()
    {
        if (messageText != null)
        {
            float textWidth = messageText.preferredWidth;
            SetBubbleSize(textWidth + paddingHorizontal);
        }
    }

    // 액션 버튼 표시
    private void ShowActionButton()
    {
        if (actionButton != null)
        {
            actionButton.gameObject.SetActive(true);
            actionButtonText.text = "주문하기";
        }
    }

    // 액션 버튼 클릭 처리
    private void OnActionButtonClick()
    {
        // 버튼 클릭 시 페이드 아웃
        FadeOut();

        // 여기에 주문 처리 로직 추가
        Debug.Log("주문 처리 시작");
    }

    // 페이드 아웃 효과
    private void FadeOut()
    {
        if (canvasGroup != null)
        {
            fadeOutTweener = canvasGroup.DOFade(0f, 0.5f)
                .OnComplete(() => {
                    actionButton.gameObject.SetActive(false);
                    messageText.text = "...";
                    SetBubbleSize(minWidth);
                    canvasGroup.alpha = 1f;
                });
        }
    }

    // 말풍선 위치 업데이트
    private void Update()
    {
        if (targetCustomer != null)
        {
            // 캐릭터 위치를 따라다니도록 업데이트
            Vector3 targetPosition = targetCustomer.transform.position + Vector3.up * bubbleOffset;
            transform.position = targetPosition;
        }
    }

    private void OnDestroy()
    {
        // DOTween 정리
        fadeOutTweener?.Kill();

        // 버튼 이벤트 제거
        if (actionButton != null)
        {
            actionButton.onClick.RemoveListener(OnActionButtonClick);
        }
    }
}