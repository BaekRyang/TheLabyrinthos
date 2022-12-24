using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] GameObject floorPrefab;
    int buildingLevel = 50;
    public GameObject[] levels;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        levels= new GameObject[buildingLevel];
        for (int i = 0; i < buildingLevel; i++)
        {
            levels[i] = Instantiate(floorPrefab, new Vector3(0, 1.6f * i, 0), Quaternion.identity);
            levels[i].name = (i + 1).ToString();
            //if ((i + 1) % 2 == 0)
            //{
            //    levels[i].layer = LayerMask.NameToLayer("LevelEven");
            //    foreach (Transform child in levels[i].transform)
            //    {
            //        foreach (Transform child2 in child)
            //        {
            //            child2.gameObject.layer = LayerMask.NameToLayer("LevelEven");
            //        }
            //    }
            //} else
            //{
            //    levels[i].layer = LayerMask.NameToLayer("LevelOdd");
            //    foreach (Transform child in levels[i].transform)
            //    {
            //        foreach (Transform child2 in child)
            //        {
            //            child2.gameObject.layer = LayerMask.NameToLayer("LevelOdd");
            //        }
            //    }
            //}
        }
    }

    void Update()
    {
        
    }
}
