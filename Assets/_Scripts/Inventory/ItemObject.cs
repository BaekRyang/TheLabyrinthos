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

    public bool isUIElement = false;    //인벤토리 UI인지 표시
    public bool hasItem = false;        //인벤토리 UI 전용

    public Item I_item;


    private void Awake()
    {
        if (isUIElement)
        {
            SR_parent = transform.parent.parent.parent.GetComponent<ScrollRect>();
        }
    }
    public void OnScroll(PointerEventData eventData) //스크롤 요소때문에 이벤트를 넘겨준다.
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
