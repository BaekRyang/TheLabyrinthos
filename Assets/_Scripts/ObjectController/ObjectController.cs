using Tayx.Graphy.Utils.NumString;
using TypeDefs;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [Header("Set in Inspector")]
    [Range(0, 100)]
    [SerializeField] int i_percent = 100;
    [SerializeField] bool b_customCreature;

    [Header("CustomCreature")]
    [SerializeField] private float damage;
    [SerializeField] private float defense;
    [SerializeField] private float health;
    [SerializeField] private float speed;
    [SerializeField] private float prepareSpeed;
    [SerializeField] private CreatureSpritePack sprites;

    [Header("Set Automatically")]
    [SerializeField] int i_randNum;
    [SerializeField] bool b_isCreature;
    [SerializeField] Creature CR_creature;

    void Start()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        RoomController RC_room = transform.parent.parent.GetComponent<RoomController>();
        
        if (b_isCreature)
        {
            i_randNum = GameManager.Instance.dict_randomObjects["Creature"].Next(101);
            if (i_randNum > i_percent)
            {
                Destroy(gameObject);
                return;
            }

            Creature tmpCR = CreatureManager.Instance.GetRandomCreatureByLevel(GameManager.Instance.i_level);
            
            if (b_customCreature) //특수 크리쳐설정
            {
                tmpCR.damage       =  (tmpCR.damage  * damage).ToInt();
                tmpCR.defense      =  (tmpCR.defense * defense).ToInt();
                tmpCR.health       *= health;
                tmpCR.speed        *= speed;
                tmpCR.prepareSpeed =  Mathf.Clamp((tmpCR.prepareSpeed * prepareSpeed).ToInt(), 0, 100);
                
                tmpCR.spritePack = sprites.fullBody != null ? sprites : tmpCR.spritePack;
            }

            
            
            if (RC_room == null || RC_room.alreadyCleared) return;
            RC_room.hasCreature = true;
            RC_room.CR_creature   = tmpCR;
            RC_room.GO_creature   = gameObject;
        }
        else
        {
            i_randNum = GameManager.Instance.dict_randomObjects["Object"].Next(101);
            if (i_randNum > i_percent)
                Destroy(gameObject);
        }
    }
}
