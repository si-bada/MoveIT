using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Abstract class for mouse interactions
public abstract class InteractableElement : MonoBehaviour
{
    [Header("InteractableElement")]
    [SerializeField] protected WorldElement m_root;

    public WorldElement root { get => m_root; private set => m_root = value; }

    private void OnValidate()
    {
        if (!m_root) m_root = GetComponentInParent<WorldElement>();
        //if (!m_root) { Debug.LogWarning(this.name + " needs a parent WorldElement."); }
    }

    public abstract void OnInteractableMouseDown(PlayerController controller);
    public abstract void OnInteractableMouse(PlayerController controller);
    public abstract void OnInteractableMouseUp(PlayerController controller);
}
