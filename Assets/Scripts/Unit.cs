using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public string Name = "Unnamed";
    public int HitPoints = 10;
    public int Strength = 8;
    public int Movement = 2;
    public int MovementRemaining = 2;

    public Hex Hex { get; protected set; }

    // **** SET THE UNIT ON A SPECIFIC HEX **** //

    public void SetHex (Hex hex)
    {
        if (hex != null) // if unit is already on a hex, 
        {
            Hex.RemoveUnit(this); // remove it from that hex
        }

        Hex = hex;  
        Hex.AddUnit(this); // add the unit to the new hex
    }
}
