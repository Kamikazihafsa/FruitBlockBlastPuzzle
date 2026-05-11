using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockPiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Piece Data")]
    public List<Vector2Int> shapeCells = new List<Vector2Int>();

    [Header("Visual Settings")]
    public GameObject miniBlockPrefab;
    public float miniBlockSize = 45f;
    public Sprite pieceSprite;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private BoardManager boardManager;
    private PieceSpawner pieceSpawner;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        Image parentImage = GetComponent<Image>();
        if (parentImage == null)
        {
            parentImage = gameObject.AddComponent<Image>();
        }

        // Keep it clickable but almost invisible.
        parentImage.color = new Color(1f, 1f, 1f, 0.01f);
        parentImage.raycastTarget = true;

        boardManager = FindFirstObjectByType<BoardManager>();
        pieceSpawner = FindFirstObjectByType<PieceSpawner>();
    }

    public void Setup(List<Vector2Int> newShape, GameObject blockPrefab, Sprite sprite)
    {
        shapeCells = new List<Vector2Int>(newShape);
        miniBlockPrefab = blockPrefab;
        pieceSprite = sprite;

        BuildVisualShape();
    }

    private void BuildVisualShape()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Vector2Int cell in shapeCells)
        {
            GameObject block = Instantiate(miniBlockPrefab, transform);
            RectTransform blockRect = block.GetComponent<RectTransform>();

            blockRect.sizeDelta = new Vector2(miniBlockSize, miniBlockSize);
            blockRect.anchoredPosition = new Vector2(cell.x * miniBlockSize, -cell.y * miniBlockSize);

            Image img = block.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = pieceSprite;
                img.color = Color.white;
                img.preserveAspect = true;

                // Important: mini blocks should not block the parent drag.
                img.raycastTarget = false;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

 public void OnEndDrag(PointerEventData eventData)
{
    canvasGroup.blocksRaycasts = true;

    if (boardManager == null)
    {
        rectTransform.anchoredPosition = originalPosition;
        return;
    }

    // Use the actual piece position, not only the mouse position.
    Vector2 pieceScreenPosition = RectTransformUtility.WorldToScreenPoint(null, rectTransform.position);

    bool placed = boardManager.TryPlaceShape(shapeCells, pieceScreenPosition, pieceSprite);

    if (placed)
    {
        if (pieceSpawner != null)
        {
            pieceSpawner.RemovePiece(this);
        }

        Destroy(gameObject);
    }
    else
    {
        rectTransform.anchoredPosition = originalPosition;
    }
}
}