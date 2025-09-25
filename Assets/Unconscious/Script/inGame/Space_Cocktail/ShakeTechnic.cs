using UnityEngine;

public class ShakeTechnic : MonoBehaviour
{
    private MakeCocktail makecocktail;

    private void Start()
    {
        makecocktail = FindObjectOfType<MakeCocktail>();
    }
    private void OnMouseDown()
    {
        makecocktail.SetMixStateShake();
    }
}
