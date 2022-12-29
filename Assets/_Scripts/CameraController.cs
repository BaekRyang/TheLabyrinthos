using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class CameraController : MonoBehaviour
{
    static public CameraController instance;
    public Vector3 firstPoint;
    public Vector3 secondPoint;
    public float xAngle;
    public float yAngle;
    public float xAngleTemp;
    public float yAngleTemp;

    public int joysticTouchId = 0;

    private void Start()
    {
        instance = this;
        //transform.rotation = Quaternion.Euler(yAngle, xAngle, 0); 
    }
    private void Update()
    {
        if (InputManager.instance.cameraTouched)
        {
            transform.rotation = Quaternion.Euler(yAngle, xAngle, 0.0f);
        }
        
    }

    public void CameraInit(Vector2 _pos)
    {
        firstPoint = _pos;
        xAngleTemp = xAngle;
        yAngleTemp = yAngle;
    }

    public void CameraMove(Vector2 _pos)
    {
        secondPoint = _pos;
        xAngle = xAngleTemp + (secondPoint.x - firstPoint.x) * 180 / Screen.width;
        yAngle = yAngleTemp - (secondPoint.y - firstPoint.y) * 180 / Screen.height;

        //È¸Àü°ª 40 ~ 85 Clamp
        if (yAngle < -40f) yAngle = -40f;
        if (yAngle > 40f) yAngle = 40f;

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
