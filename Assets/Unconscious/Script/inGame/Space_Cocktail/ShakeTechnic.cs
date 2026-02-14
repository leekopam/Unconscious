using UnityEngine;

public class ShakeTechnic : MonoBehaviour
{
    private MakeCocktail makeCocktail;

    private void Start()
    {
        makeCocktail = FindObjectOfType<MakeCocktail>();
    }

    private void OnMouseDown()
    {
        if (makeCocktail == null)
        {
            return;
        }

        makeCocktail.SetMixStateShake();
    }
}
