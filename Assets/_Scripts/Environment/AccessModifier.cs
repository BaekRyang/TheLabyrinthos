using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessModifier : MonoBehaviour
{
    public bool west  = true;
    public bool east  = true;
    public bool south = true;
    public bool north = true;

    private int canPlaceDoor;

    public int CanPlaceDoor
    {
        get
        {
            //서, 동, 남, 북 순서대로 비트로 표현
            canPlaceDoor = 0;
            if (west) canPlaceDoor  |= 1 << 0;
            if (east) canPlaceDoor  |= 1 << 1;
            if (south) canPlaceDoor |= 1 << 2;
            if (north) canPlaceDoor |= 1 << 3;
            return canPlaceDoor;
        }
    }
}