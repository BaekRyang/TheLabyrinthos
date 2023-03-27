using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{
    public TMP_Text TT_Text;
    public Slider i_SliderValue;

    [SerializeField] bool isFloat = true;
    void Awake()
    {
        TT_Text = GetComponentInChildren<TMP_Text>();
        i_SliderValue = GetComponent<Slider>();
    }

    public void ChangeValue()
    {
        if (isFloat) TT_Text.text = i_SliderValue.value.ToString();
        else TT_Text.text = Mathf.FloorToInt(i_SliderValue.value).ToString();

        //TP���� ��� Float�� ǥ���ϸ� ���Ⱑ ������
    }
}
