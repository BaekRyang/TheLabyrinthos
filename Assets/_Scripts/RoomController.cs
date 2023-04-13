using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum SpecialRoomType
{
    Normal,
    VerticalCorridor,
    HorizontalCorridor
}

public class RoomController : MonoBehaviour
{
    [Header("Set Automatically")]
    [SerializeField] public int index;
    [SerializeField] GameObject walls;
    [SerializeField] GameObject ceilings;
    [SerializeField] public GameObject go_specialObject;
    [SerializeField] public SpecialRoomType RT_roomType;

    GameManager GM;

    private void Awake()
    {
        GM = GameManager.Instance;
    }

    public void ChangeRoomState(bool _state)
    {
        if (_state)
        {
            ceilings.SetActive(false);
        } else
        {
            ceilings.SetActive(true);
        }
    }

    public void SortObjects()
    {
        transform.name = "Room_" + index.ToString();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.StartsWith("DoorWall")) continue;

            // 오브젝트가 Instantiate로 생성된 오브젝트일 경우
            if (transform.GetChild(i).name.EndsWith("(Clone)"))
            {
                while (transform.GetChild(i).childCount != 0)
                {
                    if (transform.GetChild(i).GetChild(0).name == "Door")
                    {
                        Debug.Log("DOOR");
                        // Door 오브젝트를 doors 오브젝트의 하위로 이동
                        //transform.GetChild(i).GetChild(0).parent = doors.transform;
                    }
                    // Wall 오브젝트를 찾음
                    else if (transform.GetChild(i).GetChild(0).name == "Wall")
                    {
                        // Wall 오브젝트를 walls 오브젝트의 하위로 이동
                        transform.GetChild(i).GetChild(0).parent = walls.transform;
                    }
                    // 그 외의 오브젝트를 찾음
                    else
                    {
                        // 해당 오브젝트를 현재 게임 오브젝트의 하위로 이동
                        transform.GetChild(i).GetChild(0).parent = this.transform;
                    } 
                }
                // Instantiate로 생성된 게임 오브젝트를 삭제
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        if (index == 45)
        {
            GameObject GO_Struct = GameObject.Instantiate(GM.GetRoomObject(0), transform.position, Quaternion.identity);
            GO_Struct.transform.SetParent(transform);
            go_specialObject = GO_Struct.transform.Find("ElevatorBox").Find("Display").GetChild(0).gameObject;
            //이 방은 GameManager에서 관리해야하는 오브젝트가 있어서 여기서 등록해준다.
        } else if (RT_roomType == SpecialRoomType.Normal)
        { //특수방 아니면 랜덤으로 뽑는다.
            GameObject GO_Struct = GameObject.Instantiate(GM.GetRoomObject(), transform.position, Quaternion.identity);
            GO_Struct.transform.SetParent(transform);
        } else
        { //특수방
            GameObject GO_Struct;

            switch (RT_roomType)
            {
                case SpecialRoomType.VerticalCorridor:
                    GO_Struct = GameObject.Instantiate(GM.GO_CorridorPrefabs[0], transform.position, Quaternion.identity);
                    GO_Struct.transform.SetParent(transform);
                    break;

                case SpecialRoomType.HorizontalCorridor:
                    GO_Struct = GameObject.Instantiate(GM.GO_CorridorPrefabs[0], transform.position, Quaternion.Euler(0, 90, 0));
                    GO_Struct.transform.SetParent(transform);
                    break;

                default:
                    break;

            }

            
        }
        
    }

}
