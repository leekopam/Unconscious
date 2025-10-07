using UnityEngine;
using UnityEngine.SceneManagement;

public class MainButten : MonoBehaviour
{
    public void MoveToOrderScene()
    {
        if (Game_Manager.Instance != null)
        {
            Game_Manager.Instance.ChangeScene(SceneNames.Order);
        }
        else
        {
            SceneManager.LoadScene(SceneNames.Order);
        }
    }
}
