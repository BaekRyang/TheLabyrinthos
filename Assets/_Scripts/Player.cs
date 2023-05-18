using TypeDefs;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerStats PS_playerStats;
    public Weapon WP_weapon;
    void Awake()
    {
        PS_playerStats = new PlayerStats();
    }

    public ref PlayerStats GetPlayerStats()
    {
        return ref PS_playerStats;
    }

    public void ConsumeTurn()
    {
        //내구도 하나 빼주기
        WP_weapon.i_durability--;
    }
}


