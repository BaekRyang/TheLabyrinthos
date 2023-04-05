using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
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
            InstantiateObject(RoomDoor_t1, position, rotation, parent);
        }
    }

    private const int MAX_ATTEMPTS = 5000;

    [SerializeField] GameObject RoomBase;
    [SerializeField] GameObject RoomWall;
    [SerializeField] GameObject RoomDoor;
    [SerializeField] GameObject RoomDoor_t1;
    [SerializeField] GameObject RoomNoDoor;
    [SerializeField] GameObject Player;
    [SerializeField] int RoomCount = 10;
    int iRoomSize = 10;
    void Start()
    {
        int cnt = 0;
        //방은 언제나 10x10의 사이즈를 갖으므로 배열로 고정할당한다.
        Dictionary<int, RoomNode> roomMap = new Dictionary<int, RoomNode>();
        StructCreation structCreation = new StructCreation();
        while (true)
        {
            if (cnt >= MAX_ATTEMPTS)
            {
                Debug.LogError("TIMEOUT");
                break;
            }

            if ((structCreation.Run(RoomCount, ref roomMap)))
            {
                Debug.Log("CREATED");
                break;
            }
            else
            {
                roomMap.Clear();
                structCreation = new StructCreation();
                Debug.LogWarning("DISCARD");
                cnt++;
            }

        }

        //방 구조 베이스 생성
        for (int i = 0; i < 100; i++)
        {
            if (!roomMap.ContainsKey(i)) //key가 없으면 아무것도 없는곳임
            {
                continue;
            }
            else
            {
                GameObject tmpGO = GameObject.Instantiate(
                                                            RoomBase, new Vector3((-5 * iRoomSize) + (i % 10) * iRoomSize,  //X
                                                            0,                                                              //Y
                                                            (-4 * iRoomSize) + (i / 10) * iRoomSize),                       //Z
                                                            Quaternion.identity);                                           //R

                if(i % 10 != 0 && i % 10 != 9 && i > 9 && i < 90)
                {
                    if (roomMap.ContainsKey(i - 1) && roomMap.ContainsKey(i + 1) && roomMap[i - 1].RoomType != 0 && roomMap[i + 1].RoomType != 0 && !roomMap.ContainsKey(i - 10) && !roomMap.ContainsKey(i + 10))
                    {
                        tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#f1c94f> -- </color>";
                    }
                    else if (roomMap.ContainsKey(i - 10) && roomMap.ContainsKey(i + 10) && roomMap[i - 10].RoomType != 0 && roomMap[i + 10].RoomType != 0 && !roomMap.ContainsKey(i - 1) && !roomMap.ContainsKey(i + 1))
                    {
                        tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#f1c94f> | </color>";
                    }

                }

                if (i == 45)
                {
                    tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#16c60c> S </color>";
                }
                else if (roomMap[i].RoomType == RoomType.EndRoom)   
                {
                    tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#3b78ff> E </color>";
                }
                else if (roomMap[i].RoomType == RoomType.KeyRoom)
                {
                    tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#e74856> K </color>";
                }

                //각 방의 4방향을 조사하여 0이면 벽을, 1이면 문을
                RoomNode currentNode = roomMap[i];
                RoomNode parentNode;
                if (currentNode.ParentIndex != -1) parentNode = roomMap[currentNode.ParentIndex];

                int parentDirection = -1;
                if (currentNode.ParentIndex != -1)
                {
                    if (currentNode.ParentIndex == i - 1) parentDirection = 0;
                    else if (currentNode.ParentIndex == i + 1) parentDirection = 1;
                    else if (currentNode.ParentIndex == i - 10) parentDirection = 2;
                    else if (currentNode.ParentIndex == i + 10) parentDirection = 3;
                }

                bool shouldCreateWallLeft = i % 10 < 1 || (!currentNode.Children.Contains(i - 1) && parentDirection != 0);
                if (!shouldCreateWallLeft && parentDirection == 0) InstantiateObject(RoomNoDoor, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 270, 0)), tmpGO.transform);
                else CreateWallOrDoor(RoomWall, RoomDoor, tmpGO.transform, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 270, 0)), shouldCreateWallLeft);

                bool shouldCreateWallRight = i % 10 > 8 || (!currentNode.Children.Contains(i + 1) && parentDirection != 1);
                if (!shouldCreateWallRight && parentDirection == 1) InstantiateObject(RoomNoDoor, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 90, 0)), tmpGO.transform);
                else CreateWallOrDoor(RoomWall, RoomDoor, tmpGO.transform, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 90, 0)), shouldCreateWallRight);

                bool shouldCreateWallUp = i < 10 || (!currentNode.Children.Contains(i - 10) && parentDirection != 2);
                if (!shouldCreateWallUp && parentDirection == 2) InstantiateObject(RoomNoDoor, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 180, 0)), tmpGO.transform);
                else CreateWallOrDoor(RoomWall, RoomDoor, tmpGO.transform, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 180, 0)), shouldCreateWallUp);

                bool shouldCreateWallDown = i > 90 || (!currentNode.Children.Contains(i + 10) && parentDirection != 3);
                if (!shouldCreateWallDown && parentDirection == 3) InstantiateObject(RoomNoDoor, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), tmpGO.transform);
                else CreateWallOrDoor(RoomWall, RoomDoor, tmpGO.transform, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), shouldCreateWallDown);



                tmpGO.GetComponent<RoomController>().index = i;
                tmpGO.GetComponent<RoomController>().SortObjects();

                if (i == 45)
                {
                    GameObject.Instantiate(Player);
                }
            }
        }


    }

    


}
