using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// RequireComponent ��Ʈ����Ʈ�� �ش� Ÿ���� ������Ʈ�� �ش� ������Ʈ�� ���ӽ�Ų��.
// ������ Ŭ���� ��ܿ� ��Ʈ����Ʈ �߰�
// �ش� ������Ʈ�� ������ �߰��� ���ֱ⵵ �ϸ�, �ش� ������Ʈ�� �����Ϸ��ҽ� ������� ���̸� ���� �Ұ��ϰ� �Ѵ�.
[RequireComponent(typeof(ItemGrid))]
public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // �κ��丮 ��Ʈ�ѷ��� ����ī�޶� �Ҵ��ص���
    InventoryController inventoryController;

    // ���� ������Ʈ�� ItemGrid ��ũ��Ʈ�� �������
    ItemGrid itemGrid;

    private void Awake()
    {
        inventoryController = FindObjectOfType(typeof(InventoryController)) as InventoryController;
        itemGrid = GetComponent<ItemGrid>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ���콺�� ���������� �κ��丮 ��Ʈ�ѷ��� �ش� �κ��丮 �Ҵ�
        inventoryController.SelectedItemGrid = itemGrid;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ���콺�� �κ��丮 ������ �ش� �κ��丮 ����
        inventoryController.SelectedItemGrid = null;

    }
}
