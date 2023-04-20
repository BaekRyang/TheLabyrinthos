using System.Collections;
using System.Collections.Generic;
using TypeDefs;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [Header("Set in Inspector")]
    [Range(0, 100)]
    [SerializeField] int i_percent = 100;
    [SerializeField] CreatureSpritePack CSP_spritePack;
    [SerializeField] bool b_customCreature = false;
    [SerializeField] Creature CR_customCreature;

    [Header("Set Automatically")]
    [SerializeField] int i_randNum;
    [SerializeField] bool b_isCreature = false;
    [SerializeField] Creature CR_creature;
    void Start()
    {
        if (b_isCreature)
        {
            i_randNum = GameManager.Instance.dict_randomObjects["Creature"].Next(101);
            if (i_randNum > i_percent) Destroy(this.gameObject);
            else
            {
                if (b_customCreature)
                {
                    Creature tmpCR = GameManager.Instance.CR_levelDefault; //베이스 크리쳐
                    CR_creature = new Creature( //새로 만들어서 할당
                        tmpCR.damage * CR_customCreature.damage,
                        tmpCR.defense * CR_customCreature.defense,
                        tmpCR.health * CR_customCreature.health,
                        tmpCR.speed * CR_customCreature.speed,
                        tmpCR.prepareSpeed + CR_customCreature.prepareSpeed,
                        tmpCR.spritePack = CSP_spritePack
                        );
                } else
                {   //커스텀 크리쳐 아니면 해당 레벨 기본 스텟
                    CR_creature = new Creature(GameManager.Instance.CR_levelDefault, CSP_spritePack);
                }
                
                RoomController RC_room = transform.parent.parent.GetComponent<RoomController>();
                RC_room.b_hasCreature = true;
                RC_room.CR_creature = CR_creature;
                RC_room.GO_creature = this.gameObject;
            }
        }
        else
        {
            i_randNum = GameManager.Instance.dict_randomObjects["Object"].Next(101);
            if (i_randNum > i_percent) Destroy(this.gameObject);
        }
        
    }

    void Update()
    {

    }
}
