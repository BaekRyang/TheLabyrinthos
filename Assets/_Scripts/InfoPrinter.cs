using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfoPrinter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [SerializeField] string        s_name;
    [SerializeField] string        s_desc;
    bool                           b_hover;
    private PopUpManager.PopUpInfo descBoxPack;

    public void Start()
    {
        descBoxPack = PopUpManager.Instance.descBoxP;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        b_hover = true;
        if (name == "ItemCell(Clone)") return;

        descBoxPack.box.gameObject.SetActive(true);

        descBoxPack.box.position = Input.mousePosition;
        
        descBoxPack.title.text = s_name;
        descBoxPack.desc.text  = s_desc.Replace("\\n", "\n");

        StartCoroutine(PopUpManager.UpdateUI(descBoxPack));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        b_hover            = false;
        descBoxPack.box.gameObject.SetActive(false);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (b_hover)
        {
            descBoxPack.box.position = Input.mousePosition + new Vector3(10, -20, 0);
        }
    }
}