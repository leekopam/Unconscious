using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalSliderManager : MonoBehaviour
{
    [Header("카테고리별 컨텐츠 패널")]
    [SerializeField] private RectTransform snackContentPanel;
    [SerializeField] private RectTransform glassContentPanel;
    [SerializeField] private RectTransform garnishContentPanel;

    [Header("버튼 참조")]
    [SerializeField] private Button snackLeftButton;
    [SerializeField] private Button snackRightButton;
    [SerializeField] private Button glassLeftButton;
    [SerializeField] private Button glassRightButton;
    [SerializeField] private Button garnishLeftButton;
    [SerializeField] private Button garnishRightButton;

    [Header("카테고리별 아이템 설정")]
    [SerializeField] private List<CategoryItem> snackItems;
    [SerializeField] private List<CategoryItem> glassItems;
    [SerializeField] private List<CategoryItem> garnishItems;

    [Header("슬라이더 기본 설정")]
    [SerializeField] private float itemSpacing = 200f;
    [SerializeField] private float slideSpeed = 0.3f;
    [SerializeField] private float selectedScale = 1.3f;
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float sideItemScale = 0.8f;

    private int currentSnackIndex = 0;
    private int currentGlassIndex = 0;
    private int currentGarnishIndex = 0;
    private Canvas buttonCanvas;
    private Dictionary<RectTransform, bool> isSliding = new Dictionary<RectTransform, bool>();

    [System.Serializable]
    private class CategoryItem
    {
        public GameObject itemObject;
        public Image previewImage;
        public GameObject linkedObject;
        public CanvasGroup canvasGroup;
    }

    private void CreateButtonCanvas()
    {
        GameObject canvasObj = new GameObject("ButtonCanvas");
        buttonCanvas = canvasObj.AddComponent<Canvas>();
        buttonCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        buttonCanvas.sortingOrder = 100;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
    }

    private void Awake()
    {
        InitializeSlidingState();
        SetupButtonListeners();
    }

    private void Start()
    {
        InitializeAllSliders();
        UpdateAllVisibility();
        SetupGarnishItems();
        OptimizeAllButtons();

        // 초기 상태 설정
        UpdateButtonState("snack", currentSnackIndex, snackItems);
        UpdateButtonState("glass", currentGlassIndex, glassItems);
        UpdateButtonState("garnish", currentGarnishIndex, garnishItems);
    }

    private void InitializeSlidingState()
    {
        isSliding[snackContentPanel] = false;
        isSliding[glassContentPanel] = false;
        isSliding[garnishContentPanel] = false;
    }

    private void SetupButtonListeners()
    {
        if (garnishLeftButton != null)
        {
            garnishLeftButton.onClick.AddListener(() => SlideCategory("garnish", false));
        }
        if (garnishRightButton != null)
        {
            garnishRightButton.onClick.AddListener(() => SlideCategory("garnish", true));
        }
        if (snackLeftButton != null)
        {
            snackLeftButton.onClick.AddListener(() => SlideCategory("snack", false));
        }
        if (snackRightButton != null)
        {
            snackRightButton.onClick.AddListener(() => SlideCategory("snack", true));
        }
        if (glassLeftButton != null)
        {
            glassLeftButton.onClick.AddListener(() => SlideCategory("glass", false));
        }
        if (glassRightButton != null)
        {
            glassRightButton.onClick.AddListener(() => SlideCategory("glass", true));
        }
    }

    private void InitializeAllSliders()
    {
        InitializeSlider(snackItems, snackContentPanel);
        InitializeSlider(glassItems, glassContentPanel);
        InitializeSlider(garnishItems, garnishContentPanel);
    }


    private void InitializeSlider(List<CategoryItem> items, RectTransform contentPanel)
    {
        if (items == null || contentPanel == null) return;

        contentPanel.anchorMin = Vector2.one * 0.5f;
        contentPanel.anchorMax = Vector2.one * 0.5f;
        contentPanel.pivot = Vector2.one * 0.5f;

        GameObject container = new GameObject("ItemContainer");
        container.transform.SetParent(contentPanel, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = Vector2.one * 0.5f;
        containerRect.anchorMax = Vector2.one * 0.5f;
        containerRect.pivot = Vector2.one * 0.5f;

        float totalWidth = (items.Count - 1) * itemSpacing;
        containerRect.sizeDelta = new Vector2(totalWidth, contentPanel.sizeDelta.y);

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            if (item.previewImage == null) continue;

            item.previewImage.transform.SetParent(containerRect, false);
            RectTransform imageRect = item.previewImage.rectTransform;
            imageRect.anchorMin = Vector2.one * 0.5f;
            imageRect.anchorMax = Vector2.one * 0.5f;
            imageRect.pivot = Vector2.one * 0.5f;
            imageRect.sizeDelta = new Vector2(200f, 200f);
            imageRect.anchoredPosition = new Vector2(i * itemSpacing - totalWidth / 2, 0);

            if (item.canvasGroup == null)
                item.canvasGroup = item.previewImage.gameObject.AddComponent<CanvasGroup>();

            item.previewImage.transform.localScale = normalScale * Vector3.one;

            if (item.linkedObject != null)
                item.linkedObject.SetActive(i == 0);
        }
    }

    private void SetupGarnishItems()
    {
        foreach (var glassItem in glassItems)
        {
            if (glassItem.itemObject != null)
            {
                var garnishChildren = glassItem.itemObject.GetComponentsInChildren<Transform>(true);
                int garnishIndex = 0;

                foreach (var child in garnishChildren)
                {
                    if (child.CompareTag("Garnish") && garnishIndex < garnishItems.Count)
                    {
                        garnishItems[garnishIndex].linkedObject = child.gameObject;
                        child.gameObject.SetActive(false);
                        garnishIndex++;
                    }
                }
            }
        }

        if (garnishItems.Count > 0 && garnishItems[0].linkedObject != null)
        {
            garnishItems[0].linkedObject.SetActive(true);
        }
    }

    private void UpdateAllVisibility()
    {
        UpdateCategoryVisibility(snackItems, currentSnackIndex);
        UpdateCategoryVisibility(glassItems, currentGlassIndex);
        UpdateCategoryVisibility(garnishItems, currentGarnishIndex);
    }

    private void UpdateCategoryVisibility(List<CategoryItem> items, int currentIndex)
    {
        if (items == null) return;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].previewImage == null || items[i].canvasGroup == null)
                continue;

            int distance = Mathf.Abs(i - currentIndex);

            // 즉시 적용할 스케일과 알파값 설정
            float scale;
            float alpha;

            if (distance == 0)
            {
                scale = selectedScale;
                alpha = 1f;
            }
            else if (distance == 1)
            {
                scale = sideItemScale;
                alpha = 1f;
            }
            else
            {
                scale = normalScale;
                alpha = 0f;
            }

            // 즉시 적용
            items[i].canvasGroup.alpha = alpha;
            items[i].previewImage.transform.localScale = new Vector3(scale, scale, scale);

            // 연결된 오브젝트 처리
            if (items[i].linkedObject != null)
            {
                items[i].linkedObject.SetActive(i == currentIndex);
            }
        }
    }


    public void SlideCategory(string category, bool isRight)
    {
        switch (category.ToLower())
        {
            case "snack":
                if (isRight) SlideRight(ref currentSnackIndex, snackItems, snackContentPanel);
                else SlideLeft(ref currentSnackIndex, snackItems, snackContentPanel);
                UpdateButtonState(category, currentSnackIndex, snackItems);
                break;
            case "glass":
                if (isRight) SlideRight(ref currentGlassIndex, glassItems, glassContentPanel);
                else SlideLeft(ref currentGlassIndex, glassItems, glassContentPanel);
                UpdateButtonState(category, currentGlassIndex, glassItems);
                break;
            case "garnish":
                if (isRight) SlideRight(ref currentGarnishIndex, garnishItems, garnishContentPanel);
                else SlideLeft(ref currentGarnishIndex, garnishItems, garnishContentPanel);
                UpdateButtonState(category, currentGarnishIndex, garnishItems);
                break;
        }
    }

    private void UpdateButtonState(string category, int currentIndex, List<CategoryItem> items)
    {
        switch (category)
        {
            case "snack":
                snackLeftButton.interactable = currentIndex > 0;
                snackRightButton.interactable = currentIndex < items.Count - 1;
                break;
            case "glass":
                glassLeftButton.interactable = currentIndex > 0;
                glassRightButton.interactable = currentIndex < items.Count - 1;
                break;
            case "garnish":
                garnishLeftButton.interactable = currentIndex > 0;
                garnishRightButton.interactable = currentIndex < items.Count - 1;
                break;
        }
    }

    private void SlideRight(ref int currentIndex, List<CategoryItem> items, RectTransform contentPanel)
    {
        if (isSliding[contentPanel] || currentIndex >= items.Count - 1) return;
        isSliding[contentPanel] = true;
        currentIndex++;
        SlideToIndex(currentIndex, items, contentPanel);
    }

    private void SlideLeft(ref int currentIndex, List<CategoryItem> items, RectTransform contentPanel)
    {
        if (isSliding[contentPanel] || currentIndex <= 0) return;
        isSliding[contentPanel] = true;
        currentIndex--;
        SlideToIndex(currentIndex, items, contentPanel);
    }

    private void SlideToIndex(int index, List<CategoryItem> items, RectTransform contentPanel)
    {
        if (items == null || contentPanel == null) return;

        float totalWidth = (items.Count - 1) * itemSpacing;
        float targetX = index * itemSpacing - totalWidth / 2;

        Sequence sequence = DOTween.Sequence();

        // 패널 이동 애니메이션
        sequence.Append(
            contentPanel.DOAnchorPosX(-targetX, slideSpeed)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true)
        );

        // 애니메이션 완료 후 처리
        sequence.OnComplete(() => {
            isSliding[contentPanel] = false;
            UpdateCategoryVisibility(items, index);
        });

        sequence.Play();
    }

    private void OptimizeAllButtons()
    {
        OptimizeButton(snackLeftButton);
        OptimizeButton(snackRightButton);
        OptimizeButton(glassLeftButton);
        OptimizeButton(glassRightButton);
        OptimizeButton(garnishLeftButton);
        OptimizeButton(garnishRightButton);
    }

    private void OptimizeButton(Button button)
    {
        if (button == null) return;

        button.navigation = new Navigation { mode = Navigation.Mode.None };
        button.transition = Selectable.Transition.ColorTint;

        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        button.colors = colors;

        var image = button.GetComponent<Image>();
        if (image != null)
        {
            image.raycastTarget = true;
        }
    }

    private void OnDestroy()
    {
        DOTween.Kill(snackContentPanel);
        DOTween.Kill(glassContentPanel);
        DOTween.Kill(garnishContentPanel);
    }
}