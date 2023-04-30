using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemObject : MonoBehaviour, IScrollHandler
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

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    throw new System.NotImplementedException();
    //}
}
