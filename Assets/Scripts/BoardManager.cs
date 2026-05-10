using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardManager : MonoBehaviour
{
    [Header("Board Settings")]
    public int rows = 8;
    public int columns = 8;
    public float cellSize = 70f;
    public float spacing = 8f;

    [Header("References")]
    public RectTransform boardParent;
    public GameObject cellPrefab;
    public TextMeshProUGUI scoreText;

    private GridCell[,] grid;
    private int score = 0;
    private bool isClearing = false;

    private Color normalBlockColor = new Color(0.2f, 0.75f, 0.35f, 1f);

    private Color[] fruitColors =
    {
        new Color(1f, 0.1f, 0.1f, 1f),
        new Color(1f, 0.75f, 0.1f, 1f),
        new Color(1f, 0.25f, 0.35f, 1f),
        new Color(0.2f, 0.05f, 0.3f, 1f),
        new Color(1f, 0.9f, 0.1f, 1f),
        new Color(1f, 0.5f, 0.05f, 1f),
        new Color(0.4f, 0.9f, 0.2f, 1f)
    };

    void Start()
    {
        CreateBoard();
        UpdateScoreText();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FillRandomRowForTesting();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            FillRandomColumnForTesting();
        }
    }

    void CreateBoard()
    {
        grid = new GridCell[rows, columns];

        float boardWidth = columns * cellSize + (columns - 1) * spacing;
        float boardHeight = rows * cellSize + (rows - 1) * spacing;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                GameObject newCell = Instantiate(cellPrefab, boardParent);
                newCell.name = "Cell_" + r + "_" + c;

                RectTransform rect = newCell.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(cellSize, cellSize);

                float x = c * (cellSize + spacing) - boardWidth / 2f + cellSize / 2f;
                float y = -r * (cellSize + spacing) + boardHeight / 2f - cellSize / 2f;

                rect.anchoredPosition = new Vector2(x, y);

                GridCell cell = newCell.GetComponent<GridCell>();
                cell.Setup(r, c);

                grid[r, c] = cell;
            }
        }
    }

    void FillRandomRowForTesting()
    {
        if (isClearing) return;

        int randomRow = Random.Range(0, rows);

        for (int c = 0; c < columns; c++)
        {
            grid[randomRow, c].SetBlock(normalBlockColor);
        }

        AddScore(80);
        CheckCompletedLines();
    }

    void FillRandomColumnForTesting()
    {
        if (isClearing) return;

        int randomColumn = Random.Range(0, columns);

        for (int r = 0; r < rows; r++)
        {
            grid[r, randomColumn].SetBlock(normalBlockColor);
        }

        AddScore(80);
        CheckCompletedLines();
    }

    void CheckCompletedLines()
    {
        List<GridCell> completedCells = new List<GridCell>();

        for (int r = 0; r < rows; r++)
        {
            bool fullRow = true;

            for (int c = 0; c < columns; c++)
            {
                if (!grid[r, c].isOccupied)
                {
                    fullRow = false;
                    break;
                }
            }

            if (fullRow)
            {
                for (int c = 0; c < columns; c++)
                {
                    if (!completedCells.Contains(grid[r, c]))
                    {
                        completedCells.Add(grid[r, c]);
                    }
                }
            }
        }

        for (int c = 0; c < columns; c++)
        {
            bool fullColumn = true;

            for (int r = 0; r < rows; r++)
            {
                if (!grid[r, c].isOccupied)
                {
                    fullColumn = false;
                    break;
                }
            }

            if (fullColumn)
            {
                for (int r = 0; r < rows; r++)
                {
                    if (!completedCells.Contains(grid[r, c]))
                    {
                        completedCells.Add(grid[r, c]);
                    }
                }
            }
        }

        if (completedCells.Count > 0)
        {
            StartCoroutine(FruitClearEffect(completedCells));
        }
    }

    IEnumerator FruitClearEffect(List<GridCell> completedCells)
    {
        isClearing = true;

        foreach (GridCell cell in completedCells)
        {
            Color fruitColor = fruitColors[Random.Range(0, fruitColors.Length)];
            cell.SetFruitColor(fruitColor);
        }

        AddScore(completedCells.Count * 20);

        yield return new WaitForSeconds(0.35f);

        foreach (GridCell cell in completedCells)
        {
            cell.transform.localScale = Vector3.one * 1.3f;
        }

        yield return new WaitForSeconds(0.15f);

        foreach (GridCell cell in completedCells)
        {
            cell.ClearCell();
        }

        isClearing = false;
    }

    void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
    public bool TryPlaceShape(List<Vector2Int> shapeCells, Vector2 screenPosition)
{
    if (isClearing)
    {
        return false;
    }

    GridCell closestCell = GetClosestCell(screenPosition);

    if (closestCell == null)
    {
        return false;
    }

    int startRow = closestCell.row;
    int startColumn = closestCell.column;

    if (!CanPlaceShape(shapeCells, startRow, startColumn))
    {
        return false;
    }

    foreach (Vector2Int part in shapeCells)
    {
        int r = startRow + part.y;
        int c = startColumn + part.x;

        grid[r, c].SetBlock(normalBlockColor);
    }

    AddScore(shapeCells.Count * 10);
    CheckCompletedLines();

    return true;
}

public bool CanPlaceShape(List<Vector2Int> shapeCells, int startRow, int startColumn)
{
    foreach (Vector2Int part in shapeCells)
    {
        int r = startRow + part.y;
        int c = startColumn + part.x;

        if (r < 0 || r >= rows || c < 0 || c >= columns)
        {
            return false;
        }

        if (grid[r, c].isOccupied)
        {
            return false;
        }
    }

    return true;
}

private GridCell GetClosestCell(Vector2 screenPosition)
{
    GridCell closestCell = null;
    float closestDistance = float.MaxValue;

    foreach (GridCell cell in grid)
    {
        RectTransform cellRect = cell.GetComponent<RectTransform>();
        Vector2 cellScreenPosition = RectTransformUtility.WorldToScreenPoint(null, cellRect.position);

        float distance = Vector2.Distance(screenPosition, cellScreenPosition);

        if (distance < closestDistance)
        {
            closestDistance = distance;
            closestCell = cell;
        }
    }

    if (closestDistance > 60f)
    {
        return null;
    }

    return closestCell;
}
}