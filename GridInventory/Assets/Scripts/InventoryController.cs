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
    {  
        // 변수를 람다로? 결국 해당 변수를 보여준다인듯?
        get => selectedItemGrid;
        set
        {
            // 인벤토리 그리드들은 IpoitnerEnter 인터페이스로 자신에게 마우스가 들어왔다는 이벤트를 통해 현재 선택된 그리드가 무엇인지 신호를 보낸다.
            selectedItemGrid = value;

            // 현재 강조되는 아이템이 위치한 그리드에 하이라이트 타일 종속
            inventoryHighlgiht.SetParent(value);
        }
    }

    // 마우스가 고른 아이템
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
        // selectedItem을 마우스 포지션으로 따라오게 한다.
        ItemIconDarg();

        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(selectedItem == null)
            {
                CreateRandomItem();
            }
        }

        // 인벤토리 내부 아이템 생성
        // TODO : 해당 기능으로 먹은 아이템을 창고에 정렬하는?...
        // 아냐 어차피 창고에는 아이템을 1타일씩 갖고 갯수를 늘릴뿐.
        if(Input.GetKeyDown(KeyCode.W))
        {
            InsertRandomItem();
        }

        // 아이템 회전
        if(Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
        }

        // 현재 마우스가 인벤토리에 들어가있는 상태가 아니면 아이템을 내려놓는등의 동작을 못한다.
        // 마우스가 인벤토리에 들어가있는지는 selectedItemGrid에 IPointerEnter, IPointerExit 인터페이스로 할당, 해제를 통해 체크된다.
        if (selectedItemGrid == null)
        {
            // 마우스가 인벤토리 상단에 올려져있는 상태가 아니면 아이템 강조효과 off
            inventoryHighlgiht.Show(false);
            return;
        }

        HandleHighlight();

        if (Input.GetMouseButtonDown(0))
        {            
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

    /// <summary>
    /// 인벤토리에 아이템을 추가하는 메소드
    /// </summary>
    /// <param name="itemToInsert"></param>
    private void InsertItem(InventoryItem itemToInsert)
    {        
        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);

        // 전달받은 타일값이 없다면 진행못한다.
        if(posOnGrid == null)
        {
            return;
        }

        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    // TODO : 아이템을 회전 시켰을때 강조효과가 한 턴 뒤늦게 회전함.
    /// <summary>
    /// 강조효과의 각종 행동을 정해주는 메소드
    /// <para>
    /// Show() => 강조효과 활성화, 비활성화
    /// </para>
    /// <para>
    /// SetSize() => 강조효과의 사이즈 계산
    /// </para> 
    /// <para>
    /// SetPosition() => 강조효과의 포지션 계산
    /// </para>
    /// </summary>
    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();

        // 23.11.20
        // LEGACY : 주석처리해도 동작에 큰 문제는 없음
        // 무엇을 방지하는것인지 파악이 안됨.
        if (oldPosition == positionOnGrid)
        {
            return;
        }

        oldPosition = positionOnGrid;   

        // 마우스가 아이템을 안들고 있다면
        if(selectedItem == null)
        {
            // 마우스가 위치한 타일의 아이템 정보를 가져온다.
            itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);

            // 마우스 포지션의 위치에 아이템이 있다면 하이라이트 on
            if(itemToHighlight != null)
            {
                // 하이라이트 오브젝트를 활성화 하고, 사이즈를 정한뒤 포지션을 잡아준다.
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
            // 마우스가 아이템을 들고 있는 상황, 아이템을 놓을때처럼 BoundryCheck를 통해 인벤토리창을 벗어나는것을 확인한다.
            inventoryHighlgiht.Show(selectedItemGrid.BoundryCheck(
                positionOnGrid.x,
                positionOnGrid.y,
                selectedItem.WIDTH,
                selectedItem.HEIGHT));

            inventoryHighlgiht.SetSize(selectedItem);            
            inventoryHighlgiht.SetPosition(selectedItemGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
        }
    }

    // 이 기능을 사용할까?
    /// <summary>
    /// 랜덤 아이템을 내 마우스에 생성하는 메소드
    /// </summary>
    private void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;

        itemRect = inventoryItem.GetComponent<RectTransform>();

        // 생성된 아이템 아이콘이 화면안에서 움직이게 캔버스 RectTransform에 종속시킨다.
        itemRect.SetParent(canvasTransform);

        int selectedItemID = UnityEngine.Random.Range(0, items.Count);

        // 생성되는 아이템은 현재 리스트로 들고있는 아이템데이터 스크럽터블중에서 랜덤으로 생성한다.
        inventoryItem.Set(items[selectedItemID]);
    }

    /// <summary>
    /// 마우스 클릭을 했을시 행동을 정해주는 메소드
    /// <para>
    /// 아이템을 안들고 있는 상태 => PickUpItem(아이템을 들어올린다.) 
    /// </para>
    /// <para>
    /// 아이템을 들고 있는 상태 => PlaceItem(아이템을 마우스가 위치한 타일에 배치한다.)  => 이 때 인벤토리를 벗어나는지, 아이템 타일이 겹치는지 체크를 진행하게 된다.
    /// </para>
    /// </summary>
    private void LeftMouseButtonClick()
    {
        // ItemGrid 클래스에 있는 메소드를 통해 현재 마우스가 위치한 인벤토리의 타일 포지션을 가져온다.
        Vector2Int tileGirdPosition = GetTileGridPosition();


        // 현재 마우스에 아이템이 선택 안된 상태라면
        if (selectedItem == null)
        {
            // 아이템을 집어올리는 행동을 한다.
            PickUpItem(tileGirdPosition);
        }
        else
        {
            // 마우스에 아이템이 선택된 상태에서 마우스를 누른다면 아이템을 인벤토리에 배치한다.
            PlaceItem(tileGirdPosition);
        }
    }

    /// <summary>
    /// LeftMouseButtonClick => selectedItem == null 일때
    /// <para>
    /// selectedItem에 클릭한 위치의 아이템을 반환해줌과 동시에 해당 아이템이 차지한 인벤토리슬롯을 비워준다.
    /// </para>    
    /// </summary>
    /// <param name="tileGirdPosition">현재 마우스가 위치한 타일 포지션 (Vector2Int)</param>
    private void PickUpItem(Vector2Int tileGirdPosition)
    {
        
        selectedItem = selectedItemGrid.PickUpItem(tileGirdPosition.x, tileGirdPosition.y);

        if (selectedItem != null)
        {
            // ItemIconDrag() 메소드에 사용될 현재 선택된 아이템 RectTransform값을 넣어준다.
            itemRect = selectedItem.GetComponent<RectTransform>();
        }
    }

    /// <summary>
    /// LeftMouseButtonClick => selectedItem != null 일때
    /// <para>
    /// selectedItem을 클릭한 타일 위치에 내려놓는다. 이때 내려놓는 위치에 아이템이 인벤토리를 벗어나는지, 겹치는 아이템이 있는지 체크를 진행한다.
    /// </para>   
    /// </summary>
    /// <param name="tileGirdPosition">현재 마우스가 위치한 타일 포지션 (Vector2Int)</param>
    private void PlaceItem(Vector2Int tileGirdPosition)
    {
        // 아이템이 인벤토리를 벗어나는지(BoundryCheck)
        // 아이템을 놓는 타일에 이미 아이템이 존재하는지 (OverlapCheck)를 체크 한다.
        bool complete = selectedItemGrid.PlaceItem(selectedItem, tileGirdPosition.x, tileGirdPosition.y, ref overlapItem);

        if (complete == true)
        {
            // 아이템을 내려놓았으니 선택된 아이템 해제
            selectedItem = null;

            // 해당 타일에 아이템이 존재한다면
            if (overlapItem != null)
            {
                // 내 마우스에 타일에 존재한 아이템을 올린다.
                selectedItem = overlapItem;

                // 이후 overlapItem 할당 해제
                overlapItem = null;

                // 아이템 RectTransform은 overlapItem 값으로 교체해준다.
                itemRect = selectedItem.GetComponent<RectTransform>();
            }
        }
    }

    private Vector2Int GetTileGridPosition()
    {
        // 현재 마우스 위치값 : 캔버스상에 위치한 포지션 x,y값
        Vector2 position = Input.mousePosition;

        // 현재 마우스에 아이템이 올라온 상태라면
        if (selectedItem != null)
        {
            // TODO : 왜 아이템 데이터의 크기 - 1을 하는걸까?
            // 마우스와 아이템의 위치의 오차를 없앤다???
            position.x -= (selectedItem.WIDTH -1) * ItemGrid.tileSizeWidth / 2;
            position.y += (selectedItem.HEIGHT -1) * ItemGrid.tileSizeHeight / 2;

        }

        return selectedItemGrid.GetTileGridPosition(position);
    }
    /// <summary>
    /// 아이템 스프라이트 이미지를 마우스 위치로 따라오게하는 메소드
    /// </summary>
    private void ItemIconDarg()
    {
        if (selectedItem != null)
        {
            itemRect.position = Input.mousePosition;
        }
    }
}
