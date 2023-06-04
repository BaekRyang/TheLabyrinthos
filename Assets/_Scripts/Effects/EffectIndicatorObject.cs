using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EffectIndicatorObject : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public  Effect                 effect;

    public void OnPointerEnter(PointerEventData eventData)
    {
        var infoBox = PopUpManager.Instance.descBoxP;
        
        infoBox.box.gameObject.SetActive(true);

        infoBox.box.position = transform.position;

        infoBox.title.text = GameManager.Instance.effectsManager.GetEffectDesc(effect, true);
        infoBox.desc.text  = GameManager.Instance.effectsManager.GetEffectDetails(effect);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PopUpManager.Instance.descBox.gameObject.SetActive(false);
    }
}