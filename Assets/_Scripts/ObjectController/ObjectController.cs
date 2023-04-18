using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [Header("The probability of occurrence of an object")]
    [Range(0, 100)]
    [SerializeField] int i_percent = 100;
    [SerializeField] int i_randNum;

    void Start()
    {
        i_randNum = GameManager.Instance.dict_randomObjects["Object"].Next(101);
        if (i_randNum > i_percent) Destroy(this.gameObject);
    }

    void Update()
    {

    }
}
