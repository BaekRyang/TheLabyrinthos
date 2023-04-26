using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{

    [Header("Set Automatically")]
    public TMP_Text text;
    public Slider sliderValue;

    [SerializeField] bool isFloat = true;
    [SerializeField] bool hasText = false;
    void Awake()
    {
        if (hasText) text = transform.parent.Find("HPText").GetComponentInChildren<TMP_Text>();
        sliderValue = GetComponent<Slider>();
    }

    public void ChangeValue()
    {
        if (hasText)
        {
            if (isFloat) text.text = sliderValue.value.ToString();
            else text.text = Mathf.FloorToInt(sliderValue.value).ToString();
        }
        //TP같은 경우 Float로 표시하면 보기가 안좋음
    }
}
