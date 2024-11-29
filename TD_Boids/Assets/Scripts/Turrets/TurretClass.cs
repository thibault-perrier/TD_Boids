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
    [SerializeField] protected private float _detectionRange;
    [SerializeField] protected private float _shootingCD;
    [SerializeField] protected private bool _isShooting;
    [SerializeField] protected private int _upgradeLvlPath1;
    [SerializeField] protected private int _upgradeLvlPath2;
    [SerializeField] protected private int _upgradeLvlPath3;
    protected private int _LockedPath = -1;


    [Serializable]
    public struct boidDebug
    {
        public Vector3 position;
    }

    [Header("   Debug")]
    [SerializeField]
    private List<boidDebug> boidData = new List<boidDebug>();
    [SerializeField] private int _numberOfDebugs;


    void Awake()
    {
        _turretTransform = transform;

    }

    [ContextMenu("Debug/DetectionTest")]
    void debugSetter()
    {
        for (int i = 0; i < _numberOfDebugs; ++i)
        {
            boidData.Add(new boidDebug() { position = UnityEngine.Random.Range(0, _detectionRange * 2) * UnityEngine.Random.onUnitSphere + _turretTransform.position });
            // boidData[i] = new boidDebug() { position = UnityEngine.Random.Range(0, _detectionRange * 2) * UnityEngine.Random.onUnitSphere + _turretTransform.position };
        }

        LookForEnemies(out Vector3 targetPos, out float distToTarget);
    }

    protected virtual bool LookForEnemies(out Vector3 targetPos, out float distToTarget)
    {
        // Dictionary<int, BoidManager.BoidData> boidData = BoidManager.Instance.AliveBoidData;

        // bool enemyFound = false;
        // float currDist = 0.0f;
        // float minDist = float.PositiveInfinity;
        // Vector3 nextTragetPos = Vector3.zero;
        // Vector3 turretPos = _turretTransform.position;

        List<boidDebug> boidDebugs = new List<boidDebug>();

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


        if (boidData.Count > 0)
        {
            boidData = boidData.OrderBy(b => (_turretTransform.position - b.position).magnitude).ToList();
            targetPos = boidData[0].position;
            distToTarget = (_turretTransform.position - boidData[0].position).magnitude;

            Debug.Log($"The boid i see is at a distance of {distToTarget}, situated at {targetPos}");
            return true;
        }
        else
        {
            targetPos = Vector3.zero;
            distToTarget = 0.0f;

            Debug.Log($"I don't see any boids");
            return false;
        }

        // enemyFound = minDist != float.PositiveInfinity;

        // targetPos = enemyFound ? nextTragetPos : Vector3.zero;
        // distToTarget = enemyFound ? minDist : 0.0f;

        // Debug.Log($"The boid i see is at a distance of {distToTarget}, situated at {targetPos}");
        // return enemyFound;
    }
}
