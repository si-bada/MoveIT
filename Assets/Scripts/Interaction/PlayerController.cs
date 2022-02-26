using UnityEngine;

// Mouse control
// Passes the mouse activity when interacting with InteractableElements
public class PlayerController : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] Camera m_camera;

    [Header("Selections")]
    [SerializeField] WorkerController m_activeWorker;
    [Header("Status")]
    [SerializeField] InteractableElement m_currentInteractable = null;
    [SerializeField] private Ray m_mouseRay;

    public Ray mouseRay { get => m_mouseRay; private set => m_mouseRay = value; }
    public WorkerController activeWorker { get => m_activeWorker; private set => m_activeWorker = value; }

    private void Start()
    {
    }

    void OnValidate()
    {
        if (!m_camera) m_camera = Camera.main;
    }

    void Update()
    {

        mouseRay = m_camera.ScreenPointToRay(Input.mousePosition);
        // pick, drag, and drop movable object
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInteractable;
            if (Physics.Raycast(mouseRay, out hitInteractable, float.MaxValue, GlobalOptions.main.InteractableLayer))
            {
                if (hitInteractable.collider.TryGetComponent<InteractableElement>(out m_currentInteractable))
                {
                    m_currentInteractable.OnInteractableMouseDown(this);

                    // this is not very elegant, but the alternative implies a larger codebase
                    WorkerController newWorker;
                    if(m_currentInteractable.root.TryGetComponent<WorkerController>(out newWorker))
                    {
                        activeWorker = newWorker;
                    }
                }
                else
                {
                    Debug.LogWarning("The object " + hitInteractable.collider.name + " does not have an Interactable script.");
                }
            }
        }
        else if (Input.GetMouseButton(0) && m_currentInteractable)
        {
            m_currentInteractable.OnInteractableMouse(this);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (m_currentInteractable)
            {
                m_currentInteractable.OnInteractableMouseUp(this);
                m_currentInteractable = null;
            }
            
        }
    }
}
