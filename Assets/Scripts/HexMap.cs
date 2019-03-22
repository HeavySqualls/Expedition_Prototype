using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QPath;
using System;

public class HexMap : MonoBehaviour, IQPathWorld
{
    
    void Start()
    {
        GenerateMap();
    }

    void Update()
    {
        // TESTING: Hit spacebar to advance to next turn
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (units != null)
            {
                foreach (Unit u in units)
                {
                    u.DoMove();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (units != null)
            {
                foreach (Unit u in units)
                {
                    u.DUMMY_PATHING_FUNCTION();
                }
            }
        }
    }
    
//-------- VARIABLES ---------//     
    
    public GameObject HexPrefab;

    // Create meshes to hold more detailed tile information 
    public Mesh MeshWater;
    public Mesh MeshFlat;
    public Mesh MeshHill;
    public Mesh MeshMountain;

    public GameObject ForestPrefab;
    public GameObject JunglePrefab;

    // Create specific meshes for more detailed map generation
    public Material MatWater;
    public Material MatDesert;
    public Material MatGrassland;
    public Material MatMountain;
    public Material MatPlains;

    // Units 
    public GameObject PlayerPrefab;

    // Determining factors in moisture distribution 
    [System.NonSerialized] public float MoistureJungle = 0.66f;
    [System.NonSerialized] public float MoistureForest = 0.2f;
    [System.NonSerialized] public float MoistureGrasslands = 0f;
    [System.NonSerialized] public float MoisturePlains = -0.5f;

    // Tiles with height above whatever is whatever
    [System.NonSerialized] public float HeightMountain = 0.85f;
    [System.NonSerialized] public float HeightHill = 0.6f;
    [System.NonSerialized] public float HeightFlat = 0.0f;

    [System.NonSerialized] public int numRows = 30;
    [System.NonSerialized] public int numColumns = 60;

    // Wrapping options
    [System.NonSerialized] public bool allowWrapEastWest = true;
    [System.NonSerialized] public bool allowWrapNorthSouth = false;

    // Hex Dictionaries
    private Hex[,] hexes;
    private Dictionary<Hex, GameObject> hexToGOMap;
    private Dictionary<GameObject, Hex> gOToHexMap;

    // Unit Dictionaries
    private HashSet<Unit> units;
    private Dictionary<Unit, GameObject> unitToGameObjectMap;


    public Hex GetHexAt(int x, int y)
    {
        if (hexes == null)
        {
            Debug.LogError("Hexes array not yet instantiated!");
            return null;
        }

        if (allowWrapEastWest)
        {
            x = x % numColumns;
            if (x < 0)
            {
                x += numColumns;
            }
        }

        if (allowWrapNorthSouth)
        {
            y = y % numRows;
            if (y < 0)
            {
                y += numRows;
            }
        }
        
        return hexes[x, y];
    }

    public Hex GetHexFromGO(GameObject hexGO)
    {
        if (gOToHexMap.ContainsKey(hexGO))
        {
            return gOToHexMap[hexGO];
        }

        return null; 
    }

    public GameObject GetHexGO(Hex h)
    {
        if (hexToGOMap.ContainsKey(h))
        {
            return hexToGOMap[h];
        }

        return null;
    }

    public Vector3 GetHexPosition(int q, int r)
    {
        Hex hex = GetHexAt(q, r);

        return GetHexPosition(hex);
    }

    public Vector3 GetHexPosition(Hex hex)
    {
        return hex.PositionFromCamera(Camera.main.transform.position, numRows, numColumns);
    }

    public virtual void GenerateMap()
    {
        hexes = new Hex[numColumns, numRows];
        hexToGOMap = new Dictionary<Hex, GameObject>();
        gOToHexMap = new Dictionary<GameObject, Hex>();

        // Generate a map filled with ocean
        for (int column = 0; column < numColumns; column++)
        { 
            for (int row = 0; row < numRows; row++)
            {
                // Instantiate a Hex
                Hex h = new Hex( this, column, row);
                h.Elevation = -0.5f; // <------------------ BASE OCEAN LEVEL!

                hexes[column, row] = h;  

                Vector3 pos = h.PositionFromCamera(Camera.main.transform.position, numRows, numColumns);
                
                GameObject hexGO = (GameObject)Instantiate(HexPrefab, pos, Quaternion.identity, this.transform);

                hexToGOMap[h] = hexGO;
                gOToHexMap[hexGO] = h;

                h.TerrainType = Hex.TERRAIN_TYPE.OCEAN;
                h.ElevationType = Hex.ELEVATION_TYPE.WATER;

                hexGO.name = string.Format("HEX: {0},{1}", column, row);
                hexGO.GetComponent<HexComponent>().Hex = h;
                hexGO.GetComponent<HexComponent>().NEWHexTileMap = this;

                     
            }
        }
        
        UpdateHexVisuals();
        
        // StaticBatchingUtility.Combine(this.gameObject); FOR RENDERING ALL TILES AS STATIC FOR EFFICIENCY
    }


    public void UpdateHexVisuals()
    {
        for (int column = 0; column < numColumns; column++)
        {
            for (int row = 0; row < numRows; row++)
            {
                Hex h = hexes[column, row];
                GameObject hexGO = hexToGOMap[h];
                
                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                MeshFilter mf = hexGO.GetComponentInChildren<MeshFilter>();

                // MOISTURE
                if (h.Elevation >= HeightFlat && h.Elevation < HeightMountain)
                {
                    if (h.Moisture >= MoistureJungle)
                    {
                        mr.material = MatGrassland;
                        h.TerrainType = Hex.TERRAIN_TYPE.GRASSLANDS;
                        h.FeatureType = Hex.FEATURE_TYPE.RAINFOREST;

                        Vector3 p = hexGO.transform.position;
                        if (h.Elevation >= HeightHill)
                        {
                            p.y += 0.3f;
                        }
                       
                        GameObject.Instantiate(JunglePrefab, p, Quaternion.identity, hexGO.transform);
                    }
                    else if (h.Moisture >= MoistureForest)
                    {
                        mr.material = MatGrassland;
                        h.TerrainType = Hex.TERRAIN_TYPE.GRASSLANDS;
                        h.FeatureType = Hex.FEATURE_TYPE.FOREST;

                        // Spawn Forest
                        Vector3 p = hexGO.transform.position;
                        if (h.Elevation >= HeightHill)
                        {
                            p.y += 0.3f;
                        }
                        
                        GameObject.Instantiate(ForestPrefab, p, Quaternion.identity, hexGO.transform);
                    }
                    else if (h.Moisture >= MoistureGrasslands)
                    {
                        mr.material = MatGrassland;
                        h.TerrainType = Hex.TERRAIN_TYPE.GRASSLANDS;
                    }
                    else if (h.Moisture >= MoisturePlains)
                    {
                        mr.material = MatPlains;
                        h.TerrainType = Hex.TERRAIN_TYPE.PLAINS;
                    }
                    else
                    {
                        mr.material = MatDesert;
                        h.TerrainType = Hex.TERRAIN_TYPE.DESERT;
                    }
                }

                // ELEVATION 
                if (h.Elevation >= HeightMountain)
                {
                    mr.material = MatMountain;
                    mf.mesh = MeshMountain;
                    h.ElevationType = Hex.ELEVATION_TYPE.MOUNTAIN;
                }
                else if (h.Elevation >= HeightHill)
                {
                    mf.mesh = MeshHill;
                    h.ElevationType = Hex.ELEVATION_TYPE.HILL;
                }
                else if (h.Elevation >= HeightFlat)
                {
                    mf.mesh = MeshFlat;
                    h.ElevationType = Hex.ELEVATION_TYPE.FLAT;
                }
                else
                {
                    mr.material = MatWater;
                    mf.mesh = MeshWater;
                    h.ElevationType = Hex.ELEVATION_TYPE.WATER;
                }
                hexGO.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}\n{2}", column, row, h.BaseMovementCost(false, false, false));

            }
        }
    }

    public Hex[] GetHexesWithinRangeOf(Hex centerHex, int range)
    {
        List<Hex> results = new List<Hex>();
        
        for (int dx = -range;  dx < range-1; dx++)
        {
            for (int dy = Mathf.Max(-range+1, -dx-range); dy < Mathf.Min(range, -dx+range-1); dy++)
            {
                results.Add (  GetHexAt(centerHex.Q + dx, centerHex.R +dy) );
            }
        }

        return results.ToArray();
    }

    public void SpawnUnitAt (Unit unit, GameObject prefab, int q, int r)
    {
        if (units == null)
        {
            units = new HashSet<Unit>();
            unitToGameObjectMap = new Dictionary<Unit, GameObject>();
        }

        Hex myHex = GetHexAt(q, r);
        GameObject myHexGO = hexToGOMap[myHex];
        unit.SetHex(myHex);

        GameObject unitGO = (GameObject)Instantiate(prefab, myHexGO.transform.position, Quaternion.identity, myHexGO.transform);
        unit.OnUnitMoved += unitGO.GetComponent<UnitView>().OnUnitMoved;

        units.Add(unit);
        unitToGameObjectMap.Add(unit, unitGO);
    }
}
