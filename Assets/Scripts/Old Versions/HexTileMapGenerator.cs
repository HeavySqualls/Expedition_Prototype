using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TUTORIAL INFORMATION: Aeonic Softworks
// Pt. #1 - https://www.youtube.com/watch?v=rjBD-4gNcfA
// Pt. #2 - https://www.youtube.com/watch?v=BE54igXh5-Q

public class HexTileMapGenerator : MonoBehaviour
{
    public GameObject hexTilePrefab;
    
    private int mapWidth = 25;
    private int mapHeight = 12;

    float tileXOffset = 3.55f;
    float tileZOffset = 3.05f;
    
    void Start()
    {
        CreateHexTileMap();
    }

    // Generates the tile map
    void CreateHexTileMap()
    {
        for (int x = 0; x <= mapWidth; x++)
        {
            for (int z = 0; z <= mapHeight; z++)
            {
                GameObject TempGO = Instantiate(hexTilePrefab);

                if (z % 2 == 0)
                {
                    TempGO.transform.position = new Vector3(x * tileXOffset, 0, z * tileZOffset); 
                }
                else
                {
                    TempGO.transform.position = new Vector3(x * tileXOffset + tileXOffset / 2, 0, z *tileZOffset);
                }
                SetTileInfo(TempGO, x, z);
            }
        }
    }

    // Organize all tiles to become parent of Tile Generator
    void SetTileInfo(GameObject GO, int x, int z)
    {
        GO.transform.parent = transform;
        GO.name = x.ToString() + "," + z.ToString();
    }
}
