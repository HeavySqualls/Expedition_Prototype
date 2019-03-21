using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    // Generic bookkeeping variables 
    Vector3 LastMousePosition; // from Input.mousePosition

    // Camera Dragging bookkeeping variables
    int mouseDragThreshhold = 1; // Threshold of mouse movement to start dragging
    Vector3 lastMouseGroundPlanePosition;
    Vector3 cameraTargetOffset;

    // Unit movement
    Unit selectedUnit = null;

    delegate void UpdateFunc();
    UpdateFunc Update_CurrentFunc;

    public LayerMask LayerID_HexTiles;

    void Start()
    {
        Update_CurrentFunc = Update_DetectModeStart;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelUpdateFunc();
        }

        Update_CurrentFunc();

        Update_ScrollZoom();

        LastMousePosition = Input.mousePosition;
    }


    void CancelUpdateFunc()
    {
        Update_CurrentFunc = Update_DetectModeStart;

        // Also do cleanup with any UI stuff associated with modes. 
    }

    // ****** DEFAULT MOUSE MODE ******* //

    void Update_DetectModeStart()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Button DOWN");
            // Left mouse button just went down.
            // This doesnt do anything by itself actually. 
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Mouse button UP.... CLICK!");
            // TOD: Are we clicking on a hex with a unit? if so, select it. 
            MouseToHex();
        }
        else if (Input.GetMouseButton(0) && Vector3.Distance(Input.mousePosition, LastMousePosition) > mouseDragThreshhold)
        {
            // Left mouse button is being held down AND the mouse moved? == camera drag!
            Update_CurrentFunc = Update_CameraDrag;
            lastMouseGroundPlanePosition = MouseToGroundPlane(Input.mousePosition);
            Update_CurrentFunc();
        }
        else if (selectedUnit != null && Input.GetMouseButton(1))
        {
            // we have a selected unit and we are holding down the mouse button. 
            // We are in unit movement mode, show a path from unit to mouse position via the pathfinding system. 
        }
    }

    // Take the mouse position, turn it into a ray, then use physics.raycast to return hit info
    Hex MouseToHex()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        int layerMask = LayerID_HexTiles.value;

        if (Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity, layerMask))
        {
            // Something got hit 
            Debug.Log(hitInfo.collider.name);
            return null;
        }
        Debug.Log("Found nothing");
        return null;
    }

    Vector3 MouseToGroundPlane(Vector3 mousePos)
    {
        Ray mouseRay = Camera.main.ScreenPointToRay (mousePos);
        // What is the point at which the mouse ray intersects y+0
        if (mouseRay.direction.y >= 0)
        {
            return Vector3.zero;
        }
        float rayLength = (mouseRay.origin.y / mouseRay.direction.y);
        return mouseRay.origin - (mouseRay.direction * rayLength);
    }


    void Update_UnitMovement()
    {
        if (Input.GetMouseButtonUp(1))
        {
            Debug.Log("Complete unit movement.");

            //TODO: copy pathfinding path to units movement queue

            CancelUpdateFunc();
            return;
        }
    } 


    void Update_CameraDrag()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Cancelling camera drag.");
            CancelUpdateFunc();
            return;
        }

        // Sends out the ray on the Y axis and returns with a value unless Y is 0
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (mouseRay.direction.y >= 0)
        {
            return;
        }
        Vector3 hitPos = MouseToGroundPlane(Input.mousePosition);

        Vector3 diff = lastMouseGroundPlanePosition - hitPos;
        Camera.main.transform.Translate(diff, Space.World);

        lastMouseGroundPlanePosition = hitPos = MouseToGroundPlane(Input.mousePosition);
    }  


    void Update_ScrollZoom()
        {
            float scrollAmmount = Input.GetAxis("Mouse ScrollWheel");
            float minHeight = 2f; // <------------------------------------------------------------ADJUSTMENT FOR CLOSE ZOOM
            float maxHeight = 30f; // <-----------------------------------------------------------ADJUSTMENT FOR FAR ZOOM
            // Move camera towards hitPos
            Vector3 hitPos = MouseToGroundPlane(Input.mousePosition);
            Vector3 dir = hitPos - Camera.main.transform.position;

            Vector3 p = Camera.main.transform.position;
 
            // TODO: Maybe you should still slide around at 20 zoom??

            // Stop zooming out at a certain distance
            if (scrollAmmount > 0 || p.y < (maxHeight - 0.1f))
            {
                cameraTargetOffset += dir * scrollAmmount;
            }
            Vector3 lastCameraPosition = Camera.main.transform.position;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, Camera.main.transform.position + cameraTargetOffset, Time.deltaTime * 5f);
            cameraTargetOffset -= Camera.main.transform.position - lastCameraPosition;

            p = Camera.main.transform.position;
            if (p.y < minHeight)// Minimum zoom-in distance
            {
                p.y = minHeight;
            }
            if (p.y > maxHeight) // Maximum zoom-in distance
            {
                p.y = maxHeight;
            }
            Camera.main.transform.position = p;

        Camera.main.transform.rotation = Quaternion.Euler(
        Mathf.Lerp(25, 80, Camera.main.transform.position.y / maxHeight),
        Camera.main.transform.rotation.eulerAngles.y,
        Camera.main.transform.rotation.eulerAngles.z
        );
    }
}
