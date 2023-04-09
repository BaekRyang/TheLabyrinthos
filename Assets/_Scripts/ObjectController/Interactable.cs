using System.Collections;
using UnityEngine;
using UnityEngine.UI;

enum ObjectType
{
    MoveDoor,
    Door,
    Elevator,
    Item
}

    
public class Interactable : MonoBehaviour
{
    const float CHANGE_ROOM_DELAY = 1f;

    public float f_fadePercent = 0;
    public bool b_fading = false;

    Coroutine crt_coroutine = null;
    [SerializeField] ObjectType type;

    private void Start()
    {
    }

    public void Run(object obj = null)
    {
        switch (type)
        {
            case ObjectType.MoveDoor:
                //문 열때 페이드인/아웃 시키면서 문 반대쪽 점대칭 위치로 순간이동 시켜준다. (카메라도 돌려줌)
                StartCoroutine(MoveCameraAroundDoor(transform.gameObject, obj as PlayerController));
                break;

            case ObjectType.Door:
                StartCoroutine(OpenDoor());
                this.enabled = false;
                this.tag = "Untagged";
                GetComponent<Outline>().enabled = false;
                //문은 한번 열었으면 일반 오브젝트로 바꿔준다.
                break;

            case ObjectType.Elevator:
                if (GameManager.Instance.hasKey)
                {
                    //키패트 라이팅 정보 초기화
                    b_fading = false;
                    f_fadePercent = 0;

                    //코루틴도 종료
                    StopCoroutine(crt_coroutine);
                    crt_coroutine = null;

                    GameObject[] go_doors = { transform.parent.Find("Doors").GetChild(0).gameObject, transform.parent.Find("Doors").GetChild(1).gameObject };   //문 열기 위해서 문 객체 저장하고
                    transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(0, 8, 0)) ;                                    //키패드 초록색으로 바꾼뒤
                    StartCoroutine(OpenElevator(go_doors, 1f));                                                                                                 //문열기 시작
                } else
                {
                    if (b_fading) f_fadePercent = 3f;                                                                                                           //3초간 Fading / 이미 Fading중에 또 누르면 시간만 초기화
                    else crt_coroutine = StartCoroutine(ChangeColor(transform.GetChild(0).GetComponent<Renderer>().material, 3f, new Color(8, 0, 0)));          //Emission을 Intensity가 3인 빨간색을 주기 위하여 RGB 8,0,0으로 준다.
                }
                break;

            case ObjectType.Item:
                break;

            default:
                break;
        }
    }

    IEnumerator MoveCameraAroundDoor(GameObject door, PlayerController player)
    { //회전할 필요 없음 / 플레이어 위치만 설정해주면 알아서 위치가 잡힘
      
        StartCoroutine(OpenDoor(CHANGE_ROOM_DELAY, true));

        //플레이어 입력을 잠금
        player.b_camControll = true;

        //화면 암전
        StartCoroutine(GameManager.Instance.CurtainModify(false, CHANGE_ROOM_DELAY));
        yield return new WaitForSeconds(CHANGE_ROOM_DELAY);
        StartCoroutine(GameManager.Instance.CurtainModify(true, CHANGE_ROOM_DELAY));
        
        //카메라 회전 시키기
        player.yaw += 180; //우클릭 회전기능때문에 해당 값을 180도 회전 시켜야 적용됨
        StartCoroutine(MovePlayerThroughDoor(door, player));
        //카메라의 상대 위치 계산

        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 relativeCameraPosition = new Vector3(door.transform.position.x - (cameraPosition.x - door.transform.position.x), cameraPosition.y, door.transform.position.z - (cameraPosition.z - door.transform.position.z));
        Quaternion relativeCameraRotation = Quaternion.LookRotation(door.transform.position - relativeCameraPosition, Vector3.up);

        //카메라를 180도 회전
        Camera.main.transform.position = relativeCameraPosition;
        Camera.main.transform.Rotate(0f, 180f, 0f);

        //플레이어 입력을 풀어줌
        player.b_camControll = false;
        yield return null;
    }

    IEnumerator MovePlayerThroughDoor(GameObject door, PlayerController player)
    {
        MeshRenderer meshRenderer = door.GetComponent<MeshRenderer>();
        Vector3 doorCenter = meshRenderer.bounds.center;

        //플레이어의 상대 위치 계산 (문 기준 좌표계 사용, Y축 값 유지)
        Vector3 playerPosition = player.transform.position;
        Vector3 relativePlayerPosition = -(playerPosition - doorCenter);
        Vector3 newPosition = doorCenter + relativePlayerPosition;
        newPosition.y = player.transform.position.y;

        //플레이어를 문 기준 180도 회전한 위치로 이동
        player.transform.position = newPosition;

        yield return null;
    }

    private IEnumerator OpenDoor(float delay = 1.5f, bool reuseable = false)
    {
        Quaternion startRotation = transform.rotation;
        Vector3 targetRotationEulerAngles = transform.rotation.eulerAngles;
        targetRotationEulerAngles.y -= 120;
        Quaternion targetRotation = Quaternion.Euler(targetRotationEulerAngles);

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * delay;

            //현재 회전에서 다음 회전까지의 각도
            float currentAngle = Mathf.LerpAngle(startRotation.eulerAngles.y, targetRotation.eulerAngles.y, t);

            //각도를 회전 값으로 변환
            Quaternion currentRotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, currentAngle, transform.rotation.eulerAngles.z));

            //문을 회전시킴
            transform.rotation = currentRotation;
            yield return null;
        }

        //재사용을 한다고 하면 문을 다시 닫아둔다.
        if(!reuseable) transform.rotation = targetRotation;
        else transform.rotation = startRotation;

    }

    private IEnumerator OpenElevator(GameObject[] obj, float duration)
    {
        float elapsedTime = 0f;

        Vector3 obj1StartPosition = obj[0].transform.localPosition;
        Vector3 obj1EndPosition = new Vector3(obj1StartPosition.x, obj1StartPosition.y, obj1StartPosition.z + obj[0].transform.localScale.z);

        Vector3 obj2StartPosition = obj[1].transform.localPosition;
        Vector3 obj2EndPosition = new Vector3(obj2StartPosition.x, obj2StartPosition.y, obj2StartPosition.z - obj[1].transform.localScale.z);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            obj[0].transform.localPosition = Vector3.Lerp(obj1StartPosition, obj1EndPosition, t);
            obj[1].transform.localPosition = Vector3.Lerp(obj2StartPosition, obj2EndPosition, t);

            yield return null;
        }

        obj[0].transform.localPosition = obj1EndPosition;
        obj[1].transform.localPosition = obj2EndPosition;
    }

    private IEnumerator ChangeColor(Material mat, float duration, Color targetColor)
    {
        f_fadePercent = duration;
        b_fading = true;
        Color originalColor = mat.GetColor("_EmissionColor");

        while (f_fadePercent >= 0)
        {
            f_fadePercent -= Time.deltaTime;

            mat.SetColor("_EmissionColor", Color.Lerp(targetColor, originalColor, (duration - f_fadePercent) / duration));
            yield return null;
        }
        b_fading = false;

        mat.SetColor("_EmissionColor", originalColor);
    }

}
