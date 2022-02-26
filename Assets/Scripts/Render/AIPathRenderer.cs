using UnityEngine;
using UnityEngine.AI;


// Simple controller for the lineRenderer. 
// Listens for changes in the Worker path to update the line positions
[RequireComponent(typeof(LineRenderer))]
public class AIPathRenderer : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] WorkerController m_worker;
    [SerializeField] LineRenderer m_lineRenderer;

    [Header("Options")]
    [SerializeField] Vector3 offset;

    [Header("Status")]
    [SerializeField] NavMeshPath path;

    private void OnValidate()
    {
        if (!m_lineRenderer) m_lineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        m_worker.OnPathChanged += OnPathChanged;
    }

    private void OnPathChanged()
    {
        UpdatePathGeometry();
    }

    void UpdatePathGeometry()
    {
        path = m_worker.GetPath();
        if (path!=null)
        {
            Vector3[] cornersCopy = path.corners;
            m_lineRenderer.positionCount = cornersCopy.Length;
            for(int c = 0; c < cornersCopy.Length;++c)
            {
                cornersCopy[c] += offset;
            }
            m_lineRenderer.SetPositions(cornersCopy);
        }
        else
        {
            m_lineRenderer.positionCount = 0;
        }
    }
}
