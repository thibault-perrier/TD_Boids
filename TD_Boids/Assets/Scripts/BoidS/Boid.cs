using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    BoidSettings settings;

    // State
    [HideInInspector] public Vector3 position;
    [HideInInspector] public Vector3 forward;
    Vector3 velocity;

    // To update:
    Vector3 acceleration;
    [HideInInspector] public Vector3 avgFlockHeading;
    [HideInInspector] public Vector3 avgAvoidanceHeading;
    [HideInInspector] public Vector3 centreOfFlockmates;
    [HideInInspector] public int numPerceivedFlockmates;

    // Cached
    Material material;
    Transform cachedTransform;
    Transform target;
    private Transform _objectiveTrans;
    [SerializeField] private const float _minDistForObjective = 1.5f;
    [SerializeField] private const float _distForColl = 0.2f;


    // Rendered
    [HideInInspector] public int ToRender = 1;

    void Awake()
    {
        material = transform.GetComponentInChildren<MeshRenderer>().material;
        cachedTransform = transform;
        _objectiveTrans = GameObject.FindWithTag("WhiteHouse").transform;
    }

    public void Initialize(BoidSettings settings, Transform target)
    {
        this.target = target;
        this.settings = settings;

        position = cachedTransform.position;
        forward = cachedTransform.forward;

        float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
        velocity = cachedTransform.forward * startSpeed;
    }

    public void SetColour(Color col)
    {
        if (material != null)
        {
            material.color = col;
        }
    }

    public void UpdateBoid()
    {
        Vector3 acceleration = Vector3.zero;

        if (target != null)
        {
            Vector3 offsetToTarget = target.position - position;

            if (offsetToTarget.magnitude <= _minDistForObjective)
            {
                if (target == _objectiveTrans && offsetToTarget.magnitude <= _distForColl)
                {
                    ToRender = 0;
                    // Debug.Log("an alien has crashed into the target building");
                    return;
                }
                target = _objectiveTrans;
                offsetToTarget = (target.position - position);
            }

            acceleration = SteerTowards(offsetToTarget) * settings.targetWeight;
        }

        if (numPerceivedFlockmates != 0)
        {
            centreOfFlockmates /= numPerceivedFlockmates;

            Vector3 offsetToFlockmatesCentre = (centreOfFlockmates - position);

            var alignmentForce = SteerTowards(avgFlockHeading) * settings.alignWeight;
            var cohesionForce = SteerTowards(offsetToFlockmatesCentre) * settings.cohesionWeight;
            var seperationForce = SteerTowards(avgAvoidanceHeading) * settings.seperateWeight;

            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += seperationForce;
        }

        //! This is the big performance hit per boid
        if (IsHeadingForCollision())
        {
            Vector3 collisionAvoidDir = ObstacleRays();
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * settings.avoidCollisionWeight;
            acceleration += collisionAvoidForce;
        }

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed);
        velocity = dir * speed;

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;
        position = cachedTransform.position;
        forward = dir;
    }

    // TODO : OPTIMIZE COLLISION DETECTION
    bool IsHeadingForCollision()
    {
        RaycastHit hit;
        if (Physics.SphereCast(position, settings.boundsRadius, forward, out hit, settings.collisionAvoidDst, settings.obstacleMask))
        {
            return true;
        }
        else { }
        return false;
    }

    // TODO : OPTIMIZE COLLISION DETECTION
    Vector3 ObstacleRays()
    {
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(position, dir);
            if (!Physics.SphereCast(ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask))
            {
                return dir;
            }
        }

        return forward;
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * settings.maxSpeed - velocity;
        return Vector3.ClampMagnitude(v, settings.maxSteerForce);
    }
}