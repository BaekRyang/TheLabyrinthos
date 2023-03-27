using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Creatures creatures;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        creatures = new Creatures();
    }

    void Update()
    {
        
    }
}
