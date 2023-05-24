using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EffectIndicatorObject : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public  Effect        effect;
    private RectTransform infoBox;

    private void Start()
    {
        infoBox = InventoryManager.Instance.RT_descBox;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoBox.gameObject.SetActive(true);
        infoBox.localScale = Vector3.one;

        infoBox.position = this.transform.position;
        
        infoBox.GetChild(0).GetChild(0).GetComponentInChildren<TMP_Text>().text =
            GameManager.Instance.effectsManager.GetEffectDesc(effect, true);
        
        infoBox.GetChild(0).GetChild(1).GetComponentInChildren<TMP_Text>().text = 
            GameManager.Instance.effectsManager.GetEffectDetails(effect);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        infoBox.localScale = Vector3.zero;
    }
}
