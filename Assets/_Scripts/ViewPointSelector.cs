using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPointSelector : MonoBehaviour
{
    private const float RAY_DISTANCE = 2f;
    private int groundLayer;

    // Raycast에 사용되는 변수
    RaycastHit roomCheck;
    Ray roomCheckRay;

    // 이전 방의 이름과 게임 오브젝트
    [SerializeField] string prevRoomName = "null";
    [SerializeField] GameObject prevRoom;

    void Awake()
    {
        // 레이어 설정
        groundLayer = 1 << LayerMask.NameToLayer("Ground");
    }

    private void Start()
    {
        // 초기 위치에서 레이캐스트를 이용하여 플레이어가 위치한 방을 감지하고 해당 방의 상태를 변경
        Physics.Raycast(transform.position, Vector3.down, out roomCheck, RAY_DISTANCE, groundLayer);
        prevRoom = roomCheck.transform.root.gameObject; // 플레이어가 위치한 방의 루트 오브젝트를 저장
        prevRoomName = prevRoom.name; // 플레이어가 위치한 방의 이름을 저장
        prevRoom.GetComponent<RoomController>().ChangeRoomState(true); // 플레이어가 위치한 방의 상태를 변경
    }

    private void OnTriggerEnter(Collider other)
    {
        // 트리거 영역 안으로 들어갈 때, 레이캐스트를 이용하여 플레이어가 위치한 방을 감지하고 해당 방의 상태를 변경
        Debug.Log(Physics.Raycast(transform.position, Vector3.down, out roomCheck, RAY_DISTANCE, groundLayer)); // 레이캐스트 결과를 로그로 출력
        Debug.Log(roomCheck.transform.root.name); // 레이캐스트 결과로 감지된 방의 이름을 로그로 출력

        prevRoom.GetComponent<RoomController>().ChangeRoomState(false); // 이전 방의 상태를 변경
        prevRoom = roomCheck.transform.root.gameObject; // 플레이어가 위치한 방의 루트 오브젝트를 저장
        prevRoomName = prevRoom.name; // 플레이어가 위치한 방의 이름을 저장
        prevRoom.GetComponent<RoomController>().ChangeRoomState(true); // 플레이어가 위치한 방의 상태를 변경
    }

}
