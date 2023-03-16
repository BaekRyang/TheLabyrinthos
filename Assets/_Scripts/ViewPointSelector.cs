using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPointSelector : MonoBehaviour
{
    private const float RAY_DISTANCE = 2f;
    private int groundLayer;

    RaycastHit roomCheck;
    [SerializeField] string prevRoomName = "null";
    [SerializeField] GameObject prevRoom;
    Ray roomCheckRay;

    void Awake()
    {
        groundLayer = 1 << LayerMask.NameToLayer("Ground");
    }

    private void Start()
    {
        Physics.Raycast(transform.position, Vector3.down, out roomCheck, RAY_DISTANCE, groundLayer);
        prevRoom = roomCheck.transform.root.gameObject;
        prevRoomName = prevRoom.name;
        prevRoom.GetComponent<RoomController>().ChangeRoomState(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(Physics.Raycast(transform.position, Vector3.down, out roomCheck, RAY_DISTANCE, groundLayer));
        Debug.Log(roomCheck.transform.root.name);

        prevRoom.GetComponent<RoomController>().ChangeRoomState(false);
        prevRoom = roomCheck.transform.root.gameObject;
        prevRoomName = prevRoom.name;
        prevRoom.GetComponent<RoomController>().ChangeRoomState(true);
    }
}
