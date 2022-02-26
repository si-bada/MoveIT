using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Simple button to set the current activeWorker target destination as the one of the root of this object
public class SelectTarget : InteractableElement
{
    public override void OnInteractableMouseDown(PlayerController controller)
    {
        if (controller.activeWorker)
        {
            controller.activeWorker.Target =this.transform;
        }
    }

    public override void OnInteractableMouse(PlayerController controller)
    {
        // do nothing
    }

    public override void OnInteractableMouseUp(PlayerController controller)
    {
        // do nothing
    }
}
