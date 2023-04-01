using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    PlayerStats PS_PlayerStats;

    void Awake()
    {
        PS_PlayerStats = new PlayerStats();
    }

    void Start()
    {
        //인벤토리 key, value 형식으로 사용
        Dictionary<string, int> inventory = new Dictionary<string, int>();
    }

    void Update()
    {
        
    }
    
    public ref PlayerStats GetPlayerStats()
    {
        return ref PS_PlayerStats;
    }
}

public class PlayerStats { 
    //기본 스텟
    public float f_Health = 100.0f;
    public float f_MaxHealth = 100.0f;
    public float f_Exp = 1.0f;
    public float f_Speed = 1.0f;
    public int i_Defense = 5;
    public int i_PrepareSpeed = 0;
    public int i_Damage = 10;
}
