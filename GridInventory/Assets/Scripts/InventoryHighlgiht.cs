using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHighlgiht : MonoBehaviour
{
    [SerializeField] RectTransform highlighter;

    public void Show(bool b)
    {
        highlighter.gameObject.SetActive(b);
    }

    /// <summary>
    /// 강조효과의 크기를 정해주는 메소드
    /// </summary>
    /// <param name="targetItem">마우스가 들고있는, 마우스가 올라간 아이템</param>
    public void SetSize(InventoryItem targetItem)
    {
        Vector2 size = new Vector2();

        // 전달받은 아이템의 크기와 1타일의 사이즈를 곱하여 강조효과의 크기를 정한다.
        size.x = targetItem.WIDTH * ItemGrid.tileSizeWidth;
        size.y = targetItem.HEIGHT * ItemGrid.tileSizeHeight;
        highlighter.sizeDelta = size;

    }

    /// <summary>
    /// 강조효과의 포지션을 계산해주는 메소드
    /// </summary>
    /// <param name="targetGrid">현재 마우스가 올라간 인벤토리</param>
    /// <param name="targetItem">인벤토리에 놓인 아이템</param>
    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem)
    {
        SetParent(targetGrid);

        Vector2 pos = targetGrid.CalculatePositionOngrid(
            targetItem,
            targetItem.onGridPositionX,
            targetItem.onGridPositionY);

        highlighter.localPosition = pos;

    }

    /// <summary>
    /// 강조효과를 해당 인벤토리에 종속 시킨 이후에 RectTransform을 가져오는 메소드
    /// </summary>
    /// <param name="targetGrid">현재 마우스가 올라간 인벤토리</param>
    public void SetParent(ItemGrid targetGrid)
    {
        if(targetGrid == null)
        {
            return;
        }

        // 강조효과를 해당 그리드에 종속 시킨 이후에 RectTransform을 가져온다.
        highlighter.SetParent(targetGrid.GetComponent<RectTransform>());
    }

    /// <summary>
    /// 마우스에 아이템이 들려져 있을 때 강조 효과
    /// </summary>
    /// <param name="targetGrid">현재 마우스가 올라가 있는 아이템 인벤토리</param>
    /// <param name="targetItem">마우스가 들고있는 아이템</param>
    /// <param name="posX">타일의 x인덱스 값</param>
    /// <param name="posY">타일의 y인덱스 값</param>
    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem, int posX, int posY)
    {
        Vector2 pos = targetGrid.CalculatePositionOngrid(
            targetItem,
            posX,
            posY);

        highlighter.localPosition = pos;
    }
}
