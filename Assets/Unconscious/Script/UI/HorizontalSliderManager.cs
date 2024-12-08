using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalSliderManager : MonoBehaviour
{
    [Header("슬라이더 설정")]
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private List<CategoryItem> categoryItems;
    [SerializeField] private float itemSpacing = 200f;
    [SerializeField] private float slideSpeed = 0.3f;
    [SerializeField] private float selectedScale = 1.3f;
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float sideItemScale = 0.8f;  // 좌우 아이템 크기

    private int currentIndex = 0;
    private bool isSliding = false;

    [System.Serializable]
    private class CategoryItem
    {
        public Image icon;
        public GameObject linkedObject;
        public CanvasGroup canvasGroup;  // 투명도 조절을 위한 컴포넌트
    }

    private void Start()
    {
        InitializeSlider();
        UpdateVisibility();
    }

    private void InitializeSlider()
    {
        for (int i = 0; i < categoryItems.Count; i++)
        {
            RectTransform itemRect = categoryItems[i].icon.rectTransform;
            float currentY = itemRect.anchoredPosition.y;
            itemRect.anchoredPosition = new Vector2(i * itemSpacing, currentY);

            // CanvasGroup 컴포넌트 추가
            if (categoryItems[i].canvasGroup == null)
            {
                categoryItems[i].canvasGroup = categoryItems[i].icon.gameObject.AddComponent<CanvasGroup>();
            }
        }
    }

    private void UpdateVisibility()
    {
        for (int i = 0; i < categoryItems.Count; i++)
        {
            bool isVisible = (i == currentIndex ||
                            i == (currentIndex - 1) ||
                            i == (currentIndex + 1));

            // 보이는 아이템들의 크기와 투명도 설정
            float targetScale;
            float targetAlpha;

            if (i == currentIndex)
            {
                targetScale = selectedScale;
                targetAlpha = 1f;
            }
            else if (isVisible)
            {
                targetScale = sideItemScale;
                targetAlpha = 0.7f;
            }
            else
            {
                targetScale = normalScale;
                targetAlpha = 0f;
            }

            // 크기와 투명도 애니메이션 적용
            categoryItems[i].icon.transform.DOScale(targetScale, slideSpeed);
            categoryItems[i].canvasGroup.DOFade(targetAlpha, slideSpeed);

            // 연결된 오브젝트 활성화/비활성화
            if (categoryItems[i].linkedObject != null)
            {
                categoryItems[i].linkedObject.SetActive(i == currentIndex);
            }
        }
    }

    public void SlideRight()
    {
        if (isSliding || currentIndex >= categoryItems.Count - 1) return;

        isSliding = true;
        currentIndex++;
        SlideToCurrentIndex();
    }

    public void SlideLeft()
    {
        if (isSliding || currentIndex <= 0) return;

        isSliding = true;
        currentIndex--;
        SlideToCurrentIndex();
    }

    private void SlideToCurrentIndex()
    {
        float targetX = -currentIndex * itemSpacing;
        contentPanel.DOAnchorPosX(targetX, slideSpeed)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                isSliding = false;
                UpdateVisibility();
            });
    }
}