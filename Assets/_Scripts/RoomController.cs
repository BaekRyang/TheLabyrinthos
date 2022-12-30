using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] GameObject doors;
    [SerializeField] GameObject ceilings;
    [SerializeField] bool startActive;

    void Start()
    {
        if (!startActive)
        {
            doors.SetActive(false);
            ceilings.SetActive(true);
        }
        
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
}
