using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject floorPrefab;
    int buildingLevel = 50;
    public GameObject[] levels;

    void Start()
    {
        levels= new GameObject[buildingLevel];
        for (int i = 0; i < buildingLevel - 1; i++)
        {
            levels[i + 1] = Instantiate(floorPrefab, new Vector3(0, 1.6f * (i + 1), 0), Quaternion.identity);
            levels[i + 1].name = "Level" + (i + 2).ToString();
        }
    }

    void Update()
    {
        
    }
}
