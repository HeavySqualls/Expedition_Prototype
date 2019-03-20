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

    public delegate void UnitMovedDelegate(Hex oldHex, Hex newHex);
    public event UnitMovedDelegate OnUnitMoved;

    // **** SET THE UNIT ON A SPECIFIC HEX **** //

    public void SetHex (Hex newHex)
    {
        Hex oldHex = Hex;
        if (Hex != null) // if unit is already on a hex, 
        {
            Hex.RemoveUnit(this); // remove it from that hex
        }

        Hex = newHex;  

        Hex.AddUnit(this); // add the unit to the new hex

        if (OnUnitMoved != null)
        {
            OnUnitMoved(oldHex, newHex);
        }
    }

    public void DoTurn()
    {
        Debug.Log("DoTurn");
        // do queued move?

        // TESTING: Move us one tile to the right

        Hex oldHex = Hex;
        Hex newHex = oldHex.NewHexTileMap.GetHexAt(oldHex.Q + 1, oldHex.R);

        SetHex(newHex);
    }
}
