using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class GarnishSelecter : MonoBehaviour
{
    [SerializeField] private Image[] carouselImages;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [SerializeField] private List<GameObject> oliveObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> flowerObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> lemonObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> sugarObjects = new List<GameObject>();

    private List<List<GameObject>> garnishLists;

    private int currentIndex = 1; // 초기 중앙 인덱스
    [SerializeField] private float imageMovementDistance = 150f;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float selectImgScale = 1.5f;
    [SerializeField] private float normalImgScale = 1.0f;
    [SerializeField] private float imageSpacing = 50f;

    private void Start()
    {
        InitializeGarnishLists();
        InitializeCarousel();
        SetupButtonListeners();
    }

    private void InitializeGarnishLists()
    {
        garnishLists = new List<List<GameObject>>
        {
            oliveObjects,
            flowerObjects,
            lemonObjects,
            sugarObjects
        };
    }

    private void InitializeCarousel()
    {
        for (int i = 0; i < carouselImages.Length; i++)
        {
            float xPosition = CalculateImagePosition(i);
            carouselImages[i].rectTransform.anchoredPosition = new Vector2(
                xPosition,
                carouselImages[i].rectTransform.anchoredPosition.y
            );

            if (i == currentIndex)
            {
                carouselImages[i].gameObject.SetActive(true);
                ScaleImage(carouselImages[i], Vector3.one * selectImgScale);
                ToggleGarnishObjects(i, true);
            }
            else
            {
                carouselImages[i].gameObject.SetActive(false);
                ScaleImage(carouselImages[i], Vector3.one * normalImgScale);
                ToggleGarnishObjects(i, false);
            }
        }
    }

    private float CalculateImagePosition(int index)
    {
        return (index - currentIndex) * (imageMovementDistance + imageSpacing);
    }

    private void SetupButtonListeners()
    {
        leftButton.onClick.AddListener(() => MoveCarousel(-1));
        rightButton.onClick.AddListener(() => MoveCarousel(1));
    }

    private void MoveCarousel(int direction)
    {
        if (currentIndex + direction < 0 || currentIndex + direction >= carouselImages.Length)
            return;

        currentIndex += direction;

        for (int i = 0; i < carouselImages.Length; i++)
        {
            AnimateAnchoredPosX(carouselImages[i].rectTransform, CalculateImagePosition(i), animationDuration);

            if (i == currentIndex)
            {
                carouselImages[i].gameObject.SetActive(true);
                carouselImages[i].rectTransform.DOScale(Vector3.one * selectImgScale, animationDuration);
                ToggleGarnishObjects(i, true);
            }
            else
            {
                carouselImages[i].gameObject.SetActive(false);
                carouselImages[i].rectTransform.DOScale(Vector3.one * normalImgScale, animationDuration);
                ToggleGarnishObjects(i, false);
            }
        }
    }

    private void AnimateAnchoredPosX(RectTransform rectTransform, float targetX, float duration)
    {
        if (rectTransform == null)
        {
            return;
        }

        Vector2 current = rectTransform.anchoredPosition;
        Vector2 target = new Vector2(targetX, current.y);

        DOTween.To(
            () => rectTransform.anchoredPosition,
            value => rectTransform.anchoredPosition = value,
            target,
            duration
        );
    }

    private void ToggleGarnishObjects(int index, bool isActive)
    {
        if (index >= 0 && index < garnishLists.Count)
        {
            foreach (GameObject obj in garnishLists[index])
            {
                obj.SetActive(isActive);
            }
        }
    }

    private void ScaleImage(Image image, Vector3 scale)
    {
        image.rectTransform.localScale = scale;
    }
}
