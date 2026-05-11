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

    [Header("Drag Settings")]
    public float dragLiftOffset = 35f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private BoardManager boardManager;
    private PieceSpawner pieceSpawner;

    private Transform placementAnchorBlock;

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

        // Keep the object clickable, but remove visible background.
        parentImage.sprite = null;
        parentImage.color = new Color(1f, 1f, 1f, 0f);
        parentImage.raycastTarget = true;

        boardManager = FindFirstObjectByType<BoardManager>();
        pieceSpawner = FindFirstObjectByType<PieceSpawner>();
    }

    public void Setup(List<Vector2Int> newShape, GameObject blockPrefab, Sprite sprite)
    {
        shapeCells = NormalizeShape(newShape);
        miniBlockPrefab = blockPrefab;
        pieceSprite = sprite;

        BuildVisualShape();
    }

    private List<Vector2Int> NormalizeShape(List<Vector2Int> originalShape)
    {
        List<Vector2Int> normalized = new List<Vector2Int>();

        if (originalShape == null || originalShape.Count == 0)
        {
            return normalized;
        }

        int minX = originalShape[0].x;
        int minY = originalShape[0].y;

        foreach (Vector2Int cell in originalShape)
        {
            if (cell.x < minX) minX = cell.x;
            if (cell.y < minY) minY = cell.y;
        }

        foreach (Vector2Int cell in originalShape)
        {
            normalized.Add(new Vector2Int(cell.x - minX, cell.y - minY));
        }

        return normalized;
    }

    private void BuildVisualShape()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        placementAnchorBlock = null;

        if (shapeCells == null || shapeCells.Count == 0)
        {
            return;
        }

        int maxX = 0;
        int maxY = 0;

        foreach (Vector2Int cell in shapeCells)
        {
            if (cell.x > maxX) maxX = cell.x;
            if (cell.y > maxY) maxY = cell.y;
        }

        float pieceWidth = (maxX + 1) * miniBlockSize;
        float pieceHeight = (maxY + 1) * miniBlockSize;

        rectTransform.sizeDelta = new Vector2(pieceWidth, pieceHeight);

        foreach (Vector2Int cell in shapeCells)
        {
            GameObject block = Instantiate(miniBlockPrefab, transform);
            block.name = "MiniBlock_" + cell.x + "_" + cell.y;

            RectTransform blockRect = block.GetComponent<RectTransform>();
            blockRect.sizeDelta = new Vector2(miniBlockSize, miniBlockSize);

            // Center the full shape inside the parent.
            float x = (cell.x * miniBlockSize) - (pieceWidth / 2f) + (miniBlockSize / 2f);
            float y = -(cell.y * miniBlockSize) + (pieceHeight / 2f) - (miniBlockSize / 2f);

            blockRect.anchoredPosition = new Vector2(x, y);

            Image img = block.GetComponent<Image>();

            if (img != null)
            {
                img.sprite = pieceSprite;
                img.color = Color.white;
                img.preserveAspect = true;
                img.raycastTarget = false;
            }

            // Use the first cell as the drop reference.
            if (cell.x == 0 && cell.y == 0)
            {
                placementAnchorBlock = block.transform;
            }
        }

        if (placementAnchorBlock == null && transform.childCount > 0)
        {
            placementAnchorBlock = transform.GetChild(0);
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
        rectTransform.position = eventData.position + new Vector2(0f, dragLiftOffset);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (boardManager == null)
        {
            rectTransform.anchoredPosition = originalPosition;
            return;
        }

        Vector2 dropPosition;

        if (placementAnchorBlock != null)
        {
            dropPosition = RectTransformUtility.WorldToScreenPoint(null, placementAnchorBlock.position);
        }
        else
        {
            dropPosition = eventData.position;
        }

        bool placed = boardManager.TryPlaceShape(shapeCells, dropPosition, pieceSprite);

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