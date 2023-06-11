using System.Collections;
using TypeDefs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Interactable : MonoBehaviour
{
    const float CHANGE_ROOM_DELAY = 1f;
    const float CHANGE_LEVEL_DELAY = 3f;

    float f_fadePercent;
    bool b_fading;

    Coroutine crt_coroutine;
    [Header("This Object Type")]
    [SerializeField] public ObjectType type;
    [SerializeField]        Outline.Mode mode                 = Outline.Mode.OutlineVisible;
    [SerializeField]        Color        enabledOutlineColor  = Color.white;
    [SerializeField]        Color        disabledOutlineColor = Color.white;
    [SerializeField]        float        enabledOutlineWidth  = 1f;
    [SerializeField]        float        disabledOutlineWidth = 1f;
    private static readonly int          EmissionColor        = Shader.PropertyToID("_EmissionColor");

    public bool interacted = false;


    private void Awake()
    {
        enabledOutlineColor = new Color(0, 1, 0.4078431f, 1);
        switch (type)
        {
            case ObjectType.MoveDoor:
            case ObjectType.Door:
                disabledOutlineColor = new Color(0, 0, 1, 0.05f);
                enabledOutlineWidth = 4;
                disabledOutlineWidth = 4;
                break;
            case ObjectType.Keypad:
                disabledOutlineColor = new Color(0.5f, 1, 0, 0.05f);
                enabledOutlineWidth = 4;
                disabledOutlineWidth = 4;
                break;
            case ObjectType.Item:
                disabledOutlineColor = new Color(1, 1, 0, 0.05f);
                enabledOutlineWidth = 4;
                disabledOutlineWidth = 4;
                break;
            case ObjectType.CraftingTable:
                disabledOutlineColor = new Color(0, 1, 0.75f, 0.05f);
                enabledOutlineWidth = 4;
                disabledOutlineWidth = 6;
                break;
        }
    }

    private void Start()
    {
        if (gameObject.GetComponent<Outline>() == null)
        {
            Outline tmpOutline = gameObject.AddComponent<Outline>();
            tmpOutline.Instruct(mode, disabledOutlineColor, disabledOutlineWidth);
        }
        
    }

    public void Run(object obj = null)
    {
        switch (type)
        {
            case ObjectType.MoveDoor:
                StartCoroutine(OpenDoor(CHANGE_ROOM_DELAY));
                enabled = false;
                GetComponent<Outline>().enabled = false;
                gameObject.layer = LayerMask.NameToLayer("Default");
                GetComponent<AudioSource>()?.Play();
                break;

            case ObjectType.Door:
                StartCoroutine(OpenDoor());
                enabled = false;
                GetComponent<Outline>().enabled = false;
                gameObject.layer = LayerMask.NameToLayer("Default");
                GetComponent<AudioSource>()?.Play();
                break;

            case ObjectType.Keypad:
                if (InventoryManager.Instance.HasItem(306) || GameManager.Instance.b_hasKey)
                {
                    InventoryManager.Instance.RemoveItem(306);
                    //키패트 라이팅 정보 초기화
                    b_fading = false;
                    f_fadePercent = 0;

                    //코루틴도 종료
                    if (!crt_coroutine.IsUnityNull())
                        StopCoroutine(crt_coroutine);
                    crt_coroutine = null;

                    //문 열기 위해서 문 객체 저장하고
                    GameObject[] go_doors =  {
                        transform.parent.Find("Doors").GetChild(0).gameObject,
                        transform.parent.Find("Doors").GetChild(1).gameObject };

                    //키패드 초록색으로 바꾼뒤
                    transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(0, 8, 0));
                    GameManager.Instance.StartCoroutine(GameManager.Instance.OpenElevator(go_doors, 1f, CHANGE_LEVEL_DELAY, obj as PlayerController));                                                                                                 //문열기 시작
                }
                else
                {
                    GameManager.Instance.systemAlerts.Alert("카드키가 필요합니다");
                    if (b_fading)
                        f_fadePercent = 3f; //3초간 Fading, 이미 Fading중에 또 누르면 시간만 초기화
                    else
                        crt_coroutine = StartCoroutine(ChangeColor(transform.GetChild(0).GetComponent<Renderer>().material, 3f, new Color(8, 0, 0)));          //Emission을 Intensity가 3인 빨간색을 주기 위하여 RGB 8,0,0으로 준다.
                }
                break;

            case ObjectType.Item:
                //아이템 인벤토리에 집어넣고
                GameManager.Instance.GetComponent<InventoryManager>().AddItem(GetComponent<ItemObject>().I_item);

                //아이템 오브젝트 비활성화
                DisableItem();
                break;

            case ObjectType.CraftingTable:
                GameManager.Instance.Inventory("Crafting");
                break;
        }

        interacted = true;
    }

    public void DisableItem()
    {
        //만약 아이템에 MeshRenderer가 있다면
        if (gameObject.TryGetComponent(out MeshRenderer meshRenderer))
            meshRenderer.enabled = false;
        else //아이템에 없다면
        {
            //자식에 있는 MeshRender를 전부 비활성화
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out MeshRenderer childMeshRenderer))
                    childMeshRenderer.enabled = false;
            }
        }

        //Collider도 똑같이 작업
        if (gameObject.TryGetComponent(out Collider _))
            //여러개가 있다면 전부 비활성화
            foreach (var foundCollider in GetComponents<Collider>())
                foundCollider.enabled = false;
        else
        {
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out Collider _))
                    foreach (var foundCollider in child.GetComponents<Collider>())
                        foundCollider.enabled = false;
            }
        }

        //Rigidbody도 똑같지만 중력만 꺼주기
        if (gameObject.TryGetComponent(out Rigidbody _))
            GetComponent<Rigidbody>().useGravity = false;
        else
        {
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out Rigidbody _))
                    child.GetComponent<Rigidbody>().useGravity = false;
            }
        }
    }

    private IEnumerator OpenDoor(float delay = 1.5f)
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

        ////재사용을 한다고 하면 문을 다시 닫아둔다.
        //if(!reuseable) transform.rotation = targetRotation;
        //else transform.rotation = startRotation;

    }

    private IEnumerator ChangeColor(Material mat, float duration, Color targetColor)
    {

        f_fadePercent = duration;
        b_fading = true;
        Color originalColor = mat.GetColor(EmissionColor);

        while (f_fadePercent >= 0)
        {
            f_fadePercent -= Time.deltaTime;

            mat.SetColor(EmissionColor, Color.Lerp(targetColor, originalColor, (duration - f_fadePercent) / duration));
            yield return null;
        }
        b_fading = false;

        mat.SetColor(EmissionColor, originalColor);
    }

    public void ChangeState(bool setEnabled)
    {
        Outline outline = GetComponent<Outline>();

        outline.OutlineColor = setEnabled ? enabledOutlineColor : disabledOutlineColor;
        outline.OutlineWidth = setEnabled ? enabledOutlineWidth : disabledOutlineWidth;
        
        GameManager.Instance.systemAlerts.ShowInspect(gameObject.name);
    }
}