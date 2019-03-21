using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QPath;

public class Unit : IQPathUnit
{
    public string Name = "Unnamed";
    public int HitPoints = 10;
    public int Strength = 8;
    public int Movement = 2;
    public int MovementRemaining = 2;

    public Hex Hex { get; protected set; }

    public delegate void UnitMovedDelegate(Hex oldHex, Hex newHex);
    public event UnitMovedDelegate OnUnitMoved;

    /// <summary>
    /// List of hexes to walk through (from pathfinder). 
    /// NOTE: First item is always the hex we are standing in. 
    /// </summary>
    Queue<Hex> hexPath;

    //TODO: This should probably be moved to some kind of central option/config file
    const bool MOVEMENT_RULES_LIKE_CIV6 = false;


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

    public void DUMMY_PATHING_FUNCTION()
    {
        Hex[] p = (Hex[])QPath.QPath.FindPath<Hex>(
            Hex.NewHexTileMap, 
            this, 
            Hex, 
            Hex.NewHexTileMap.GetHexAt(Hex.Q +5, Hex.R), 
            Hex.CostEstimate
            );
        Debug.Log("Got pathfinding path of length: " + p.Length);

        SetHexPath(p);
    }

    public void ClearHexPath()// < ------------ Function to call to clear units current pathing. 
    {
        this.hexPath = new Queue<Hex>();
    }

    public Hex[] GetHexPath()
    {
        return (hexPath == null) ? null : hexPath.ToArray();
    }

    public void SetHexPath(Hex[] hexArray)
    {
        this.hexPath = new Queue<Hex>(hexArray);
    }

    public void DoTurn()
    {
        Debug.Log("Do Action");
        // do queued move?

        if (hexPath == null || hexPath.Count == 0)
        {
            return;
        }

        // Grab the first hex from our queue
        /*Hex actualCurrentHex =*/ hexPath.Dequeue();
        Hex newHex = hexPath.Peek();

        if (hexPath.Count == 1)
        {
            // The only hex left in the list is the one we are moving to now, therefore we
            // have no more path to follow, so we clear the queue completely to avoid confusion. 
            hexPath = null;
        }

        // Move to the new Hex 
        SetHex(newHex);
    }

    public float MovementCostToEnterHex(Hex hex)
    {
        //TODO: Implement different movement traits 

        // EXAMPLE: - potential way to call and override the base movement cost for a specific tile depending on the unit that is crossing it. 
        //if (weAreAHillWalker && hex.ElevationType == Hex.ELEVATION_TYPE.HILL)
        //{
        //    return 1;
        //}

        return hex.BaseMovementCost(false, false, false);
    }

    public float AggregateTurnsToEnterHex(Hex hex, float turnsToDate)
    {
        // The issue at hand is that if you are trying to enter a tile with a movement cost greater than your current 
        // remaining movement points, this will eiter result in a cheaper-than expected turn cost (Civ5) or a more expensive
        // than expected turn cost (Civ6)

        float baseTurnsToEnterHex = MovementCostToEnterHex(hex) / Movement; // Ex: Entering a forest is "1" turn.
        if (baseTurnsToEnterHex < 0)
        {
            //Debug.Log("Impassable Terrain at: " + hex.ToString());
            return -99;
        }
        if (baseTurnsToEnterHex > 1)
        {
            // Even if something costs 3 to enter and we have a max move of 2, you can always enter it using a full turn of movement. 
            baseTurnsToEnterHex = 1;
        }

        float turnsRemaining = MovementRemaining / Movement; // Ex: If we are at 1/2 move, then we have .5 turns left.

        float turnsToDateWhole = Mathf.Floor(turnsToDate); // Ex: 4.33 becomes 4
        float turnsToDateFraction = turnsToDate - turnsToDateWhole; // Ex: 4.33 becomes 0.33

        if (turnsToDateFraction > 0 && turnsToDateFraction < 0.01f || turnsToDateFraction > 0.99)
        {
            Debug.LogError("Looks like we've got floating-point drift." + turnsToDate);
            //TODO: Round things?

            if (turnsToDateFraction < 0.01f)
            {
                turnsToDateFraction = 0;
            }
            if (turnsToDateFraction >0.99f)
            {
                turnsToDateWhole += 1;
                turnsToDateFraction = 0;
            }
        }

        float turnsUsedAfterThisMove = turnsToDateFraction + baseTurnsToEnterHex; // Ex: 0.33 + 1

        if(turnsUsedAfterThisMove > 1)
        {
            // We have hit the situation where we don't have enough turns to complete this move. What do we do?
            if (MOVEMENT_RULES_LIKE_CIV6)
            {
                // We arent allowed to enter the tile this move. That means we have to...
                if (turnsToDateFraction == 0)
                {
                // We have full movement, but its not enough to enter the tile. 
                // Ex: We have a move of 2, but the tile costs 3 to enter
                // We are good to go...
                }
                else
                {
                    // We are NOT on a fresh turn -- therefore we need to sit idle for the remainder of this turn. 
                    turnsToDateWhole += 1;
                    turnsToDateFraction = 0;
                }

                // So now we know for a fact that we are starting the move into diffcult terrain on a fresh turn. 
                turnsUsedAfterThisMove = baseTurnsToEnterHex;              
            }
            else
            {
                // Civ5-style movement state that we can always enter a tile, even if we don't have enough movement left.
                turnsUsedAfterThisMove = 1;
            }
        }

        // turnsUsedAfterThisMove is now some value from 0...1...(this includes the fractional part of moves from previous turns).

        // Do we return the number of turns THIS move is going to take? 
        // I say no, this is an "aggregate" function, so return the total turn cost of turnsToDate + turns for this move. 

        return turnsToDate + turnsUsedAfterThisMove;
    }


    /// <summary>
    /// Turn cost to enter a hex (Ex: 0.5 turns if movement cost is 1 and we have 2 max movement)
    /// </summary>
    public float CostToEnterHex(IQPathTile sourceTile, IQPathTile destinationTile)
    {
        return 1;
    }
}
