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

    void Start()
    {

    }

    public void ChangeRoomState(bool _state)
    {
        if (_state)
        {
            doors.SetActive(true);
            ceilings.SetActive(false);
        } else
        {
            doors.SetActive(false);
            ceilings.SetActive(true);
        }
    }

    public void SortObjects()
    {
        transform.name = "Room_" + index.ToString();
        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    // 오브젝트가 Instantiate로 생성된 오브젝트일 경우
        //    if (transform.GetChild(i).name.EndsWith("(Clone)"))
        //    {
        //        while (transform.GetChild(i).childCount != 0)
        //        {
        //            if (transform.GetChild(i).GetChild(0).name == "Door")
        //            {
        //                Debug.Log("DOOR");
        //                // Door 오브젝트를 doors 오브젝트의 하위로 이동
        //                transform.GetChild(i).GetChild(0).parent = doors.transform;
        //            }
        //            // Wall 오브젝트를 찾음
        //            else if (transform.GetChild(i).GetChild(0).name == "Wall")
        //            {
        //                // Wall 오브젝트를 walls 오브젝트의 하위로 이동
        //                transform.GetChild(i).GetChild(0).parent = walls.transform;
        //            }
        //            // 그 외의 오브젝트를 찾음
        //            else
        //            {
        //                // 해당 오브젝트를 현재 게임 오브젝트의 하위로 이동
        //                transform.GetChild(i).GetChild(0).parent = this.transform;
        //            }
        //        }
        //        // Instantiate로 생성된 게임 오브젝트를 삭제
        //        Destroy(transform.GetChild(i).gameObject);
        //    }
        //}
        //doors.SetActive(true);
        //ceilings.SetActive(false);
    }

}
