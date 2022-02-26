using UnityEngine;


// Simple draggable interaction, while the mouse button is pressed, updates the root position of this object with the one obtained by raycasting against the floor
public class MovableElement : InteractableElement
{
    public override void OnInteractableMouseDown(PlayerController controller)
    {
        // do nothing
    }

    public override void OnInteractableMouse(PlayerController controller)
    {
        RaycastHit hitFloor;
        if (Physics.Raycast(controller.mouseRay, out hitFloor, float.MaxValue, GlobalOptions.main.FloorLayer))
        {
            if(hitFloor.transform.name.Equals("Room floor"))
            {
                m_root.transform.position = hitFloor.point;
            }
            Debug.LogWarning(hitFloor.transform.name);
            //@TODO: implement rotation
            if(Input.GetKey(GlobalOptions.main.RotateLeft))
            {
                m_root.transform.Rotate(0, Input.GetAxis("Horizontal") * GlobalOptions.main.RotationSpeed * Time.deltaTime, 0);
            }
            if(Input.GetKey(GlobalOptions.main.RotateRight))
            {
                m_root.transform.Rotate(0, Input.GetAxis("Horizontal") * GlobalOptions.main.RotationSpeed * Time.deltaTime, 0);
            }
        }
    }

    public override void OnInteractableMouseUp(PlayerController controller)
    {
        // do nothing
    }
}
