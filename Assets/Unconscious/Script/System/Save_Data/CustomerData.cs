using UnityEngine;

public class CustomerData : MonoBehaviour
{
    public static CustomerData Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void customerData_Save()
    {

    }
}
