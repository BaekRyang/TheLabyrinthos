using System.Collections;
using UnityEngine;

public class BattleAnimate : MonoBehaviour
{
    RectTransform RT_rect;
    public bool b_isPlayer;
    void Start()
    {
        RT_rect = GetComponent<RectTransform>();
    }

    public IEnumerator Run(bool attackerIsPlayer, float duration)
    {
        if (attackerIsPlayer)
        {
            
        }
        else
        {
            
        }

        yield return null;
    }
}
