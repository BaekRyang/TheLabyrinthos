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
    float doorRaycastDistance = 1.0f; // 충돌 감지를 위한 레이캐스트 거리
    LayerMask doorCollisionLayerMask = ~0; // 충돌 감지 대상 레이어 (0은 모든 레이어와 충돌 감지)

    while (t < 1)
    {
        t += Time.deltaTime * 1.5f;

        // 현재 회전에서 다음 회전까지의 각도
        float currentAngle = Mathf.LerpAngle(startRotation.eulerAngles.y, targetRotation.eulerAngles.y, t);

        // 각도를 회전 값으로 변환
        Quaternion currentRotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, currentAngle, transform.rotation.eulerAngles.z));

        //// 레이캐스트 시작 위치와 방향 설정
        //Vector3 raycastOrigin = transform.position;
        //Vector3 raycastDirection = currentRotation * Vector3.forward;

        //// 레이캐스트를 사용하여 문의 경로에 물체가 있는지 확인
        //RaycastHit hit;
        //if (Physics.Raycast(raycastOrigin, raycastDirection, out hit, doorRaycastDistance, doorCollisionLayerMask))
        //{
        //    // 물체와 충돌이 감지되면 코루틴 종료
        //    yield break;
        //}

        // 충돌이 감지되지 않으면 문을 회전시킴
        transform.rotation = currentRotation;
        yield return null;
    }

    // 완전히 열린 상태로 설정
    transform.rotation = targetRotation;
}

}
