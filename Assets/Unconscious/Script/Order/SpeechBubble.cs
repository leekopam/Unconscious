using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SpeechBubble : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Size Settings")]
    [SerializeField] private float bubbleScale = 0.3f;    // 말풍선 전체 크기
    [SerializeField] private float textScale = 0.5f;      // 텍스트 크기
    [SerializeField] private float padding = 5f;          // 말풍선 여백

    [Header("Typing Settings")]
    [SerializeField] private float typingSpeed = 0.05f;

    private RectTransform bubbleRect;
    private RectTransform textRect;
    private List<string> dialogueLines;
    private int currentLineIndex;
    private bool isTyping;
    public System.Action OnDialogueComplete;

    private void Awake()
    {
        bubbleRect = GetComponent<RectTransform>();
        textRect = dialogueText.GetComponent<RectTransform>();

        // 말풍선 전체 크기 설정
        transform.localScale = Vector3.one * bubbleScale;

        // 텍스트 크기 설정
        dialogueText.fontSize *= textScale;

        Button button = gameObject.AddComponent<Button>();
        button.onClick.AddListener(OnBubbleClick);
    }

    public void SetScales(float newBubbleScale, float newTextScale)
    {
        bubbleScale = newBubbleScale;
        textScale = newTextScale;
        transform.localScale = Vector3.one * bubbleScale;
        dialogueText.fontSize *= textScale;
        AdjustBubbleSize();
    }

    public void Initialize(List<string> lines)
    {
        dialogueLines = lines;
        currentLineIndex = 0;
        gameObject.SetActive(true);
        ShowNextLine();
    }

    private void OnBubbleClick()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = dialogueLines[currentLineIndex];
            isTyping = false;
            AdjustBubbleSize();
        }
        else
        {
            currentLineIndex++;
            if (currentLineIndex >= dialogueLines.Count)
            {
                OnDialogueComplete?.Invoke();  // 대화가 끝났을 때 이벤트 발생
                gameObject.SetActive(false);
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    private void ShowNextLine()
    {
        StartCoroutine(TypeText(dialogueLines[currentLineIndex]));
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            AdjustBubbleSize();
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void AdjustBubbleSize()
    {
        Vector2 textSize = dialogueText.GetPreferredValues();
        bubbleRect.sizeDelta = new Vector2(
            textSize.x + padding * 2,
            textSize.y + padding * 2
        );
    }
}