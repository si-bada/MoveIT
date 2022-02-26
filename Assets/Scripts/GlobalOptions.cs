using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains all parameters for the simulation
// This is not the most elegant way of doing it, but having a delegate for the configuration centralizes the parameters
// using ScriptableObjects would be preferable for more complex applications
public class GlobalOptions : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] LayerMask floorLayer;
    [Header("AI")]
    [SerializeField] int stoppingAreaMask; // valid area mask for the worker to stop
    [SerializeField] float _AISearchDiameterMax = 2;
    [SerializeField] float _AISearchDiameterMin = 0.5f;
    [SerializeField] int _AISearchdSteps = 4;
    [SerializeField] int _AIAngleSteps = 8;
    [SerializeField] float _AIWeightDistanceToWorker = 1;
    [SerializeField] float _AIWeightDistanceToTarget = 1;
    [Header("Controls")]
    //@TODO: use
    [SerializeField] KeyCode rotateLeft = KeyCode.LeftArrow;
    //@TODO: use
    [SerializeField] KeyCode rotateRight = KeyCode.RightArrow;
    //@TODO: use
    [SerializeField] float rotationSpeed = 15;

    public static GlobalOptions main;

    public LayerMask InteractableLayer { get => interactableLayer; private set => interactableLayer = value; }
    public LayerMask FloorLayer { get => floorLayer; private set => floorLayer = value; }
    public KeyCode RotateLeft { get => rotateLeft; private set => rotateLeft = value; }
    public KeyCode RotateRight { get => rotateRight; private set => rotateRight = value; }
    public float RotationSpeed { get => rotationSpeed; private set => rotationSpeed = value; }
    public int StoppingAreaMask { get => stoppingAreaMask; private set => stoppingAreaMask = value; }
    public LayerMask ObstacleLayer { get => obstacleLayer; private set => obstacleLayer = value; }
    public float AISearchDiameterMax { get => _AISearchDiameterMax; private set => _AISearchDiameterMax = value; }
    public int AIAngleSteps { get => _AIAngleSteps; private set =>  _AIAngleSteps = value; }
    public int AISearchdSteps { get => _AISearchdSteps; private set => _AISearchdSteps = value; }
    public float AISearchDiameterMin { get => _AISearchDiameterMin; set => _AISearchDiameterMin = value; }
    public float AIWeightDistanceToWorker { get => _AIWeightDistanceToWorker; private set => _AIWeightDistanceToWorker = value; }
    public float AIWeightDistanceToTarget { get => _AIWeightDistanceToTarget; private set => _AIWeightDistanceToTarget = value; }

    void Awake()
    {
        if (!main) main = this;
        else { Debug.LogWarning("More than one instance of GlobalOptions"); }
    }
}
