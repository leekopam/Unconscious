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

        // 애니메이션 완료 이벤트 리스너 추가
        playableDirector.stopped += OnPlayableDirectorStopped;
    }

    private void OnPlayableDirectorStopped(PlayableDirector playable)
    {
        // 애니메이션 완료 후 씬 전환
        if (isPlay)
        {
            SceneManager.LoadScene("Order");
            isPlay = false;
        }
    }

    private void OnDestroy()
    {
        // 애니메이션 완료 이벤트 리스너 제거
        playableDirector.stopped -= OnPlayableDirectorStopped;
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
        // 애니메이션 재생
        playableDirector.Play();
        isPlay = true;
    }
}
