using Unity.Collections;
using UnityEngine;

public class RewardData : MonoBehaviour
{
    private static RewardData instance;
    public static RewardData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RewardData>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("RewardData");
                    instance = obj.AddComponent<RewardData>();
                    DontDestroyOnLoad(obj);
                }
            }
            return instance;
        }
    }


    [SerializeField] private int reward_gold; // 칵테일 성공 보상

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public int Reward_Gold
    {
        get => reward_gold;
        set
        {
            reward_gold = Mathf.Max(0, value); // 0 이하로 내려가지 않도록 방지
            reward_gold = value;
            Debug.Log($"보유중인 골드 수량: {value}");  // 로그 출력
        }
    }
    public void AddGold(int amount)
    {
        Reward_Gold += amount;
    }
}
