using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SpeechBubbleManager : MonoBehaviour
{
    public static SpeechBubbleManager Instance { get; private set; }

    [SerializeField] private GameObject speechBubblePrefab;
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private float BubblePos = 0f;

    private Dictionary<GameObject, SpeechBubble> activeBubbles = new Dictionary<GameObject, SpeechBubble>();
    private Dictionary<int, Vector3> savedBubblePositions = new Dictionary<int, Vector3>();
    private bool isInitialized = false;
    private bool isTransitioning = false;
    private static bool isFirstLoad = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManager();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeManager()
    {
        if (!isInitialized)
        {
            LoadResources();
            isInitialized = true;
        }
    }

    private void LoadResources()
    {
        if (speechBubblePrefab == null)
        {
            speechBubblePrefab = Resources.Load<GameObject>("Prefabs/SpeechBubblePrefab");
            if (speechBubblePrefab == null)
            {
                Debug.LogWarning("SpeechBubblePrefab을 Resources/Prefabs 폴더에서 찾을 수 없습니다.");
            }
        }

        if (dialogueData == null)
        {
            dialogueData = Resources.Load<DialogueData>("Data/DialogueData");
            if (dialogueData == null)
            {
                Debug.LogWarning("DialogueData를 Resources/Data 폴더에서 찾을 수 없습니다.");
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SaveBubblePositions();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Order")
        {
            StartCoroutine(SetupAfterSceneLoad());
        }
    }

    private IEnumerator SetupAfterSceneLoad()
    {
        yield return new WaitForSeconds(0.1f);
        SetupCanvas();

        string lastScene = PlayerPrefs.GetString("LastScene", "");
        if (lastScene == "Dessert")
        {
            ClearAllBubbles();
            yield return new WaitForSeconds(0.2f);
            RestoreBubbles();
        }
    }

    private void ClearAllBubbles()
    {
        // 기존 버블과 관련 객체들 정리
        foreach (var kvp in activeBubbles)
        {
            if (kvp.Key != null)
            {
                Destroy(kvp.Key); // 고객 프리팹 제거
            }
            if (kvp.Value != null)
            {
                Destroy(kvp.Value.gameObject); // 말풍선 프리팹 제거
            }
        }
        activeBubbles.Clear();
        savedBubblePositions.Clear();
    }

    public void ShowSpeechBubble(GameObject customer)
    {
        if (isTransitioning || customer == null) return;

        // 이미 말풍선이 있다면 제거
        if (activeBubbles.ContainsKey(customer))
        {
            RemoveSpeechBubble(customer);
        }

        CustomerController controller = customer.GetComponent<CustomerController>();
        if (controller == null) return;

        if (targetCanvas == null)
        {
            SetupCanvas();
        }

        GameObject bubbleObj = Instantiate(speechBubblePrefab, targetCanvas.transform);
        bubbleObj.SetActive(true);

        SpeechBubble bubble = bubbleObj.GetComponent<SpeechBubble>();
        bubble.OnDialogueComplete += () =>
        {
            isTransitioning = true;
            SaveBubblePositions();
            PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
            RemoveSpeechBubble(customer);
            SceneManager.LoadScene("Cocktail");
        };

        List<string> dialogueLines = dialogueData.GetDialogueForCustomer(controller.CustomerType);
        bubble.Initialize(dialogueLines);

        PositionBubbleAboveCustomer(bubbleObj, customer);
        activeBubbles.Add(customer, bubble);

        // 말풍선 프리팹을 DontDestroyOnLoad로 설정
        DontDestroyOnLoad(bubbleObj);
    }

    private void SaveBubblePositions()
    {
        savedBubblePositions.Clear();
        foreach (var bubble in activeBubbles)
        {
            if (bubble.Key != null)
            {
                CustomerController controller = bubble.Key.GetComponent<CustomerController>();
                if (controller != null)
                {
                    savedBubblePositions[controller.CustomerType] = bubble.Key.transform.position;
                    PlayerPrefs.SetInt($"CustomerType_{controller.CustomerType}", controller.CustomerType);
                    PlayerPrefs.SetFloat($"PosX_{controller.CustomerType}", bubble.Key.transform.position.x);
                    PlayerPrefs.SetFloat($"PosY_{controller.CustomerType}", bubble.Key.transform.position.y);
                    PlayerPrefs.SetFloat($"PosZ_{controller.CustomerType}", bubble.Key.transform.position.z);
                }
            }
        }
        PlayerPrefs.Save();
    }

    private void RestoreBubbles()
    {
        CustomerManager customerManager = FindObjectOfType<CustomerManager>();
        if (customerManager != null)
        {
            // 기존 고객들 제거
            customerManager.ClearAllCustomers();

            foreach (int customerType in savedBubblePositions.Keys)
            {
                if (PlayerPrefs.HasKey($"CustomerType_{customerType}"))
                {
                    Vector3 savedPosition = new Vector3(
                        PlayerPrefs.GetFloat($"PosX_{customerType}"),
                        PlayerPrefs.GetFloat($"PosY_{customerType}"),
                        PlayerPrefs.GetFloat($"PosZ_{customerType}")
                    );
                    customerManager.SpawnCustomerAtPosition(customerType, savedPosition);
                }
            }
        }
    }

    private void RemoveSpeechBubble(GameObject customer)
    {
        if (customer != null && activeBubbles.TryGetValue(customer, out SpeechBubble bubble))
        {
            if (bubble != null)
            {
                bubble.OnDialogueComplete = null;
                Destroy(bubble.gameObject);
            }
            activeBubbles.Remove(customer);
            Destroy(customer); // 말풍선과 함께 고객 프리팹도 제거
        }
    }

    private void SetupCanvas()
    {
        if (targetCanvas == null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
            if (targetCanvas == null)
            {
                Debug.LogWarning("Canvas를 찾을 수 없어 새로 생성합니다.");
                CreateNewCanvas();
            }
        }
    }

    private void CreateNewCanvas()
    {
        GameObject canvasObject = new GameObject("MainCanvas");
        targetCanvas = canvasObject.AddComponent<Canvas>();
        targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
    }

    private void PositionBubbleAboveCustomer(GameObject bubble, GameObject customer)
    {
        Vector3 customerPosition = customer.transform.position;
        Vector3 bubblePosition = new Vector3(
            customerPosition.x,
            customerPosition.y + BubblePos,
            customerPosition.z
        );
        bubble.transform.position = bubblePosition;
    }

    private void OnDestroy()
    {
        // 매니저가 파괴될 때 모든 리소스 정리
        ClearAllBubbles();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        PlayerPrefs.DeleteKey("LastScene");
        foreach (int customerType in savedBubblePositions.Keys)
        {
            PlayerPrefs.DeleteKey($"CustomerType_{customerType}");
            PlayerPrefs.DeleteKey($"PosX_{customerType}");
            PlayerPrefs.DeleteKey($"PosY_{customerType}");
            PlayerPrefs.DeleteKey($"PosZ_{customerType}");
        }
        PlayerPrefs.Save();
    }
}

