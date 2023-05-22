using TypeDefs;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [Header("Set in Inspector")]
    [Range(0, 100)]
    [SerializeField] int i_percent = 100;
    [SerializeField] CreatureSpritePack CSP_spritePack;
    [SerializeField] bool b_customCreature;
    [SerializeField] Creature CR_customCreature;

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

        if (b_isCreature)
        {
            i_randNum = GameManager.Instance.dict_randomObjects["Creature"].Next(101);
            if (i_randNum > i_percent)
            {
                Destroy(gameObject);
                return;
            }

            Creature tmpCR = GameManager.Instance.CR_levelDefault;
            if (b_customCreature)
            {
                CR_creature = new Creature(
                    tmpCR.damage * CR_customCreature.damage,
                    tmpCR.defense * CR_customCreature.defense,
                    tmpCR.health * CR_customCreature.health,
                    tmpCR.speed * CR_customCreature.speed,
                    tmpCR.prepareSpeed + CR_customCreature.prepareSpeed,
                    tmpCR.spritePack = CSP_spritePack
                );
            }
            else
                CR_creature = new Creature(tmpCR, CSP_spritePack);

            RoomController RC_room = transform.parent.parent.GetComponent<RoomController>();
            if (RC_room != null)
            {
                RC_room.b_hasCreature = true;
                RC_room.CR_creature = CR_creature;
                RC_room.GO_creature = gameObject;
            }
        }
        else
        {
            i_randNum = GameManager.Instance.dict_randomObjects["Object"].Next(101);
            if (i_randNum > i_percent)
                Destroy(gameObject);
        }
    }
}
