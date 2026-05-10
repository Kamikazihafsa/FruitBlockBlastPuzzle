using UnityEngine;
using UnityEngine.UI;

public class GridCell : MonoBehaviour
{
    public int row;
    public int column;
    public bool isOccupied = false;

    public Image cellImage;

    private void Awake()
    {
        if (cellImage == null)
        {
            cellImage = GetComponent<Image>();
        }
    }

    public void Setup(int r, int c)
    {
        row = r;
        column = c;
        ClearCell();
    }

    public void SetBlock(Color blockColor)
    {
        isOccupied = true;

        if (cellImage != null)
        {
            cellImage.color = blockColor;
        }

        transform.localScale = Vector3.one;
    }

    public void SetFruitColor(Color fruitColor)
    {
        if (cellImage != null)
        {
            cellImage.color = fruitColor;
        }

        transform.localScale = Vector3.one * 1.15f;
    }

    public void ClearCell()
    {
        isOccupied = false;

        if (cellImage != null)
        {
            cellImage.color = new Color(1f, 1f, 1f, 0.25f);
        }

        transform.localScale = Vector3.one;
    }
}