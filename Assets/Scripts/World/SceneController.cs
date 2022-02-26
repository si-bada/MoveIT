using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] WorkerController[] m_workers;
    [SerializeField] Vector3[] m_startPositions;
    [SerializeField] bool m_autoStart;

    private void Start()
    {
        m_workers = GetComponentsInChildren<WorkerController>();
        m_startPositions = new Vector3[m_workers.Length];
        for (int w = 0; w < m_workers.Length; ++w)
        {
            m_startPositions[w] = m_workers[w].transform.position;
        }

        if (m_autoStart)
            StartSimulation();
    }
    public void StartSimulation()
    {
        foreach (var worker in m_workers)
        {
            worker.DoStart();
        }
    }
    public void ResetSimulation()
    {
        for (int w = 0; w < m_workers.Length; ++w)
        {
            m_workers[w].ResetPath();
            m_workers[w].transform.position = m_startPositions[w];
        }
    }
}
