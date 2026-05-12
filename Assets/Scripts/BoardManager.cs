using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Score UI")]
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalBestScoreText;

    [Header("Fruit Sprites For Clear Effect")]
    public Sprite[] fruitSprites;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;

    private GridCell[,] grid;
    private int score = 0;
    private int bestScore = 0;
    private bool isClearing = false;
    private bool isGameOver = false;

    private void Start()
    {
        bestScore = PlayerPrefs.GetInt("BestScore", 0);

        ConfigureScoreTextUI();
        ConfigureGameOverPanelUI();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        CreateBoard();
        UpdateScoreText();
    }

    private void ConfigureScoreTextUI()
    {
        if (scoreText != null)
        {
            RectTransform rect = scoreText.GetComponent<RectTransform>();

            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 1f);
                rect.anchorMax = new Vector2(0.5f, 1f);
                rect.pivot = new Vector2(0.5f, 1f);
                rect.anchoredPosition = new Vector2(0f, -25f);
                rect.sizeDelta = new Vector2(700f, 90f);
            }

            scoreText.fontSize = 48;
            scoreText.color = Color.yellow;
            scoreText.alignment = TextAlignmentOptions.Center;
            scoreText.textWrappingMode = TextWrappingModes.NoWrap;
            scoreText.transform.SetAsLastSibling();
        }

        if (bestScoreText != null)
        {
            RectTransform bestRect = bestScoreText.GetComponent<RectTransform>();

            if (bestRect != null)
            {
                bestRect.anchorMin = new Vector2(1f, 1f);
                bestRect.anchorMax = new Vector2(1f, 1f);
                bestRect.pivot = new Vector2(1f, 1f);
                bestRect.anchoredPosition = new Vector2(-30f, -30f);
                bestRect.sizeDelta = new Vector2(350f, 70f);
            }

            bestScoreText.fontSize = 36;
            bestScoreText.color = Color.yellow;
            bestScoreText.alignment = TextAlignmentOptions.Right;
            bestScoreText.textWrappingMode = TextWrappingModes.NoWrap;
            bestScoreText.transform.SetAsLastSibling();
        }
    }

    private void ConfigureGameOverPanelUI()
    {
        if (gameOverPanel == null)
        {
            return;
        }

        RectTransform panelRect = gameOverPanel.GetComponent<RectTransform>();

        if (panelRect != null)
        {
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(620f, 430f);
        }

        Image panelImage = gameOverPanel.GetComponent<Image>();

        if (panelImage != null)
        {
            panelImage.color = new Color(0.12f, 0.06f, 0.02f, 0.92f);
            panelImage.raycastTarget = true;
        }

        Transform gameOverTitle = gameOverPanel.transform.Find("GAME OVER");

        if (gameOverTitle != null)
        {
            TextMeshProUGUI titleText = gameOverTitle.GetComponent<TextMeshProUGUI>();
            RectTransform titleRect = gameOverTitle.GetComponent<RectTransform>();

            if (titleText != null)
            {
                titleText.text = "GAME OVER";
                titleText.fontSize = 58;
                titleText.color = Color.yellow;
                titleText.alignment = TextAlignmentOptions.Center;
                titleText.textWrappingMode = TextWrappingModes.NoWrap;
            }

            if (titleRect != null)
            {
                titleRect.anchorMin = new Vector2(0.5f, 0.5f);
                titleRect.anchorMax = new Vector2(0.5f, 0.5f);
                titleRect.pivot = new Vector2(0.5f, 0.5f);
                titleRect.anchoredPosition = new Vector2(0f, 130f);
                titleRect.sizeDelta = new Vector2(560f, 80f);
            }
        }

        if (finalScoreText != null)
        {
            RectTransform finalScoreRect = finalScoreText.GetComponent<RectTransform>();

            finalScoreText.fontSize = 36;
            finalScoreText.color = Color.white;
            finalScoreText.alignment = TextAlignmentOptions.Center;
            finalScoreText.textWrappingMode = TextWrappingModes.NoWrap;

            if (finalScoreRect != null)
            {
                finalScoreRect.anchorMin = new Vector2(0.5f, 0.5f);
                finalScoreRect.anchorMax = new Vector2(0.5f, 0.5f);
                finalScoreRect.pivot = new Vector2(0.5f, 0.5f);
                finalScoreRect.anchoredPosition = new Vector2(0f, 45f);
                finalScoreRect.sizeDelta = new Vector2(560f, 60f);
            }
        }

        if (finalBestScoreText != null)
        {
            RectTransform finalBestRect = finalBestScoreText.GetComponent<RectTransform>();

            finalBestScoreText.fontSize = 36;
            finalBestScoreText.color = Color.yellow;
            finalBestScoreText.alignment = TextAlignmentOptions.Center;
            finalBestScoreText.textWrappingMode = TextWrappingModes.NoWrap;

            if (finalBestRect != null)
            {
                finalBestRect.anchorMin = new Vector2(0.5f, 0.5f);
                finalBestRect.anchorMax = new Vector2(0.5f, 0.5f);
                finalBestRect.pivot = new Vector2(0.5f, 0.5f);
                finalBestRect.anchoredPosition = new Vector2(0f, -20f);
                finalBestRect.sizeDelta = new Vector2(560f, 60f);
            }
        }

        Transform restartButton = gameOverPanel.transform.Find("Restart Game");

        if (restartButton == null)
        {
            restartButton = gameOverPanel.transform.Find("Restart");
        }

        if (restartButton != null)
        {
            RectTransform buttonRect = restartButton.GetComponent<RectTransform>();

            if (buttonRect != null)
            {
                buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
                buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
                buttonRect.pivot = new Vector2(0.5f, 0.5f);
                buttonRect.anchoredPosition = new Vector2(0f, -120f);
                buttonRect.sizeDelta = new Vector2(260f, 70f);
            }

            Image buttonImage = restartButton.GetComponent<Image>();

            if (buttonImage != null)
            {
                buttonImage.color = Color.white;
                buttonImage.raycastTarget = true;
            }

            Button button = restartButton.GetComponent<Button>();

            if (button == null)
            {
                button = restartButton.gameObject.AddComponent<Button>();
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(RestartGame);
            button.interactable = true;

            TextMeshProUGUI buttonText = restartButton.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
            {
                buttonText.text = "Restart Game";
                buttonText.fontSize = 30;
                buttonText.color = Color.black;
                buttonText.alignment = TextAlignmentOptions.Center;
                buttonText.textWrappingMode = TextWrappingModes.NoWrap;
            }
        }
        else
        {
            Debug.LogWarning("Restart button was not found. Rename the button to 'Restart Game' or 'Restart'.");
        }
    }

    private void CreateBoard()
    {
        if (boardParent == null || cellPrefab == null)
        {
            Debug.LogError("BoardManager is missing Board Parent or Cell Prefab.");
            return;
        }

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

                if (cell == null)
                {
                    Debug.LogError("CellPrefab is missing GridCell script.");
                    continue;
                }

                cell.Setup(r, c);
                grid[r, c] = cell;
            }
        }
    }

    public bool IsBoardReady()
    {
        return grid != null;
    }

public bool TryPlaceShape(List<Vector2Int> shapeCells, Vector2 screenPosition, Sprite blockSprite)
{
    return TryPlaceShapeWithAnchor(shapeCells, Vector2Int.zero, screenPosition, blockSprite);
}

public bool TryPlaceShapeWithAnchor(
    List<Vector2Int> shapeCells,
    Vector2Int grabbedCellOffset,
    Vector2 screenPosition,
    Sprite blockSprite
)
{
    if (isClearing || isGameOver)
    {
        Debug.Log("Cannot place: clearing or game over.");
        return false;
    }

    if (grid == null || shapeCells == null)
    {
        Debug.Log("Cannot place: grid or shape is null.");
        return false;
    }

    GridCell targetCell = GetNearestCell(screenPosition);

    if (targetCell == null)
    {
        Debug.Log("Cannot place: no valid grid cell under mouse.");
        return false;
    }

    int startRow = targetCell.row - grabbedCellOffset.y;
    int startColumn = targetCell.column - grabbedCellOffset.x;

    if (!CanPlaceShape(shapeCells, startRow, startColumn))
    {
        Debug.Log("Cannot place: shape does not fit at row " + startRow + ", col " + startColumn);
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

    Debug.Log("Placed successfully at row " + startRow + ", col " + startColumn);
    return true;
}

private GridCell GetNearestCell(Vector2 screenPosition)
{
    if (grid == null)
    {
        return null;
    }

    GridCell nearestCell = null;
    float nearestDistance = float.MaxValue;

    foreach (GridCell cell in grid)
    {
        if (cell == null)
        {
            continue;
        }

        RectTransform cellRect = cell.GetComponent<RectTransform>();

        Vector2 cellScreenPosition = RectTransformUtility.WorldToScreenPoint(
            null,
            cellRect.position
        );

        float distance = Vector2.Distance(screenPosition, cellScreenPosition);

        if (distance < nearestDistance)
        {
            nearestDistance = distance;
            nearestCell = cell;
        }
    }

    float allowedDistance = (cellSize + spacing) * 1.3f;

    if (nearestDistance > allowedDistance)
    {
        return null;
    }

    return nearestCell;
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
            return true;
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
                if (cell != null)
                {
                    Sprite randomFruit = fruitSprites[Random.Range(0, fruitSprites.Length)];
                    cell.SetFruitSprite(randomFruit);
                }
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

        Debug.Log("Score increased by " + amount + ". Total Score: " + score);
    }

    private void UpdateScoreText()
    {
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
        }

        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
            scoreText.transform.SetAsLastSibling();
        }

        if (bestScoreText != null)
        {
            bestScoreText.text = "Best: " + bestScore;
            bestScoreText.transform.SetAsLastSibling();
        }
    }

    public void ShowGameOver()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;

        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + score;
        }

        if (finalBestScoreText != null)
        {
            finalBestScoreText.text = "Best Score: " + bestScore;
        }

        if (gameOverPanel != null)
        {
            ConfigureGameOverPanelUI();

            gameOverPanel.SetActive(true);
            gameOverPanel.transform.SetAsLastSibling();
        }
        else
        {
            Debug.LogWarning("GameOverPanel is not assigned in BoardManager.");
        }

        if (scoreText != null)
        {
            scoreText.transform.SetAsLastSibling();
        }

        if (bestScoreText != null)
        {
            bestScoreText.transform.SetAsLastSibling();
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void ResetBestScore()
    {
        PlayerPrefs.DeleteKey("BestScore");
        bestScore = 0;
        UpdateScoreText();
    }
}