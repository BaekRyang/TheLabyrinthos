using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TypeDefs;

public class Player : MonoBehaviour
{
    public PlayerStats PS_playerStats;
    public Weapon WP_weapon;
    void Awake()
    {
        PS_playerStats = new PlayerStats();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    
    public ref PlayerStats GetPlayerStats()
    {
        return ref PS_playerStats;
    }
}


