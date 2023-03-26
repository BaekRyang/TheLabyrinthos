using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPointSelector : MonoBehaviour
{
    private const float RAY_DISTANCE = 2f;
    private int groundLayer;

    // Raycast�� ���Ǵ� ����
    RaycastHit roomCheck;
    Ray roomCheckRay;

    // ���� ���� �̸��� ���� ������Ʈ
    [SerializeField] string prevRoomName = "null";
    [SerializeField] GameObject prevRoom;

    void Awake()
    {
        // ���̾� ����
        groundLayer = 1 << LayerMask.NameToLayer("Ground");
    }

    private void Start()
    {
        // �ʱ� ��ġ���� ����ĳ��Ʈ�� �̿��Ͽ� �÷��̾ ��ġ�� ���� �����ϰ� �ش� ���� ���¸� ����
        Physics.Raycast(transform.position, Vector3.down, out roomCheck, RAY_DISTANCE, groundLayer);
        prevRoom = roomCheck.transform.root.gameObject; // �÷��̾ ��ġ�� ���� ��Ʈ ������Ʈ�� ����
        prevRoomName = prevRoom.name; // �÷��̾ ��ġ�� ���� �̸��� ����
        prevRoom.GetComponent<RoomController>().ChangeRoomState(true); // �÷��̾ ��ġ�� ���� ���¸� ����
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ʈ���� ���� ������ �� ��, ����ĳ��Ʈ�� �̿��Ͽ� �÷��̾ ��ġ�� ���� �����ϰ� �ش� ���� ���¸� ����
        Debug.Log(Physics.Raycast(transform.position, Vector3.down, out roomCheck, RAY_DISTANCE, groundLayer)); // ����ĳ��Ʈ ����� �α׷� ���
        Debug.Log(roomCheck.transform.root.name); // ����ĳ��Ʈ ����� ������ ���� �̸��� �α׷� ���

        prevRoom.GetComponent<RoomController>().ChangeRoomState(false); // ���� ���� ���¸� ����
        prevRoom = roomCheck.transform.root.gameObject; // �÷��̾ ��ġ�� ���� ��Ʈ ������Ʈ�� ����
        prevRoomName = prevRoom.name; // �÷��̾ ��ġ�� ���� �̸��� ����
        prevRoom.GetComponent<RoomController>().ChangeRoomState(true); // �÷��̾ ��ġ�� ���� ���¸� ����
    }

}
