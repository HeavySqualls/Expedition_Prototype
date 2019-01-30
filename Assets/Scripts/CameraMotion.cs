using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour
{
   
    void Start()
    {
        oldPosition = this.transform.position;
    }

    private Vector3 oldPosition;
   
    void Update()
    {
        //TODO: Code to click and drag camera
        // WASD
        // Zoom in and out


        CheckIfCameraMoved();
    }

    public void PanToHex(Hex hex)
    {
        // TODO: Move camera to hex
    }

    void CheckIfCameraMoved()
    {
        if (oldPosition != this.transform.position)
        {
            // SOMETHING moved the camera.
            oldPosition = this.transform.position;

            // TODO: probably hexMap will have a dictionary of all these later
            HexComponent[] hexes = GameObject.FindObjectsOfType<HexComponent>();

            //TODO: Maybe theres a better way to call what hexes get updated?
            
            foreach (HexComponent hex in hexes)
            {
                hex.UpdatePosition();     
            }
        }
    }
    
}
