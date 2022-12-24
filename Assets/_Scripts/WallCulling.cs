using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCulling : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }


    //Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("Group");
    //// CullingMask에 "Group" Layer를 제거합니다.
    //Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Group"));
    //// Nothing 상태인 CullingMask에서 Group Layer를 추가합니다.

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "MainFloor":
                
                break;

            case "Stairs":
                break;

            case "Elevator":
                break;

            default:
                break;
        }
        Debug.Log(collision.transform.parent.parent.gameObject.name + " -> " + collision.transform.parent.gameObject.name);
    }
}
