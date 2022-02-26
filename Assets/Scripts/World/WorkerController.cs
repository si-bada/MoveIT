using System.Collections;
using UnityEngine;
using UnityEngine.AI;


// Main class for the sample. Controls the behavior of the worker, mainly the construction of the NavMeshAgent path checking for the good destination position
[RequireComponent(typeof(NavMeshAgent))]
public class WorkerController : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] NavMeshAgent m_agent;
    [Header("Control")]
    [SerializeField] bool m_previewPath = false;
    [SerializeField] PostController[] m_targets;
    [SerializeField] GameObject stock;

    [Header("Status")]
    [SerializeField] bool m_isCarryingItems = false;
    [SerializeField] Transform m_target = default;
    [SerializeField] NavMeshPath m_path;
    [SerializeField] int m_currentTarget = 0;
    [SerializeField] Vector3 m_currentTargetBestPosition;

    public delegate void OnPathChangedDelegate();
    public OnPathChangedDelegate OnPathChanged;

    public Transform Target
    {
        get => m_target;
        set
        {
            m_target = value;
            RecomputePath();
        }
    }

    private void OnValidate()
    {
        if (!m_agent) m_agent = GetComponent<NavMeshAgent>();
    }

    public NavMeshPath GetPath()
    {
        return m_path;
    }

    public void DoStart()
    {
        m_currentTarget = 0;
        Target = m_targets[0].depot;
        GoToTarget();
        StopCoroutine(DoUpdate());
        StartCoroutine(DoUpdate());
    }
    IEnumerator DoUpdate()
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, m_currentTargetBestPosition) < 0.5f)
            {
                Debug.LogWarning("waiting");
                yield return new WaitForSeconds(1.0f);

                if (m_isCarryingItems)
                {
                    Debug.LogWarning("deposit stock");
                    depositStock();
                }
                else
                {
                    Debug.LogWarning("go carry stock");
                    carryStock();
                }
                m_isCarryingItems = !m_isCarryingItems;
                switchTarget();
                RecomputePath();
                GoToTarget();
            }
            else if (m_previewPath && transform.hasChanged)
            {
                RecomputePath();
            }
            yield return null;
        }
    }

    void carryStock()
    {
        if(m_targets[m_currentTarget].Pick(1))
        {
            stock.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Stock Empty !");
        }
    }
    void depositStock()
    {
        stock.SetActive(false);
        m_targets[m_currentTarget].Drop(1);
    }

    void switchTarget()
    {
        m_currentTarget = (m_currentTarget + 1) % m_targets.Length;
        Target = m_targets[m_currentTarget].depot;
    }

    public void RecomputePath()
    {
        if (m_path == null)
        {
            m_path = new NavMeshPath();
        }
        m_currentTargetBestPosition = Target.position;
        if (RectifyTargetPosition(ref m_currentTargetBestPosition))
        {
            NavMesh.CalculatePath(transform.position, m_currentTargetBestPosition, NavMesh.AllAreas, m_path);
            // informs to any listeners that the path has changed
            OnPathChanged?.Invoke();
        }
        else
        {
            m_path = null;
            OnPathChanged?.Invoke();
        }
    }
    public void GoToTarget()
    {
        if (m_path == null) RecomputePath();
        m_agent.path = m_path;
        m_agent.isStopped = false;
    }
    public void ResetPath()
    {
        StopCoroutine(DoUpdate());
        m_currentTarget = 0;
        m_path = null;
        m_agent.velocity = Vector3.zero;
        m_agent.isStopped = true;
        OnPathChanged?.Invoke();
    }

    // gets the closest valid position
    // performs an inside-out radial search, picking the closest position to the player at the minimum radial distance to the target
    public bool RectifyTargetPosition(ref Vector3 desiredPosition)
    {
        {
            bool valid = (NavMesh.SamplePosition(desiredPosition, out _, m_agent.radius, GlobalOptions.main.StoppingAreaMask));
            if (valid)
            {
                return true;
            }
        }
        float dDelta = GlobalOptions.main.AISearchDiameterMax / GlobalOptions.main.AISearchdSteps;
        float aDelta = 360f / GlobalOptions.main.AIAngleSteps * Mathf.Deg2Rad;
        bool found = false;
        // not used, but could be kept for smarter decision making
        //float currentDistanceToWorker = float.MaxValue;
        //float currentDistanceToTarget = float.MaxValue;
        float score = float.MaxValue;
        Vector3 foundPosition = Vector3.zero;
        for (float r = GlobalOptions.main.AISearchDiameterMin; r <= GlobalOptions.main.AISearchDiameterMax; r += dDelta)
        {
            for (float a = 0; a < 360 * Mathf.Deg2Rad; a += aDelta)
            {
                Vector3 deltaPosition = new Vector3(Mathf.Cos(a) * r, 0, Mathf.Sin(a) * r);
                var candidatePosition = desiredPosition + deltaPosition;
                bool valid = (NavMesh.SamplePosition(candidatePosition, out _, m_agent.radius, GlobalOptions.main.StoppingAreaMask));
                if (valid)
                {
                    float candidateDistanceToTarget = deltaPosition.magnitude;
                    float candidateDistanceToWorker = (this.transform.position - candidatePosition).magnitude;

                    float candidateScore = candidateDistanceToWorker* GlobalOptions.main.AIWeightDistanceToWorker + candidateDistanceToTarget * GlobalOptions.main.AIWeightDistanceToTarget;
                    if (candidateScore < score) {
                        // not used, but could be kept for smarter decision making
                        //currentDistanceToWorker = candidateDistanceToWorker;
                        //currentDistanceToTarget = candidateDistanceToTarget;
                        score = candidateScore;
                        found = true;
                        foundPosition = candidatePosition;
                    }
                }
            }
        }
        if (found)
        {
            desiredPosition = foundPosition;
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (m_agent.hasPath)
        {
            NavMeshPath agentPath = m_agent.path;
            Vector3[] pathCorners = agentPath.corners;
            for (int checkpoint = 0; checkpoint < pathCorners.Length - 1; ++checkpoint)
            {
                Gizmos.DrawLine(pathCorners[checkpoint], pathCorners[checkpoint + 1]);
            }
        }

        if (m_path != null)
        {
            Vector3[] pathCorners = m_path.corners;
            for (int checkpoint = 0; checkpoint < pathCorners.Length - 1; ++checkpoint)
            {
                Gizmos.DrawLine(pathCorners[checkpoint], pathCorners[checkpoint + 1]);
            }
        }
        if (Application.isPlaying)
        {
            Vector3 desiredPosition = Target.position;
            float diameter = GlobalOptions.main.AISearchDiameterMax; int dSteps = GlobalOptions.main.AISearchdSteps; int angleSteps = GlobalOptions.main.AIAngleSteps;

            {
                bool valid = (NavMesh.SamplePosition(desiredPosition, out _, m_agent.radius, GlobalOptions.main.StoppingAreaMask));
                if (valid)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(desiredPosition, 0.5f);
                }
            }
            float dDelta = diameter / dSteps;
            float aDelta = 360f / angleSteps * Mathf.Deg2Rad;
            for (float r = GlobalOptions.main.AISearchDiameterMin; r <= diameter; r += dDelta)
            {
                for (float a = 0; a < 360 * Mathf.Deg2Rad; a += aDelta)
                {
                    var candidatePosition = desiredPosition + new Vector3(Mathf.Cos(a) * r, 0, Mathf.Sin(a) * r);
                    bool valid = (NavMesh.SamplePosition(candidatePosition, out _, m_agent.radius, GlobalOptions.main.StoppingAreaMask));
                    if (valid)
                    {
                        Gizmos.color = Color.green;
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                    }
                    Gizmos.DrawWireSphere(candidatePosition, 0.15f);
                }
            }
            
        }
    }
}
