using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemObject : MonoBehaviour, IScrollHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    ScrollRect SR_parent;

    public bool b_isInventoryUIElement = false;       //인벤토리 UI인지 표시
                                                      //조합대에서도 이거 쓰니까 있는 설정
    public bool b_canClick = false;
    public Item I_item;

    private void Awake()
    {
        if (b_isInventoryUIElement)
            SR_parent = transform.parent.parent.parent.GetComponent<ScrollRect>();
    }

    public void UpdateItem(int amount = 1)
    {
        if (I_item == null)
            transform.GetChild(0).GetComponent<Image>().sprite = InventoryManager.Instance.dict_imgList[-1];
        else 
            transform.GetChild(0).GetComponent<Image>().sprite = InventoryManager.Instance.dict_imgList[I_item.i_id];

        if (amount > 1)
        {
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(1).GetComponent<TMP_Text>().text = amount.ToString();
        } else
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void TansparentItem(bool transparent)
    {
        if (transparent)
            transform.GetChild(0).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        else
            transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    public void OnScroll(PointerEventData eventData) //스크롤 요소때문에 이벤트를 넘겨준다.
    {
        if(!SR_parent.IsUnityNull())
            SR_parent.OnScroll(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (I_item.IT_type == TypeDefs.ItemType.Undefined) return;

        var infoBox = InventoryManager.Instance.RT_infoBox;
        infoBox.gameObject.SetActive(true);

        var textComponents = infoBox.GetComponentsInChildren<TMP_Text>();

        if (I_item.IT_type == TypeDefs.ItemType.Weapon)
        {
            infoBox.GetChild(1).transform.localScale = Vector3.one;
            Weapon tmpWeapon = I_item as Weapon;
            textComponents[2].text = tmpWeapon.s_inspectText.Replace("\\n", "\n");
            textComponents[2].text += "\nDUR : " + tmpWeapon.i_durability + "/" + tmpWeapon.i_maxDurability;
        }
        else infoBox.GetChild(1).transform.localScale = Vector3.zero;

        infoBox.localScale = Vector3.one;
        infoBox.position = transform.position;
        textComponents[0].text = I_item.s_name;
        textComponents[1].text = I_item.s_description.Replace("\\n", "\n");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var infoBox = InventoryManager.Instance.RT_infoBox;
        infoBox.GetChild(1).transform.localScale = Vector3.one;
        infoBox.localScale = Vector3.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (b_canClick)
        {
            GetComponentInParent<Crafting>().CraftItem();
            OnPointerExit(eventData);
        }
    }
}
