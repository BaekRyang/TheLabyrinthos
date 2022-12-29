using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

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
        instance = this;
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
                                cameraTouched = true;
                                cameraId = Input.GetTouch(i).fingerId;
                                joystick1.GetComponent<RectTransform>().position = Input.GetTouch(i).position;
                                CameraController.instance.CameraInit(Input.GetTouch(i).position);
                            }
                            else Debug.Log("CAM_Denied");
                        } else
                        {
                            if (!joystickTouched)
                            {
                                Debug.Log("Joystic");
                                joystickTouched = true;
                                joystickId = Input.GetTouch(i).fingerId;
                                joystick2.GetComponent<RectTransform>().position = Input.GetTouch(i).position;
                            }
                            else Debug.Log("JOY_Denied");
                        }
                        break;
                    case TouchPhase.Moved:
                        if (Input.GetTouch(i).fingerId == cameraId)
                        {
                            joystick1.transform.Find("InnerCircle").GetComponent<RectTransform>().position = Input.GetTouch(i).position;
                            CameraController.instance.CameraMove(Input.GetTouch(i).position);
                        }
                        else if (Input.GetTouch(i).fingerId == joystickId)
                        {
                            joystick2.transform.Find("InnerCircle").GetComponent<RectTransform>().position = Input.GetTouch(i).position;
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
