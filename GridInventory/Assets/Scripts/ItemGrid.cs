using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    // ���콺�� ��ġ�� Ÿ��, �������� ���ĵ� ��ġ���� ����� 1Ÿ���� ������
    public const float tileSizeWidth = 32;
    public const float tileSizeHeight = 32;

    RectTransform myRect;

    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();    
    
    // TODO : �ش� Ŭ������ �����۵��� �Ӽ����� �ο��ϸ� �ɱ�??
    // �����۵��� ��ġ���� ������ �迭
    InventoryItem[,] inventoryItemSlot;

    // �κ��丮 ������ �ν����Ϳ��� ���ϱ�
    [SerializeField] int gridSizeWidth = 10;
    [SerializeField] int gridSizeHeight = 10;
    
    private void Start()
    {        
        myRect = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);

    }

    // ���콺 �����ǰ��� �ް� ���� ���콺 �������� ��ġ�� Ÿ���� ��ġ�� (2���� �迭)�� ����Ͽ� return
    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        // ���콺 �����ǰ� �׸����� ��ġ�� ���
        positionOnTheGrid.x = mousePosition.x - myRect.position.x;
        positionOnTheGrid.y = myRect.position.y - mousePosition.y;

        // ���� �κ��丮 1Ÿ���� ������� ���� �� int������ ��ȯ�Ͽ� 2���� �迭���·� ����� �ش�.
        tileGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth);
        tileGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight);

        return tileGridPosition;
    }

    // �ʱ� �κ��丮 ���� ũ�� �����ϴ� �޼ҵ�
    void Init(int width, int height)
    {
        inventoryItemSlot = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
        myRect.sizeDelta = size;
    }


    // �������� �κ��丮�� ���Ľ�Ű��
    public bool PlaceItem(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
    {
        // 1ĭ¥���� �ƴ� �������� ũ�Ⱑ ���ä�� �κ��丮�� ���ĵǷ� �ϴ°��� üũ
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
        // �ش� �������� RectTransform�� ��������
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();

        // �ش��ϴ� �κ��丮ĭ�� ����(�κ��丮�� ĵ������ �°� ���ĵǾ�� �ϴϱ�)
        rectTransform.SetParent(myRect);


        // TODO : �� �ݺ��� ���� �ʿ�
        for (int x = 0; x < inventoryItem.WIDTH; x++)
        {
            for (int y = 0; y < inventoryItem.HEIGHT; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = inventoryItem;
            }
        }

        inventoryItem.onGridPositionX = posX;
        inventoryItem.onGridPositionY = posY;


        // �ش��ϴ� Ÿ����ġ = pos * Ÿ�� ������ ũ��
        // ���� ������ ����Ÿ�� �����Ͽ� (�������� ũ�� * 1Ÿ���� ũ��) / 2�� ���ָ� �ش� Ÿ���� �߾���ġ ����
        Vector2 position = CalculatePositionOngrid(inventoryItem, posX, posY);

        Debug.Log($"���ĵ� ������ : {position}");

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
        // �ش��ϴ� �κ��丮 �迭�� ������ ������ �����Ѵ�
        InventoryItem toReturn = inventoryItemSlot[x, y];

        if (toReturn == null)
        {
            return null;
        }

        // TODO : �� �ݺ��� ���� �ʿ�
        CleanGirdReference(toReturn);

        // ���� �ش� �迭�� ����ش�.
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

    // �κ��丮�� ����� ��ġ�� �������� ���δ��� üũ
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

    // �κ��丮�� ����� ��ġ�� �������� ���δ��� üũ
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
        // TODO : ���� �� �� +1??
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
