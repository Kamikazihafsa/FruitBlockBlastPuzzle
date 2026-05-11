using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

    [Header("Fruit Sprites For Clear Effect")]
    public Sprite[] fruitSprites;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;

    private GridCell[,] grid;
    private int score = 0;
    private bool isClearing = false;
    private bool isGameOver = false;

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        CreateBoard();
        UpdateScoreText();
    }

    private void CreateBoard()
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

    public bool TryPlaceShape(List<Vector2Int> shapeCells, Vector2 screenPosition, Sprite blockSprite)
    {
        if (isClearing || isGameOver)
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

            grid[r, c].SetBlock(blockSprite);
        }

        AddScore(shapeCells.Count * 10);
        CheckCompletedLines();

        return true;
    }

    public bool CanPlaceShape(List<Vector2Int> shapeCells, int startRow, int startColumn)
    {
        if (grid == null || shapeCells == null)
        {
            return false;
        }

        foreach (Vector2Int part in shapeCells)
        {
            int r = startRow + part.y;
            int c = startColumn + part.x;

            if (r < 0 || r >= rows || c < 0 || c >= columns)
            {
                return false;
            }

            if (grid[r, c] == null || grid[r, c].isOccupied)
            {
                return false;
            }
        }

        return true;
    }

    public bool CanAnyShapeFit(List<Vector2Int> shapeCells)
    {
        if (grid == null || shapeCells == null)
        {
            return false;
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (CanPlaceShape(shapeCells, r, c))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private GridCell GetClosestCell(Vector2 screenPosition)
    {
        if (grid == null)
        {
            return null;
        }

        GridCell closestCell = null;
        float closestDistance = float.MaxValue;

        foreach (GridCell cell in grid)
        {
            if (cell == null)
            {
                continue;
            }

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

    private void CheckCompletedLines()
    {
        List<GridCell> completedCells = new List<GridCell>();

        for (int r = 0; r < rows; r++)
        {
            bool fullRow = true;

            for (int c = 0; c < columns; c++)
            {
                if (grid[r, c] == null || !grid[r, c].isOccupied)
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
                if (grid[r, c] == null || !grid[r, c].isOccupied)
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

    private IEnumerator FruitClearEffect(List<GridCell> completedCells)
    {
        isClearing = true;

        if (fruitSprites != null && fruitSprites.Length > 0)
        {
            foreach (GridCell cell in completedCells)
            {
                Sprite randomFruit = fruitSprites[Random.Range(0, fruitSprites.Length)];
                cell.SetFruitSprite(randomFruit);
            }
        }

        AddScore(completedCells.Count * 20);

        yield return new WaitForSeconds(0.35f);

        foreach (GridCell cell in completedCells)
        {
            if (cell != null)
            {
                cell.transform.localScale = Vector3.one * 1.25f;
            }
        }

        yield return new WaitForSeconds(0.15f);

        foreach (GridCell cell in completedCells)
        {
            if (cell != null)
            {
                cell.ClearCell();
            }
        }

        isClearing = false;
    }

    private void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public void ShowGameOver()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}