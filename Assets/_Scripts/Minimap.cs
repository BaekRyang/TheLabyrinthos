using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public static Minimap instance;
    [SerializeField] int i_boxSize = 75;
    [SerializeField] GameObject GO_roomPrefab;
    [SerializeField] GameObject GO_anchor;
    [SerializeField] public GameObject GO_arrow;
    [SerializeField] public GameObject GO_minimapAnchor;

    GameObject[] GO_rooms;

    private void Awake()
    {
        instance = this;
    }                                                

    private void Start()
    {
        GO_roomPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(i_boxSize, i_boxSize);
        //���� �ڽ� ������ ���̸� �������� ũ�⵵ �ٿ��� �ϴϱ�
    }


    public void CreateMinimap(Dictionary<int, RoomNode> map)
    {
        if (GO_anchor.transform.childCount > 0) //���� ������ ����ϴ� �̴ϸ� ������Ʈ�� ����������
        {
            foreach (Transform child in GO_anchor.transform)
            {
                Destroy(child.gameObject);
            }
            //���� ������� ���� Destroy�Ѵ�.
        }
        GO_rooms = new GameObject[100]; //������ �ִ� �����ʹ� �ʱ�ȭ

        for (int i = 0; i < 100; i++) //���� �ε��� �ѹ��� ����Ǵϱ� 100�� �� ������.
        {
            if (!map.ContainsKey(i)) continue;
            else
            {
                GO_rooms[i] = Instantiate(GO_roomPrefab, GO_anchor.transform);
                GO_rooms[i].transform.localPosition = new Vector3(  ((-5 * i_boxSize) + (i % 10) * i_boxSize),
                                                                    ((-4 * i_boxSize) + (i / 10) * i_boxSize),
                                                                        0);
                //�� �����ϴ� �˰����̶� �����ϰ� �̴ϸʿ� ������Ʈ�� �ٿ����´�.
                //localPosition�� Instantiate�� ������ �� ��� ����� �����Ѵ�.

                GO_rooms[i].name = i.ToString(); //�ĺ���
            }
        }

        return;
    }

    public GameObject GetRoom(int roomNum)
    {
        return GO_rooms[roomNum];
    }

    public void SetAnchor(int roomIdx)
    { //���� �̵��Ҷ� ������ �÷��̾ �ִ� ���� �̴ϸ� �� ����� ������ �Ѵ�.
        GO_anchor.transform.localPosition = new Vector3(-((-5 * i_boxSize) + (roomIdx % 10) * i_boxSize),
                                                        -((-4 * i_boxSize) + (roomIdx / 10) * i_boxSize),
                                                           0);
        //�׳� ���� �˰��� ������ �ݴ�� �ϸ� ��
        return;
    }
}
