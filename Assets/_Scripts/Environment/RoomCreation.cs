using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;
using TypeDefs;

public class RoomCreation : MonoBehaviour
{
    public Dictionary<int, RoomNode> roomMap; //맵 저장용

    private const int MAX_ATTEMPTS = 10000;

    [SerializeField] GameObject RoomBase;
    [SerializeField] GameObject RoomWall;
    [SerializeField] GameObject RoomDoor_t1;
    [SerializeField] GameObject RoomNoDoor;
    int RoomCount = 10;
    int iRoomSize = 10;

    GameObject go_roomsRoot;

    void Start()
    {
        
    }
    public void CreateSeed(ref string uSeed)
    {
        Random random = new Random();
        uSeed = random.Next(0x000000, 0xFFFFFF).ToString("X"); //6자리 16진수 시드 생성
    }

    private void CreateWallOrDoor(Transform parent, Vector3 position, Quaternion rotation, bool condition)
    //벽이나 문을 판정후 생성
    {
        if (condition)
        {
            GameObject.Instantiate(RoomWall, position, rotation, parent);
        }
        else
        {
            GameObject.Instantiate(RoomDoor_t1, position, rotation, parent);
        }
    }

    public void InitStruct(int seed, int roomCount)
    {
        RoomCount = roomCount;

        int cnt = 0;
        roomMap = new Dictionary<int, RoomNode>(); //방의 각 노드를 저장할 Dictionary
        StructCreation structCreation = new StructCreation(); //구조 생성용 클래스 객체
        while (true)
        {
            if (cnt >= MAX_ATTEMPTS)
            {
                Debug.LogError("TIMEOUT");
                break;
            }

            if ((structCreation.Run(RoomCount, ref roomMap, seed, cnt)))
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
    }

    public void PlaceRoom()
    {
        if (!go_roomsRoot.IsDestroyed()) Destroy(go_roomsRoot); //이전게 남아있으면 Destory한다.
        go_roomsRoot = new GameObject("Rooms");

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
                                                            RoomBase,
                                                            new Vector3((-5 * iRoomSize) + (i % 10) * iRoomSize,            //X
                                                            0,                                                              //Y
                                                            (-4 * iRoomSize) + (i / 10) * iRoomSize),                       //Z
                                                            Quaternion.identity);                                           //R

                if (i % 10 != 0 && i % 10 != 9)
                {
                    if (roomMap.ContainsKey(i - 1) && roomMap.ContainsKey(i + 1) && roomMap[i - 1].RoomType != 0 && roomMap[i + 1].RoomType != 0 && !roomMap.ContainsKey(i - 10) && !roomMap.ContainsKey(i + 10))
                    {
                        tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#f1c94f>--</color>";
                        tmpGO.GetComponent<RoomController>().RT_roomType = RoomType.HorizontalCorridor;
                    }
                }

                if (i > 9 && i < 90)
                {
                    if (roomMap.ContainsKey(i - 10) && roomMap.ContainsKey(i + 10) && roomMap[i - 10].RoomType != 0 && roomMap[i + 10].RoomType != 0 && !roomMap.ContainsKey(i - 1) && !roomMap.ContainsKey(i + 1))
                    {
                        tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#f1c94f>|</color>";
                        tmpGO.GetComponent<RoomController>().RT_roomType = RoomType.VerticalCorridor;
                    }
                }




                if (i == 45)
                {
                    tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#16c60c>Start</color>";
                    tmpGO.GetComponent<RoomController>().RT_roomType = RoomType.StartRoom;
                }
                else
                {
                    switch (roomMap[i].RoomType)
                    {
                        case RoomType.EndRoom:
                            tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#3b78ff>End</color>";
                            break;
                        case RoomType.CraftingRoom:
                            tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#524f6a>Craft</color>";
                            tmpGO.GetComponent<RoomController>().RT_roomType = RoomType.CraftingRoom;
                            break;
                        case RoomType.Shop:
                            tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#efdb17>Shop</color>";
                            tmpGO.GetComponent<RoomController>().RT_roomType = RoomType.Shop;
                            break;
                        case RoomType.KeyRoom:
                            tmpGO.GetComponentInChildren<TMP_Text>().text = "<color=#e74856>Key</color>";
                            tmpGO.GetComponent<RoomController>().RT_roomType = RoomType.KeyRoom;
                            break;
                    }
                }

                //각 방의 4방향을 조사하여 0이면 벽을, 1이면 문을
                RoomNode currentNode = roomMap[i];

                int parentDirection = -1;
                if (currentNode.ParentNode != null) //시작노드는 Root가 없으므로 제외
                {
                    //상위 노드의 상대위치를 저장해둔다.
                    if (currentNode.ParentNode.Id == i - 1) parentDirection = 0;
                    else if (currentNode.ParentNode.Id == i + 1) parentDirection = 1;
                    else if (currentNode.ParentNode.Id == i - 10) parentDirection = 2;
                    else if (currentNode.ParentNode.Id == i + 10) parentDirection = 3;
                }

                //문이 생기는 자리인데, 해당 방향이 상위 노드이면 문을 비워둔 벽 생성
                //이외에는 문이 생겨야하는지 조건에 따라 문 또는 벽을 생성

                //상(0)
                bool shouldCreateWall = i > 90 || (!currentNode.Children.Contains(i + 10) && parentDirection != 3);
                if (!shouldCreateWall && parentDirection == 3) GameObject.Instantiate(RoomNoDoor, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), tmpGO.transform);
                else CreateWallOrDoor(tmpGO.transform, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), shouldCreateWall);

                //우(90)
                shouldCreateWall = i % 10 > 8 || (!currentNode.Children.Contains(i + 1) && parentDirection != 1);
                if (!shouldCreateWall && parentDirection == 1) GameObject.Instantiate(RoomNoDoor, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 90, 0)), tmpGO.transform);
                else CreateWallOrDoor(tmpGO.transform, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 90, 0)), shouldCreateWall);

                //하(180)
                shouldCreateWall = i < 10 || (!currentNode.Children.Contains(i - 10) && parentDirection != 2);
                if (!shouldCreateWall && parentDirection == 2) GameObject.Instantiate(RoomNoDoor, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 180, 0)), tmpGO.transform);
                else CreateWallOrDoor(tmpGO.transform, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 180, 0)), shouldCreateWall);

                //좌(270)
                shouldCreateWall = i % 10 < 1 || (!currentNode.Children.Contains(i - 1) && parentDirection != 0);
                if (!shouldCreateWall && parentDirection == 0) GameObject.Instantiate(RoomNoDoor, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 270, 0)), tmpGO.transform);
                else CreateWallOrDoor(tmpGO.transform, tmpGO.transform.position, Quaternion.Euler(new Vector3(0, 270, 0)), shouldCreateWall);


                tmpGO.GetComponent<RoomController>().index = i;
                tmpGO.GetComponent<RoomController>().SortObjects();

                //if (i == 45) GameObject.Instantiate(Player);

                roomMap[i].RoomObject = tmpGO; //만들어진 오브젝트를 그래프에 넣어준다.
                tmpGO.transform.SetParent(go_roomsRoot.transform);
            }

        }
    }

}
