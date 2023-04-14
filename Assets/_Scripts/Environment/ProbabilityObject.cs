using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbabilityObject : MonoBehaviour
{
    [Header("The probability of occurrence of an object")]
    [Range(0, 100)]
    [SerializeField] int i_Percent = 100;
    [SerializeField] int i_randNum;

    void Start()
    {
        //i_randNum = GameManager.Instance.randomObjects["Object"].Next(101);
        //if (i_randNum < i_Percent) ;
    }

    void Update()
    {
        
    }
}
