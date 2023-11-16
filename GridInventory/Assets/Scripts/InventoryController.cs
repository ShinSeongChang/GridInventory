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

    // 아이템
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
        // selectedItem이 null이 아닐경우 == 현재 선택한 아이템이 존재한다
        // => 선택한 아이템은 내 마우스 포지션을 따라온다.
        ItemIconDarg();

        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(selectedItem == null)
            {
                CreateRandomItem();
            }
        }


        // 인벤토리 내부 아이템 생성
        if(Input.GetKeyDown(KeyCode.W))
        {
            InsertRandomItem();
        }

        // 아이템 회전
        if(Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
        }

        if (selectedItemGrid == null)
        {
            // 선택된 아이템이 없다면 하이라이트 off
            inventoryHighlgiht.Show(false);
            return;
        }

        HandleHighlight();

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("이것은 현재 마우스가 위치한 타일의 위치 : " + selectedItemGrid.GetTileGridPosition(Input.mousePosition));
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
                // 마우스 포지션의 위치에 아이템이 있다면 하이라이트 on
                inventoryHighlgiht.Show(true);
                inventoryHighlgiht.SetSize(itemToHighlight);                
                inventoryHighlgiht.SetPosition(selectedItemGrid, itemToHighlight);
            }
            else
            {
                // 없다면 하이라이트 off
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
            // 현재 선택한 아이템이 비워진 상태라면
            // 아이템을 고르는 메소드를 통해 리턴받는다.
            PickUpItem(tileGirdPosition);
        }
        else
        {
            // 선택한 아이템이 존재한다면 아이템을 클릭한 위치에 정렬
            PlaceItem(tileGirdPosition);
        }
    }

    private Vector2Int GetTileGridPosition()
    {
        Vector2 position = Input.mousePosition;

        if (selectedItem != null)
        {
            // TODO : 왜 아이템 데이터의 크기 - 1을 하는걸까?
            // 마우스와 아이템의 위치의 오차를 없앤다???
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
