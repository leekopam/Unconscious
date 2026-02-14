using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class Bell : MonoBehaviour
{
    private PlayableDirector playableDirector;
    private bool isPlay = false;

    private void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();

        // PlayableDirector가 자동으로 재생되지 않도록 설정
        if (playableDirector != null)
        {
            playableDirector.playOnAwake = false;
            playableDirector.stopped += OnPlayableDirectorStopped;
        }
    }

    private void OnDestroy()
    {
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
        if (Game_Manager.Instance != null)
        {
            Game_Manager.Instance.ChangeScene(SceneNames.Order);
        }
        else
        {
            SceneManager.LoadScene(SceneNames.Order);
        }

        isPlay = false;
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
        if (playableDirector == null)
        {
            Debug.LogError("PlayableDirector가 할당되지 않았습니다.");
            return;
        }

        if (playableDirector.playableAsset == null)
        {
            Debug.LogError("PlayableDirector에 연결된 PlayableAsset이 없습니다.");
            return;
        }

        Debug.Log("PlayableDirector가 재생됩니다.");
        playableDirector.Play();
        isPlay = true;
    }
}
