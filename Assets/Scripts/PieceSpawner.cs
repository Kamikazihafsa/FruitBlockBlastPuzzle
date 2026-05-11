using System.Collections;
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
        // Single block
        new List<Vector2Int>
        {
            new Vector2Int(0, 0)
        },

        // 2 horizontal
        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0)
        },

        // 2 vertical
        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(0, 1)
        },

        // 3 horizontal
        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(2, 0)
        },

        // 3 vertical
        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, 2)
        },

        // 4 horizontal
        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(2, 0),
            new Vector2Int(3, 0)
        },

        // 4 vertical
        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, 2),
            new Vector2Int(0, 3)
        },

        // 5 horizontal
        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(2, 0),
            new Vector2Int(3, 0),
            new Vector2Int(4, 0)
        },

        // 5 vertical
        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, 2),
            new Vector2Int(0, 3),
            new Vector2Int(0, 4)
        },

        // 2x2 square
        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1)
        },

        // 3x3 square
        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(2, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(2, 1),
            new Vector2Int(0, 2),
            new Vector2Int(1, 2),
            new Vector2Int(2, 2)
        },

        // Small L
        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1)
        },

        // L shape
        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, 2),
            new Vector2Int(1, 2)
        },

        // Reverse L
        new List<Vector2Int>
        {
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(1, 2),
            new Vector2Int(0, 2)
        },

        // Wide L
        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(2, 1)
        },

        // Reverse wide L
        new List<Vector2Int>
        {
            new Vector2Int(2, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(2, 1)
        },

        // T shape
        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(2, 0),
            new Vector2Int(1, 1)
        },

        // Upside-down T
        new List<Vector2Int>
        {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(2, 1)
        },

        // Plus shape
        new List<Vector2Int>
        {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(2, 1),
            new Vector2Int(1, 2)
        },

        // Z shape
        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(2, 1)
        },

        // S shape
        new List<Vector2Int>
        {
            new Vector2Int(1, 0),
            new Vector2Int(2, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1)
        },

        // Corner 3 blocks
        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1)
        },

        // Diagonal style 3
        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 1),
            new Vector2Int(2, 2)
        },

        // Stair shape
        new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 2)
        }
    };

    private IEnumerator Start()
    {
        boardManager = FindFirstObjectByType<BoardManager>();

        yield return null;

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
            rect.sizeDelta = new Vector2(180, 160);
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
                selectedFruit = fruitSprites[Random.Range(0, fruitSprites.Length)];
            }

            blockPiece.Setup(randomShape, miniBlockPrefab, selectedFruit);
            activePieces.Add(blockPiece);
        }

        StartCoroutine(CheckGameOverDelayed());
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
            StartCoroutine(CheckGameOverDelayed());
        }
    }

    private IEnumerator CheckGameOverDelayed()
    {
        yield return new WaitForSeconds(0.7f);
        CheckGameOver();
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