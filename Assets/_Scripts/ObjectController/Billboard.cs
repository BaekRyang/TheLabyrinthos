using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform targetPosition;
    
    private void Start()
    {
        targetPosition = Player.Instance.GetComponent<PlayerController>().transform;
    }
    
    private void Update()
    {
        //플레이어를 바라보게 한다. (y축만 회전)
        transform.LookAt(targetPosition); 
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }
}
