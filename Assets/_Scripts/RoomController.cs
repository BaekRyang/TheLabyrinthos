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
        for (int i = 0; i < transform.childCount; i++)
        {
            // ������Ʈ�� Instantiate�� ������ ������Ʈ�� ���
            if (transform.GetChild(i).name.EndsWith("(Clone)"))
            {
                while (transform.GetChild(i).childCount != 0)
                {
                    if (transform.GetChild(i).GetChild(0).name == "Door")
                    {
                        Debug.Log("DOOR");
                        // Door ������Ʈ�� doors ������Ʈ�� ������ �̵�
                        transform.GetChild(i).GetChild(0).parent = doors.transform;
                    }
                    // Wall ������Ʈ�� ã��
                    else if (transform.GetChild(i).GetChild(0).name == "Wall")
                    {
                        // Wall ������Ʈ�� walls ������Ʈ�� ������ �̵�
                        transform.GetChild(i).GetChild(0).parent = walls.transform;
                    }
                    // �� ���� ������Ʈ�� ã��
                    else
                    {
                        // �ش� ������Ʈ�� ���� ���� ������Ʈ�� ������ �̵�
                        transform.GetChild(i).GetChild(0).parent = this.transform;
                    }
                }
                // Instantiate�� ������ ���� ������Ʈ�� ����
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

}
