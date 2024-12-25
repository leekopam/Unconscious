using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class Bell : MonoBehaviour
{
    private PlayableDirector playableDirector;
    private CustomerDialogue customerDialogue; // CustomerDialogue 참조
    private bool isPlay = false;

    private void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
        customerDialogue = FindAnyObjectByType<CustomerDialogue>(); // CustomerDialogue 참조를 찾음

        // PlayableDirector가 자동으로 재생되지 않도록 설정
        if (playableDirector != null)
        {
            playableDirector.playOnAwake = false; // 자동 재생 비활성화
            playableDirector.stopped += OnPlayableDirectorStopped;
        }
    }

    private void OnDestroy()
    {
        // 애니메이션 완료 이벤트 리스너 제거
        if (playableDirector != null)
        {
            playableDirector.stopped -= OnPlayableDirectorStopped;
        }
    }

    /// <summary>
    /// PlayableDirector가 멈췄을 때 호출
    /// </summary>
    private void OnPlayableDirectorStopped(PlayableDirector director)
    {
        if (customerDialogue != null)
        {
            // 대화 상태를 중간 상태로 설정
            customerDialogue.SetDialogueStateToMiddle();
            customerDialogue.SaveState(); // 상태를 저장

            // 씬 전환 정보 저장
            PlayerPrefs.SetString("LastScene", "Dessert");
            PlayerPrefs.Save();

            // Order 씬으로 이동
            SceneManager.LoadScene("Order");
        }
        else
        {
            Debug.LogError("CustomerDialogue가 할당되지 않았습니다.");
        }
        isPlay = false; // 애니메이션이 멈췄으므로 isPlay를 false로 설정
    }

    /// <summary>
    /// CustomerDialogue 설정
    /// </summary>
    public void SetCustomerDialogue(CustomerDialogue dialogue)
    {
        customerDialogue = dialogue;
    }

    // 마우스 클릭했을때 애니메이션 재생
    private void OnMouseDown()
    {
        if (!isPlay)
        {
            PlayAnimation();
        }
    }

    private void PlayAnimation()
    {
        if (playableDirector != null)
        {
            if (playableDirector.playableAsset != null)
            {
                Debug.Log("PlayableDirector가 재생됩니다.");
                playableDirector.Play();
                isPlay = true;
            }
            else
            {
                Debug.LogError("PlayableDirector에 연결된 PlayableAsset이 없습니다.");
            }
        }
        else
        {
            Debug.LogError("PlayableDirector가 할당되지 않았습니다.");
        }
    }
}
