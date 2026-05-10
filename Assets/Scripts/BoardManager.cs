using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        new Color(1f, 0.1f, 0.1f, 1f),      // Apple
        new Color(1f, 0.75f, 0.1f, 1f),     // Pineapple
        new Color(1f, 0.25f, 0.35f, 1f),    // Strawberry
        new Color(0.2f, 0.05f, 0.3f, 1f),   // Blackberry
        new Color(1f, 0.9f, 0.1f, 1f),      // Banana
        new Color(1f, 0.5f, 0.05f, 1f),     // Mango
        new Color(0.4f, 0.9f, 0.2f, 1f)     // Kiwi
    };

    private void Start()
    {
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

    private void Update()
    {
        // Temporary testing controls.
        // Press SPACE to fill a random row and see the fruit blast effect.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FillRandomRowForTesting();
        }

        // Press C to fill a random column and see the fruit blast effect.
        if (Input.GetKeyDown(KeyCode.C))
        {
            FillRandomColumnForTesting();
        }
    }

    private void FillRandomRowForTesting()
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

    private void FillRandomColumnForTesting()
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

    private void CheckCompletedLines()
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

    private IEnumerator FruitClearEffect(List<GridCell> completedCells)
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
}