using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField] GameObject joystick1;
    [SerializeField] GameObject joystick2;

    private float screenWidth;
    private float screenHeight;

    public bool cameraTouched = false;
    public bool joystickTouched = false;

    public int cameraId = -1;
    public int joystickId = -1;
    void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                switch (Input.GetTouch(i).phase)
                {
                    case TouchPhase.Began:
                        if ((Input.GetTouch(i).position.x >= screenWidth / 2))
                        {
                            if (!cameraTouched)
                            {
                                Debug.Log("CAM");
                                cameraTouched = true;
                                cameraId = Input.GetTouch(i).fingerId;
                            }
                            else Debug.Log("CAM_Denied");
                        } else
                        {
                            if (!joystickTouched)
                            {
                                Debug.Log("Joystic");
                                joystickTouched = true;
                                joystickId = Input.GetTouch(i).fingerId;
                            }
                            else Debug.Log("JOY_Denied");
                        }
                        break;
                    case TouchPhase.Moved:
                        if (Input.GetTouch(i).fingerId == cameraId)
                        {
                            joystick1.GetComponent<RectTransform>().position = Input.GetTouch(i).position;
                        }
                        else if (Input.GetTouch(i).fingerId == joystickId)
                        {
                            joystick2.GetComponent<RectTransform>().position = Input.GetTouch(i).position;
                        }
                        
                        break;
                    case TouchPhase.Stationary:
                        break;
                    case TouchPhase.Ended:
                        if (Input.GetTouch(i).fingerId == cameraId)
                        {
                            cameraTouched = false;
                            cameraId = -1;
                        }
                        else if (Input.GetTouch(i).fingerId == joystickId)
                        {
                            joystickTouched = false;
                            joystickId = -1;
                        }
                        break;
                    case TouchPhase.Canceled:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
