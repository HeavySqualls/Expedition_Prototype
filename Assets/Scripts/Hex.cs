using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using System.Linq;
using QPath;

/// <summary>
/// The Hex class defines the grid position, world space position, size,
/// neighbours, etc... of a Hex tile. However, it does NOT interact with
/// Unity directly in any way. 
/// </summary>

public class Hex : IQPathTile
{
    public Hex(HexMap newHexTileMap, int q, int r)
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

    public enum TERRAIN_TYPE { PLAINS, GRASSLANDS, MARSH, FLOODPLAINS, DESERT, LAKE, OCEAN}
    public enum ELEVATION_TYPE { FLAT, HILL, MOUNTAIN, WATER}

    public TERRAIN_TYPE TerrainType { get; set; }
    public ELEVATION_TYPE ElevationType { get; set; }

    public enum FEATURE_TYPE { NONE, FOREST, RAINFOREST, MARSH}
    public FEATURE_TYPE FeatureType { get; set; }

    //TODO: Need some kind of property to track hex type (plains, grasslands, etc.....)
    //TODO: Need property to track hex details (forest, mine, farm...)

    public readonly HexMap NewHexTileMap;
    
    static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(2.992f) / 2; // <--------------------- HEX HORIZONTAL SPACING VARIABLE!!

    float radius = 2f;

    HashSet<Unit> units;

    public override string ToString()
    {
        return Q + ", " + R;
    }

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

    public static float CostEstimate(IQPathTile aa, IQPathTile bb)
    {
        return Distance((Hex)aa, (Hex)bb);
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

    /// <summary>
    /// Returns the most common movement cost for this tile for a basic unit.
    /// (booleans are used to check if the unit entering the hex has one of these traits that will override the base tile cost)
    /// </summary>
    /// <returns> The Movement Cost </returns>
    public float BaseMovementCost(bool isHillWalker, bool isForestWalker, bool isFlyer)
    {
        if ((ElevationType == ELEVATION_TYPE.MOUNTAIN || ElevationType == ELEVATION_TYPE.WATER) && isFlyer == false)
            return -99;

        float moveCost = 1.01f;

        if (ElevationType == ELEVATION_TYPE.HILL && isHillWalker == false)
            moveCost += 1;

        if ((FeatureType == FEATURE_TYPE.FOREST || FeatureType == FEATURE_TYPE.RAINFOREST) && isForestWalker == false)
            moveCost += 2;

        return moveCost;
    }

    Hex[] neighbours;

    #region IQPathTile implementation
    public IQPathTile[] GetNeighbours()
    {
        if (this.neighbours != null)
            return this.neighbours;        

        List<Hex> neighbours = new List<Hex>();

        neighbours.Add( NewHexTileMap.GetHexAt(Q + 1, R + 0));
        neighbours.Add(NewHexTileMap.GetHexAt(Q + -1, R + 0));
        neighbours.Add(NewHexTileMap.GetHexAt(Q + 0, R + +1));
        neighbours.Add(NewHexTileMap.GetHexAt(Q + 0, R + -1));
        neighbours.Add(NewHexTileMap.GetHexAt(Q + 1, R + -1));
        neighbours.Add(NewHexTileMap.GetHexAt(Q + -1, R + +1));

        List<Hex> neighbours2 = new List<Hex>();

        foreach (Hex h in neighbours)
        {
            if (h != null)
            {
                neighbours2.Add(h);
            }
        }

        this.neighbours = neighbours2.ToArray();

        return this.neighbours;

    }

    public float AggregateCostToEnter(float costSoFar, IQPathTile sourceTile, IQPathUnit theUnit)
    {
        //TODO: We are ignoring source tile right now, this will have to change when we have rivers...
        return ((Unit)theUnit).AggregateTurnsToEnterHex(this, costSoFar);
    }
    #endregion
}
