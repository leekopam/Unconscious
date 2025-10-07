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

    [SerializeField] private int reward_gold; // 현재 보유 골드

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int Reward_Gold
    {
        get => reward_gold;
        set
        {
            reward_gold = Mathf.Max(0, value); // 0 이하로 내려가지 않도록 방지
            Debug.Log($"보유중인 골드 수량: {reward_gold}");
        }
    }

    public void AddGold(int amount)
    {
        Reward_Gold += amount;
    }
}
