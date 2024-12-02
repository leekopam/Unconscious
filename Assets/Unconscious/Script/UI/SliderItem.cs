using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class SliderItem : MonoBehaviour
{
    [SerializeField] private float slideSpeed = 0.5f;
    [SerializeField] private float spacing = 200f;

    private List<RectTransform> images = new List<RectTransform>();
    private int currentIndex = 0;
    private RectTransform contentPanel;

    private void Start()
    {
        contentPanel = GetComponent<RectTransform>();
        // 모든 이미지를 리스트에 추가
        foreach (RectTransform child in transform)
        {
            images.Add(child);
        }

        // 초기 위치 설정
        ArrangeImages();
    }

    // 이미지 위치 설정
    private void ArrangeImages()
    {
        for (int i = 0; i < images.Count; i++)
        {
            Vector2 targetPosition = new Vector2(i * spacing, 0);
            images[i].anchoredPosition = targetPosition;
        }
    }

    // 오른쪽으로 이동
    public void SlideRight()
    {
        if (currentIndex < images.Count - 1)
        {
            currentIndex++;
            MoveToCurrentIndex();
        }
    }

    // 왼쪽으로 이동
    public void SlideLeft()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            MoveToCurrentIndex();
        }
    }

    // 현재 인덱스로 이동
    private void MoveToCurrentIndex()
    {
        float targetX = -currentIndex * spacing;
        contentPanel.DOAnchorPosX(targetX, slideSpeed)
            .SetEase(Ease.OutQuad);

        // 선택된 이미지 크기 조정
        for (int i = 0; i < images.Count; i++)
        {
            float scale = (i == currentIndex) ? 1.2f : 1f;
            images[i].DOScale(new Vector3(scale, scale, 1f), slideSpeed);
        }
    }
}