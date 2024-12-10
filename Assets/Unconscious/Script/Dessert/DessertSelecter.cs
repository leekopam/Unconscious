using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DessertSelecter : MonoBehaviour
{
    [SerializeField] private Image[] carouselImages;
    [SerializeField] private GameObject[] relatedObjects;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    private int currentIndex = 1; // 초기 중앙 인덱스
    [SerializeField] private float imageMovementDistance = 150f; // 이미지 이동 거리
    [SerializeField] private float animationDuration = 0.5f; // 애니메이션 지속 시간
    [SerializeField] private float selectImgScale = 1.5f;
    [SerializeField] private float normalImgScale = 1.0f;
    [SerializeField] private float imageSpacing = 50f; // 이미지 간 간격

    private void Start()
    {
        InitializeCarousel();
        SetupButtonListeners();
    }

    private void InitializeCarousel()
    {
        for (int i = 0; i < carouselImages.Length; i++)
        {
            // 이미지 위치 및 크기 조정
            float xPosition = CalculateImagePosition(i);
            carouselImages[i].rectTransform.anchoredPosition = new Vector2(
                xPosition,
                carouselImages[i].rectTransform.anchoredPosition.y
            );

            // 이미지 선택 상태에 따른 처리
            if (i == currentIndex)
            {
                carouselImages[i].gameObject.SetActive(true);
                ScaleImage(carouselImages[i], Vector3.one * selectImgScale);
                ToggleRelatedObject(i, true);
            }
            else
            {
                carouselImages[i].gameObject.SetActive(false);
                ScaleImage(carouselImages[i], Vector3.one * normalImgScale);
                ToggleRelatedObject(i, false);
            }
        }
    }

    private float CalculateImagePosition(int index)
    {
        // 이미지 위치 계산 메서드
        return (index - currentIndex) * (imageMovementDistance + imageSpacing);
    }

    private void SetupButtonListeners()
    {
        leftButton.onClick.AddListener(() => MoveCarousel(-1));
        rightButton.onClick.AddListener(() => MoveCarousel(1));
    }

    private void MoveCarousel(int direction)
    {
        // 범위 체크
        if (currentIndex + direction < 0 || currentIndex + direction >= carouselImages.Length)
            return;

        currentIndex += direction;

        for (int i = 0; i < carouselImages.Length; i++)
        {
            // 이미지 위치 애니메이션
            carouselImages[i].rectTransform.DOAnchorPosX(
                CalculateImagePosition(i),
                animationDuration
            );

            // 이미지 활성화/비활성화 및 크기 조정
            if (i == currentIndex)
            {
                carouselImages[i].gameObject.SetActive(true);
                carouselImages[i].rectTransform.DOScale(Vector3.one * selectImgScale, animationDuration);
                ToggleRelatedObject(i, true);
            }
            else
            {
                carouselImages[i].gameObject.SetActive(false);
                carouselImages[i].rectTransform.DOScale(Vector3.one * normalImgScale, animationDuration);
                ToggleRelatedObject(i, false);
            }
        }
    }

    private void ScaleImage(Image image, Vector3 scale)
    {
        image.rectTransform.localScale = scale;
    }

    private void ToggleRelatedObject(int index, bool isActive)
    {
        if (index >= 0 && index < relatedObjects.Length)
        {
            relatedObjects[index].SetActive(isActive);
        }
    }
}
