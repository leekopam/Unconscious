using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Manager : MonoBehaviour
{
    static Game_Manager instance;
    public static Game_Manager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Game_Manager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("Game_Manager");
                    instance = obj.AddComponent<Game_Manager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 씬 로드 이벤트 등록
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 씬이 로드된 후 호출되는 콜백
    /// </summary>
    /// <param name="scene">로드된 씬</param>
    /// <param name="mode">로드 모드</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // CustomerData 복원
        if (CustomerData.Instance != null)
        {
            CustomerData.Instance.OnSceneLoaded();
        }
    }

    /// <summary>
    /// 씬 전환 메서드 (고객 데이터 저장 포함)
    /// </summary>
    /// <param name="sceneName">전환할 씬 이름</param>
    public void ChangeSecene(string sceneName)
    {
        // 씬 전환 전 고객 데이터 저장
        if (CustomerData.Instance != null)
        {
            CustomerData.Instance.OnSceneChanging();
        }
        
        SceneManager.LoadScene(sceneName);
    }
}
