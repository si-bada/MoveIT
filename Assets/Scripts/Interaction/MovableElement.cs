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

            m_root.transform.position = hitFloor.point;

            //@TODO: implement rotation
        }
    }

    public override void OnInteractableMouseUp(PlayerController controller)
    {
        // do nothing
    }
}
