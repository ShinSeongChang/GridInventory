using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;

    public int onGridPositionX;
    public int onGridPositionY;

    public bool rotated = false;


    public int HEIGHT
    {
        get
        {
            if (rotated == false)
            {
                return itemData.height;
            }

            return itemData.width;
        }
    }

    public int WIDTH
    {
        get
        {
            if(rotated == false)
            {
                return itemData.width;
            }

            return itemData.height;
        }
    }

    /// <summary>
    /// 아이템 스크럽터블을 통해 인벤토리 캔버스에 맞게 아이템 텍스처를 생성하는 메소드
    /// </summary>
    /// <param name="itemData">생성할 아이템 데이터 스크럽터블 
    /// <para>
    /// 담길 정보는 아이템 타일크기, 텍스처 이미지
    /// </para>
    /// </param>
    internal void Set(ItemData itemData)
    {
        this.itemData = itemData;

        GetComponent<Image>().sprite = itemData.itemIcon;

        Vector2 size = new Vector2();

        // 아이템 크기 * 인벤토리 1타일의 사이즈를 해줘야 타일 크기에맞게 
        size.x = WIDTH * ItemGrid.tileSizeWidth;
        size.y = HEIGHT * ItemGrid.tileSizeHeight;

        GetComponent<RectTransform>().sizeDelta = size;
    }
    internal void Rotate()
    {
        rotated = !rotated;

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(0, 0, rotated == true ? 90f : 0f);       
    }
}
