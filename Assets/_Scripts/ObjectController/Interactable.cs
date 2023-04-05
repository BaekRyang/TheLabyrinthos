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
                GetComponent<Animator>().SetBool("isOpen", true);
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
}
