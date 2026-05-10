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
    public Color pieceColor = new Color(0.2f, 0.75f, 0.35f, 1f);

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

        boardManager = FindFirstObjectByType<BoardManager>();
        pieceSpawner = FindFirstObjectByType<PieceSpawner>();
    }

    public void Setup(List<Vector2Int> newShape, GameObject blockPrefab, Color color)
    {
        shapeCells = new List<Vector2Int>(newShape);
        miniBlockPrefab = blockPrefab;
        pieceColor = color;

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
                img.color = pieceColor;
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

        bool placed = boardManager.TryPlaceShape(shapeCells, eventData.position);

        if (placed)
        {
            pieceSpawner.RemovePiece(this);
            Destroy(gameObject);
        }
        else
        {
            rectTransform.anchoredPosition = originalPosition;
        }
    }
}