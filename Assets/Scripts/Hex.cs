using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using System.Linq;

/// <summary>
/// The Hex class defines the grid position, world space position, size,
/// neighbours, etc... of a Hex tile. However, it does NOT interact with
/// Unity directly in any way. 
/// </summary>

public class Hex
{
    public Hex(NEWHexTileMap newHexTileMap, int q, int r)
    {
        this.NewHexTileMap = newHexTileMap;
         
        this.Q = q;
        this.R = r;
        this.S = -(q + r);
    }
    
    // Q + R + S = 0
    // S = -(Q + R)
    
    public readonly int Q; // Column
    public readonly int R; // Row
    public readonly int S; // Sum

    // Data for map generation and maybe in-game effects
    public float Elevation;
    public float Moisture;

    //TODO: Need some kind of property to track hex type (plains, grasslands, etc.....)
    //TODO: Need property to track hex details (forest, mine, farm...)

    public readonly NEWHexTileMap NewHexTileMap;
    
    static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(2.992f) / 2; // <--------------------- HEX HORIZONTAL SPACING VARIABLE!!

    float radius = 2f;

    HashSet<Unit> units;
    
    /// <summary>
    /// Retuns the world-space position of this hex
    /// </summary>

    public Vector3 Position()
    {
        return new Vector3(
            HexHorizontalSpacing() * (this.Q + this.R/2f), 
            0, 
            HexVerticalSpacing() * this.R);
    }

    public float HexHeight()
    {
        return radius * 2;
    }

    public float HexWidth()
    {
        return WIDTH_MULTIPLIER * HexHeight();
    }

    public float HexVerticalSpacing()
    {
        return HexHeight() * 0.75f; // <------------------------------------------------------ HEX VERTICAL SPACING VARIABLE!!
    }

    public float HexHorizontalSpacing()
    {
        return HexWidth();
    }

    public Vector3 PositionFromCamera()
    {
        return NewHexTileMap.GetHexPosition(this);
    }

    //CODE FOR SETTING UP A GLOBAL CAMERA (eg. a map that wraps around from left to right, top to bottom like a globe)
    public Vector3 PositionFromCamera(Vector3 cameraPosition, float numRows, float numColumns)
    {
        float mapHeight = numRows * HexVerticalSpacing();
        float mapWidth = numColumns * HexHorizontalSpacing();

        Vector3 position = Position();

        if (NewHexTileMap.allowWrapEastWest)
        {

            float howManyWidthsFromCamera = (position.x - cameraPosition.x) / mapWidth;

            // We want howManyWidthsFromCamera to be between -0.5 to 0.5
            if (Mathf.Abs(howManyWidthsFromCamera) <= 0.5f)
            {
                return position;
            }

            if (howManyWidthsFromCamera > 0)
            {
                howManyWidthsFromCamera += 0.5f;
            }
            else
            {
                howManyWidthsFromCamera -= 0.5f;
            }

            int howManyWidthsToFix = (int) howManyWidthsFromCamera;

            position.x -= howManyWidthsToFix * mapWidth;
        }
        
        if (NewHexTileMap.allowWrapNorthSouth)
        {

            float howManyHeightsFromCamera = (position.z - cameraPosition.z) / mapHeight;

            // We want howManyWidthsFromCamera to be between -0.5 to 0.5
            if (Mathf.Abs(howManyHeightsFromCamera) <= 0.5f)
            {
                return position;
            }

            if (howManyHeightsFromCamera > 0)
            {
                howManyHeightsFromCamera += 0.5f;
            }
            else
            {
                howManyHeightsFromCamera -= 0.5f;
            }

            int howManyHeightsToFix = (int) howManyHeightsFromCamera;

            position.z -= howManyHeightsToFix * mapWidth;
        }

        return position;
    }

    public static float Distance(Hex a, Hex b)
    {
        // WARNING: PROBABLY WRONG for wrapping
        int distanceQ = Mathf.Abs(a.Q - b.Q);
        
        if (a.NewHexTileMap.allowWrapEastWest)
        {
            if (distanceQ > a.NewHexTileMap.numColumns / 2)
                distanceQ = a.NewHexTileMap.numColumns - distanceQ;
        }

        if (a.NewHexTileMap.allowWrapNorthSouth)
        {
            int distanceR = Mathf.Abs(a.R - b.R);
            if (distanceR > a.NewHexTileMap.numRows / 2)
                distanceR = a.NewHexTileMap.numRows - distanceR;   
        }
        
        return
            Mathf.Max(
                Mathf.Abs(a.Q - b.Q),
                Mathf.Abs(a.R - b.R),
                Mathf.Abs(a.S - b.S)
            );
    }

    public void AddUnit( Unit unit)
    {
        if (units == null)
        {
            units = new HashSet<Unit>();
        }

        units.Add(unit);
    }

    public void RemoveUnit (Unit unit)
    {
        if (units != null)
        {
            units.Remove(unit);
        }       
    }

    public Unit[] Units()
    {
        return units.ToArray();
    }

    public int BaseMovementCost()
    {
        //TODO: Factor in terrain type & features
        return 1;
    }
}
