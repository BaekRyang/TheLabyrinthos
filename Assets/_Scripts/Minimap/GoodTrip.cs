using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoodTrip : MonoBehaviour, IPointerClickHandler
{
    public int     index;
    public bool    entered;
    public Vector2 position;
    //클릭 이벤트 추가
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!entered)
            GetComponent<MMF_Player>().PlayFeedbacks();
        else
        {
            if (index == Player.Instance.GetComponent<PlayerController>().roomIndex)
                return;
            //현재방의 인덱스를 통해 방의 위치를 구해서 플레이어를 해당 위치로 옮긴다.
            Player.Instance.transform.position = new Vector3(
                (index % 10 - 5) * 10,
                0,
                (index / 10 - 4) * 10
            );
        }
    }
}
