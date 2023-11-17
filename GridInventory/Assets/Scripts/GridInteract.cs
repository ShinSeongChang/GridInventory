using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// RequireComponent 어트리뷰트는 해당 타입의 컴포넌트를 해당 오브젝트에 종속시킨다.
// 사용법은 클래스 상단에 어트리뷰트 추가
// 해당 컴포넌트가 없을시 추가를 해주기도 하며, 해당 컴포넌트를 제거하려할시 경고문구를 보이며 삭제 불가하게 한다.
[RequireComponent(typeof(ItemGrid))]
public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // 인벤토리 컨트롤러는 메인카메라에 할당해뒀음
    InventoryController inventoryController;

    // 현재 오브젝트가 ItemGrid 스크립트도 들고있음
    ItemGrid itemGrid;
    private void Awake()
    {
        // as를 통하여 캐싱하는 부분에서 안정감을 높이는 듯?
        inventoryController = FindObjectOfType(typeof(InventoryController)) as InventoryController;
        itemGrid = GetComponent<ItemGrid>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스가 들어왔을때만 인벤토리 컨트롤러에 해당 인벤토리 할당
        inventoryController.SelectedItemGrid = itemGrid;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스가 인벤토리 나갈시 해당 인벤토리 해제
        inventoryController.SelectedItemGrid = null;

    }
}
