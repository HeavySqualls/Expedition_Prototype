using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouseController : MonoBehaviour
{
    bool isDraggingCamera = false;
    Vector3 lastMousePosition;

    void Update()
    {

            // ******* CAMERA CONTROLS ******* //


        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition); //<--- Sends out the ray on the Y axis and returns with a value unless Y is 0 

        if (mouseRay.direction.y >= 0)
        {
            //Debug.LogError("why is mouse pointing up?");
            return;
        }
        float rayLength = (mouseRay.origin.y / mouseRay.direction.y);
        Vector3 hitPos = mouseRay.origin - (mouseRay.direction * rayLength);

        if (Input.GetMouseButtonDown(0))
        {
            // Mouse button just went down, start drag
            isDraggingCamera = true;
            
            lastMousePosition = hitPos; // <---- updates hitPos 
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Mouse button went up, stop drag
            isDraggingCamera = false;
        }

        if (isDraggingCamera)
        {
            Vector3 diff = lastMousePosition - hitPos;
            Camera.main.transform.Translate(diff, Space.World);

            mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (mouseRay.direction.y >= 0)
            {
               // Debug.LogError("why is mouse pointing up?");
                return;
            }
            rayLength = (mouseRay.origin.y / mouseRay.direction.y);
            lastMousePosition = mouseRay.origin - (mouseRay.direction * rayLength);
        }

                // ***** ZOOM WITH SCROLLWHEEL ***** //

        float scrollAmmount = Input.GetAxis("Mouse ScrollWheel");
        float minHeight = 2f; // <------------------------------------------------------------ADJUSTMENT FOR CLOSE ZOOM
        float maxHeight = 30f; // <-----------------------------------------------------------ADJUSTMENT FOR FAR ZOOM

        if (Mathf.Abs(scrollAmmount) > 0.01f)
        {
            // zoom camera towards location of the mouse pointer (hitPos)
            Vector3 dir = hitPos - Camera.main.transform.position;

            Vector3 p = Camera.main.transform.position;

           
            // Stop zooming out at a certain distance
            if (scrollAmmount > 0 || p.y < (maxHeight-0.1f))
            {
                Camera.main.transform.Translate(dir * scrollAmmount, Space.World);
            }

            // Minimum zoom-in distance
            p = Camera.main.transform.position;
            if (p.y < minHeight)
            {
                p.y = minHeight;
            }

            // Maximum zoom-in distance
            if (p.y > maxHeight)
            {
                p.y = maxHeight;
            }

            Camera.main.transform.position = p;


                    // ***** ADJUST ANGLE WITH ZOOM ***** //

            float lowZoom = minHeight +5f;
            float highZoom = maxHeight -5f;

            //if (p.y < lowZoom)                                 // for a controlled angle adjustment while zooming
            //{
            //    Camera.main.transform.rotation = Quaternion.Euler(
            //        Mathf.Lerp(10, 55, ((p.y-minHeight) /(lowZoom-minHeight))),
            //        Camera.main.transform.rotation.eulerAngles.y,
            //        Camera.main.transform.rotation.eulerAngles.z
            //        );
            //}
            //else if (p.y > highZoom)
            //{
            //    Camera.main.transform.rotation = Quaternion.Euler(
            //        Mathf.Lerp(55, 90, ((p.y-highZoom) / (maxHeight - highZoom))),
            //        Camera.main.transform.rotation.eulerAngles.y,
            //        Camera.main.transform.rotation.eulerAngles.z
            //        );
            //}
            //else
            //{
            //    Camera.main.transform.rotation = Quaternion.Euler(
            //       55,
            //       Camera.main.transform.rotation.eulerAngles.y,
            //       Camera.main.transform.rotation.eulerAngles.z
            //       );
            //}
        }

        Camera.main.transform.rotation = Quaternion.Euler( // for a constant & smooth angle adjustment while zooming
             Mathf.Lerp(25, 80, Camera.main.transform.position.y / (maxHeight / 1f)), // <--------------------------------------------------------- Adjustments for min/max zoom angle
             Camera.main.transform.rotation.eulerAngles.y,
             Camera.main.transform.rotation.eulerAngles.z
             );
    }
}
