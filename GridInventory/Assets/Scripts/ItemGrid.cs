using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    // 마우스가 위치한 타일, 아이템이 정렬될 위치등을 계산할 1타일의 사이즈
    public const float tileSizeWidth = 32;
    public const float tileSizeHeight = 32;

    RectTransform myRect;

    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();    
    
    // TODO : 해당 클래스에 아이템들의 속성값을 부여하면 될까??
    // 아이템들의 위치값을 저장할 배열
    InventoryItem[,] inventoryItemSlot;

    // 인벤토리 슬롯을 인스펙터에서 정하기
    [SerializeField] int gridSizeWidth = 10;
    [SerializeField] int gridSizeHeight = 10;
    
    private void Start()
    {        
        myRect = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);

    }

    // 마우스 포지션값을 받고 현재 마우스 포지션이 위치한 타일의 위치값 (2차원 배열)을 계산하여 return
    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        // 마우스 포지션과 그리드의 위치값 계산
        positionOnTheGrid.x = mousePosition.x - myRect.position.x;
        positionOnTheGrid.y = myRect.position.y - mousePosition.y;

        // 이후 인벤토리 1타일의 사이즈로 나눈 뒤 int형으로 변환하여 2차원 배열형태로 만들어 준다.
        tileGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth);
        tileGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight);

        return tileGridPosition;
    }

    // 초기 인벤토리 슬롯 크기 설정하는 메소드
    void Init(int width, int height)
    {
        inventoryItemSlot = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
        myRect.sizeDelta = size;
    }


    // 아이템을 인벤토리에 정렬시키기
    public bool PlaceItem(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
    {
        // 1칸짜리가 아닌 아이템이 크기가 벗어난채로 인벤토리에 정렬되려 하는것을 체크
        if (BoundryCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT) == false)
        {
            return false;
        }

        if (OverlapCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null)
        {
            CleanGirdReference(overlapItem);
        }

        PlaceItem(inventoryItem, posX, posY);

        return true;
    }

    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY)
    {
        // 해당 아이템의 RectTransform을 가져오고
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();

        // 해당하는 인벤토리칸에 종속(인벤토리의 캔버스에 맞게 정렬되어야 하니까)
        rectTransform.SetParent(myRect);


        // TODO : 이 반복문 이해 필요
        for (int x = 0; x < inventoryItem.WIDTH; x++)
        {
            for (int y = 0; y < inventoryItem.HEIGHT; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = inventoryItem;
            }
        }

        inventoryItem.onGridPositionX = posX;
        inventoryItem.onGridPositionY = posY;


        // 해당하는 타일위치 = pos * 타일 사이즈 크기
        // 이후 아이템 데이타를 참조하여 (아이템의 크기 * 1타일의 크기) / 2를 해주면 해당 타일의 중앙위치 정렬
        Vector2 position = CalculatePositionOngrid(inventoryItem, posX, posY);

        Debug.Log($"정렬된 포지션 : {position}");

        rectTransform.localPosition = position;
    }

    public Vector2 CalculatePositionOngrid(InventoryItem inventoryItem, int posX, int posY)
    {
        Vector2 position = new Vector2();
        position.x = posX * tileSizeWidth + tileSizeWidth * inventoryItem.WIDTH / 2;
        position.y = -(posY * tileSizeWidth + tileSizeHeight * inventoryItem.HEIGHT / 2);
        return position;
    }

    private bool OverlapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem)
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if (inventoryItemSlot[posX + x, posY + y] != null)
                {
                    if(overlapItem == null)
                    {
                        overlapItem = inventoryItemSlot[posX + x, posY + y];
                    }
                    else
                    {
                        if(overlapItem != inventoryItemSlot[posX + x, posY + y])
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    public InventoryItem PickUpItem(int x, int y)
    {
        // 해당하는 인벤토리 배열의 아이템 슬롯을 리턴한다
        InventoryItem toReturn = inventoryItemSlot[x, y];

        if (toReturn == null)
        {
            return null;
        }

        // TODO : 이 반복문 이해 필요
        CleanGirdReference(toReturn);

        // 이후 해당 배열은 비워준다.
        inventoryItemSlot[x, y] = null;

        return toReturn;
    }

    private void CleanGirdReference(InventoryItem item)
    {
        for (int ix = 0; ix < item.WIDTH; ix++)
        {
            for (int iy = 0; iy < item.HEIGHT; iy++)
            {
                inventoryItemSlot[item.onGridPositionX + ix, item.onGridPositionY + iy] = null;
            }
        }
    }

    // 인벤토리를 벗어나는 위치에 아이템을 놔두는지 체크
    bool PositionCheck(int posX, int posY)
    {
        if(posX < 0 || posY < 0)
        {
            return false;
        }

        if(posX >= gridSizeWidth || posY >= gridSizeHeight)
        {
            return false;
        }

        return true;
    }

    // 인벤토리를 벗어나는 위치에 아이템을 놔두는지 체크
    public bool BoundryCheck(int posX, int posY, int width, int height)
    {
        if(PositionCheck(posX, posY) == false)
        {
            return false;
        }

        posX += width - 1;
        posY += height - 1;

        if(PositionCheck(posX, posY) == false)
        {
            return false;
        }

        return true;
    }

    internal InventoryItem GetItem(int x, int y)
    {
        return inventoryItemSlot[x, y];
    }

    public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
    {
        // TODO : 여긴 또 왜 +1??
        int height = gridSizeHeight - itemToInsert.WIDTH + 1;
        int width = gridSizeWidth - itemToInsert.HEIGHT + 1;

        for(int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if(CheckAvailableSpace(x, y, itemToInsert.WIDTH, itemToInsert.HEIGHT) == true)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return null;
    }

    private bool CheckAvailableSpace(int posX, int posY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemSlot[posX + x, posY + y] != null)
                {  
                    return false;                                            
                }
            }
        }

        return true;
    }
}
