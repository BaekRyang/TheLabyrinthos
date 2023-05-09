using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DataCarrier : MonoBehaviour
{
    public bool useSeed;
    public string seed;

    public void GetBoolSeedData(GameObject sender)
    {
        useSeed = sender.GetComponent<Toggle>().isOn;
    }
}
