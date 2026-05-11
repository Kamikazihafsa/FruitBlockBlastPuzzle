using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    [Header("References")]
    public RectTransform piecePanel;
    public GameObject blockPiecePrefab;
    public GameObject miniBlockPrefab;

    [Header("Fruit Sprites")]
    public Sprite[] fruitSprites;

    [Header("Settings")]
    public int piecesPerRound = 3;

    private BoardManager boardManager;
    private List<BlockPiece> activePieces = new List<BlockPiece>();

    private List<List<Vector2Int>> shapes = new List<List<Vector2Int>>
    {
        new List<Vector2Int>
        {
            new Vector2Int(0, 0)
        },

        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0)
        },

        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(2, 0)
        },

        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, 2)
        },

        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1)
        },

        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1)
        },

        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(2, 0),
            new Vector2Int(1, 1)
        }
    };

    private void Start()
    {
        boardManager = FindFirstObjectByType<BoardManager>();
        SpawnThreePieces();
    }

    public void SpawnThreePieces()
    {
        ClearOldPieces();

        if (piecePanel == null || blockPiecePrefab == null || miniBlockPrefab == null)
        {
            Debug.LogError("PieceSpawner references are missing. Assign PiecePanel, BlockPiecePrefab, and MiniBlockPrefab.");
            return;
        }

        for (int i = 0; i < piecesPerRound; i++)
        {
            GameObject pieceObj = Instantiate(blockPiecePrefab, piecePanel);
            pieceObj.name = "BlockPiece_" + (i + 1);

            RectTransform rect = pieceObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(160, 140);
            rect.anchoredPosition = new Vector2(-300 + (i * 300), 20);

            BlockPiece blockPiece = pieceObj.GetComponent<BlockPiece>();

            if (blockPiece == null)
            {
                Debug.LogError("BlockPiecePrefab is missing the BlockPiece script.");
                continue;
            }

            List<Vector2Int> randomShape = shapes[Random.Range(0, shapes.Count)];

            Sprite selectedFruit = null;

            if (fruitSprites != null && fruitSprites.Length > 0)
            {
                selectedFruit = fruitSprites[i % fruitSprites.Length];
            }

            blockPiece.Setup(randomShape, miniBlockPrefab, selectedFruit);
            activePieces.Add(blockPiece);
        }

        CheckGameOver();
    }

    public void RemovePiece(BlockPiece piece)
    {
        if (activePieces.Contains(piece))
        {
            activePieces.Remove(piece);
        }

        if (activePieces.Count == 0)
        {
            SpawnThreePieces();
        }
        else
        {
            CheckGameOver();
        }
    }

    private void CheckGameOver()
    {
        if (boardManager == null)
        {
            return;
        }

        foreach (BlockPiece piece in activePieces)
        {
            if (piece != null && boardManager.CanAnyShapeFit(piece.shapeCells))
            {
                return;
            }
        }

        boardManager.ShowGameOver();
    }

    private void ClearOldPieces()
    {
        foreach (BlockPiece piece in activePieces)
        {
            if (piece != null)
            {
                Destroy(piece.gameObject);
            }
        }

        activePieces.Clear();
    }
}