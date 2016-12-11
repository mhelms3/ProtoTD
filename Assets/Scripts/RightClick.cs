using UnityEngine;
using System.Collections;

public class RightClick : MonoBehaviour {

    void Update()
        {
            if (Input.GetMouseButtonDown(1))
                OnRightClick();
        }

        // Check for Right-Click
        void OnRightClick()
        {
            // Cast a ray from the mouse
            // cursors position
            Debug.Log("Pew");
            Ray clickPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitPoint;

            // See if the ray collided with an object
            if (Physics.Raycast(clickPoint, out hitPoint))
            {
            // Make sure this object was the
            // one that received the right-click
            
            if (hitPoint.collider == GetComponent<Collider>())
               
                {
                    // Put code for the right click event
                    Debug.Log("Right Clicked on " + this.name);
                }
            }
        }
}
