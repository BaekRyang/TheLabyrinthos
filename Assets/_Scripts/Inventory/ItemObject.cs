using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemObject : MonoBehaviour, IScrollHandler
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

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    throw new System.NotImplementedException();
    //}
}
