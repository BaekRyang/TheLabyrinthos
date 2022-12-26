using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class CameraController : MonoBehaviour
{
    [SerializeField] Vector3 firstPoint;
    [SerializeField] Vector3 secondPoint;
    [SerializeField] float xAngle;
    [SerializeField] float yAngle;
    [SerializeField] float xAngleTemp;
    [SerializeField] float yAngleTemp;

    public int joysticTouchId = 0;

    private void Start()
    {
        //transform.rotation = Quaternion.Euler(yAngle, xAngle, 0); 
    }
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(Input.touchCount-1).fingerId != joysticTouchId)
            {
                
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    firstPoint = Input.GetTouch(0).position;
                    xAngleTemp = xAngle;
                    yAngleTemp = yAngle;
                }
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    secondPoint = Input.GetTouch(0).position;
                    xAngle = xAngleTemp + (secondPoint.x - firstPoint.x) * 180 / Screen.width;
                    yAngle = yAngleTemp - (secondPoint.y - firstPoint.y) * 180 / Screen.height;

                    //È¸Àü°ª 40 ~ 85 Clamp
                    if (yAngle < -40f) yAngle = -40f;
                    if (yAngle > 40f) yAngle = 40f;
                }
            }
        }
        
        transform.rotation = Quaternion.Euler(yAngle, xAngle, 0.0f);
    }

    public bool IsPointerOverUIObject(Vector2 touchPos)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        eventDataCurrentPosition.position = touchPos;

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }

}
