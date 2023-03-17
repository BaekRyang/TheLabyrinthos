using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

public class RoomCreation : MonoBehaviour
{
    [SerializeField] GameObject RoomBase;
    int iRoomSize = 10;
    void Start()
    {
        int cnt = 0;
        int[] iaMap = new int[100];
        StructCreation structCreation = new StructCreation();
        while (true)
        {
            if (cnt >= 5000)
            {
                Debug.LogError("TIMEOUT");
                break;
            }

            if ((structCreation.Run(40, ref iaMap)))
            {
                Debug.Log("CREATED");
                break;
            }
            else
            {
                Array.Clear(iaMap, 0, 100);
                structCreation = new StructCreation();
                Debug.LogWarning("DISCARD");
                cnt++;
            }

        }

        for(int i = 0; i < 100; i++)
        {
            if (iaMap[i] == 0) continue;
            else
            {
                GameObject tmpGO = GameObject.Instantiate(
                                                            RoomBase, new Vector3((-5 * iRoomSize) + (i % 10) * iRoomSize,  //X
                                                            0,                                                              //Y
                                                            (-4 * iRoomSize) + (i / 10) * iRoomSize),                       //Z
                                                            Quaternion.identity);                                           //R

                if (i == 45)
                {
                    tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#16c60c> S </color>";
                } else if (iaMap[i] == 2)
                {
                    tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#3b78ff> E </color>";
                } else if (iaMap[i] == 9)
                {
                    tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#e74856> K </color>";
                }
            }
        }


        //规 备炼 海捞胶 积己
        
        

    }
    
    void Update()
    {
        
    }

}
