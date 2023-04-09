using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] public int index;
    [SerializeField] public bool startActive;
    [SerializeField] GameObject doors;
    [SerializeField] GameObject walls;
    [SerializeField] GameObject ceilings;

    [SerializeField] GameObject[] GO_RoomPrefabs;

    [SerializeField] Material[] M_Ceiling;

    void Awake()
    {
        GO_RoomPrefabs = Resources.LoadAll<GameObject>("RoomStructures");
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
            GameObject GO_Struct = GameObject.Instantiate(GO_RoomPrefabs[0], transform.position, Quaternion.identity);
            GO_Struct.transform.SetParent(transform);
        } else
        {
            GameObject GO_Struct = GameObject.Instantiate(GO_RoomPrefabs[1], transform.position, Quaternion.identity);
            GO_Struct.transform.SetParent(transform);
        }
        
    }

}
