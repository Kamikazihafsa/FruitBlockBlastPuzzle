using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    [Header("References")]
    public RectTransform piecePanel;
    public GameObject blockPiecePrefab;
    public GameObject miniBlockPrefab;

    [Header("Settings")]
    public int piecesPerRound = 3;

    private List<BlockPiece> activePieces = new List<BlockPiece>();

    private Color[] pieceColors =
    {
        new Color(0.95f, 0.2f, 0.2f, 1f),
        new Color(1f, 0.55f, 0.1f, 1f),
        new Color(1f, 0.85f, 0.1f, 1f),
        new Color(0.35f, 0.85f, 0.25f, 1f),
        new Color(0.2f, 0.65f, 1f, 1f),
        new Color(0.65f, 0.3f, 0.85f, 1f)
    };

    private List<List<Vector2Int>> shapes = new List<List<Vector2Int>>
    {
        new List<Vector2Int> { new Vector2Int(0,0) },

        new List<Vector2Int> { new Vector2Int(0,0), new Vector2Int(1,0) },

        new List<Vector2Int> { new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,0) },

        new List<Vector2Int> { new Vector2Int(0,0), new Vector2Int(0,1), new Vector2Int(0,2) },

        new List<Vector2Int> { new Vector2Int(0,0), new Vector2Int(1,0),
                               new Vector2Int(0,1), new Vector2Int(1,1) },

        new List<Vector2Int> { new Vector2Int(0,0), new Vector2Int(0,1), new Vector2Int(1,1) },

        new List<Vector2Int> { new Vector2Int(0,0), new Vector2Int(1,0),
                               new Vector2Int(2,0), new Vector2Int(1,1) }
    };

    private void Start()
    {
        SpawnThreePieces();
    }

    public void SpawnThreePieces()
    {
        ClearOldPieces();

        for (int i = 0; i < piecesPerRound; i++)
        {
            GameObject pieceObj = Instantiate(blockPiecePrefab, piecePanel);
            pieceObj.name = "BlockPiece_" + (i + 1);

            RectTransform rect = pieceObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(160, 140);
            rect.anchoredPosition = new Vector2(-250 + (i * 250), 10);

            BlockPiece blockPiece = pieceObj.GetComponent<BlockPiece>();

            List<Vector2Int> randomShape = shapes[Random.Range(0, shapes.Count)];
            Color randomColor = pieceColors[Random.Range(0, pieceColors.Length)];

            blockPiece.Setup(randomShape, miniBlockPrefab, randomColor);

            activePieces.Add(blockPiece);
        }
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