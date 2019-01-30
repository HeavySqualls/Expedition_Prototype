using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEWHexTileMap : MonoBehaviour
{
    
    void Start()
    {
        GenerateMap();
    }
    
    public GameObject HexPrefab;

    // Create an array of different terrain materials 
    public Material[] HexMaterials;

    public int numRows = 20;
    public int numColumns = 40; 

    
    void GenerateMap()
    {
        for (int column = 0; column < numColumns; column++)
        {
            for (int row = 0; row < numRows; row++)
            {
                // Instantiate a Hex
                Hex h = new Hex(column, row);

                Vector3 pos = h.PositionFromCamera(Camera.main.transform.position, numRows, numColumns);
                
                GameObject hexGO = (GameObject)Instantiate(HexPrefab, pos, Quaternion.identity, this.transform);

                //hexGO.name = string.Format("HEX: {0},{1}", column, row);
                hexGO.GetComponent<HexComponent>().Hex = h;
                hexGO.GetComponent<HexComponent>().NEWHexTileMap = this;

                // Creates terrain based off of a random selection of materials from the hex materials array
                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                mr.material = HexMaterials[Random.Range(0, HexMaterials.Length)];
            }
        }
        
        // StaticBatchingUtility.Combine(this.gameObject); FOR RENDERING ALL TILES AS STATIC FOR EFFICIENCY
    }
}
