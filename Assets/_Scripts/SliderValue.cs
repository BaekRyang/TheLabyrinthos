using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{
    public TMP_Text text;
    public Slider sliderValue;

    [SerializeField] bool isFloat = true;
    void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
        sliderValue = GetComponent<Slider>();
    }

    public void ChangeValue()
    {
        if (isFloat) text.text = sliderValue.value.ToString();
        else text.text = Mathf.FloorToInt(sliderValue.value).ToString();

        //TP같은 경우 Float로 표시하면 보기가 안좋음
    }
}
