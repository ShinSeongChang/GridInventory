using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [HideInInspector]
    private ItemGrid selectedItemGrid;
    public ItemGrid SelectedItemGrid 
    {   get => selectedItemGrid;
        set
        {
            selectedItemGrid = value;
            inventoryHighlgiht.SetParent(value);
        }
    }

    InventoryItem selectedItem;
    InventoryItem overlapItem;
    InventoryItem itemToHighlight;

    Vector2Int oldPosition;

    // ������
    RectTransform itemRect;

    [SerializeField] List<ItemData> items;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;

    InventoryHighlgiht inventoryHighlgiht;


    private void Awake()
    {
        inventoryHighlgiht = GetComponent<InventoryHighlgiht>();
    }

    private void Update()
    {
        // selectedItem�� null�� �ƴҰ�� == ���� ������ �������� �����Ѵ�
        // => ������ �������� �� ���콺 �������� ����´�.
        ItemIconDarg();

        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(selectedItem == null)
            {
                CreateRandomItem();
            }
        }


        // �κ��丮 ���� ������ ����
        if(Input.GetKeyDown(KeyCode.W))
        {
            InsertRandomItem();
        }

        // ������ ȸ��
        if(Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
        }

        if (selectedItemGrid == null)
        {
            // ���õ� �������� ���ٸ� ���̶���Ʈ off
            inventoryHighlgiht.Show(false);
            return;
        }

        HandleHighlight();

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("�̰��� ���� ���콺�� ��ġ�� Ÿ���� ��ġ : " + selectedItemGrid.GetTileGridPosition(Input.mousePosition));
            LeftMouseButtonClick();
        }

    }

    private void RotateItem()
    {
        if(selectedItem == null)
        {
            return;
        }

        selectedItem.Rotate();
    }

    private void InsertRandomItem()
    {
        if(selectedItemGrid == null)
        {
            return;
        }

        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
    }

    private void InsertItem(InventoryItem itemToInsert)
    {        
        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);

        if(posOnGrid == null)
        {
            return;
        }

        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();

        if(oldPosition == positionOnGrid)
        {
            return;
        }

        oldPosition = positionOnGrid;   

        if(selectedItem == null)
        {
            itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);

            if(itemToHighlight != null)
            {
                // ���콺 �������� ��ġ�� �������� �ִٸ� ���̶���Ʈ on
                inventoryHighlgiht.Show(true);
                inventoryHighlgiht.SetSize(itemToHighlight);                
                inventoryHighlgiht.SetPosition(selectedItemGrid, itemToHighlight);
            }
            else
            {
                // ���ٸ� ���̶���Ʈ off
                inventoryHighlgiht.Show(false);
            }
        }
        else
        {
            inventoryHighlgiht.Show(selectedItemGrid.BoundryCheck(
                positionOnGrid.x,
                positionOnGrid.y,
                selectedItem.WIDTH,
                selectedItem.HEIGHT));

            inventoryHighlgiht.SetSize(selectedItem);            
            inventoryHighlgiht.SetPosition(selectedItemGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
        }
    }

    private void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;

        itemRect = inventoryItem.GetComponent<RectTransform>();
        itemRect.SetParent(canvasTransform);

        int selectedItemID = UnityEngine.Random.Range(0, items.Count);
        inventoryItem.Set(items[selectedItemID]);
    }

    private void LeftMouseButtonClick()
    {
        Vector2Int tileGirdPosition = GetTileGridPosition();

        if (selectedItem == null)
        {
            // ���� ������ �������� ����� ���¶��
            // �������� ���� �޼ҵ带 ���� ���Ϲ޴´�.
            PickUpItem(tileGirdPosition);
        }
        else
        {
            // ������ �������� �����Ѵٸ� �������� Ŭ���� ��ġ�� ����
            PlaceItem(tileGirdPosition);
        }
    }

    private Vector2Int GetTileGridPosition()
    {
        Vector2 position = Input.mousePosition;

        if (selectedItem != null)
        {
            // TODO : �� ������ �������� ũ�� - 1�� �ϴ°ɱ�?
            // ���콺�� �������� ��ġ�� ������ ���ش�???
            position.x -= (selectedItem.WIDTH -1) * ItemGrid.tileSizeWidth / 2;
            position.y += (selectedItem.HEIGHT -1) * ItemGrid.tileSizeHeight / 2;

        }

        return selectedItemGrid.GetTileGridPosition(position);
    }

    private void PlaceItem(Vector2Int tileGirdPosition)
    {
        bool complete = selectedItemGrid.PlaceItem(selectedItem, tileGirdPosition.x, tileGirdPosition.y, ref overlapItem);

        if (complete == true)
        {
            selectedItem = null;
            if(overlapItem != null)
            {
                selectedItem = overlapItem;
                overlapItem = null;
                itemRect = selectedItem.GetComponent<RectTransform>();
            }
        }
    }

    private void PickUpItem(Vector2Int tileGirdPosition)
    {
        selectedItem = selectedItemGrid.PickUpItem(tileGirdPosition.x, tileGirdPosition.y);

        if (selectedItem != null)
        {
            itemRect = selectedItem.GetComponent<RectTransform>();
        }
    }

    private void ItemIconDarg()
    {
        if (selectedItem != null)
        {
            itemRect.position = Input.mousePosition;
        }
    }
}
