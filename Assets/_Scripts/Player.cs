using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TypeDefs;

public class Player : MonoBehaviour
{
    PlayerStats PS_playerStats;

    void Awake()
    {
        PS_playerStats = new PlayerStats();
    }

    void Start()
    {
        //인벤토리 key, value 형식으로 사용
        Dictionary<string, int> dict_inventory = new Dictionary<string, int>();
    }

    void Update()
    {
        
    }
    
    public ref PlayerStats GetPlayerStats()
    {
        return ref PS_playerStats;
    }
}


