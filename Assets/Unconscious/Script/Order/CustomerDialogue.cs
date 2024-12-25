using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class CustomerDialogue : MonoBehaviour
{
    public enum DialogueState
    {
        Start,
        Middle,
        Clear,
        Fail
    }

    private int customerIndex;
    private List<string> dialogueLines;
    private List<string> middleDialogueLines;
    private List<string> clearDialogue;
    private List<string> failDialogue;

    public SpeechBubble speechBubble; // SpeechBubble 참조

    private Vector3 initialPosition;
    private CustomerState customerState;

    public DialogueState CurrentState { get; private set; } // 현재 대화 상태

    private void Start()
    {
        initialPosition = transform.position;
        customerState = new CustomerState();
        LoadState(); // 상태를 로드

        // 씬 전환 정보 확인
        string lastScene = PlayerPrefs.GetString("LastScene", "");
        if (lastScene == "Dessert" && CurrentState == DialogueState.Start)
        {
            Debug.Log("Dessert 씬에서 넘어왔고, CurrentState가 Start로 설정되어 있습니다.");
            SetDialogueStateToMiddle(); // 대화 상태를 중간 상태로 설정
        }

        // SpeechBubble 오브젝트 활성화
        if (speechBubble != null)
        {
            speechBubble.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 손님 대화 초기화
    /// </summary>
    public void Initialize(int index, List<string> dialogueLines, List<string> middleDialogueLines, List<string> clearDialogue, List<string> failDialogue, SpeechBubble speechBubble)
    {
        this.customerIndex = index;
        this.dialogueLines = dialogueLines;
        this.middleDialogueLines = middleDialogueLines;
        this.clearDialogue = clearDialogue;
        this.failDialogue = failDialogue;
        this.speechBubble = speechBubble;

        // SpeechBubble 할당
        if (speechBubble == null)
        {
            Debug.LogError("SpeechBubble이 할당되지 않았습니다.");
        }
    }

    /// <summary>
    /// 대화 출력
    /// </summary>
    public void PrintDialogues()
    {
        if (speechBubble == null)
        {
            Debug.LogError("SpeechBubble이 할당되지 않았습니다.");
            return;
        }

        List<string> allDialogues = new List<string>();

        switch (CurrentState)
        {
            case DialogueState.Start:
                allDialogues.Add("Initial Dialogues:");
                allDialogues.AddRange(dialogueLines);
                break;
            case DialogueState.Middle:
                allDialogues.Add("Middle Dialogues:");
                allDialogues.AddRange(middleDialogueLines);
                break;
            case DialogueState.Clear:
                allDialogues.Add("Clear Dialogues:");
                allDialogues.AddRange(clearDialogue);
                break;
            case DialogueState.Fail:
                allDialogues.Add("Fail Dialogues:");
                allDialogues.AddRange(failDialogue);
                break;
        }

        speechBubble.Initialize(allDialogues); // 모든 대화를 SpeechBubble에 초기화
    }

    /// <summary>
    /// 현재 상태 저장
    /// </summary>
    public void SaveState()
    {
        customerState.Save(
            transform.position,
            customerIndex,
            dialogueLines,
            middleDialogueLines,
            clearDialogue,
            failDialogue,
            CurrentState); // 현재 상태를 저장
    }

    /// <summary>
    /// 저장된 상태 로드
    /// </summary>
    public void LoadState()
    {
        Vector3 position;
        DialogueState state;
        if (customerState.Load(
            out position,
            out customerIndex,
            out dialogueLines,
            out middleDialogueLines,
            out clearDialogue,
            out failDialogue,
            out state))
        {
            transform.position = position; // 저장된 위치로 이동
            CurrentState = state; // 저장된 상태로 설정
        }
    }

    /// <summary>
    /// 대화 상태를 중간 상태로 설정
    /// </summary>
    public void SetDialogueStateToMiddle()
    {
        CurrentState = DialogueState.Middle;
    }
}

public class CustomerState
{
    /// <summary>
    /// 상태 저장
    /// </summary>
    public void Save(Vector3 position, int customerIndex, List<string> dialogueLines, List<string> middleDialogueLines, List<string> clearDialogue, List<string> failDialogue, CustomerDialogue.DialogueState state)
    {
        PlayerPrefs.SetFloat("CustomerPosX", position.x);
        PlayerPrefs.SetFloat("CustomerPosY", position.y);
        PlayerPrefs.SetFloat("CustomerPosZ", position.z);
        PlayerPrefs.SetInt("CustomerIndex", customerIndex);
        PlayerPrefs.SetString("DialogueLines", string.Join(",", dialogueLines));
        PlayerPrefs.SetString("MiddleDialogueLines", string.Join(",", middleDialogueLines));
        PlayerPrefs.SetString("ClearDialogue", string.Join(",", clearDialogue));
        PlayerPrefs.SetString("FailDialogue", string.Join(",", failDialogue));
        PlayerPrefs.SetInt("DialogueState", (int)state);
        PlayerPrefs.Save(); // 상태를 PlayerPrefs에 저장
    }

    /// <summary>
    /// 상태 로드
    /// </summary>
    public bool Load(out Vector3 position, out int customerIndex, out List<string> dialogueLines, out List<string> middleDialogueLines, out List<string> clearDialogue, out List<string> failDialogue, out CustomerDialogue.DialogueState state)
    {
        if (PlayerPrefs.HasKey("CustomerPosX"))
        {
            float x = PlayerPrefs.GetFloat("CustomerPosX");
            float y = PlayerPrefs.GetFloat("CustomerPosY");
            float z = PlayerPrefs.GetFloat("CustomerPosZ");
            position = new Vector3(x, y, z);
            customerIndex = PlayerPrefs.GetInt("CustomerIndex");
            dialogueLines = new List<string>(PlayerPrefs.GetString("DialogueLines").Split(','));
            middleDialogueLines = new List<string>(PlayerPrefs.GetString("MiddleDialogueLines").Split(','));
            clearDialogue = new List<string>(PlayerPrefs.GetString("ClearDialogue").Split(','));
            failDialogue = new List<string>(PlayerPrefs.GetString("FailDialogue").Split(','));
            state = (CustomerDialogue.DialogueState)PlayerPrefs.GetInt("DialogueState");
            return true; // 상태를 성공적으로 로드
        }
        else
        {
            position = Vector3.zero;
            customerIndex = 0;
            dialogueLines = new List<string>();
            middleDialogueLines = new List<string>();
            clearDialogue = new List<string>();
            failDialogue = new List<string>();
            state = CustomerDialogue.DialogueState.Start;
            return false; // 상태 로드 실패
        }
    }
}

