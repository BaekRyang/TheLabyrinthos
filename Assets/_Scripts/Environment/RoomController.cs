using TypeDefs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class RoomController : MonoBehaviour
{
    [Header("Set Automatically")]
    [SerializeField] public int index;
    [SerializeField] GameObject walls;
    [SerializeField] GameObject ceilings;
    [SerializeField] public GameObject go_specialObject;
    [SerializeField] public RoomType RT_roomType;
    
    [Header("Battle Encounter System")]
    [SerializeField] public bool hasCreature;
    [SerializeField] public bool forcedBattle;
    [SerializeField] public Creature CR_creature;
    [SerializeField] public GameObject GO_creature;
    
    public void ChangeRoomState(bool _state)
    {
        ceilings.SetActive(!_state);
    }

    public void SortObjects()
    {
        transform.name = "Room_" + index;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.StartsWith("DoorWall"))
                continue;

            //오브젝트가 Instantiate로 생성된 오브젝트일 경우
            if (transform.GetChild(i).name.EndsWith("(Clone)"))
            {
                while (transform.GetChild(i).childCount != 0)
                {
                    if (transform.GetChild(i).GetChild(0).name == "Door")
                        Debug.Log("DOOR");
                        //Door 오브젝트를 doors 오브젝트의 하위로 이동
                    //Wall 오브젝트를 찾음
                    else if (transform.GetChild(i).GetChild(0).name == "Wall")
                        //Wall 오브젝트를 walls 오브젝트의 하위로 이동
                        transform.GetChild(i).GetChild(0).parent = walls.transform;
                    //그 외의 오브젝트를 찾음
                    else
                        //해당 오브젝트를 현재 게임 오브젝트의 하위로 이동
                        transform.GetChild(i).GetChild(0).parent = transform;
                }
                //Instantiate로 생성된 게임 오브젝트를 삭제
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        GameObject randomPickedRoom = GameManager.Instance.GetRoomObject(RT_roomType);
        GameObject GO_Struct        = Instantiate(randomPickedRoom, transform.position, Quaternion.identity);
        if (RT_roomType == RoomType.HorizontalCorridor)
            GO_Struct.transform.Rotate(new Vector3(0, 90, 0)); //세로방은 90도 회전
        GO_Struct.transform.SetParent(transform);

        if (index == 45)
            go_specialObject = GO_Struct.transform.Find("ElevatorBox").Find("Display").GetChild(0).gameObject;
        //이 방은 GameManager에서 관리해야하는 오브젝트가 있어서 여기서 등록해준다.

        if (GO_Struct.TryGetComponent(out BGMPlayer bgmPlayer))
        {
            var newBGMPlayer = transform.AddComponent<BGMPlayer>();
            newBGMPlayer.bgm        = bgmPlayer.bgm;
            newBGMPlayer.middleTime = bgmPlayer.middleTime;
            newBGMPlayer.LoadSetting();
        }
    }

    public void DestroyCreature()
    {
        Destroy(GO_creature);
        hasCreature = false;
        GO_creature = null;
    }
}
