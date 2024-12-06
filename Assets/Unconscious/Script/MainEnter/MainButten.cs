using UnityEngine;
using UnityEngine.SceneManagement;

public class MainButten : MonoBehaviour
{
    public void MoveToOrderScene()
    {
        SceneManager.LoadScene("Order");
    }
}
