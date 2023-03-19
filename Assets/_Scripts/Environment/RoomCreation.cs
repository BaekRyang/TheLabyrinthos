using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

public class RoomCreation : MonoBehaviour
{
    private GameObject InstantiateObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        //오브젝트 생성후 Parent 변경
    {
        GameObject tmpGO = GameObject.Instantiate(prefab, position, rotation);
        tmpGO.transform.SetParent(parent);
        return tmpGO;
    }

    private void CreateWallOrDoor(GameObject wallPrefab, GameObject doorPrefab, Transform parent, Vector3 position, Quaternion rotation, bool condition)
        //벽이나 문을 판정후 생성
    {
        if (condition)
        {
            InstantiateObject(wallPrefab, position, rotation, parent);
        }
        else
        {
            InstantiateObject(doorPrefab, position, rotation, parent);
        }
    }

    private const int MAX_ATTEMPTS = 5000;
    private const int MAP_SIZE = 100;

    [SerializeField] GameObject RoomBase;
    [SerializeField] GameObject RoomWall;
    [SerializeField] GameObject RoomDoor;
    [SerializeField] GameObject Player;
    [SerializeField] int RoomCount = 10;
    int iRoomSize = 10;
    void Start()
    {
        int cnt = 0;
        //방은 언제나 10x10의 사이즈를 갖으므로 배열로 고정할당한다.
        int[] iaMap = new int[MAP_SIZE];
        StructCreation structCreation = new StructCreation();
        while (true)
        {
            if (cnt >= MAX_ATTEMPTS)
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
                Array.Clear(iaMap, 0, MAP_SIZE);
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


                CreateWallOrDoor(RoomWall, RoomDoor, tmpGO.transform, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 270, 0)), i % 10 < 1 || iaMap[i - 1] == 0);
                CreateWallOrDoor(RoomWall, RoomDoor, tmpGO.transform, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 90, 0)), i % 10 > 8 || iaMap[i + 1] == 0);
                CreateWallOrDoor(RoomWall, RoomDoor, tmpGO.transform, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 180, 0)), i < 10 || iaMap[i - 10] == 0);
                CreateWallOrDoor(RoomWall, RoomDoor, tmpGO.transform, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), i > 90 || iaMap[i + 10] == 0);

                tmpGO.GetComponent<RoomController>().index = i;
                tmpGO.GetComponent<RoomController>().SortObjects();

                if(i == 45)
                {
                    GameObject.Instantiate(Player);
                }
            }
        }


    }

    


}
