using System.Collections.Generic;
using TMPro;
using TypeDefs;
using UnityEngine;
using UnityEngine.UI;

public class EffectIndicator : MonoBehaviour
{
    public GameObject UIObject;
    
    public void UpdateUI()
    {
        if (transform.childCount > 0)
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        

        //Player.Instance.effectList 각 요소를 순환한다.
        //UIObject를 현재 transform의 자식으로 Instantiate 한다.
        foreach (var effect in Player.Instance.effectList)
        {
            var obj = Instantiate(UIObject, transform);
            Image effectIcon = obj.transform.GetChild(0).GetComponent<Image>();
            
            obj.GetComponent<EffectIndicatorObject>().effect        = effect;
            obj.transform.GetChild(1).GetComponent<TMP_Text>().text = effect.expirationRemain.ToString();
            // effectIcon.sprite = GameManager.Instance.effectsManager.GetEffectIcon(effect.effectType);

        }
    }
}