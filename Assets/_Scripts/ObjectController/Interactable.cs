using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ObjectType
{
    Door,
    Item
}
public class Interactable : MonoBehaviour
{
    [SerializeField] ObjectType type;
    public void Run()
    {
        switch (type)
        {
            case ObjectType.Door:
                StartCoroutine(OpenDoor());
                this.enabled = false;
                this.tag = "Untagged";
                GetComponent<Outline>().enabled = false;
                //한번 상호작용 했으면 일반 오브젝트로 편입
                break;
            case ObjectType.Item:
                break;
            default:
                break;
        }
    }

    private IEnumerator OpenDoor()
    {
        Quaternion startRotation = transform.rotation;
        Vector3 targetRotationEulerAngles = transform.rotation.eulerAngles;
        targetRotationEulerAngles.y -= 120;
        Quaternion targetRotation = Quaternion.Euler(targetRotationEulerAngles);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 1.5f;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }

        // 완전히 열린 상태로 설정
        transform.rotation = targetRotation;
    }
}
