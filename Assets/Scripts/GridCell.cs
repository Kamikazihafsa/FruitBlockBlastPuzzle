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

    public void SetBlock(Sprite blockSprite)
    {
        isOccupied = true;

        if (cellImage != null)
        {
            cellImage.sprite = blockSprite;
            cellImage.color = Color.white;
            cellImage.preserveAspect = true;
        }

        transform.localScale = Vector3.one;
    }

    public void SetFruitSprite(Sprite fruitSprite)
    {
        if (cellImage != null)
        {
            cellImage.sprite = fruitSprite;
            cellImage.color = Color.white;
            cellImage.preserveAspect = true;
        }

        transform.localScale = Vector3.one * 1.15f;
    }

    public void ClearCell()
    {
        isOccupied = false;

        if (cellImage != null)
        {
            cellImage.sprite = null;
            cellImage.color = new Color(1f, 1f, 1f, 0.25f);
        }

        transform.localScale = Vector3.one;
    }
}