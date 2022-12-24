using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCulling : MonoBehaviour
{
    [SerializeField] string nowStay;
    [SerializeField] int nowLevel;

    //Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("Group");
    //// CullingMask에 "Group" Layer를 제거합니다.
    //Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Group"));
    //// Nothing 상태인 CullingMask에서 Group Layer를 추가합니다.


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.name != "RoomColider") return;
        else
        {
            nowStay = other.transform.parent.name;
            nowLevel = int.Parse(other.transform.parent.parent.name);

            if (nowLevel % 2 == 0)
            {
                Camera.main.cullingMask = ~(1 << LayerMask.NameToLayer("LevelOdd"));
            } else
            {
                Camera.main.cullingMask = ~(1 << LayerMask.NameToLayer("LevelEven"));
            }
        }
    }
}
