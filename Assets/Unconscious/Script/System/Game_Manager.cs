using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneNames
{
    public const string Order = "Order";
    public const string Cocktail = "Cocktail";
    public const string Dessert = "Dessert";
}

public class Game_Manager : MonoBehaviour
{
    private static Game_Manager instance;

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
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CustomerData customerData = FindObjectOfType<CustomerData>();
        if (customerData != null)
        {
            customerData.OnSceneLoaded();
        }
    }

    /// <summary>
    /// 씬 전환 메서드 (고객 데이터 저장 포함)
    /// </summary>
    public void ChangeScene(string sceneName)
    {
        CustomerData customerData = FindObjectOfType<CustomerData>();
        if (customerData != null)
        {
            customerData.OnSceneChanging();
        }

        SceneManager.LoadScene(sceneName);
    }

    // 기존 오탈자 메서드와의 호환
    public void ChangeSecene(string sceneName)
    {
        ChangeScene(sceneName);
    }
}

