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

    /// <summary>
    /// 마우스가 위치한 인벤토리 타일의 2차원 배열값을 return해주는 메소드
    /// </summary>
    /// <param name="mousePosition">Input.mouseposition값</param>
    /// <returns></returns>
    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        // 마우스 포지션과 그리드의 위치값 계산
        // 현재 마우스 포지션x는 캔버스크기의 왼쪽부터 0 ~ 캔버스 크기값까지 입력
        // 그리드의 x 피벗위치를 0으로 하여 x축을 왼쪽부터 계산하게 한다.
        positionOnTheGrid.x = mousePosition.x - myRect.position.x;

        // 그리드의 y 피벗위치는 1로 상단을 축으로 계산하게 된다.
        // 그리드가 최상단에 위치한다면 그리드의 y축값은 캔버스 Height값부터 시작
        positionOnTheGrid.y = myRect.position.y - mousePosition.y;

        //Debug.Log($"마우스 포지션 x({mousePosition.x}) - 렉트 포지션 x({myRect.position.x}) = {positionOnTheGrid.x}");
        //Debug.Log($"렉트포지션y{myRect.position.y} - 마우스 포지션 y{mousePosition.y} = {positionOnTheGrid.y}");

        // 이후 인벤토리 1타일의 사이즈로 나눈 뒤 int형으로 변환하여 2차원 배열형태로 만들어 준다.
        tileGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth);
        tileGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight);

        return tileGridPosition;
    }

    /// <summary>
    /// 인벤토리 슬롯크기를 정해주는 메소드
    /// <para>
    /// 그리드 크기, 실제 아이템 데이터값이 저장되는 invetorySlot의 2차원 배열값도 설정값만큼 세팅해준다.
    /// </para>
    /// </summary>
    /// <param name="width">넓이</param>
    /// <param name="height">높이</param>
    void Init(int width, int height)
    {
        // 인벤토리 슬롯 생성 
        inventoryItemSlot = new InventoryItem[width, height];

        // 인스펙터창에서 설정한 칸수 * 타일텍스처 사이즈를 하면 해당 칸수만큼의 크기 그리드가 생성된다.
        // 해당 내용이 되는 이유는 타일텍스처 옵션중 Warpmode가 Repeat이기 때문에
        Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);

        // sizeDelta인 이유는 타일이 캔버스의 자식이기에 (localposition 쓰는것과 같은?)
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

        // 오버로딩된 메소드
        // => 최종적으로 아이템을 인벤토리 타일에 배치
        PlaceItem(inventoryItem, posX, posY);

        return true;
    }

    /// <summary>
    /// 최종적으로 아이템을 마우스로 옮기는 상태에서 인벤토리에 내려놓는 메소드
    /// </summary>
    /// <param name="inventoryItem">현재 마우스가 들고있는 아이템</param>
    /// <param name="posX">마우스가 위치한 타일 x값</param>
    /// <param name="posY">마우스가 위치한 타일 y값</param>
    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY)
    {
        // 해당 아이템의 RectTransform을 가져오고
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();

        // 해당하는 인벤토리칸에 종속(인벤토리의 그리드에 맞게 정렬되어야 하니까)
        rectTransform.SetParent(myRect);

        // 현재 마우스가 위치한 타일값(Vector2Int 값을 받고 해당 타일 기준으로 아이템 크기만큼 인벤토리슬롯 2차원 배열을 채워준다.)
        for (int x = 0; x < inventoryItem.WIDTH; x++)
        {
            for (int y = 0; y < inventoryItem.HEIGHT; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = inventoryItem;
            }
        }

        // 해당 아이템이 몇번째 타일 (2차원배열) 을 사용하는지 저장한다.
        inventoryItem.onGridPositionX = posX;
        inventoryItem.onGridPositionY = posY;
        
        // 아이템 아이콘이 정렬될 포지션을 계산해주는 메소드
        Vector2 position = CalculatePositionOngrid(inventoryItem, posX, posY);

        // 이후 아이템을 계산된 위치에 놓는다.
        rectTransform.localPosition = position;
    }

    /// <summary>
    /// 아이템 아이콘이 정렬 되어야 할 위치를 계산해주는 메소드
    /// </summary>
    /// <param name="inventoryItem">현재 들고있는 아이템의 정보(타일크기 체크해야됨)</param>
    /// <param name="posX">타일의 2차원배열 x값</param>
    /// <param name="posY">타일의 2차원배열 y값</param>
    /// <returns></returns>
    public Vector2 CalculatePositionOngrid(InventoryItem inventoryItem, int posX, int posY)
    {
        Vector2 position = new Vector2();
        // (전달받은 타일 위치 (2차원 배열 형태) * 1타일 크기) + (1타일 사이즈 * 아이템이 차지할 타일크기 / 2 )
        // 최종적으로 차지하는 타일의 중앙위치에 아이콘을 위치 시킨다.
        position.x = posX * tileSizeWidth + tileSizeWidth * inventoryItem.WIDTH / 2;
        // y 피벗이 1 즉 상단에 위치하기에 y축은 -값으로 계산된다.
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

    /// <summary>
    /// 인벤토리에 놓인 아이템을 들어올리는 메소드
    /// <para>
    /// 들어올리는 아이템을 리턴함과 동시에 아이템이 위치한 타일에서 아이템 크기만큼 인벤토리 슬롯을 비운다.
    /// </para>
    /// </summary>
    /// <param name="x">아이템이 위치한 타일 x값. 기준 위치는 LeftTop</param>
    /// <param name="y">아이템이 위치한 타일 y값. 기준 위치는 LeftTop</param>
    /// <returns></returns>
    public InventoryItem PickUpItem(int x, int y)
    {
        // 해당하는 인벤토리 배열의 아이템 슬롯을 리턴한다
        InventoryItem toReturn = inventoryItemSlot[x, y];

        // 해당 위치에 아이템이 없다면 null 값 반환
        if (toReturn == null)
        {
            return null;
        }

        // 마우스가 위치했던 타일의 아이템 크기만큼 인벤토리를 비워주는 메소드
        CleanGirdReference(toReturn);

        // LEGACY : 상단 CleanGridReference와 겹치는 동작으로 판단 => 주석처리
        //inventoryItemSlot[x, y] = null;

        return toReturn;
    }

    /// <summary>
    /// 아이템의 스크럽터블에 설정한 아이템 크기만큼 인벤토리 슬롯 크기를 비워주는 메소드
    /// </summary>
    /// <param name="item">어떤 아이템인지 전달해주고 메소드 내에서 아이템 크기만큼 인벤토리 슬롯을 비워준다.</param>
    private void CleanGirdReference(InventoryItem item)
    {       
        // 해당하는 2차원 배열위치에서 아이템 크기만큼 null값을 만들어준다.
        // TODO : 여기서 반복문을 응용하면 테트리스 형식으로 (ㄱ자나 ㄹ자 블록) 타일차지하는 블록 생성가능할듯?
        for (int ix = 0; ix < item.WIDTH; ix++)
        {
            for (int iy = 0; iy < item.HEIGHT; iy++)
            {
                inventoryItemSlot[item.onGridPositionX + ix, item.onGridPositionY + iy] = null;
            }
        }
    }


    /// <summary>
    /// 아이템이 인벤토리를 벗어난 위치에 놓이는지 체크
    /// </summary>
    /// <param name="posX">사용될 타일의 x 인덱스 값</param>
    /// <param name="posY">사용될 타일의 y 인덱스 값</param>
    /// <returns></returns>
    bool PositionCheck(int posX, int posY)
    {
        // 타일 배열값 최소가 0
        if(posX < 0 || posY < 0)
        {
            return false;
        }

        // 현재 인벤토리 슬롯의 크기
        // 배열값이라 설정한 크기보다 -1 로 생성되기에 인벤토리 크기와 값이 같아져도 벗어난것으로 간주
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
