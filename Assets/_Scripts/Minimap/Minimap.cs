using MoreMountains.Feedbacks;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public static           Minimap                     Instance;
    [SerializeField] public int                         i_boxSize = 75;
    [SerializeField]        GameObject                  GO_roomPrefab;
    [SerializeField] public GameObject                  GO_anchor;
    [SerializeField] public GameObject                  GO_arrow;
    [SerializeField] public GameObject                  GO_minimapAnchor;
    [SerializeField] public MMF_Player                  textMMF;
    [DoNotNormalize] public Dictionary<int, GameObject> rooms;

    GameObject[] GO_rooms;

    private void Awake()
    {
        Instance = this;
    }                                                

    private void Start()
    {
        GO_roomPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(i_boxSize, i_boxSize);
        //만약 박스 사이즈 줄이면 프리팹의 크기도 줄여야 하니까
    }


    public void CreateMinimap(Dictionary<int, RoomNode> map)
    {
        rooms = new Dictionary<int, GameObject>();
        if (GO_anchor.transform.childCount > 0) //만약 기존에 사용하던 미니맵 오브젝트가 남아있으면
        {
            foreach (Transform child in GO_anchor.transform)
            {
                if (child.name == "Arrow")
                    continue;
                
                Destroy(child.gameObject);
                //화살표를 제외하고 전부 없앤다.
            }
        }
        GO_rooms = new GameObject[100]; //이전에 있던 데이터는 초기화

        for (int i = 0; i < 100; i++) //레벨 로딩때 한번만 실행되니까 100개 다 돌린다.
        {
            if (!map.ContainsKey(i)) continue;
            GO_rooms[i] = Instantiate(GO_roomPrefab, GO_anchor.transform);
            var goodTrip = GO_rooms[i].GetComponent<GoodTrip>();
            goodTrip.index = i;
            goodTrip.position = new Vector2(
                -5 * i_boxSize + i % 10 * i_boxSize,
                -4 * i_boxSize + i / 10 * i_boxSize);
            
            GO_rooms[i].transform.localPosition = goodTrip.position;
            //방 생성하는 알고리즘이랑 동일하게 미니맵에 오브젝트를 붙여놓는다.
            //localPosition은 Instantiate때 설정할 수 없어서 나누어서 설정한다.

            GO_rooms[i].name = i.ToString(); //식별용

            if (i == 45)
                GO_rooms[i].GetComponent<GoodTrip>().entered = true;
        }
    }

    public GameObject GetRoom(int roomNum)
    {
        return GO_rooms[roomNum];
    }

    public void SetAnchor(int roomIdx)
    { //방을 이동할때 언제나 플레이어가 있는 방이 미니맵 한 가운데에 오도록 한다.
        GO_anchor.transform.localPosition = new Vector3(-((-5 * i_boxSize) + (roomIdx % 10) * i_boxSize),
                                                        -((-4 * i_boxSize) + (roomIdx / 10) * i_boxSize),
                                                           0);
        //그냥 생성 알고리즘 방향을 반대로 하면 됨
    }
}
