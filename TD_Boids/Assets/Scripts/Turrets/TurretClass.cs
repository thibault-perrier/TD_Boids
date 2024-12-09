using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class TurretClass : MonoBehaviour
{
    protected private Transform _turretTransform;
    protected private Vector3 _turretPos;
    [SerializeField] protected private float _detectionRange;
    [SerializeField] protected private float _shootingCD;
    [SerializeField] protected private bool _isShooting;
    [SerializeField] protected private int _upgradeLvlPath1;
    [SerializeField] protected private int _upgradeLvlPath2;
    [SerializeField] protected private int _upgradeLvlPath3;
    protected private int _LockedPath = -1;


    [Header("   Debug")]
    [SerializeField]
    private List<Transform> boidData = new List<Transform>();
    [SerializeField] private int _numberOfDebugs;
    [SerializeField] private GameObject _targetPrefab;


    void Awake()
    {
        _turretTransform = transform;
        _turretPos = _turretTransform.position;
    }

    [ContextMenu("Debug/DetectionTest")]
    void debugSetter()
    {
        for (int i = 0; i < _numberOfDebugs; ++i)
        {
            Targets target = Instantiate(_targetPrefab).GetComponent<Targets>();
            target._centerTrans = _turretTransform;
            target.Init();
            boidData.Add(target.transform);
            // boidData[i] = new boidDebug() { position = UnityEngine.Random.Range(0, _detectionRange * 2) * UnityEngine.Random.onUnitSphere + _turretTransform.position };
        }

        LookForEnemies(out Vector3 targetPos, out float distToTarget);
    }

    void Start()
    {
        debugSetter();
    }

    void Update()
    {
        LookForEnemies(out Vector3 targetPos, out float distToTarget);
    }

    // TODO: BoidData watchers that takes actions from other classes 
    //  -> allows to iterate through all boids one time per frame rather than one time in every classes
    protected virtual bool LookForEnemies(out Vector3 targetPos, out float distToTarget)
    {
        targetPos = Vector3.zero;
        distToTarget = float.PositiveInfinity;
        if (boidData.Count > 0)
        {
            float currDist;
            foreach (/*BoidManager.BoidData*/ Transform bTrans in boidData) // do the same but use the real boid data (array.tolist()) list from BoidManager
            {
                Targets b = bTrans.GetComponent<Targets>();
                if (b.boidData.ToRender == 0) continue;
                if ((currDist = (_turretPos - b.boidData.position).magnitude) < distToTarget)
                {
                    distToTarget = currDist;
                    targetPos = b.boidData.position;
                }
            }
            Debug.Log($"The boid i see is at a distance of {distToTarget}, situated at {targetPos}");
            return true;
        }
        else
        {
            Debug.Log($"I don't see any boids");
            return false;
        }







        // Dictionary<int, BoidManager.BoidData> boidData = BoidManager.Instance.AliveBoidData;

        // bool enemyFound = false;
        // float currDist = 0.0f;
        // float minDist = float.PositiveInfinity;
        // Vector3 nextTragetPos = Vector3.zero;
        // Vector3 turretPos = _turretTransform.position;

        // List<boidDebug> boidDebugs = new List<boidDebug>();

        // Parallel.For(0, boidData.Count, i =>
        // {
        //     if ((turretPos - boidData[i].position).magnitude <= _detectionRange)
        //     {
        //         boidDebugs.Add(boidData[i]);
        //         Debug.Log("I SEE");
        //     }
        // });

        // foreach (boidDebug b in boidDebugs)
        // {
        //     if ((currDist = (turretPos - b.position).magnitude) <= _detectionRange)
        //     {
        //         Debug.Log("I SEE");
        //         if (currDist < minDist)
        //         {
        //             Debug.Log("AND I SHALL SHOOT");
        //             minDist = currDist;
        //             nextTragetPos = b.position;
        //         }
        //     }
        // }

        // enemyFound = minDist != float.PositiveInfinity;

        // targetPos = enemyFound ? nextTragetPos : Vector3.zero;
        // distToTarget = enemyFound ? minDist : 0.0f;

        // Debug.Log($"The boid i see is at a distance of {distToTarget}, situated at {targetPos}");
        // return enemyFound;
    }
}
