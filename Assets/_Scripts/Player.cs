using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class PlayerStats { 
    //기본 스텟
    public float health = 100.0f;
    public float maxHealth = 100.0f;
    public float exp = 1.0f;
    public float speed = 1.0f;
    public int defense = 5;
    public int prepareSpeed = 0;
    public int damage = 10;
}
