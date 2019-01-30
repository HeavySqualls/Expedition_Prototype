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

    // Create meshes to hold more detailed tile information 
    public Mesh MeshWater;
    public Mesh MeshFlat;
    public Mesh MeshHill;
    public Mesh MeshMountain;
    

    // Create an array of different terrain materials 
    //public Material[] HexMaterials;

    // Create specific meshes for more detailed map generation
    public Material MatWater;
    public Material MatDesert;
    public Material MatGrassland;
    public Material MatMountain;
    public Material MatPlains;

    public int numRows = 20;
    public int numColumns = 40; 

    
    public virtual void GenerateMap()
    {
        for (int column = 0; column < numColumns; column++)
        {
            
            // Generate a map filled with ocean
            
            for (int row = 0; row < numRows; row++)
            {
                // Instantiate a Hex
                Hex h = new Hex(column, row);

                Vector3 pos = h.PositionFromCamera(Camera.main.transform.position, numRows, numColumns);
                
                GameObject hexGO = (GameObject)Instantiate(HexPrefab, pos, Quaternion.identity, this.transform);

                //hexGO.name = string.Format("HEX: {0},{1}", column, row);
                hexGO.GetComponent<HexComponent>().Hex = h;
                hexGO.GetComponent<HexComponent>().NEWHexTileMap = this;

                hexGO.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", column, row);

                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                mr.material = MatWater;
                
                MeshFilter mf = hexGO.GetComponentInChildren<MeshFilter>();
                mf.mesh = MeshWater;
            }
        }
        
        // StaticBatchingUtility.Combine(this.gameObject); FOR RENDERING ALL TILES AS STATIC FOR EFFICIENCY
    }
}
