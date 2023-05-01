using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemObject : MonoBehaviour, IScrollHandler, IPointerEnterHandler, IPointerExitHandler
{
    ScrollRect SR_parent;

    public bool isUIElement = false;    //�κ��丮 UI���� ǥ��
    public bool hasItem = false;        //�κ��丮 UI ����

    public Item I_item;


    private void Awake()
    {
        if (isUIElement)
        {
            SR_parent = transform.parent.parent.parent.GetComponent<ScrollRect>();
        }
    }
    public void OnScroll(PointerEventData eventData) //��ũ�� ��Ҷ����� �̺�Ʈ�� �Ѱ��ش�.
    {
        SR_parent.OnScroll(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.name == "ItemCell(Clone)") return;
        var infoBox = InventoryManager.Instance.RT_infoBox;
        infoBox.gameObject.SetActive(true);

        if (this.I_item.IT_type == TypeDefs.ItemType.Weapon)
        {
            infoBox.GetChild(1).transform.localScale = Vector3.one;
            infoBox.GetChild(1).GetComponentInChildren<TMP_Text>().text = I_item.s_description.Replace("\\n", "\n");
        }
        else infoBox.GetChild(1).transform.localScale = Vector3.zero;

        infoBox.localScale = Vector3.one;
        infoBox.position = transform.position;
        infoBox.GetChild(0).GetChild(0).GetComponentInChildren<TMP_Text>().text = I_item.s_name;
        infoBox.GetChild(0).GetChild(1).GetComponentInChildren<TMP_Text>().text = I_item.s_description.Replace("\\n", "\n");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var infoBox = InventoryManager.Instance.RT_infoBox;
        infoBox.GetChild(1).transform.localScale = Vector3.one;
        infoBox.localScale = Vector3.zero;

        
    }
}
