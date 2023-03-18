using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

public class RoomCreation : MonoBehaviour
{
    [SerializeField] GameObject RoomBase;
    [SerializeField] GameObject RoomWall;
    [SerializeField] GameObject RoomDoor;
    [SerializeField] int RoomCount = 10;
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

            if ((structCreation.Run(RoomCount, ref iaMap)))
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

        //방 구조 베이스 생성
        for (int i = 0; i < 100; i++)
        {
            if (iaMap[i] == 0)
            {
                continue;
                //GameObject tmpGO = GameObject.Instantiate(
                //                                            RoomBase, new Vector3((-5 * iRoomSize) + (i % 10) * iRoomSize,  //X
                //                                            0,                                                              //Y
                //                                            (-4 * iRoomSize) + (i / 10) * iRoomSize),                       //Z
                //                                            Quaternion.identity);                                           //R
                //tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#111111> X </color>";
            }
            else
            {
                GameObject tmpGO = GameObject.Instantiate(
                                                            RoomBase, new Vector3((-5 * iRoomSize) + (i % 10) * iRoomSize,  //X
                                                            0,                                                              //Y
                                                            (-4 * iRoomSize) + (i / 10) * iRoomSize),                       //Z
                                                            Quaternion.identity);                                           //R

                //가장 왼쪽/오른쪽이 아니고 양옆에 방이 있으면
                if (i % 10 != 0 && i % 10 != 9 && iaMap[i - 1] != 0 && iaMap[i + 1] != 0 && iaMap[i - 10] == 0 && iaMap[i + 10] == 0)
                {
                    tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#f1c94f> -- </color>";
                }
                //가장 위/아래가 아니고 위 아래에 방이 있으면
                else if (i  > 9 && i  < 90 && iaMap[i - 10] != 0 && iaMap[i + 10] != 0 && iaMap[i - 1] == 0 && iaMap[i + 1] == 0)
                {
                    tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#f1c94f> | </color>";
                }
                if (i == 45)
                {
                    tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#16c60c> S </color>";
                }
                else if (iaMap[i] == 2)
                {
                    tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#3b78ff> E </color>";
                }
                else if (iaMap[i] == 9)
                {
                    tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#e74856> K </color>";
                } 


                if (i % 10 < 0) //가장 왼쪽?
                {
                    GameObject.Instantiate(RoomWall, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 270, 0))).transform.SetParent(tmpGO.transform);

                }
                else if (iaMap[i - 1] == 0)
                {
                    GameObject.Instantiate(RoomWall, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 270, 0))).transform.SetParent(tmpGO.transform);
                }
                else
                {
                    GameObject.Instantiate(RoomDoor, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 270, 0))).transform.SetParent(tmpGO.transform);
                }

                if (i % 10 > 8) //가장 오른쪽?
                {
                    GameObject.Instantiate(RoomWall, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 90, 0))).transform.SetParent(tmpGO.transform);
                }
                else if (iaMap[i + 1] == 0) //오른쪽이 0?
                {
                    GameObject.Instantiate(RoomWall, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 90, 0))).transform.SetParent(tmpGO.transform);
                }
                else
                {
                    GameObject.Instantiate(RoomDoor, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 90, 0))).transform.SetParent(tmpGO.transform);
                }

                if (i < 9) //가장 위쪽?
                {
                    GameObject.Instantiate(RoomWall, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 180, 0))).transform.SetParent(tmpGO.transform);
                }
                else if (iaMap[i - 10] == 0) //왼쪽이 0?
                {
                    GameObject.Instantiate(RoomWall, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 180, 0))).transform.SetParent(tmpGO.transform);
                }
                else
                {
                    GameObject.Instantiate(RoomDoor, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 180, 0))).transform.SetParent(tmpGO.transform);
                }

                if (i > 90) //가장 아래쪽?
                {
                    GameObject.Instantiate(RoomWall, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 0, 0))).transform.SetParent(tmpGO.transform);
                }
                else if (iaMap[i + 10] == 0) //아래쪽이 0?
                {
                    GameObject.Instantiate(RoomWall, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 0, 0))).transform.SetParent(tmpGO.transform);
                }
                else
                {
                    GameObject.Instantiate(RoomDoor, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 0, 0))).transform.SetParent(tmpGO.transform);
                }


                tmpGO.GetComponent<RoomController>().index = i;
            }
        }


    }
    
    void Update()
    {
        
    }

}
